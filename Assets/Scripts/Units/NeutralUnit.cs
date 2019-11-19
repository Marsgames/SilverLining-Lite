#pragma warning disable CS0618 // type deprecated
#region Author
/////////////////////////////////////////
// Guillaume Quiniou
/////////////////////////////////////////
#endregion
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Networking;
using UnityEngine.UI;

public abstract class NeutralUnit : CombatUnit
{
    #region Variables
    private NeutralCamp m_camp;
    private Transform m_spawnPosition;
    #endregion

    #region Unity's functions
    [ServerCallback]
    private new void Start()
    {
        m_networkAnimator = GetComponent<NetworkAnimator>();
        m_agent = GetComponent<NavMeshAgent>();
        m_rangeAttackInWorldSpace = m_rangeAttack * gameObject.transform.lossyScale.x;
        
        m_currentHealth = m_maxHealth;
        m_lifeBar.transform.parent.GetComponent<Image>().enabled = false;

        CheckRequiered();
    }
    #endregion

    #region Function

    public void AddCurrentHealth(int value)
    {
        m_currentHealth += value;
        if (m_currentHealth > m_maxHealth)
        {
            m_currentHealth = m_maxHealth;
        }
    }

    [Server]
    protected override void NotInFightMode()
    {
        m_networkAnimator.SetTrigger(Constant.ListOfAnimTrigger.s_run);
        m_agent.isStopped = false;
        m_agent.SetDestination(m_spawnPosition.position);
        CheckForIdleAnimation();
        m_myEmitter.StopAttackSound();
    }

    [Server]
    protected override void Attack(CombatUnit enemyUnit)
    {
        if (enemyUnit.GetCurrentState() != State.Dead && Time.time > m_nextAttack)
        {
            m_nextAttack = Time.time + m_attackSpeed;
            enemyUnit.TakeDamage(m_attackDamage, PlayerEntity.Player.Neutre);
        }
    }

    public override void RemoveUnit(CombatUnit unit)
    {
        base.RemoveUnit(unit);
        CleanCamp();
    }

    [Server]
    public override void TakeDamage(int damageTaken, PlayerEntity.Player enemyId)
    {
        m_currentHealth -= damageTaken;

        if (m_currentHealth <= 0)
        {
            m_camp.DecrementGolemsNumber(enemyId);
            StopAllCoroutines();
            CleanEnemyListOnDeath();
            CleanCamp();
            NetworkServer.Destroy(gameObject);
        }
    }

    private void CleanCamp()
    {
        m_camp.CleanNullInEnemyList();
    }

    public void CheckForIdleAnimation()
    {
        StartCoroutine(IdleAnimation());
    }

    private IEnumerator IdleAnimation()
    {       
        while (m_agent.remainingDistance > m_agent.stoppingDistance)
        {
            if (!m_agent.hasPath || m_agent.velocity.sqrMagnitude == 0f)
            {
                break;
            }
            yield return null;
        }
    
        m_networkAnimator.SetTrigger(Constant.ListOfAnimTrigger.s_idle);
    }

    #endregion

    #region Accessors

    public void SetCamp(NeutralCamp camp)
    {
        m_camp = camp;
    }
    public NeutralCamp GetCamp()
    {
        return m_camp;
    }

    public void SetSpawnPosition(Transform position)
    {
        m_spawnPosition = position;
    }

    public Vector3 GetSpawnPosition()
    {
        return m_spawnPosition.position;
    }

    public NavMeshAgent GetAgent()
    {
        return m_agent;
    }
    #endregion

    void CheckRequiered()
    {
#if UNITY_EDITOR
        if (null == m_camp)
        {
            Debug.LogError("m_camp cannot be null", this);
            UnityEditor.EditorApplication.isPlaying = false;
        }

        if (null == m_spawnPosition)
        {
            Debug.LogError("m_spawnPosition cannot be null", this);
            UnityEditor.EditorApplication.isPlaying = false;
        }

        if (null == m_agent)
        {
            Debug.LogError("m_agent cannot be null", this);
            UnityEditor.EditorApplication.isPlaying = false;
        }
#endif
    }

}
