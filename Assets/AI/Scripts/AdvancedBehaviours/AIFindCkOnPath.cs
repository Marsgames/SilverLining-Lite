#region Authors
/*****************************
 * Corentin Couderc
 * Dylan Tosti
 * ***************************/
#endregion
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;


public class AIFindCkOnPath : StateMachineBehaviour
{
    #region Variables
    private AIBuildObstacle m_aiBuildObstacles;
    private SpawnUnits m_spawnunit;
    private SpawnUnits[] m_spawnunits;
    private CheckpointCollider[] m_checkpoints;
    private BoxCollider m_ckAreaCollider;
    private BoxCollider m_colliderOnAreaCollider;
    private NavMeshPath m_newPath;
    private List<CheckpointCollider> m_listCkAreaCollider = new List<CheckpointCollider>();
    private bool m_isCollider = false;
    private List<ObstacleConstructor> m_listObstacleConstructors;
    private CheckpointBase[] m_listCheckpointsArea;
    private GameObject m_virtualWall;
    private GameObject m_virtualArea;
    private int m_ind;
    #endregion

    #region main function

    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        m_aiBuildObstacles = animator.GetBehaviour<AIBuildObstacle>();
        m_isCollider = false;
        //m_listObstacleConstructors[12].BuildObstacle(Constant.SpawnType.SimpleWall);

        m_ind = 0;
        m_newPath = new NavMeshPath();

        //get area collider

        foreach (CheckpointCollider ck in m_checkpoints)
        {
            if (ck.GetColliderType() == CheckpointCollider.ECollider.areaCollider)
            {
                m_ckAreaCollider = ck.GetComponent<BoxCollider>();
            }
        }
        m_virtualArea = new GameObject();

        m_colliderOnAreaCollider = m_virtualArea.AddComponent<BoxCollider>();
        m_colliderOnAreaCollider.transform.position = m_ckAreaCollider.transform.position;
        m_colliderOnAreaCollider.size = m_ckAreaCollider.gameObject.transform.localScale + m_ckAreaCollider.size;
        m_colliderOnAreaCollider.size += new Vector3(0f, 50f, 0f);
        m_colliderOnAreaCollider.isTrigger = true;


    }

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        ObstacleConstructor currentObstacle = m_listObstacleConstructors[m_ind];

        if (currentObstacle.GetCurrentState() == ObstacleConstructor.EState.Buildable)
        {
            NavMeshHit hit;
            NavMesh.SamplePosition(m_spawnunit.GetTarget().transform.position, out hit, Vector3.Distance(m_spawnunit.transform.position, m_spawnunit.GetTarget().transform.position) + 100, NavMesh.AllAreas);
            NavMeshAgent tempAgent = m_spawnunit.gameObject.GetComponent<NavMeshAgent>();
            if (tempAgent == null)
                if (tempAgent == null)
                {
                    tempAgent = m_spawnunit.gameObject.AddComponent<NavMeshAgent>();
                }
            tempAgent.CalculatePath(hit.position, m_newPath);

            //get intersection navmesh vs ckcollider
            Vector3[] pathCorners = m_newPath.corners;
            RaycastHit colliderHit;
            //m_isCollider = (m_ind == m_listObstacleConstructors.Length - 1);

            for (int i = 0; i < pathCorners.Length - 1; i++)
            {

                Vector2 line = new Vector2(pathCorners[i + 1].x - pathCorners[i].x, pathCorners[i + 1].z - pathCorners[i].z);
                Vector2 lineDir = line.normalized;
                Ray lineRay = new Ray(pathCorners[i], lineDir);
                
                Debug.DrawLine(pathCorners[i], pathCorners[i + 1], Color.green, 1000, false);
                if (m_colliderOnAreaCollider.Raycast(lineRay, out colliderHit, Vector3.Distance(pathCorners[i], pathCorners[i + 1])))
                {                    
                    if (m_ind == 0)
                    {
                        animator.SetTrigger(Constant.BotTransition.s_return);
                    }
                    else
                    {
                        m_isCollider = true;
                        break;
                    }
                }
            }

            if (null != m_virtualWall)
            {
                Destroy(m_virtualWall);
            }

            if (m_isCollider)
            {
                Destroy(m_virtualArea);
                // Prepare pour Build Obstacles

                m_aiBuildObstacles.SetTypeBuild(ActiveObstacle.ObstacleType.SimpleWall);
                m_aiBuildObstacles.SetIndex(m_ind - 2);
                m_aiBuildObstacles.SetObstacleConstructorList(m_listObstacleConstructors);

                animator.SetTrigger(Constant.BotTransition.s_buildObstacle);
            }

            // construct virtual wall to calculate new path
            m_virtualWall = new GameObject();
            m_virtualWall.AddComponent<NavMeshObstacle>();
            m_virtualWall.GetComponent<NavMeshObstacle>().size = new Vector3(3, 3, 3);
            m_virtualWall.transform.position = currentObstacle.transform.position;
            m_virtualWall.GetComponent<NavMeshObstacle>().carving = true;
        }
        if (m_ind < m_listObstacleConstructors.Count - 1)
        {
            m_ind += 1;
        }
    }
    #endregion

    #region setters

    public void SetCkAreaCollider(CheckpointCollider[] ckAreaCollider)
    {
        m_checkpoints = ckAreaCollider;
    }

    public void SetSpawnUnit(SpawnUnits spawnUnit)
    {
        m_spawnunit = spawnUnit;
    }

    public void SetObstacleConstructorList(List<ObstacleConstructor> list)
    {
        m_listObstacleConstructors = list;
    }
    #endregion
}
