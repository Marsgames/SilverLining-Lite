#region Authors
/*****************************
 * Corentin Couderc
 * Dylan Tosti
 * ***************************/
#endregion
using System.Collections.Generic;
using UnityEngine;

public class AIMirrorPlayer : StateMachineBehaviour
{
    #region Variables
    private List<SpawnUnits> m_botSpawnUnits;
    private List<SpawnUnits> m_playerSpawnUnits;
    private List<Constant.SpawnTypeUnit> m_unitsQueue;
    private int m_indexActiveSpawnUnit;
    private AISpawnUnit m_aiSpawnUnit;
    #endregion


    #region main functions
    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        m_aiSpawnUnit = animator.GetBehaviour<AISpawnUnit>();
    }

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        for (int i = 0; i < m_botSpawnUnits.Count; i++)
        {
            if (m_playerSpawnUnits[i].IsSpawning()) // unitsQueue player pas vide
            {
                if (!m_botSpawnUnits[i].IsSpawning()) // unitsQueue bot vide
                {
                    m_unitsQueue = m_playerSpawnUnits[i].GetUnitsQueue();
                    m_botSpawnUnits[i].SetUnitsQueue(m_unitsQueue);
                }

                m_aiSpawnUnit.SetIndexSpawUnits(i);
                m_aiSpawnUnit.SetListeSpawnUnits(m_botSpawnUnits);
                animator.SetTrigger(Constant.BotTransition.s_spawnUnit);
            }            
        }
    }
    #endregion

    #region Accessors
    public void SetBotSpawnUnitsList(List<SpawnUnits> list)
    {
        m_botSpawnUnits = list;
    }

    public void SetPlayerSpawnUnitsList(List<SpawnUnits> list)
    {
        m_playerSpawnUnits = list;
    }
    #endregion
}
