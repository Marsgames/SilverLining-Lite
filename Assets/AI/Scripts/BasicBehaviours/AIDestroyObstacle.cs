#region Authors
/*****************************
 * Corentin Couderc
 * Dylan Tosti
 * ***************************/
#endregion
using System.Collections.Generic;
using UnityEngine;

public class AIDestroyObstacle : StateMachineBehaviour
{

    #region Variables
    private List<ObstacleConstructor> m_obstacleConstructorList = new List<ObstacleConstructor>();
    private int m_index;
    private PlayerEntity.Player m_playerNumber;
    private ActiveObstacle m_activeObstacle;
    private PlayerEntity m_playerEntity;
    #endregion

    #region main function
    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        m_activeObstacle = m_obstacleConstructorList[m_index].transform.GetComponentInChildren<ActiveObstacle>();
        m_playerNumber = m_activeObstacle.GetPlayerNumber();

        if (m_playerNumber != PlayerEntity.Player.Bot &&
            m_playerEntity.GetComponent<Bomb>().GetBombStack() > 0 &&
            m_obstacleConstructorList[m_index].GetCurrentState() == ObstacleConstructor.EState.Built)
        {
            m_playerEntity.GetComponent<Bomb>().UseBombStack();
            m_playerEntity.CmdBomb(m_activeObstacle.gameObject);
        }

        if (m_playerNumber == PlayerEntity.Player.Bot &&
            m_obstacleConstructorList[m_index].GetCurrentState() == ObstacleConstructor.EState.Built)
        {
            m_activeObstacle.CmdDestroyObstacle();
        }

        animator.SetTrigger(Constant.BotTransition.s_return);
    }
    #endregion

    #region setter
    public void SetIndex(int ind)
    {
        m_index = ind;
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
