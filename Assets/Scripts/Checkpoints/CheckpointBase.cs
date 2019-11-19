#pragma warning disable CS0618 // type deprecated
#region Author
/////////////////////////////////////////
//   Yannig Smagghe 
//   https://gitlab.com/YannigSmagghe
///////////////////////////////////////// 
//   RAPHAËL DAUMAS 
//   https://raphdaumas.wixsite.com/portfolio
//   https://github.com/Marsgames
/////////////////////////////////////////
/////   Guillaume Quiniou 
/////////////////////////////////////////
#endregion

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public abstract class CheckpointBase : NetworkBehaviour
{
    #region Variables
    [SerializeField] protected int m_maxStockNumber = 5;
    [SerializeField] protected GameObject m_stockUnits = null;
    [SerializeField] protected GameObject m_area = null;
    [SerializeField] protected GameObject m_friendsUnits = null;
    [SerializeField] protected GameObject m_enemiesUnits = null;
    [SerializeField] protected UIReleaseUnit m_releaseUnitUI = null;
    [SerializeField] protected GameObject m_sycaCheckpointZone = null;
    [SerializeField] protected GameObject m_sycaCheckpointParticles = null;
    [SerializeField] protected Material m_sycaMaterial = null;
    [SerializeField] protected GameObject m_arcaCheckpointZone = null;
    [SerializeField] protected GameObject m_arcaCheckpointParticles = null;
    [SerializeField] protected Material m_arcaMaterial = null;
    [SerializeField] protected Material m_neutralMaterial = null;
    [SerializeField] protected EmitterBehaviour m_soundEmitter;

    [SyncVar] protected PlayerEntity.Player m_playerOwner = PlayerEntity.Player.Neutre;

    protected bool m_onFight = false;
    protected Dictionary<PlayerEntity.Player, bool> m_checkpointOpenPlayer = new Dictionary<PlayerEntity.Player, bool>();
    protected AlertManager m_alertManager;
    protected Territory m_territory;

    private GameObject m_currentCheckpointzone = null;

    #endregion

    #region Unity's functions

    protected virtual IEnumerator Start()
    {
        m_checkpointOpenPlayer.Add(PlayerEntity.Player.Player1, true);
        m_checkpointOpenPlayer.Add(PlayerEntity.Player.Player2, true);
        m_checkpointOpenPlayer.Add(PlayerEntity.Player.Bot, true);

        m_alertManager = AlertManager.Instance;
        m_territory = transform.parent.transform.parent.GetComponent<Territory>();
        yield return new WaitUntil(() => GameManager.Instance != null);
        GameManager.Instance.AddCheckpointToList(this);
        CheckIfOk();
        m_soundEmitter.StopSound();
    }

    [Server]
    public void TriggerExit(Collider other)
    {
        if (!other.tag.Equals(Constant.ListOfTag.s_unit) ||
            other.GetComponent<CombatUnit>().GetCurrentState() != CombatUnit.State.NotInFight)
        {
            return;
        }
        UnitController unit = other.GetComponent<UnitController>();
        unit.transform.SetParent(null);
        unit.SetCheckpointBase(null);
        unit.SetCheckpointState(UnitController.EUnitCheckpointState.lookingForStock);
        unit.GetAggroRangeCollider().SetActive(true);
        SetCheckpointNeutral();
    }

    [Server]
    public virtual void TriggerEnter(Collider other)
    {
        if (!other.tag.Equals(Constant.ListOfTag.s_unit))
        {
            return;
        }
        UnitController unitController = other.GetComponent<UnitController>();
        unitController.GetAggroRangeCollider().SetActive(false);
        unitController.SetCheckpointBase(this);
        CheckAndSetOwnership(other.GetComponent<UnitController>());
    }

    [Server]
    public void TriggerStay(Collider other)
    {
        if (!other.tag.Equals(Constant.ListOfTag.s_unit))
        {
            return;
        }

        if (null != other.transform.parent && other.transform.parent.name.Equals(m_stockUnits.name))
        {
            UnitController unitController = other.GetComponent<UnitController>();
            if (CombatUnit.State.NotInFight == other.GetComponent<CombatUnit>().GetCurrentState() &&
                UnitController.EUnitCheckpointState.bannedFromStock != unitController.GetCheckpointState())
            {
                if (unitController.GetCheckpointState() == UnitController.EUnitCheckpointState.fightingForCheckpoint && m_onFight)
                {
                    FightIsOver();

                }
                unitController.SetCheckpointState(UnitController.EUnitCheckpointState.inCheckpointBarrack);
                unitController.RpcSetActive(false);
                unitController.gameObject.SetActive(false);
            }
        }
    }

    #endregion

    #region Functions

    /// <summary>
    /// Handle the case where unit defend, win the fight and comeback to garrison
    /// </summary>
    [Server]
    protected virtual void FightIsOver()
    {
        RpcSetIsOnFight(false);
    }


    /// <summary>
    /// Handle when a unit enter in the checkpoint
    /// </summary>
    /// <param name="unit">Unit that entered in checkpoint</param>
    [Server]
    public void CheckAndSetOwnership(UnitController unit)
    {
        if (unit.GetCheckpointState() == UnitController.EUnitCheckpointState.fightingForCheckpoint ||
            unit.GetCheckpointState() == UnitController.EUnitCheckpointState.bannedFromStock ||
            unit.GetComponent<CombatUnit>().GetCurrentState() == CombatUnit.State.Fighting)
        {
            return;
        }

        if (m_playerOwner == unit.GetPlayerNumber())
        {
            CheckOwnershipForOwnerUnit(unit);
        }
        else
        {
            if (m_stockUnits.transform.childCount == 0 && m_friendsUnits.transform.childCount == 0)
            {
                CaptureCheckpoint(unit);
            }
            else
            {
                AttackCheckpoint(unit);
            }
        }
    }

    /// <summary>
    /// Handle case of enemy unit enter checkpoint and attack it
    /// </summary>
    /// <param name="unit">Unit that entered in checkpoint</param>
    [Server]
    protected virtual void AttackCheckpoint(UnitController unit)
    {
        if (m_enemiesUnits.transform.childCount == 0 && m_playerOwner != PlayerEntity.Player.Bot && m_playerOwner != PlayerEntity.Player.Neutre)
        {
            NetworkConnection connectionToOwner = GameManager.Instance.GetPlayer(m_playerOwner).connectionToClient;
            TargetSoundAttacked(connectionToOwner);
            RpcSetIsOnFight(true);
            TargetHideUIOnFight(connectionToOwner);
        }

        unit.transform.SetParent(m_enemiesUnits.transform);
        AttackInRangeOfCheckpoint(unit.GetComponent<CombatUnit>());
    }

    [ClientRpc]
    private void RpcSetIsOnFight(bool value)
    {
        m_onFight = value;
    }

    [TargetRpc]
    private void TargetHideUIOnFight(NetworkConnection target)
    {
        if (null != m_releaseUnitUI)
        {
            m_releaseUnitUI.HideUIRelease();
        }
    }

    /// <summary>
    ///  Handle case of enemy unit enter empty checkpoint and capture it
    /// </summary>
    /// <param name="unit">Unit that entered in checkpoint</param>
    [Server]
    protected virtual void CaptureCheckpoint(UnitController unit)
    {

        PlayerEntity.Player unitOwner = unit.GetPlayerNumber();
        SetCheckpointAuthority(unitOwner);
        RpcSetIsOnFight(false);
        RpcChangeIconOn2DView();
        RpcCheckpointZoneTransition(unitOwner);
        RpcSetMaterial(GameManager.Instance.GetPlayer(m_playerOwner).GetPlayerNumber());
        if (PlayerEntity.Player.Bot != m_playerOwner)
        {
            TargetPlaySoundCapture(GameManager.Instance.GetPlayer(m_playerOwner).connectionToClient);
            RpcAllPlaySoundCapture();
        }
        CheckAndSetOwnership(unit);
    }

    /// <summary>
    /// Handle case of ally unit enter checkpoint
    /// </summary>
    /// <param name="unit">Unit that entered in checkpoint</param>
    [Server]
    private void CheckOwnershipForOwnerUnit(UnitController unit)
    {
        CombatUnit combatUnit = unit.GetComponent<CombatUnit>();

        if (GetCheckpointOpen(m_playerOwner) &&
            m_stockUnits.transform.childCount < m_maxStockNumber &&
            combatUnit.GetUnitType() != CombatUnit.UnitType.GiantGolem &&
            combatUnit.GetUnitType() != CombatUnit.UnitType.MediumGolem &&
            CombatUnit.State.NotInFight == combatUnit.GetCurrentState() &&
            unit.GetCheckpointState() != UnitController.EUnitCheckpointState.bannedFromStock)
        {
            AddToStockUnits(unit);
            return;
        }
        else
        {
            unit.transform.SetParent(m_friendsUnits.transform);
        }
    }

    /// <summary>
    /// Add unit to garrison, set unit checkpointstate and add UI button. Call PopulateStockUnits at the end
    /// </summary>
    /// <param name="unit"></param>
    [Server]
    protected void AddToStockUnits(UnitController unit)
    {
        unit.transform.SetParent(m_stockUnits.transform);
        unit.SetObjective(m_stockUnits.transform.position);
        unit.SetCheckpointState(UnitController.EUnitCheckpointState.inCheckpointBarrack);
        ChangeNbUnitsStocked(true, unit.gameObject);
        PopulateStockUnits();
    }

    /// <summary>
    /// If garrison isnt full, check friendsUnits and add more units to garrison
    /// </summary>
    [Server]
    public void PopulateStockUnits()
    {
        if (!GetCheckpointOpen(m_playerOwner) || m_friendsUnits.transform.childCount <= 0 ||
             m_stockUnits.transform.childCount >= m_maxStockNumber)
        {
            return;
        }

        GameObject child = m_friendsUnits.transform.GetChild(0).gameObject;
        UnitController childController = child.GetComponent<UnitController>();
        CombatUnit combatUnit = child.GetComponent<CombatUnit>();
        if (combatUnit.GetUnitType() != CombatUnit.UnitType.GiantGolem &&
            combatUnit.GetUnitType() != CombatUnit.UnitType.MediumGolem &&
            UnitController.EUnitCheckpointState.lookingForStock == childController.GetCheckpointState())
        {
            AddToStockUnits(childController);
        }
    }

    /// <summary>
    /// Increment or decrement the nbUnitStocked, setup UI for owner and call OnChangeNbUnitsStocked
    /// </summary>    
    [Server]
    public virtual void ChangeNbUnitsStocked(bool add, GameObject unit)
    {
        if (m_playerOwner == PlayerEntity.Player.Bot)
        {
            return;
        }
        if (add)
        {
            TargetAddUnitToUI(GameManager.Instance.GetPlayer(m_playerOwner).connectionToClient, unit, m_stockUnits.transform.childCount);
        }

        else
        {
            TargetRemoveUnitToUI(GameManager.Instance.GetPlayer(m_playerOwner).connectionToClient, unit, m_stockUnits.transform.childCount);
        }
    }

    [TargetRpc]
    private void TargetRemoveUnitToUI(NetworkConnection connectionToClient, GameObject unit, int nbStockUnit)
    {
        m_releaseUnitUI.RemoveUnitToStock(unit, nbStockUnit);
    }

    [TargetRpc]
    private void TargetAddUnitToUI(NetworkConnection target, GameObject unit, int nbStockUnit)
    {
        m_releaseUnitUI.AddUnitToStock(unit, nbStockUnit);
    }

    /// <summary>
    /// Release unit from stockUnits, set unit checkpointstate to bannedFromStock.
    /// </summary>
    /// <param name="unit"></param>
    [Command]
    public void CmdReleaseUnit(GameObject unit)
    {
        UnitController unitController = unit.GetComponent<UnitController>();
        unitController.SetCheckpointState(UnitController.EUnitCheckpointState.bannedFromStock);
        unit.SetActive(true);
        unitController.RpcSetActive(true);
        unit.transform.SetParent(m_friendsUnits.transform);
        unitController.SetObjective(Vector3.zero);
        ChangeNbUnitsStocked(false, unit);
        unitController.RpcDecrementTerritotyUnitNumberOnRelease(); //needed to cancel territory OnTriggerEnter, when unit is enable.

        if (m_enemiesUnits.transform.childCount == 0)
        {
            PopulateStockUnits();
        }
    }

    [Server]
    protected virtual void SetCheckpointNeutral()
    {
        NetworkConnection conn = GameManager.Instance.GetPlayer(m_playerOwner).connectionToClient;
        if (null == conn)
        {
            return;
        }

        if (m_stockUnits.transform.childCount == 0 && m_friendsUnits.transform.childCount == 0)
        {
            TargetDeselectCheckpoint(conn);
            m_playerOwner = PlayerEntity.Player.Neutre;
            RpcCheckpointZoneTransition(m_playerOwner);
            RpcSetMaterial(m_playerOwner);
        }
    }

    [TargetRpc]
    private void TargetDeselectCheckpoint(NetworkConnection target)
    {
        PlayerEntity localPlayer = GameManager.Instance.GetLocalPlayerEntity();
        if (localPlayer.GetSelectedItem() == GetComponentInChildren<ObjectSelection>())
        {
            localPlayer.DeselectLastItem();
        }
    }

    [TargetRpc]
    public virtual void TargetPlaySoundCapture(NetworkConnection target) { }

    /// <summary>
    /// Rpcs the set material.
    /// </summary>
    /// <param name="playerNumber">Player number of the new owner.</param>
    [ClientRpc]
    private void RpcSetMaterial(PlayerEntity.Player playerNumber)
    {
        SetMaterial(playerNumber);
    }

    [ClientRpc]
    private void RpcCheckpointZoneTransition(PlayerEntity.Player checkpointOwner)
    {
        CheckpointZoneTransition(checkpointOwner);
    }

    [ClientRpc]
    private void RpcChangeIconOn2DView()
    {
        GetComponentInChildren<UICheckpointIcon>().SelectIconToDisplay();
    }

    /// <summary>
    /// Set material of this checkpoint.
    /// </summary>
    /// <param name="playerNumber">Player number of the new owner.</param>
    public void SetMaterial(PlayerEntity.Player playerNumber)
    {
        if (!m_sycaMaterial || !m_arcaMaterial)
        {
            return;
        }

        Renderer rend = GetComponent<Renderer>();

        switch (playerNumber)
        {
            case PlayerEntity.Player.Player1:
                rend.material = m_sycaMaterial;
                break;
            case PlayerEntity.Player.Bot:
            case PlayerEntity.Player.Player2:
                rend.material = m_arcaMaterial;
                break;
            case PlayerEntity.Player.Neutre:
                if (!GetComponent<Barrack>())
                {
                    rend.material = m_neutralMaterial;
                }
                break;
        }

        if (!m_territory.GetIsAvailable())
        {
            rend.material.color = new Color(0.3f, 0.3f, 0.3f);
        }
    }

    private void CheckpointZoneTransition(PlayerEntity.Player checkpointOwner)
    {
        switch (checkpointOwner)
        {
            case PlayerEntity.Player.Player1:
                m_sycaCheckpointParticles.GetComponent<ParticleSystem>().Play();
                StartCoroutine(ChangeCheckpointZone(m_sycaCheckpointZone));
                break;
            case PlayerEntity.Player.Bot:
            case PlayerEntity.Player.Player2:
                m_arcaCheckpointParticles.GetComponent<ParticleSystem>().Play();
                StartCoroutine(ChangeCheckpointZone(m_arcaCheckpointZone));
                break;
            case PlayerEntity.Player.Neutre:
                StartCoroutine(ChangeCheckpointZone(null));
                break;
        }
    }

    private IEnumerator ChangeCheckpointZone(GameObject targetCheckpointZone)
    {
        float startTime = Time.time;
        float alpha = 0f;
        if (null != targetCheckpointZone)
        {
            targetCheckpointZone.GetComponent<MeshRenderer>().enabled = true;
        }

        while (alpha <= 1)
        {
            alpha += Time.fixedDeltaTime;

            if (null != m_currentCheckpointzone)
            {
                var currentColor = m_currentCheckpointzone.GetComponent<MeshRenderer>().material.color;
                currentColor.a = Mathf.Clamp(1f - alpha, 0f, 1f);
                m_currentCheckpointzone.GetComponent<MeshRenderer>().material.color = currentColor;
                if (currentColor.a == 0)
                {
                    m_currentCheckpointzone.GetComponent<MeshRenderer>().enabled = false;
                }
            }

            if (null != targetCheckpointZone)
            {
                var targetColor = targetCheckpointZone.GetComponent<MeshRenderer>().material.color;
                targetColor.a = alpha;
                targetCheckpointZone.GetComponent<MeshRenderer>().material.color = targetColor;
            }
            yield return new WaitForFixedUpdate();
        }
        m_currentCheckpointzone = targetCheckpointZone;
    }

    [Server]
    protected virtual void SetCheckpointAuthority(PlayerEntity.Player unitOwner)
    {
        SetPlayerOwner(unitOwner);
        RpcSetPlayerOwner(unitOwner); //Need even with syncvar
        GameManager.Instance.GetPlayer(m_playerOwner).TakeControl(GetComponent<NetworkIdentity>());
    }

    /// <summary>
    /// Make all stocks units and friends units attack the enemyUnit who enter the checkpoint
    /// </summary>
    /// <param name="enemyUnit">Enemy unit to attack</param>
    [Server]
    public void AttackInRangeOfCheckpoint(CombatUnit enemyUnit)
    {
        foreach (Transform child in m_stockUnits.transform)
        {
            UnitController unit = child.GetComponent<UnitController>();
            unit.SetCheckpointState(UnitController.EUnitCheckpointState.fightingForCheckpoint);
            unit.gameObject.SetActive(true);
            unit.RpcSetActive(true);
            unit.GetComponent<CombatUnit>().UnitEnterAggroRange(enemyUnit);
            enemyUnit.UnitEnterAggroRange(unit.GetComponent<CombatUnit>());
        }

        foreach (Transform child in m_friendsUnits.transform)
        {
            child.GetComponent<CombatUnit>().UnitEnterAggroRange(enemyUnit);
            enemyUnit.UnitEnterAggroRange(child.GetComponent<CombatUnit>());
        }
    }

    [TargetRpc]
    private void TargetSoundAttacked(NetworkConnection target)
    {
        m_alertManager.TimerVerificationAttackedPI();
    }

    [ClientRpc]
    public virtual void RpcAllPlaySoundCapture() { }
    #endregion

    #region Accessors

    public bool GetOnFight()
    {
        return m_onFight;
    }

    public UIReleaseUnit GetUIReleaseUnit()
    {
        return m_releaseUnitUI;
    }

    public PlayerEntity.Player GetPlayerOwner()
    {
        return m_playerOwner;
    }

    public virtual void SetPlayerOwner(PlayerEntity.Player player)
    {
        m_playerOwner = player;
    }

    [ClientRpc]
    public void RpcSetPlayerOwner(PlayerEntity.Player player)
    {
        SetPlayerOwner(player);
    }

    public bool GetCheckpointOpen(PlayerEntity.Player player)
    {
        if (player == PlayerEntity.Player.Neutre)
        {
            return false;
        }
        else
        {
            return m_checkpointOpenPlayer[player];
        }
    }

    public int GetNbUnitsStocked()
    {
        return m_stockUnits.transform.childCount;
    }

    public GameObject GetUnitOnStock(int index)
    {
        if (m_stockUnits.transform.childCount < index)
        {
            return null;
        }
        return m_stockUnits.transform.GetChild(index).gameObject;
    }

    public int GetNbMaxUnitsStocked()
    {
        return m_maxStockNumber;
    }

    public bool GetTerritoryCheckpointAvailable()
    {
        return m_territory.GetIsAvailable();
    }
    #endregion

    /// <summary>
    /// Checks if all required conditions are filled, otherwise crash
    /// </summary>
    protected virtual void CheckIfOk()
    {
#if UNITY_EDITOR
        if (null == m_stockUnits)
        {
            Debug.LogError("Stock Units n'est pas set sur " + name, this);
            UnityEditor.EditorApplication.isPlaying = false;
        }

        if (null == m_area)
        {
            Debug.LogError("Area n'est pas set sur " + name, this);
            UnityEditor.EditorApplication.isPlaying = false;
        }

        if (null == m_friendsUnits)
        {
            Debug.LogError("Friends Units n'est pas set sur " + name, this);
            UnityEditor.EditorApplication.isPlaying = false;
        }

        if (null == m_enemiesUnits)
        {
            Debug.LogError("Enemies Units n'est pas set sur " + name, this);
            UnityEditor.EditorApplication.isPlaying = false;
        }

        if (null == m_sycaMaterial)
        {
            Debug.LogError("Syca Material n'est pas set sur " + name, this);
            UnityEditor.EditorApplication.isPlaying = false;
        }

        if (null == m_arcaMaterial)
        {
            Debug.LogError("Arca material n'est pas set sur " + name, this);
            UnityEditor.EditorApplication.isPlaying = false;
        }

        if (null == m_territory)
        {
            Debug.LogError(name + " NOT IN A TERRITORY", this);
            UnityEditor.EditorApplication.isPlaying = false;
        }
        if (null == m_soundEmitter)
        {
            Debug.LogError("Sound emitter n'est pas set sur " + name, this);
            UnityEditor.EditorApplication.isPlaying = false;
        }
#endif
    }
}
