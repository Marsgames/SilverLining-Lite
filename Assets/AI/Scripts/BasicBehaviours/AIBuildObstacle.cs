#region Authors
/*****************************
 * Corentin Couderc
 * Dylan Tosti
 * ***************************/
#endregion
using System.Collections.Generic;
using UnityEngine;

public class AIBuildObstacle : StateMachineBehaviour
{
    #region Variables
    private List<ObstacleConstructor> m_obstacleConstructorList;
    private ActiveObstacle.ObstacleType m_typeBuild;
    private int m_index;
    private PlayerEntity m_playerEntity;
    #endregion

    #region main function
    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (m_obstacleConstructorList[m_index].GetCurrentState() == ObstacleConstructor.EState.Buildable)
        {
            m_playerEntity.CmdCreateObstacle(m_obstacleConstructorList[m_index].gameObject, m_typeBuild);
        }
        animator.SetTrigger(Constant.BotTransition.s_return);
    }

    public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        animator.SetTrigger(Constant.BotTransition.s_return);
    }
    #endregion

    #region setter
    public void SetIndex(int ind)
    {
        m_index = ind;
    }

    public void SetTypeBuild(ActiveObstacle.ObstacleType type)
    {
        m_typeBuild = type;
    }

    public void SetObstacleConstructorList(List<ObstacleConstructor> listeObstacleConstructor)
    {
        m_obstacleConstructorList = listeObstacleConstructor;
    }

    public void SetPlayerEntity(PlayerEntity playerEntity)
    {
        m_playerEntity = playerEntity;
    }
    #endregion
}
