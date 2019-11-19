//#region Authors
///*****************************
// * Corentin Couderc
// * Dylan Tosti
// * ***************************/
using System.Collections.Generic;
using UnityEngine;

public class AIGroupOfUnits : StateMachineBehaviour
{
//    #region Variables
//    private AIFindClosestBuild m_aiFindClosestBuild;

//    private List<UnitController> m_playerUnits = new List<UnitController>();
//    private float m_rayon = 4f;
//    private int m_indexUnit;
//    private int m_maxNbUnits;

    //private ActiveObstacle.ObstacleType m_obstacleToBuild;
    //#endregion

//    private Constant.SpawnType m_obstacleToBuild;
//    #endregion

//    #region main functions
//    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
//    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
//    {
//        m_aiFindClosestBuild = animator.GetBehaviour<AIFindClosestBuild>();
//    }

        //m_playerUnits.Clear();
        //foreach(UnitController unit in m_unitControllers)
        //{
        //    if (unit.GetPlayerNumber() == PlayerEntity.Player.Player1)
        //    {
        //        m_playerUnits.Add(unit);
        //    }
        //}

        //m_obstacleToBuild = ActiveObstacle.ObstacleType.SimpleWall;

//        //m_obstacleToBuild = Constant.SpawnType.SimpleWall;

//        if (m_playerUnits.Count > 0)
//        {
//            m_maxNbUnits = 0;
//            for (int i = 0; i < m_playerUnits.Count; i++)
//            {
//                int nbUnits = 0;
//                for (int j = 0; j < m_playerUnits.Count; j++)
//                {
//                    Vector2 point1 = new Vector2(m_playerUnits[i].transform.position.x, m_playerUnits[i].transform.position.z);
//                    Vector2 point2 = new Vector2(m_playerUnits[j].transform.position.x, m_playerUnits[j].transform.position.z);
//                    float distance = Vector2.Distance(point1,point2);
//                    if (m_playerUnits[i].transform.position.y == m_playerUnits[j].transform.position.y && distance < m_rayon)
//                    {
//                        nbUnits += 1; 
//                    }
//                }
//                if (m_maxNbUnits < nbUnits)
//                {
//                    m_indexUnit = i;
//                    m_maxNbUnits = nbUnits;
//                }
//            }
//            //m_aiFindClosestBuild.SetUnitController(m_playerUnits[m_indexUnit]);
//            //m_aiFindClosestBuild.SetBuildType(m_obstacleToBuild);
//            //animator.SetTrigger(Constant.BotTransition.s_findClosestBuild);
//        }
//    }
//    #endregion

//    #region getter
//    public int GetNbMaxUnits()
//    {
//        return m_maxNbUnits;
//    }

//    public UnitController GetUnitController()
//    {
//        return m_playerUnits[m_indexUnit];
//    }
//    #endregion
}