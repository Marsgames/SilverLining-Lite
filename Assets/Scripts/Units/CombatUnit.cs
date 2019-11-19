#pragma warning disable CS0618 // type deprecated
#region Author
/////////////////////////////////////////
//   Yannig Smagghe --> #CombatUnit#
//   https://gitlab.com/YannigSmagghe
/////////////////////////////////////////
//   Guillaume Quiniou 
/////////////////////////////////////////
#endregion
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Networking;
using UnityEngine.UI;

public abstract class CombatUnit : NetworkBehaviour
{
    #region Variables
    public enum State
    {
        NotInFight,
        Aggro,
        Fighting,
        Dead
    }

    public enum UnitType
    {
        Warrior,
        Range,
        Tank,
        Scout,
        SmallGolem,
        MediumGolem,
        GiantGolem
    }

    [Header("Stats")]
    [SerializeField] protected int m_maxHealth = 100;
    [SerializeField] protected float m_rangeAttack = 0;
    [SerializeField] private float m_rangeAggro = 0;
    [SerializeField] protected float m_attackSpeed = 0;
    [SerializeField] protected int m_attackDamage = 0;
    [SerializeField] protected Image m_lifeBar = null;
    [SerializeField] protected UnitEmitterBehaviour m_myEmitter = null;

    [SyncVar(hook = "OnChangeHealth")] protected int m_currentHealth;

    private State m_currentState = State.NotInFight;
    private List<CombatUnit> m_enemyInRange = new List<CombatUnit>();
    protected NavMeshAgent m_agent;
    protected float m_rangeAttackInWorldSpace;
    protected float m_nextAttack = 0;

    protected UnitController m_unitController;
    protected NetworkAnimator m_networkAnimator;
    #endregion

    #region Unity's functions
    protected void Start()
    {
        if (isServer)
        {
            m_agent = GetComponent<NavMeshAgent>();
            m_unitController = GetComponent<UnitController>();
            m_networkAnimator = GetComponent<NetworkAnimator>();
            SetRangeInWorldSpace(m_rangeAttack);
        }
        m_currentHealth = m_maxHealth;
        m_lifeBar.transform.parent.GetComponent<Image>().enabled = false;
    }
    #endregion

    #region Functions
    public abstract UnitType GetUnitType();

    [Server]
    protected void CleanNullInEnemyList()
    {
        if (m_enemyInRange.Exists(x => (x.Equals(null) || !x.isActiveAndEnabled || x.GetCurrentState() == CombatUnit.State.Dead)))
        {
            m_enemyInRange.RemoveAll(x => (x.Equals(null) || !x.isActiveAndEnabled || x.GetCurrentState() == CombatUnit.State.Dead));
        }
    }

    [Server]
    protected void CleanEnemyListOnDeath()
    {
        CleanNullInEnemyList();
        if (m_enemyInRange.Count <= 0)
        {
            return;
        }
        foreach (CombatUnit target in m_enemyInRange)
        {
            if (null == target || !target.m_enemyInRange.Contains(this))
            {
                return;
            }

            target.RemoveUnit(this);
            target.CheckCurrentState();
            UnitController unitControllerGo = target.GetComponent<UnitController>();
            if (unitControllerGo && unitControllerGo.GetCheckpointBase())
            {
                unitControllerGo.GetCheckpointBase().CheckAndSetOwnership(target.GetComponent<UnitController>());
            }
        }
    }

    [Server]
    private void CheckCurrentState()
    {
        CleanNullInEnemyList();
        if (m_currentState == State.Dead)
        {
            return;
        }
        if (m_enemyInRange.Count == 0)
        {
            SetCurrentState(State.NotInFight);
            RpcStopAttackSound();

        }
        else if (m_currentState == State.NotInFight)
        {
            SetCurrentState(State.Aggro);
        }
    }

    [Server]
    protected void OnCurrentStateChange()
    {
        StopAllCoroutines();
        CleanNullInEnemyList();
        switch (GetCurrentState())
        {
            case State.NotInFight:
                NotInFightMode();
                break;
            case State.Aggro:
                StartCoroutine(AggroMode());
                break;
            case State.Fighting:
                StartCoroutine(FightingMode());
                break;
            case State.Dead:
                StartCoroutine(DeathMode());
                break;
        }
    }

    [Server]
    protected virtual IEnumerator DeathMode()
    {
        transform.SetParent(null);
        DecrementGlobalUnit();
        CleanEnemyListOnDeath();

        m_networkAnimator.SetTrigger(Constant.ListOfAnimTrigger.s_death);
        m_unitController.SetAgentActive(false);

        float start = 0f;
        float end = 5.0f;

        yield return new WaitForSecondsRealtime(3f);

        while (start < end)
        {
            transform.position += new Vector3(0,-1* 0.035f,0);
            start += 0.5f;
            yield return null;
        }
        NetworkServer.Destroy(gameObject);
    }

    [Server]
    private void DecrementGlobalUnit()
    {
        GameManager.Instance.GetPlayer(m_unitController.GetPlayerNumber()).DecrementNbUnitInGame();

        if (UnitController.EUnitCheckpointState.inCheckpointBarrack == m_unitController.GetCheckpointState() ||
            UnitController.EUnitCheckpointState.fightingForCheckpoint == m_unitController.GetCheckpointState())
        {
            m_unitController.GetCheckpointBase().ChangeNbUnitsStocked(false, gameObject);
        }
    }

    [Server]
    protected IEnumerator FightingMode()
    {
        m_networkAnimator.SetTrigger(Constant.ListOfAnimTrigger.s_attack);
        RpcPlaySmoke();
        RpcPlayAttackSound();
        m_nextAttack = Time.time + m_attackSpeed;

        while (true)
        {
            CleanNullInEnemyList();
            if (m_currentHealth <= 0)
            {
                break;
            }
            m_agent.isStopped = true;
            if (!m_enemyInRange.Exists(x => x.Equals(null)) && m_enemyInRange.Count > 0)
            {
                transform.LookAt(m_enemyInRange.First().transform.position);
                Attack(m_enemyInRange.First());
            }
            else if (m_enemyInRange.Count == 0)
            {
                SetCurrentState(State.NotInFight);
            }
            yield return null;
        }
    }

    [Server]
    protected IEnumerator AggroMode()
    {
        RpcStopAttackSound();
        while (true)
        {
            CleanNullInEnemyList();
            if (!m_enemyInRange.Exists(x => x.Equals(null)) && m_enemyInRange.Count > 0)
            {
                if (Vector3.Distance(m_enemyInRange.First().transform.position, transform.position) <= m_rangeAttackInWorldSpace)
                {
                    SetCurrentState(State.Fighting);
                }
                else
                {
                    if (!m_agent.isActiveAndEnabled)
                    {
                        m_unitController.SetAgentActive(true);
                    }
                    m_agent.isStopped = false;
                    m_agent.SetDestination(m_enemyInRange.First().transform.position);
                }
            }
            else if (m_enemyInRange.Count == 0)
            {
                SetCurrentState(State.NotInFight);
            }
            yield return null;
        }
    }

    [Server]
    protected virtual void NotInFightMode()
    {
        m_networkAnimator.SetTrigger(Constant.ListOfAnimTrigger.s_run);
        m_agent.isStopped = false;
        m_agent.SetDestination(m_unitController.GetObjective());
        RpcStopSmoke();
    }

    [Server]
    public void UnitEnterAggroRange(CombatUnit enemyUnit)
    {
        if (!m_enemyInRange.Contains(enemyUnit))
        {
            m_enemyInRange.Add(enemyUnit);
        }
        CheckCurrentState();
    }

    [Server]
    public virtual void RemoveUnit(CombatUnit unit)
    {
        m_enemyInRange.Remove(unit);
        if (!m_enemyInRange.Exists(x => x.Equals(null)))
        {
            m_enemyInRange.Sort((x, y) => Vector3.Distance(x.transform.position, transform.position).CompareTo(Vector3.Distance(y.transform.position, transform.position)));
        }
        CheckCurrentState();
    }

    [Server]
    public void EnemyUnitExitAggroRange(CombatUnit unit)
    {
        RemoveUnit(unit);
        CheckCurrentState();
    }

    [Server]
    protected virtual void Attack(CombatUnit enemyUnit)
    {
        if (enemyUnit.GetCurrentState() != State.Dead && Time.time > m_nextAttack)
        {
            m_nextAttack = Time.time + m_attackSpeed;
            enemyUnit.TakeDamage(m_attackDamage, m_unitController.GetPlayerNumber());
        }
    }

    /// <summary>
    /// Inflicts damages to this unit.
    /// </summary>
    /// <param name="damageTaken">Amount of damages to inflict.</param>
    /// <param name="enemyId">Player that will deal damage.</param>
    [Server]
    public virtual void TakeDamage(int damageTaken, PlayerEntity.Player enemyId)
    {
        m_currentHealth -= damageTaken;

        if (m_currentHealth <= 0)
        {
            SetCurrentState(State.Dead);
        }
    }

    private void OnChangeHealth(int health)
    {
        m_currentHealth = health;

        if (m_currentHealth < m_maxHealth && !m_lifeBar.enabled)
        {
            m_lifeBar.enabled = true;
            m_lifeBar.transform.parent.GetComponent<Image>().enabled = true;
        }

        if (m_currentHealth >= m_maxHealth)
        {
            m_lifeBar.transform.parent.GetComponent<Image>().enabled = false;
            m_lifeBar.enabled = false;
        }

        if (m_currentHealth <= 0)
        {
            m_lifeBar.transform.parent.GetComponent<Image>().enabled = false;
            m_lifeBar.enabled = false;
        }

        m_lifeBar.fillAmount = (float)m_currentHealth / (float)m_maxHealth;
    }

    [ClientRpc]
    public void RpcPlaySmoke()
    {
        if (GetComponentInChildren<ParticleSystem>() != null)
        {
            GetComponentInChildren<ParticleSystem>().Play();
        }
    }

    [ClientRpc]
    public void RpcStopSmoke()
    {
        if (GetComponentInChildren<ParticleSystem>() != null)
        {
            GetComponentInChildren<ParticleSystem>().Stop();
        }
    }

    [ClientRpc]
    private void RpcPlayAttackSound()
    {
        m_myEmitter.PlayAttackSound();
    }

    [ClientRpc]
    private void RpcStopAttackSound()
    {
        m_myEmitter.StopAttackSound();
    }
    #endregion

    #region Accessors
    public State GetCurrentState()
    {
        return m_currentState;

    }
    public void SetCurrentState(State value)
    {
        m_currentState = value;
        OnCurrentStateChange();
    }

    public float GetRangeAggro()
    {
        return m_rangeAggro;
    }
    public float GetAttackSpeed()
    {
        return m_attackSpeed;
    }
    public int GetCurrentHealth()
    {
        return m_currentHealth;
    }

    public int GetMaxHealth()
    {
        return m_maxHealth;
    }

    public float GetAttackDamage()
    {
        return m_attackDamage;
    }

    private void SetRangeInWorldSpace(float mRangeAttack)
    {
        m_rangeAttackInWorldSpace = mRangeAttack * gameObject.transform.lossyScale.x;
    }
    #endregion
}