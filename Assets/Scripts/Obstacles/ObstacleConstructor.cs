#pragma warning disable CS0618 // type deprecated
#region Author
/////////////////////////////////////////
//   RAPHAËL DAUMAS
//   https://raphdaumas.wixsite.com/portfolio
//   https://github.com/Marsgames
/////////////////////////////////////////
//   Yannig Smagghe --> #SCRIPTNAME#
//   https://gitlab.com/YannigSmagghe
/////////////////////////////////////////
//   Guillaume Quiniou 
/////////////////////////////////////////
#endregion

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.EventSystems;
using UnityEngine.Networking;
using UnityEngine.UI;

public class ObstacleConstructor : NetworkBehaviour
{
    #region variables
    public enum EState
    {
        Buildable,
        Invulnerable,
        Built,
        BeeingDestruct,
        Resting,
    }

    [SerializeField] private int m_buildingTime = 2;
    [SerializeField] private int m_invulnerabilityTime = 3;
    [SerializeField] private int m_deconstructTime = 2;
    [SerializeField] private int m_timeBeforeBuildable = 5;
    [SerializeField] private Text m_countdownText = null;
    [SerializeField] private GameObject[] m_obstacleToSpawn_S = new GameObject[5];
    [SerializeField] private GameObject[] m_obstacleToSpawn_A = new GameObject[5];
    [SerializeField] private Material m_transparentSyca = null;
    [SerializeField] private Material m_transparentArca = null;
    [SerializeField] private Material m_activeMaterial = null;
    [SerializeField] private Material m_inactiveMaterial = null;
    [SerializeField] private Texture2D m_cursorConstruction = null;
    [SerializeField] private Texture2D m_cursorCannotBuild = null;
    [SerializeField] private ParticleSystem m_particleSystem = null;
    [SerializeField] private UISpawnObstacle m_uiObstacle = null;
    [SerializeField] private UIObstacleConstructorIcon m_uiConstructorIcon = null;

    private Dictionary<PlayerEntity.Player, Dictionary<ActiveObstacle.ObstacleType, GameObject>> m_prefabObstacle;
    private EState m_currentState = EState.Buildable;
    private float m_currentTime = 0f;
    private Territory m_territory;
    private SoundManager m_soundManager;
    private UIGlobal m_uiGlobal;
    private GameObject visualObstacle;
    private MeshRenderer m_meshRenderer;

    #endregion

    #region Unity's function

    private IEnumerator Start()
    {
        m_prefabObstacle = new Dictionary<PlayerEntity.Player, Dictionary<ActiveObstacle.ObstacleType, GameObject>>();
        Dictionary<ActiveObstacle.ObstacleType, GameObject> dicoTemp = new Dictionary<ActiveObstacle.ObstacleType, GameObject>();
        foreach (GameObject prefab in m_obstacleToSpawn_S)
        {
            ActiveObstacle prefabObstacle = prefab.GetComponent<ActiveObstacle>();
            switch (prefabObstacle.GetObstacleType())
            {
                case ActiveObstacle.ObstacleType.SimpleWall:
                    dicoTemp.Add(ActiveObstacle.ObstacleType.SimpleWall, prefab);
                    break;
                case ActiveObstacle.ObstacleType.SlopeLeft:
                    dicoTemp.Add(ActiveObstacle.ObstacleType.SlopeLeft, prefab);
                    break;
                case ActiveObstacle.ObstacleType.SlopeRight:
                    dicoTemp.Add(ActiveObstacle.ObstacleType.SlopeRight, prefab);
                    break;
                case ActiveObstacle.ObstacleType.Pillar:
                    dicoTemp.Add(ActiveObstacle.ObstacleType.Pillar, prefab);
                    break;
                case ActiveObstacle.ObstacleType.Trap:
                    dicoTemp.Add(ActiveObstacle.ObstacleType.Trap, prefab);
                    break;
            }
        }
        m_prefabObstacle.Add(PlayerEntity.Player.Player1, dicoTemp);

        dicoTemp = new Dictionary<ActiveObstacle.ObstacleType, GameObject>();
        foreach (GameObject prefab in m_obstacleToSpawn_A)
        {
            ActiveObstacle prefabObstacle = prefab.GetComponent<ActiveObstacle>();
            switch (prefabObstacle.GetObstacleType())
            {
                case ActiveObstacle.ObstacleType.SimpleWall:
                    dicoTemp.Add(ActiveObstacle.ObstacleType.SimpleWall, prefab);
                    break;
                case ActiveObstacle.ObstacleType.SlopeLeft:
                    dicoTemp.Add(ActiveObstacle.ObstacleType.SlopeLeft, prefab);
                    break;
                case ActiveObstacle.ObstacleType.SlopeRight:
                    dicoTemp.Add(ActiveObstacle.ObstacleType.SlopeRight, prefab);
                    break;
                case ActiveObstacle.ObstacleType.Pillar:
                    dicoTemp.Add(ActiveObstacle.ObstacleType.Pillar, prefab);
                    break;
                case ActiveObstacle.ObstacleType.Trap:
                    dicoTemp.Add(ActiveObstacle.ObstacleType.Trap, prefab);
                    break;
            }
        }
        m_prefabObstacle.Add(PlayerEntity.Player.Player2, dicoTemp);
        m_prefabObstacle.Add(PlayerEntity.Player.Bot, dicoTemp);

        m_meshRenderer = GetComponent<MeshRenderer>();
        m_territory = GetComponentInParent<Territory>();
        m_uiGlobal = UIGlobal.Instance;
        m_soundManager = SoundManager.Instance;
        yield return new WaitUntil(() => GameManager.Instance != null);
        GameManager.Instance.AddObstacleConstructorToList(this);
        CheckIfOk();
    }

    private void OnMouseDown()
    {
        if (!EventSystem.current.IsPointerOverGameObject())
        {
            if (m_currentState == EState.Resting)
            {
                m_soundManager.PlaySound(SoundManager.AudioClipList.AC_obstableImpossibleInteract);
            }

            if (m_territory.IsAvailable(GameManager.Instance.GetLocalPlayer()))
            {
                m_uiObstacle.SetActive(true);
                m_soundManager.PlaySound(SoundManager.AudioClipList.AC_clickOnWall);
            }
            else
            {
                m_uiObstacle.SetActive(false);
                m_uiGlobal.ShowWarning(Constant.ListOfText.s_warningTerritory);
                m_soundManager.PlaySound(SoundManager.AudioClipList.AC_obstableImpossibleInteract);
            }
        }
    }

    private void OnMouseOver()
    {
        if (!EventSystem.current.IsPointerOverGameObject())
        {
            if (m_territory.IsAvailable(GameManager.Instance.GetLocalPlayer()) && m_currentState == EState.Buildable)
            {
                Cursor.SetCursor(m_cursorConstruction, new Vector2(32, 32), CursorMode.Auto);
            }
            else if (!m_territory.IsAvailable(GameManager.Instance.GetLocalPlayer()) &&
                     m_currentState == EState.Buildable)
            {
                Cursor.SetCursor(m_cursorCannotBuild, new Vector2(32, 32), CursorMode.Auto);
            }
            else
            {
                Cursor.SetCursor(null, Vector2.zero, CursorMode.Auto);
            }
        }
        else
        {
            GetComponent<ObjectSelection>().SetKeepGlow(false);
            GetComponent<ObjectSelection>().OnMouseExit();
        }
    }

    private void OnMouseExit()
    {
        Cursor.SetCursor(null, Vector2.zero, CursorMode.Auto);
    }
    #endregion

    #region Functions    
    /// <summary>
    /// Create and set an obstacle for a player
    /// </summary>   
    /// <param name="spawnType">obstacle's spawntype</param>
    /// <param name="playerNumber">obstacle's owner</param>
    [Server]
    public void BuildObstacle(ActiveObstacle.ObstacleType obstacleType, PlayerEntity.Player playerNumber)
    {
        if (m_currentState != EState.Buildable)
        {
            return;
        }

        NetworkConnection conn = GameManager.Instance.GetPlayer(playerNumber).connectionToClient;

        GameObject obstacleToSpawn = GetObstacleToSpawn(obstacleType, playerNumber);
        int cost = obstacleToSpawn.GetComponent<ActiveObstacle>().GetBuildingCost();

        if (null != conn)
        {
            if (!m_territory.IsAvailable(playerNumber))
            {
                TargetShowWarningTerritory(conn);
            }

            if (GameManager.Instance.GetPlayer(playerNumber).GetBuildGold() < cost)
            {
                TargetShowWarningObstacle(conn);
            }

            TargetSoundConstruction(GameManager.Instance.GetLocalPlayerEntity().connectionToClient);
        }
        if (GameManager.Instance.GetPlayer(playerNumber).GetBuildGold() < cost)
        {
            if (playerNumber != PlayerEntity.Player.Bot)
            {
                TargetShowWarningObstacle(GameManager.Instance.GetPlayer(playerNumber).connectionToClient);
            }
            return;
        }
        GameManager.Instance.GetPlayer(playerNumber).GainBuildGold(-cost);
        m_currentState = EState.Invulnerable;
        RpcSetCurrentState(EState.Invulnerable);
        RpcEnableObstacleConstructor(false);
        RpcDisableUI();
        GameObject obstacleSpawnned = Instantiate(obstacleToSpawn);
        NetworkServer.Spawn(obstacleSpawnned);
        obstacleSpawnned.GetComponent<ActiveObstacle>().RpcInitOnConstruction(gameObject, playerNumber);

        int childCount = 0;
        foreach (Transform child in transform)
        {
            if (child.gameObject.GetComponent<ActiveObstacle>())
            {
                if (childCount > 1)
                {
                    Destroy(child);
                }
                childCount++;
            }
        }
    }

    /// <summary>
    /// Previews the obstacle chosen
    /// </summary>
    /// <param name="obstacleType">Type of the obstacle to preview</param>
    /// <param name="playerNumber">Determines which mesh/material will be previewed depending on the player number</param>
    [Client]
    public void PreviewObstacle(ActiveObstacle.ObstacleType obstacleType, PlayerEntity.Player playerNumber)
    {
        if (EState.Buildable != m_currentState)
        {
            return;
        }
        GameObject obstacleToSpawn = GetObstacleToSpawn(obstacleType, playerNumber);
        visualObstacle = Instantiate(obstacleToSpawn);
        if (ActiveObstacle.ObstacleType.Trap != obstacleType)
        {
            foreach (NavMeshObstacle navObs in visualObstacle.GetComponentsInChildren<NavMeshObstacle>())
            {
                navObs.enabled = false;
            }
        }
        Destroy(visualObstacle.GetComponent<ObjectSelection>());
        Destroy(visualObstacle.GetComponent<ActiveObstacle>());
        if (playerNumber == PlayerEntity.Player.Player1)
        {
            visualObstacle.GetComponent<Renderer>().material = m_transparentSyca;
        }
        else
        {
            visualObstacle.GetComponent<Renderer>().material = m_transparentArca;
        }
        visualObstacle.transform.SetParent(gameObject.transform);
        visualObstacle.transform.localPosition = Vector3.zero;
        visualObstacle.transform.localRotation = Quaternion.identity;
        if (obstacleType == ActiveObstacle.ObstacleType.SlopeLeft)
        {
            visualObstacle.transform.localPosition = new Vector3(0, 0, 1.5f);
            visualObstacle.transform.Rotate(Vector3.up, 180);
        }
        else if (obstacleType == ActiveObstacle.ObstacleType.SlopeRight)
        {
            visualObstacle.transform.localPosition = new Vector3(0, 0, -1.5f);
        }
        EnableObstacleConstructor(false);
    }

    /// <summary>
    /// If existing, delet the preview obstacle
    /// </summary>
    [Client]
    public void DeletePreviewObstacle()
    {
        if (visualObstacle)
        {
            Destroy(visualObstacle);
            if (EState.Buildable == m_currentState)
            {
                EnableObstacleConstructor(true);
            }
        }
    }

    /// <summary>
    /// Change the obstacleConstructor state's
    /// </summary>
    /// <param name="newState">New state</param>
    /// <param name="when">timer after which new state is set, default is 0</param>
    private IEnumerator ChangeState(EState newState, int when = 0)
    {
        if (when > 0)
        {
            yield return new WaitForSeconds(when);
        }
        m_currentState = newState;
    }

    /// <summary>
    /// Enable, start and run the timer
    /// </summary>
    /// <param name="newState">The new obstacleConstructor state</param>
    private IEnumerator ActiveTimer(EState newState)
    {
        if (!m_countdownText.transform.parent.GetComponent<Canvas>().isActiveAndEnabled)
        {
            m_countdownText.transform.parent.gameObject.SetActive(true);

            switch (newState)
            {
                case EState.Resting:
                    m_currentTime = m_timeBeforeBuildable;
                    transform.Find("Obstacle_Constructor_Shield").GetComponentInChildren<ParticleSystem>().Play();
                    transform.Find("FX_ObstacleConstructor").GetComponentInChildren<ParticleSystem>().Stop();
                    break;

                case EState.Invulnerable:
                    m_currentTime = m_invulnerabilityTime;
                    break;
            }
            transform.Find(Constant.ListOfUI.s_canvas).Find(Constant.ListOfUI.s_cd).GetComponent<CooldownController>().ActiveCooldown((int)m_currentTime);
            m_countdownText.text = ((int)m_currentTime).ToString();
            while (m_currentTime > 0)
            {
                m_countdownText.text = ((int)m_currentTime).ToString();
                yield return new WaitForSecondsRealtime(1);
                m_currentTime -= 1;
            }

            transform.Find(Constant.ListOfUI.s_canvas).Find(Constant.ListOfUI.s_cd).GetComponent<CooldownController>().StopCooldown();
            m_countdownText.transform.parent.gameObject.SetActive(false);
            ChangeMaterial(true);
            yield return null;
        }
    }

    /// <summary>
    /// Change the material depending the fact that obstacleConstructor is active or not
    /// </summary>
    public void ChangeMaterial(bool isActive)
    {
        if (isActive && m_territory.IsAvailable(GameManager.Instance.GetLocalPlayer()) && m_currentState == EState.Buildable)
        {
            m_meshRenderer.material = m_activeMaterial;
            m_particleSystem.Play();
        }
        else
        {
            m_meshRenderer.material = m_inactiveMaterial;
            m_meshRenderer.material.color = new Color(0.3f, 0.3f, 0.3f);
            m_particleSystem.Stop();
        }
    }

    /// <summary>
    /// Return the cost of the obstacle according to the spawn type
    /// </summary>
    /// <param name="mSpawnType">The spawn type of the desired obstacle</param>
    /// <returns>The cost of the osbtacle</returns>
    public int GetObstacleCost(ActiveObstacle.ObstacleType obstacleType)
    {
        int obstacleCost = int.MaxValue;
        switch (obstacleType)
        {
            case ActiveObstacle.ObstacleType.SimpleWall:
                obstacleCost = GetObstacleToSpawn(obstacleType, GameManager.Instance.GetLocalPlayer()).GetComponent<ActiveObstacle>().GetBuildingCost();
                break;
            case ActiveObstacle.ObstacleType.SlopeLeft:
                obstacleCost = GetObstacleToSpawn(obstacleType, GameManager.Instance.GetLocalPlayer()).GetComponent<ActiveObstacle>().GetBuildingCost();
                break;
            case ActiveObstacle.ObstacleType.SlopeRight:
                obstacleCost = GetObstacleToSpawn(obstacleType, GameManager.Instance.GetLocalPlayer()).GetComponent<ActiveObstacle>().GetBuildingCost();
                break;
            case ActiveObstacle.ObstacleType.Pillar:
                obstacleCost = GetObstacleToSpawn(obstacleType, GameManager.Instance.GetLocalPlayer()).GetComponent<ActiveObstacle>().GetBuildingCost();
                break;
            case ActiveObstacle.ObstacleType.Trap:
                obstacleCost = GetObstacleToSpawn(obstacleType, GameManager.Instance.GetLocalPlayer()).GetComponent<ActiveObstacle>().GetBuildingCost();
                break;
        }
        return obstacleCost;
    }

    /// <summary>
    /// Enable or not the renderer and set the material
    /// </summary>
    public void EnableObstacleConstructor(bool active)
    {
        m_meshRenderer.enabled = active;
        ChangeMaterial(active);
    }

    [ClientRpc]
    public void RpcEnableObstacleConstructor(bool active)
    {
        EnableObstacleConstructor(active);
    }

    [TargetRpc]
    public void TargetSoundConstruction(NetworkConnection target)
    {
        m_soundManager.PlaySound(SoundManager.AudioClipList.AC_construction);
    }

    [ClientRpc]
    public void RpcSetCurrentState(EState state)
    {
        SetCurrentState(state);
    }

    [ClientRpc]
    private void RpcDisableUI()
    {
        m_uiObstacle.SetActive(false);
        DeletePreviewObstacle();
    }

    [TargetRpc]
    public void TargetShowWarningObstacle(NetworkConnection target)
    {
        m_uiGlobal.ShowWarning(Constant.ListOfText.s_warningObstacle);
    }

    [TargetRpc]
    public void TargetShowWarningTerritory(NetworkConnection connectionToClient)
    {
        m_uiObstacle.SetActive(false);
        m_uiGlobal.ShowWarning(Constant.ListOfText.s_warningTerritory);
    }
    #endregion

    #region Accessors
    public void SetCurrentState(EState newState)
    {
        m_currentState = newState;
        switch (newState)
        {
            case EState.Built:
                m_uiConstructorIcon.HideConstructorIcon();
                break;
            case EState.Buildable:
                m_uiConstructorIcon.ShowConstructorIcon();
                break;
            case EState.Invulnerable:
                m_uiConstructorIcon.HideConstructorIcon();
                StartCoroutine(ActiveTimer(newState));
                StartCoroutine(ChangeState(EState.Built, m_invulnerabilityTime));
                break;
            case EState.BeeingDestruct:
                m_uiConstructorIcon.ShowConstructorIcon();
                StartCoroutine(ChangeState(EState.Resting, m_deconstructTime));
                break;
            case EState.Resting:
                m_uiConstructorIcon.ShowConstructorIcon();
                StartCoroutine(ActiveTimer(newState));
                StartCoroutine(ChangeState(EState.Buildable, m_timeBeforeBuildable));
                break;
        }
    }

    public EState GetCurrentState()
    {
        return m_currentState;
    }

    public int GetBuildingTime()
    {
        return m_buildingTime;
    }

    public int GetDeconstructionTime()
    {
        return m_deconstructTime;
    }

    public GameObject GetObstacleToSpawn(ActiveObstacle.ObstacleType spawnType, PlayerEntity.Player playerNumber)
    {
        return m_prefabObstacle[playerNumber][spawnType];
    }

    public UISpawnObstacle GetUISpawnObstacle()
    {
        return m_uiObstacle;
    }

    public Territory GetTerritory()
    {
        return m_territory;
    }

    public float GetCurrentRestingTime()
    {
        return m_currentTime;
    }
    #endregion

    private void CheckIfOk()
    {
#if UNITY_EDITOR
        if (null == m_territory)
        {
            Debug.LogError(name + " NOT IN A TERRITORY", this);
            UnityEditor.EditorApplication.isPlaying = false;
        }
        if (null == m_uiGlobal)
        {
            Debug.LogError(name + " CANT FIND UIGLOBAL", this);
            UnityEditor.EditorApplication.isPlaying = false;
        }
        if (null == m_soundManager)
        {
            Debug.LogError(name + " CANT FIND SOUND MANAGER", this);
            UnityEditor.EditorApplication.isPlaying = false;
        }
#endif
    }
}