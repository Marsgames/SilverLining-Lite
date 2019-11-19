#pragma warning disable CS0618 // type deprecated
#region Author
/////////////////////////////////////////
//   Guillaume Quiniou
/////////////////////////////////////////
#endregion
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

[RequireComponent(typeof(CapsuleCollider))]
public class Tower : CheckpointBase
{
    #region Variables

    [SerializeField] private Transform m_originLaser = null;
    [SerializeField] private int m_attackDamage = 0;
    [SerializeField] private float m_attackRadius = 0;
    [SerializeField] private float m_attackHeight = 0;
    [SerializeField, Tooltip("Case 0 useless => cant attack if empty")] private float[] m_attackSpeed = null;
    [SerializeField] private Material m_projectorSyca = null;
    [SerializeField] private Material m_projectorArca = null;
    [SerializeField] protected Material m_laserSyca = null;
    [SerializeField] protected Material m_laserArca = null;
    [SerializeField] private GameObject m_projector = null;

    private bool m_crRunning;
    private float m_nextAttack = 0;
    protected LineRenderer m_lineRenderer;
    private List<CombatUnit> m_enemyInRange = new List<CombatUnit>();
    #endregion

    #region Unity's function
    protected override IEnumerator Start()
    {
        yield return base.Start();
        GetComponent<CapsuleCollider>().radius = m_attackRadius;
        GetComponent<CapsuleCollider>().height = m_attackHeight;
        m_lineRenderer = GetComponent<LineRenderer>();
        m_crRunning = false;
        if (m_attackSpeed.Length != m_maxStockNumber + 1)
        {
            Debug.LogError("T'es trop con negro. T'as pas mis le bon nombre d'attackspeed", this);
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#endif
        }

    }

    [ServerCallback]
    private void OnTriggerEnter(Collider other)
    {
        if (GetPlayerOwner() == PlayerEntity.Player.Neutre ||
            !other.CompareTag(Constant.ListOfTag.s_unit) ||
            other.GetComponent<UnitController>().GetPlayerNumber() == GetPlayerOwner())
        {
            return;
        }
        m_enemyInRange.Add(other.GetComponent<CombatUnit>());
        if (!m_crRunning)
        {
            StartCoroutine(Attack());
            m_crRunning = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag(Constant.ListOfTag.s_unit) && m_enemyInRange.Contains(other.GetComponent<CombatUnit>()))
        {

            m_enemyInRange.Remove(other.GetComponent<CombatUnit>());
        }
    }

    #endregion

    #region Functions
    public override void SetPlayerOwner(PlayerEntity.Player player)
    {
        base.SetPlayerOwner(player);
        if (player == PlayerEntity.Player.Player1)
        {
            m_lineRenderer.material = m_laserSyca;
        }
        else
        {
            m_lineRenderer.material = m_laserArca;
        }
    }

    [Server]
    private IEnumerator Attack()
    {
        while (true)
        {
            CleanNullInEnemyList();
            if (m_enemyInRange.Count == 0)
            {
                RpcDisableLineRenderer();
                break;
            }
            if (m_stockUnits.transform.childCount == 0 || m_enemiesUnits.transform.childCount > 0)
            {
                m_soundEmitter.StopSound();
                RpcDisableLineRenderer();
                yield return null;
                continue;
            }

            CombatUnit combatUnitEnnemy = m_enemyInRange[0];
            if (combatUnitEnnemy.GetCurrentHealth() > 0)
            {
                RpcEnableLineRenderer(m_enemyInRange[0].transform.position);
                if (Time.time > m_nextAttack && m_attackSpeed[m_stockUnits.transform.childCount] > 0)
                {
                    m_nextAttack = Time.time + m_attackSpeed[m_stockUnits.transform.childCount];
                    combatUnitEnnemy.TakeDamage(m_attackDamage, GetPlayerOwner());
                }
            }
            yield return null;
        }
        m_crRunning = false;
    }

    [Server]
    protected override void SetCheckpointAuthority(PlayerEntity.Player unitOwner)
    {
        base.SetCheckpointAuthority(unitOwner);
        m_enemyInRange.Clear();
    }

    private void CleanNullInEnemyList()
    {
        if (m_enemyInRange.Exists(x => (
        x.Equals(null) ||
        !x.isActiveAndEnabled ||
        x.GetCurrentState() == CombatUnit.State.Dead)))
        {
            m_enemyInRange.RemoveAll(x => (x.Equals(null) || !x.isActiveAndEnabled || x.GetCurrentState() == CombatUnit.State.Dead));
        }
    }

    [ClientRpc]
    private void RpcDisableLineRenderer()
    {
        m_lineRenderer.enabled = false;
    }

    [ClientRpc]
    private void RpcEnableLineRenderer(Vector3 unitPos)
    {
        m_lineRenderer.enabled = true;
        m_lineRenderer.SetPosition(0, m_originLaser.position);
        m_lineRenderer.SetPosition(1, unitPos);
    }

    [TargetRpc]
    public override void TargetPlaySoundCapture(NetworkConnection target)
    {
        m_alertManager.TimerVerificationCapturePITower();
    }

    public void SetActiveTowerProjector(bool value)
    {
        m_projector.SetActive(value);
    }
    #endregion

    #region Accessors
    public GameObject GetProjector()
    {
        return m_projector;
    }

    public Material GetProjectorMaterialSyca()
    {
        return m_projectorSyca;
    }

    public Material GetProjectorMaterialArca()
    {
        return m_projectorArca;
    }

    #endregion Accessors
}
