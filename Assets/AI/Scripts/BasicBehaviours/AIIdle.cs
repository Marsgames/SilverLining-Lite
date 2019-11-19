using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIIdle : StateMachineBehaviour
{
    #region Variables

    private int m_nbActions = 0;

    #endregion

    #region main function
    //OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        m_nbActions++;
    }
    #endregion

    #region getter

    public int GetNbActions()
    {
        return m_nbActions;
    }
    #endregion
}
