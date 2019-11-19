#pragma warning disable CS0618 // type deprecated
/////////////////////////////////////////
//   RAPHAËL DAUMAS 
//   https://raphdaumas.wixsite.com/portfolio
//   https://github.com/Marsgames
/////////////////////////////////////////
using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.AI;
using UnityEngine.EventSystems;
using System.Collections.Generic;

public class SpawnUnits : NetworkBehaviour
{
    #region Variables

    [SerializeField, Range(0f, 2f)] private float m_secondsBeforeSpawn = 0;
    [SerializeField] private Transform m_door = null;
    [SerializeField] private ParticleSystem m_spawnParticles = null;
    [SerializeField] private EmitterBehaviour m_soundEmitter = null;

    private List<Constant.SpawnTypeUnit> m_unitsQueue = new List<Constant.SpawnTypeUnit>();
    private GameObject m_target;
    private PlayerEntity.Player m_playerNumber = PlayerEntity.Player.Neutre;
    private NavMeshPath m_path;
    private SoundManager m_soundManager;
    private UIGlobal m_uiGlobal;
    private UISpawnUnit m_uiSpawnUnit;
    private RemoveGoldsUI m_substractGoldUI;
    private UISelectSpawnUnit m_selectSpawnUnitUI;

    #endregion

    #region Unity's functions
    public IEnumerator Start()
    {
        yield return new WaitUntil(() => GameManager.Instance != null);
        yield return new WaitUntil(() => GameManager.Instance.GetAllPlayersReady());

        m_soundManager = SoundManager.Instance;

        m_uiGlobal = UIGlobal.Instance;
        m_uiSpawnUnit = m_uiGlobal.GetUISpawnUnit();
        m_substractGoldUI = m_uiGlobal.GetRemoveGoldsUI();

        CheckIfOk();

        yield return new WaitWhile(() => m_playerNumber == PlayerEntity.Player.Neutre);
        if (gameObject.CompareTag(Constant.ListOfTag.s_spawnUnit1) && m_playerNumber == GameManager.Instance.GetLocalPlayer())
        {
            m_uiSpawnUnit.SetupUI(this);
            GetComponent<ObjectSelection>().OnMouseDown();
        }
    }

    private void OnMouseDown()
    {
        if (EventSystem.current.IsPointerOverGameObject())
        {
            return;
        }

        ShowUISpawnUnit();
    }
    #endregion

    #region Functions
    public void WaitBeforeRecalculate(float time = 0)
    {
        Invoke("CmdRecalculObjectivePath", time);
    }

    [Command]
    public void CmdAddToUnitsQueue(Constant.SpawnTypeUnit typeOfUnit)
    {
        PlayerEntity player = GameManager.Instance.GetPlayer(m_playerNumber);
        GameObject prefabTemp = GameManager.Instance.GetUnitForPlayer(typeOfUnit, m_playerNumber);
        if (player.GetUnitGold() < prefabTemp.GetComponent<UnitController>().GetUnitCost())
        {
            TargetShowUIWarningUnit(player.connectionToClient);
            return;
        }
        player.SubstractUnitGold(prefabTemp.GetComponent<UnitController>().GetUnitCost());
        TargetShowSubstractGold(player.connectionToClient, typeOfUnit);
        TargetSound(player.connectionToClient, SoundManager.AudioClipList.AC_unitBuy);
        m_unitsQueue.Add(typeOfUnit);
        if (m_unitsQueue.Count > 1)
        {
            return;
        }
        InvokeRepeating("SpawnUnitsFromQueue", m_secondsBeforeSpawn, m_secondsBeforeSpawn);
    }

    /// <summary>
    /// Tells to the good player to show "-XXX golds" on its UI
    /// </summary>
    /// <param name="target">Target.</param>
    /// <param name="typeOfUnit">Type of unit (to set the cost).</param>
    [TargetRpc]
    private void TargetShowSubstractGold(NetworkConnection target, Constant.SpawnTypeUnit typeOfUnit)
    {
        int cost = GameManager.Instance.GetUnitForPlayer(typeOfUnit, m_playerNumber).GetComponent<UnitController>().GetUnitCost();
        string text = "-" + cost.ToString();

        m_substractGoldUI.SpawnText(text, .5f);
    }

    [Server]
    private void SpawnUnitsFromQueue()
    {
        if (m_unitsQueue.Count < 1)
        {
            CancelInvoke("SpawnUnitsFromQueue");
            return;
        }
        if (m_playerNumber != PlayerEntity.Player.Bot && GameManager.Instance.GetLocalPlayer() == m_playerNumber)
        {
            m_soundEmitter.TargetPlaySound(GameManager.Instance.GetPlayer(m_playerNumber).connectionToClient);
        }


        CreateUnit(m_unitsQueue[0]);
        m_unitsQueue.RemoveAt(0);
    }

    [Server]
    private void CreateUnit(Constant.SpawnTypeUnit spawnType)
    {
        GameObject go = Instantiate(GameManager.Instance.GetUnitForPlayer(spawnType, m_playerNumber), m_door.position, m_door.rotation);
        NetworkServer.Spawn(go);
        GameManager.Instance.GetPlayer(m_playerNumber).IncrementNbUnitInGame();
        RpcPlaySpawnFX();
        UnitController goUnitController = go.GetComponent<UnitController>();
        goUnitController.SetObjective(m_target.transform.position);
        goUnitController.SetPlayerNumber(m_playerNumber);
        goUnitController.SetParentPath(m_path);
        goUnitController.SetNexusEnemy(m_target.transform.position);
        if (m_playerNumber != PlayerEntity.Player.Bot)
        {
            TargetSoundEmitter(GameManager.Instance.GetPlayer(m_playerNumber).connectionToClient);
        }
    }

    [ClientRpc]
    public void RpcInitPlayerNumber(PlayerEntity.Player playerNumber)
    {
        m_playerNumber = playerNumber;
        GameManager.Instance.AddSpawnUnitToList(playerNumber, gameObject.tag, this);
    }

    [ClientRpc]
    private void RpcPlaySpawnFX()
    {
        if (null != m_spawnParticles)
        {
            m_spawnParticles.Play();
        }
    }

    [Server]
    public void RecalculObjectivePath()
    {
        NavMeshPath myPath = new NavMeshPath();
        NavMeshHit hit;
        NavMesh.SamplePosition(m_target.transform.position, out hit, Vector3.Distance(transform.position, m_target.transform.position) + 100, NavMesh.AllAreas);

        NavMeshAgent tempAgent = gameObject.AddComponent<NavMeshAgent>();
        tempAgent.CalculatePath(hit.position, myPath);
        m_path = myPath;

        Destroy(tempAgent);
    }

    [TargetRpc]
    public virtual void TargetSound(NetworkConnection target, SoundManager.AudioClipList audioClip)
    {
        m_soundManager.PlaySound(audioClip);
    }

    [TargetRpc]
    public virtual void TargetSoundEmitter(NetworkConnection target)
    {
        m_soundEmitter.PlaySound();
    }
    #endregion

    #region Accessors

    public PlayerEntity.Player GetPlayerNumber()
    {
        return m_playerNumber;
    }

    public void SetPlayerNumber(PlayerEntity.Player playerIndex)
    {
        m_playerNumber = playerIndex;
    }

    public void SetTarget(GameObject newTarget)
    {
        m_target = newTarget;
    }

    public void ShowUISpawnUnit()
    {
        if (GameManager.Instance.GetLocalPlayer() == m_playerNumber)
        {
            m_uiSpawnUnit.SetupUI(this);
            m_soundManager.PlaySound(SoundManager.AudioClipList.AC_generatorClick);
            m_selectSpawnUnitUI.OnSelection();
        }
    }

    [TargetRpc]
    public void TargetShowUIWarningUnit(NetworkConnection target)
    {
        m_uiGlobal.ShowWarning(Constant.ListOfText.s_warningUnit);
    }

    public UISelectSpawnUnit GetUISelectSpawnUnit()
    {
        return m_selectSpawnUnitUI;
    }

    public void SetUISelectSpawnUnit(UISelectSpawnUnit uISelectSpawnUnit)
    {
        m_selectSpawnUnitUI = uISelectSpawnUnit;
    }

    private void CheckIfOk()
    {
#if UNITY_EDITOR
        if (null == m_door)
        {
            Debug.LogError("Door ne peut pas être null dans " + name, this);
            UnityEditor.EditorApplication.isPlaying = false;
        }

        if (null == m_soundEmitter)
        {
            Debug.LogError("Sound emitter not set on " + name, this);
            UnityEditor.EditorApplication.isPlaying = false;
        }

        if (null == m_spawnParticles)
        {
            Debug.LogError("Spawn Particles not set on " + name, this);
            UnityEditor.EditorApplication.isPlaying = false;
        }
#endif
    }
    #endregion

    #region AI

    public List<Constant.SpawnTypeUnit> GetUnitsQueue()
    {
        return m_unitsQueue;
    }

    public GameObject GetTarget()
    {
        return m_target;
    }

    public void SetUnitsQueue(List<Constant.SpawnTypeUnit> updatedUnitsQueue)
    {
        m_unitsQueue.AddRange(updatedUnitsQueue);
    }

    public void MakeSpawnUnitsFromQueue()
    {
        if (m_unitsQueue.Count < 1)
        {
            return;
        }
        InvokeRepeating("SpawnUnitsFromQueue", m_secondsBeforeSpawn, m_secondsBeforeSpawn);
    }

    public bool IsSpawning()
    {
        return m_unitsQueue.Count > 0;
    }
    #endregion
}
