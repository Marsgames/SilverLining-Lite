#pragma warning disable CS0618 // type deprecated
#region Author
/////////////////////////////////////////
//   Yannig Smagghe 
//   https://gitlab.com/YannigSmagghe
/////////////////////////////////////////
#endregion
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Networking;
using UnityEngine.UI;

public class UnitController : NetworkBehaviour
{
    #region Variable
    [SerializeField] private float m_moveSpeedAgent = 3;
    [SerializeField] private int m_unitCost = 10;
    [SerializeField] private int m_damageToNexus = 1;
    [SerializeField] private GameObject m_aggroRangeCollider = null;
    [SerializeField] private PlayerEntity.Player m_playerNumber;
    private CheckpointBase m_CheckpointBase;
    private Vector3 m_nexusEnemy;
    private Vector3 m_objective;
    private bool m_leaveChekpoint;
    private NavMeshAgent m_agent;
    private NavMeshPath m_parentPath;
    private EUnitCheckpointState m_checkpointState = EUnitCheckpointState.lookingForStock;
    private List<Territory> m_territories = new List<Territory>();
    private Vector3[] m_corners;
    private Territory m_currentTerritory;
    private bool m_isInSlope;

    public enum EUnitCheckpointState
    {
        lookingForStock,
        bannedFromStock,
        fightingForCheckpoint,
        inCheckpointBarrack
    };
    #endregion

    #region Unity's function
    private void Start()
    {
        m_agent = GetComponent<NavMeshAgent>();

        if (!isServer)
        {
            m_agent.enabled = false;
            GetComponent<CombatUnit>().enabled = false;
        }
        else
        {
            m_agent.speed = m_moveSpeedAgent;
            m_agent.autoTraverseOffMeshLink = false;
            StartCoroutine(HandleOffMeshLink());
        }
    }

    [ServerCallback]
    private void OnEnable()
    {
        if (null != m_agent)
        {
            m_agent.autoTraverseOffMeshLink = false;
            StartCoroutine(HandleOffMeshLink());
        }
    }
    #endregion

    #region Functions
    [Server]
    private IEnumerator HandleOffMeshLink()
    {
        while (true)
        {
            if (m_agent.isOnOffMeshLink)
            {
                OffMeshLinkData data = m_agent.currentOffMeshLinkData;
                OffMeshLink link = data.offMeshLink;
                link.costOverride += 1;

                Collider[] hitColliders = Physics.OverlapSphere(transform.position, 5);
                foreach (Collider col in hitColliders)
                {
                    UnitController unit = col.GetComponent<UnitController>();
                    if (null != unit && !unit.GetAgent().isOnOffMeshLink &&
                        link.Equals(unit.GetAgent().nextOffMeshLinkData.offMeshLink))
                    {
                        unit.RecalculatePathUnit();
                    }
                }

                while (m_agent.transform.position != data.endPos)
                {
                    m_agent.transform.position = Vector3.MoveTowards(m_agent.transform.position, data.endPos, m_agent.speed * Time.deltaTime);
                    yield return null;
                }

                if (m_agent.isOnNavMesh)
                {
                    m_agent.CompleteOffMeshLink();
                }

                link.costOverride -= 1;
            }
            yield return null;
        }
    }

    [Server]
    public void SetAgentActive(bool active)
    {
        if (null == m_agent)
        {
            return;
        }
        if (active)
        {
            m_agent.enabled = active;
            m_agent.isStopped = !active;
            Image m_lifeBar = transform.Find(Constant.ListOfUI.s_canvas).transform.Find("Background").transform.Find("Health").GetComponent<Image>();
            if (GetComponent<CombatUnit>().GetCurrentHealth() < GetComponent<CombatUnit>().GetMaxHealth())
            {
                m_lifeBar.enabled = true;
                m_lifeBar.transform.parent.GetComponent<Image>().enabled = true;
            }
        }
        else
        {
            m_agent.isStopped = !active;
            m_agent.enabled = active;
            Image m_lifeBar = transform.Find(Constant.ListOfUI.s_canvas).transform.Find("Background").transform.Find("Health").GetComponent<Image>();
            m_lifeBar.enabled = false;
            m_lifeBar.transform.parent.GetComponent<Image>().enabled = false;
        }
    }

    [Server]
    public void RecalculatePathUnit()
    {
        NavMeshPath newPath = new NavMeshPath();

        GetObjective();
        m_agent.CalculatePath(m_objective, newPath);
        if (!m_agent.isOnOffMeshLink && newPath.status == NavMeshPathStatus.PathComplete)
        {
            m_agent.ResetPath();
            m_agent.SetPath(newPath);
        }
    }

    [Server]
    public bool TestIfRecalculatePathUnit(ObstacleConstructor obstacleConstructor)
    {
        Territory obstacleTerritory = obstacleConstructor.GetTerritory();

        if (m_objective != m_nexusEnemy)
        {
            return false;
        }
        if (IsUnitPastTerritory(obstacleTerritory))
        {
            return false;
        }
        return true;
    }

    [Server]
    private bool IsUnitPastTerritory(Territory obstacleTerritory)
    {
        string obstacleTerritoryName = obstacleTerritory.transform.parent.name;
        string unitTerritoryName = "";

        if (m_currentTerritory != null && m_currentTerritory.transform != null && m_currentTerritory.transform.parent != null)
        {
            unitTerritoryName = m_currentTerritory.transform.parent.name;
        }

        if (obstacleTerritoryName == Constant.ListOfTerritories.s_territoryBaseP1)
        {
            switch (m_playerNumber)
            {
                case PlayerEntity.Player.Player1:
                    if (unitTerritoryName != Constant.ListOfTerritories.s_territoryBaseP1)
                    {
                        return true;
                    }
                    return false;
                case PlayerEntity.Player.Player2:
                case PlayerEntity.Player.Bot:
                    return false;
                default:
                    return false;
            }
        }
        else if (obstacleTerritoryName == Constant.ListOfTerritories.s_territoryCenter ||
          obstacleTerritoryName == Constant.ListOfTerritories.s_territorySide1 ||
          obstacleTerritoryName == Constant.ListOfTerritories.s_territorySide2)
        {
            switch (m_playerNumber)
            {
                case PlayerEntity.Player.Player1:
                    if (unitTerritoryName == Constant.ListOfTerritories.s_territoryBaseP2)
                    {
                        return true;
                    }
                    return false;
                case PlayerEntity.Player.Player2:
                case PlayerEntity.Player.Bot:
                    if (unitTerritoryName == Constant.ListOfTerritories.s_territoryBaseP1)
                    {
                        return true;
                    }
                    return false;
                default:
                    return false;
            }
        }
        else if (obstacleTerritoryName == Constant.ListOfTerritories.s_territoryBaseP2)
        {
            switch (m_playerNumber)
            {
                case PlayerEntity.Player.Player1:
                    return false;
                case PlayerEntity.Player.Player2:
                case PlayerEntity.Player.Bot:
                    if (unitTerritoryName != Constant.ListOfTerritories.s_territoryBaseP2)
                    {
                        return true;
                    }
                    return false;
                default:
                    return false;
            }
        }

        return false;
    }

    private void OnDestroy()
    {
        if (null == this || null == m_territories)
        {
            return;
        }

        foreach (Territory terter in m_territories.ToList())
        {
            if (null == terter)
            {
                continue;
            }

            terter.OnUnitDeath(this);
        }
    }

    [ClientRpc]
    public void RpcDecrementTerritotyUnitNumberOnRelease()
    {
        foreach (Territory territory in m_territories)
        {
            territory.IncrementUnitNumber(this, -1);
        }
    }

    /// <summary>
    /// Send a trail from this unit
    /// </summary>
    public void SendTrail()
    {
        if (null == m_corners)
        {
            return;
        }

        GameObject go = Instantiate(GameManager.Instance.GetTrailPrefab(), transform.position, transform.rotation);
        go.name = "trail-" + name;
        Vector3[] cornersTrail = m_corners;
        for (int i = 0; i < cornersTrail.Length; i++)
        {
            cornersTrail[i] += Vector3.up * 120f;
        }
        go.GetComponent<TrailController>().SetWaypoints(cornersTrail);
    }
    #endregion Functions

    #region Accessors

    [ClientRpc]
    public void RpcSetActive(bool active)
    {
        gameObject.SetActive(active);
    }

    [ClientRpc]
    public void RpcSetRenderer(bool active)
    {
        GetComponent<Renderer>().enabled = active;
    }

    [ClientRpc]
    public void RpcSetPlayerNumber(PlayerEntity.Player playerIndex)
    {
        m_playerNumber = playerIndex;
    }
    public PlayerEntity.Player GetPlayerNumber()
    {
        return m_playerNumber;
    }
    public void SetPlayerNumber(PlayerEntity.Player playerNumber)
    {
        m_playerNumber = playerNumber;
    }

    public NavMeshAgent GetAgent()
    {
        return m_agent;
    }

    public int GetUnitCost()
    {
        return m_unitCost;
    }

    public bool GetLeaveChekpoint()
    {
        return m_leaveChekpoint;
    }
    public void SetLeaveCheckpoint(bool value)
    {
        m_leaveChekpoint = value;
    }

    public int GetDamageToNexus()
    {
        return m_damageToNexus;
    }

    public Vector3 GetObjective()
    {
        return m_objective;
    }

    /// <summary>
    /// Sets the objective.
    /// </summary>
    /// <param name="objective">Vector3.zero to reset objective.</param>
    public void SetObjective(Vector3 objective)
    {
        if (objective == Vector3.zero)
        {
            m_objective = m_nexusEnemy;
        }
        else
        {
            m_objective = objective;
        }

        if (null == m_agent)
        {
            m_agent = GetComponent<NavMeshAgent>();
        }
        if (!m_agent.isActiveAndEnabled)
        {
            return;
        }

        m_agent.SetDestination(m_objective);
    }

    public NavMeshPath GetParentPath()
    {
        return m_parentPath;
    }
    public void SetParentPath(NavMeshPath parentPath)
    {
        m_parentPath = parentPath;
    }

    public EUnitCheckpointState GetCheckpointState()
    {
        return m_checkpointState;
    }
    public void SetCheckpointState(EUnitCheckpointState state)
    {
        m_checkpointState = state;
    }

    public void SetNexusEnemy(Vector3 nexusEnemy)
    {
        m_nexusEnemy = nexusEnemy;
    }

    public void SetCheckpointBase(CheckpointBase ckArea)
    {
        m_CheckpointBase = ckArea;
    }

    public CheckpointBase GetCheckpointBase()
    {
        return m_CheckpointBase;
    }

    public void AddToTerritories(Territory territory)
    {
        if (m_territories.Contains(territory))
        {
            return;
        }

        m_territories.Add(territory);
    }
    public void RemoveFromTerritories(Territory territory)
    {
        if (!m_territories.Contains(territory))
        {
            return;
        }

        m_territories.Remove(territory);
    }

    public GameObject GetAggroRangeCollider()
    {
        return m_aggroRangeCollider;
    }

    public Vector3[] GetCorners()
    {
        return m_corners;
    }
    public void SetCorners(Vector3[] corners)
    {
        m_corners = corners;
    }

    public List<Territory> GetTerritories()
    {
        return m_territories;
    }

    public void SetCurrentTerritory(Territory territory)
    {
        m_currentTerritory = territory;
    }

    public bool GetIsInSlope()
    {
        return m_isInSlope;
    }
    public void SetIsInSlope(bool value)
    {
        m_isInSlope = value;
    }
    #endregion

}