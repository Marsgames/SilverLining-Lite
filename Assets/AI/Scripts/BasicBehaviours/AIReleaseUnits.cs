#region Authors
/*****************************
 * Corentin Couderc
 * Dylan Tosti
 * ***************************/
#endregion
using System.Collections.Generic;
using UnityEngine;

public class AIReleaseUnits : StateMachineBehaviour
{
    #region Variables
    private List<CheckpointBase> m_listCheckpoint;
    private int m_indexCheckpoint;
    private int m_nbUnitsReleased;
    private int m_currentNbUnitsReleased;

    #endregion

    #region main function

    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        m_currentNbUnitsReleased = 0;
    }

    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (PlayerEntity.Player.Bot == m_listCheckpoint[m_indexCheckpoint].GetPlayerOwner() && m_listCheckpoint[m_indexCheckpoint].GetNbUnitsStocked() > 0)
        {
            m_listCheckpoint[m_indexCheckpoint].CmdReleaseUnit(m_listCheckpoint[m_indexCheckpoint].GetUnitOnStock(0));
            m_currentNbUnitsReleased++;
        }
        else
        {
            animator.SetTrigger(Constant.BotTransition.s_return);
        }
    }
    #endregion

    #region setters
    public void SetListCheckpoints(List<CheckpointBase> liste)
    {
        m_listCheckpoint = liste;
    }

    public void SetIndexCheckpoint(int ind)
    {
        m_indexCheckpoint = ind;
    }

    public void SetNbUnitsReleased(int nb)
    {
        m_nbUnitsReleased = nb;
    }
    #endregion
}
