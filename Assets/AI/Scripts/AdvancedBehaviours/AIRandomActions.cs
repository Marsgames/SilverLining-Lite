#region Authors
/*****************************
 * Corentin Couderc
 * Dylan Tosti
 * ***************************/
#endregion
using System.Collections.Generic;
using UnityEngine;

public class AIRandomActions : StateMachineBehaviour
{
    #region Variables
    [SerializeField] float m_unitSpawnRatio = 0.6f;
    [SerializeField] float m_obstaclesRatio = 0.1f;
    [SerializeField] float m_checkpointsReleaseAllRatio = 0.1f;
    [SerializeField] float m_checkpointsReleaseRatio = 0.1f;

    private AIBuildObstacle m_aiBuildObstacle;
    private AIDestroyObstacle m_aiDestroyObstacle;
    private AISpawnUnit m_aiSpawnUnit;
    private List<SpawnUnits> m_botSpawnUnits;
    private List<ObstacleConstructor> m_listObstacles;
    private List<ActiveObstacle.ObstacleType> m_listSpawnTypeObstacle = new List<ActiveObstacle.ObstacleType>();
    private List<Constant.SpawnTypeUnit> m_listSpawnTypeUnit = new List<Constant.SpawnTypeUnit>();
    private List<Constant.SpawnTypeUnit> m_unitsQueue = new List<Constant.SpawnTypeUnit>();
    private float m_floatAlea;
    
    private AIReleaseUnits m_aiReleaseUnits;
    private List<CheckpointBase> m_listCheckpoints;
    private int m_indexCheckpoint;
    private int m_nbUnitsReleased;
    #endregion

    #region main functions
    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        m_listSpawnTypeObstacle.Clear();
        m_listSpawnTypeUnit.Clear();
        m_listSpawnTypeUnit.Add(Constant.SpawnTypeUnit.Warrior);
        m_listSpawnTypeUnit.Add(Constant.SpawnTypeUnit.Range);
        m_listSpawnTypeUnit.Add(Constant.SpawnTypeUnit.Tank);
        m_listSpawnTypeUnit.Add(Constant.SpawnTypeUnit.Scout);
        m_listSpawnTypeObstacle.Add(ActiveObstacle.ObstacleType.SimpleWall);
        m_listSpawnTypeObstacle.Add(ActiveObstacle.ObstacleType.Pillar);
        m_listSpawnTypeObstacle.Add(ActiveObstacle.ObstacleType.SlopeRight);
        m_listSpawnTypeObstacle.Add(ActiveObstacle.ObstacleType.SlopeLeft);
        m_listSpawnTypeObstacle.Add(ActiveObstacle.ObstacleType.Trap);

        m_aiBuildObstacle = animator.GetBehaviour<AIBuildObstacle>();
        m_aiDestroyObstacle = animator.GetBehaviour<AIDestroyObstacle>();
        m_aiSpawnUnit = animator.GetBehaviour<AISpawnUnit>();
        m_aiReleaseUnits = animator.GetBehaviour<AIReleaseUnits>();
    }

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        m_floatAlea = Random.Range(0f,100f); //Génère un float aléatoire entre 0 et 99
        if (m_floatAlea < 3f)
        {
            MakeAction(animator);
        }
    }
    #endregion

    #region used function
    public void MakeAction(Animator animator)
    {
        float ratio = Random.Range(0f, 1f);
        if (ratio < m_unitSpawnRatio) // SpawnUnits
        {
            int idSpawnUnit = Random.Range(0, m_botSpawnUnits.Count);
            Constant.SpawnTypeUnit unitType = m_listSpawnTypeUnit[Random.Range(0, m_listSpawnTypeUnit.Count)];
            GameObject prefabTemp = GameManager.Instance.GetUnitForPlayer(unitType);
            if (GameManager.Instance.GetPlayer(PlayerEntity.Player.Bot).GetUnitGold() >= prefabTemp.GetComponent<UnitController>().GetUnitCost())
            {
                m_unitsQueue.Clear();
                m_unitsQueue.Add(unitType);
                m_botSpawnUnits[idSpawnUnit].SetUnitsQueue(m_unitsQueue);
            }

            m_aiSpawnUnit.SetIndexSpawUnits(idSpawnUnit);
            m_aiSpawnUnit.SetListeSpawnUnits(m_botSpawnUnits);
            animator.SetTrigger(Constant.BotTransition.s_spawnUnit);
        }

        else if (m_unitSpawnRatio <= ratio && ratio < m_unitSpawnRatio + m_obstaclesRatio) // Obstacles Build/Destroy
        {
            int index = Random.Range(0, m_listObstacles.Count);

            if (m_listObstacles[index].GetCurrentState() == ObstacleConstructor.EState.Buildable)
            {
                m_aiBuildObstacle.SetIndex(index);
                m_aiBuildObstacle.SetObstacleConstructorList(m_listObstacles);
                int typeBuildIndex = Random.Range(0, m_listSpawnTypeObstacle.Count);
                m_aiBuildObstacle.SetTypeBuild(m_listSpawnTypeObstacle[typeBuildIndex]);
                animator.SetTrigger(Constant.BotTransition.s_buildObstacle);
            }
            
            //if (m_listObstacles[index].GetCurrentState() == ObstacleConstructor.EState.Built)
            //{
            //    m_aiDestroyObstacle.SetIndex(index);
            //    m_aiDestroyObstacle.SetObstacleConstructorList(m_listObstacles);
            //    animator.SetTrigger(Constant.BotTransition.s_destroyObstacle);
            //}
        }

        else
        {
            m_indexCheckpoint = Random.Range(0, m_listCheckpoints.Count);
            if (m_unitSpawnRatio + m_obstaclesRatio <= ratio && 
                ratio < m_unitSpawnRatio + m_obstaclesRatio + m_checkpointsReleaseAllRatio) // Checkpoints ReleaseAll
            {
                m_nbUnitsReleased = m_listCheckpoints[m_indexCheckpoint].GetNbUnitsStocked();
            }

            else if (m_unitSpawnRatio + m_obstaclesRatio + m_checkpointsReleaseAllRatio <= ratio && 
                     ratio < m_unitSpawnRatio + m_obstaclesRatio + m_checkpointsReleaseAllRatio + m_checkpointsReleaseRatio) // Checkpoints Release
            {
                m_nbUnitsReleased = 1;
            }

            if (m_listCheckpoints[m_indexCheckpoint].GetNbUnitsStocked() >= m_nbUnitsReleased && m_nbUnitsReleased != 0)
            {
                m_aiReleaseUnits.SetListCheckpoints(m_listCheckpoints);
                m_aiReleaseUnits.SetIndexCheckpoint(m_indexCheckpoint);
                m_aiReleaseUnits.SetNbUnitsReleased(m_nbUnitsReleased);
                animator.SetTrigger(Constant.BotTransition.s_releaseUnits);
            }
        }
    }
    #endregion

    #region Accessors
    public void SetCheckpointList(List<CheckpointBase> list)
    {
        m_listCheckpoints = list;
    }

    public void SetObstacleConstructorList(List<ObstacleConstructor> list)
    {
        m_listObstacles = list;
    }

    public void SetBotSpawnUnitsList(List<SpawnUnits> list)
    {
        m_botSpawnUnits = list;
    }

    public void SetRatio(float unitSpawnRatio, float obstaclesRatio, float checkpointsReleaseAllRatio, float checkpointsReleaseRatio)
    {
        m_unitSpawnRatio = unitSpawnRatio; m_obstaclesRatio = obstaclesRatio; m_checkpointsReleaseAllRatio = checkpointsReleaseAllRatio; m_checkpointsReleaseRatio = checkpointsReleaseRatio;
    }
    
    #endregion
}
