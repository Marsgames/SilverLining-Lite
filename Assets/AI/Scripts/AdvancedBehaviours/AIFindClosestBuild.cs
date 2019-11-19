#region Authors
/*****************************
 * Corentin Couderc
 * Dylan Tosti
 * ***************************/
#endregion
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class AIFindClosestBuild : StateMachineBehaviour
{
    #region Variables
    private AIBuildObstacle m_aiBuildObstacles;
    private AIDestroyObstacle m_aiDestroyObstacle;

    private UnitController m_playerUnit;
    private NavMeshPath m_parentPath;
    private Vector3[] m_pathCorners;

    private List<ObstacleConstructor> m_listObstacles;
    private int m_indexObstacle = 0;
    private ActiveObstacle.ObstacleType m_buildType;
    #endregion

    #region Unity's Functions
    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        m_aiBuildObstacles = animator.GetBehaviour<AIBuildObstacle>();
        m_aiDestroyObstacle = animator.GetBehaviour<AIDestroyObstacle>();
    }

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        m_pathCorners = m_playerUnit.GetAgent().path.corners;
        RaycastHit enter;
        float minDistance = Mathf.Infinity;
        for (int i = 0; i < m_pathCorners.Length - 1; i++)
        {
            Vector2 line = new Vector2(m_pathCorners[i + 1].x - m_pathCorners[i].x, m_pathCorners[i + 1].z - m_pathCorners[i].z);
            Vector2 lineDir = line.normalized;
            Ray lineRay = new Ray(m_pathCorners[i], lineDir);
            Debug.DrawLine(m_pathCorners[i], m_pathCorners[i+1], Color.green, 1000, false);
            
            for (int j = 0; j < m_listObstacles.Count; j++)
            {
                ObstacleConstructor obstacle = m_listObstacles[j];
                Vector3 obstaclePos = obstacle.transform.position;
                Vector3 obstacleRot = obstacle.transform.eulerAngles;
                Vector3 obstacleNormal;
                if (obstacleRot.y == 90)
                {
                    obstacleNormal = Vector3.right;
                }
                else
                {
                    obstacleNormal = Vector3.forward;
                }

                GameObject plan = new GameObject();
                plan.AddComponent<BoxCollider>();
                plan.transform.position = obstaclePos;
                plan.GetComponent<BoxCollider>().transform.localScale += new Vector3(5f, 5f, 5f);

                if (plan.GetComponent<BoxCollider>().Raycast(lineRay, out enter, Vector3.Distance(m_pathCorners[i], m_pathCorners[i + 1])))
                {
                    Vector3 hitPoint = enter.point;
                    if (Vector3.Distance(hitPoint, obstaclePos) < 5f)
                    {
                        float distance = Vector3.Distance(hitPoint, m_pathCorners[i]);
                        if ((distance < minDistance) && (distance > Vector3.Distance(m_playerUnit.transform.position, m_pathCorners[i])))
                        {
                            minDistance = distance;
                            m_indexObstacle = j;
                        }
                    }
                }
                Destroy(plan);
            }
        }

        // Prepare pour Build Obstacles


        ActiveObstacle activeObstacle = m_listObstacles[m_indexObstacle].transform.GetComponentInChildren<ActiveObstacle>();
        if (null != activeObstacle)
        {
            PlayerEntity.Player playerNumber = activeObstacle.GetPlayerNumber();
            if (playerNumber == PlayerEntity.Player.Player1)
            {
                m_aiDestroyObstacle.SetIndex(m_indexObstacle);
                animator.SetTrigger(Constant.BotTransition.s_destroyObstacle);
            }
        }
        m_aiBuildObstacles.SetTypeBuild(m_buildType);
        m_aiBuildObstacles.SetIndex(m_indexObstacle);

        animator.SetTrigger(Constant.BotTransition.s_buildObstacle);
    }
    #endregion

    #region Setters
    public void SetUnitController(UnitController unitController)
    {
        m_playerUnit = unitController;
    }

    public void SetBuildType(ActiveObstacle.ObstacleType type)
    {
        m_buildType = type;
    }

    public void SetObstacleConstructorList(List<ObstacleConstructor> list)
    {
        m_listObstacles = list;
    }

    public int GetIndexObstacle()
    {
        return m_indexObstacle;
    }
    #endregion
}
