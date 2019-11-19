#region Authors
/*****************************
 * Corentin Couderc
 * Dylan Tosti
 * ***************************/
#endregion
using System.Collections.Generic;
using UnityEngine;

public class AIReleaseAllUnits : StateMachineBehaviour
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
        // Relache toutes les unites quand rempli au max
        if (m_listCheckpoint[m_indexCheckpoint].GetNbUnitsStocked() == m_listCheckpoint[m_indexCheckpoint].GetNbMaxUnitsStocked())
        {
            animator.SetTrigger(Constant.BotTransition.s_releaseUnits);
        }
        animator.SetTrigger(Constant.BotTransition.s_return);
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


    #endregion
}
