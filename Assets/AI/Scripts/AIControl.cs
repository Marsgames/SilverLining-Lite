#region Authors
/*****************************
 * Corentin Couderc
 * Dylan Tosti
 * ***************************/
#endregion
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class AIControl : PlayerEntity
{
    #region Variables
    private BotMode m_botMode;
    [SerializeField] private BotMode m_defaultMode = BotMode.Strat;

    [SerializeField] private GameObject m_tank = null;
    [SerializeField] private GameObject m_range = null;
    [SerializeField] private GameObject m_scout = null;
    [SerializeField] private GameObject m_warrior = null;

    [SerializeField] private GameObject m_simpleWall = null;
    [SerializeField] private GameObject m_pillar = null;
    [SerializeField] private GameObject m_slopeRight = null;
    [SerializeField] private GameObject m_slopeLeft = null;
    [SerializeField] private GameObject m_trap = null;

    private Animator m_animator;

    private AISpawnUnit m_aiSpawnUnit;
    private AIBuildObstacle m_aiBuildObstacles;
    private AIDestroyObstacle m_aiDestroyObstacles;
    private AIFindClosestBuild m_aiFindClosestBuild;
    private AIRandomActions m_aiRandomActions;
    private AIMirrorPlayer m_aiMirrorPlayer;
    private AICreateTeam m_aiCreateTeam;
    private AIFindCkOnPath m_aiFindCkOnPath;
    private AIIdle m_aiIdle;
    private AIStrat m_aiStrat;

    private List<CheckpointBase> m_listCheckpoints = new List<CheckpointBase>();
    private List<ObstacleConstructor> m_listObstacles = new List<ObstacleConstructor>();
    private List<SpawnUnits> m_botSpawnUnits = new List<SpawnUnits>();
    private List<SpawnUnits> m_playerSpawnUnits = new List<SpawnUnits>();
    private List<CheckpointCollider> m_listCkAreaCollider = new List<CheckpointCollider>();
    private List<int> m_pricesBuildList = new List<int>();
    private List<ActiveObstacle.ObstacleType> m_listeTypesBuild = new List<ActiveObstacle.ObstacleType>();
    private List<int> m_pricesList = new List<int>();
    private List<Constant.SpawnTypeUnit> m_listeTypeUnits = new List<Constant.SpawnTypeUnit>();
    
    public enum BotMode
    {
        NoAction,
        RandomActions,
        Strat
    }
    #endregion

    #region main functions
    // Start is called before the first frame update

    public new IEnumerator Start()
    {
        m_uiGlobal = UIGlobal.Instance;
        InstantiateBot();
        InitBotBehaviours();
        InitBotLists();
        InitTypeAndPrices();
        yield return null;
    }

    // Update is called once per frame
    void Update()
    {
        InitSpawnUnitsList();
        if (m_botSpawnUnits.Count > 0)
        {
            switch (m_botMode)
            {
                case BotMode.NoAction:
                    break;
                case BotMode.RandomActions:
                    InitSpawnUnitsList();
                    m_animator.SetBool(Constant.BotTransition.s_onlyRandomActions, true);
                    break;
                case BotMode.Strat:
                    m_animator.SetBool(Constant.BotTransition.s_strat, true);
                    break;
            }
        }

    }
    #endregion

    #region Functions
    private void InstantiateBot()
    {
        m_playerNumber = Player.Bot;

        m_uiGlobal.InitEnemyPlayerUI(this);
        m_healthPoint = m_maxHealthPoint;
        StartCoroutine(GenerateUnitGoldEverySecond());
        m_currentBuildGold = m_maxBuildGold;
        StartCoroutine(GenerateBuildGoldEverySecond());
        m_playerNumber = Player.Bot;
        SetBotActive(true);
    }

    private void InitBotBehaviours()
    {
        m_animator = GetComponent<Animator>();

        m_aiSpawnUnit = m_animator.GetBehaviour<AISpawnUnit>();
        m_aiSpawnUnit.SetPlayerEntity(this);

        m_aiBuildObstacles = m_animator.GetBehaviour<AIBuildObstacle>();
        m_aiBuildObstacles.SetPlayerEntity(this);

        m_aiDestroyObstacles = m_animator.GetBehaviour<AIDestroyObstacle>();
        m_aiDestroyObstacles.SetPlayerEntity(this);

        m_aiFindClosestBuild = m_animator.GetBehaviour<AIFindClosestBuild>();

        m_aiRandomActions = m_animator.GetBehaviour<AIRandomActions>();
       
        m_aiMirrorPlayer = m_animator.GetBehaviour<AIMirrorPlayer>();

        m_aiCreateTeam = m_animator.GetBehaviour<AICreateTeam>();

        m_aiFindCkOnPath = m_animator.GetBehaviour<AIFindCkOnPath>();
        m_aiIdle = m_animator.GetBehaviour<AIIdle>();

        m_aiStrat = m_animator.GetBehaviour<AIStrat>();
    }

    private void InitBotLists()
    {
        CreateAndSortListCheckpoints();
        m_aiRandomActions.SetCheckpointList(m_listCheckpoints);

        m_listObstacles = FindObjectsOfType<ObstacleConstructor>().ToList();
        m_listObstacles.Sort((p1, p2) => p1.transform.position.x.CompareTo(p2.transform.position.x));

        m_aiFindClosestBuild.SetObstacleConstructorList(m_listObstacles);
        m_aiRandomActions.SetObstacleConstructorList(m_listObstacles);
        m_aiFindCkOnPath.SetObstacleConstructorList(m_listObstacles);
        m_aiBuildObstacles.SetObstacleConstructorList(m_listObstacles);
        m_aiDestroyObstacles.SetObstacleConstructorList(m_listObstacles);
        m_aiStrat.SetObstacleConstructorList(m_listObstacles);
    }

    private void InitSpawnUnitsList()
    {
        m_botSpawnUnits.Clear();
        m_playerSpawnUnits.Clear();
        SpawnUnits[] allspawnUnits = FindObjectsOfType<SpawnUnits>();
        foreach (SpawnUnits spawnUnit in allspawnUnits)
        {
            if (spawnUnit.GetPlayerNumber() == Player.Bot)
            {
                m_botSpawnUnits.Add(spawnUnit);
            }
            else if (spawnUnit.GetPlayerNumber() == Player.Player1)
            {
                m_playerSpawnUnits.Add(spawnUnit);
            }
        }
        m_aiRandomActions.SetBotSpawnUnitsList(m_botSpawnUnits);
        m_aiCreateTeam.SetBotSpawnUnitsList(m_botSpawnUnits);
        m_aiMirrorPlayer.SetBotSpawnUnitsList(m_botSpawnUnits);
        m_aiMirrorPlayer.SetPlayerSpawnUnitsList(m_playerSpawnUnits);
        m_aiStrat.SetBotSpawnUnitsList(m_botSpawnUnits);
    }

    private void InitTypeAndPrices()
    {
        m_listeTypeUnits.Add(Constant.SpawnTypeUnit.Tank);
        m_listeTypeUnits.Add(Constant.SpawnTypeUnit.Range);
        m_listeTypeUnits.Add(Constant.SpawnTypeUnit.Scout);
        m_listeTypeUnits.Add(Constant.SpawnTypeUnit.Warrior);

        m_aiStrat.SetListTypesUnits(m_listeTypeUnits);
        m_aiSpawnUnit.SetTypesList(m_listeTypeUnits);

        m_pricesList.Add(m_tank.GetComponent<UnitController>().GetUnitCost());
        m_pricesList.Add(m_range.GetComponent<UnitController>().GetUnitCost());
        m_pricesList.Add(m_scout.GetComponent<UnitController>().GetUnitCost());
        m_pricesList.Add(m_warrior.GetComponent<UnitController>().GetUnitCost());

        m_aiSpawnUnit.SetPricesList(m_pricesList);
        m_aiStrat.SetPricesList(m_pricesList);

        m_listeTypesBuild.Add(ActiveObstacle.ObstacleType.SimpleWall);
        m_listeTypesBuild.Add(ActiveObstacle.ObstacleType.Pillar);
        m_listeTypesBuild.Add(ActiveObstacle.ObstacleType.SlopeRight);
        m_listeTypesBuild.Add(ActiveObstacle.ObstacleType.SlopeLeft);
        m_listeTypesBuild.Add(ActiveObstacle.ObstacleType.Trap);

        m_aiStrat.SetBuildTypesList(m_listeTypesBuild);

        m_pricesBuildList.Add(m_simpleWall.GetComponent<ActiveWall>().GetBuildingCost());
        m_pricesBuildList.Add(m_pillar.GetComponent<ActivePillar>().GetBuildingCost());
        m_pricesBuildList.Add(m_slopeRight.GetComponent<ActiveSlope>().GetBuildingCost());
        m_pricesBuildList.Add(m_slopeLeft.GetComponent<ActiveSlope>().GetBuildingCost());
        m_pricesBuildList.Add(m_trap.GetComponent<ActiveTrap>().GetBuildingCost());

        m_aiStrat.SetPricesBuildList(m_pricesBuildList);
    }

    private void CreateAndSortListCheckpoints()
    {
        m_listCheckpoints = FindObjectsOfType<CheckpointBase>().ToList();
        m_listCheckpoints.Sort((p1, p2) => p1.transform.position.x.CompareTo(p2.transform.position.x));
        m_aiStrat.SetCheckpointsList(m_listCheckpoints);
    }

    public void SetBotActive(bool isBotActive)
    {
        m_botMode = BotMode.NoAction;
        if (isBotActive)
        {
            m_botMode = m_defaultMode;
        }
    }

    public void SetBotMode(BotMode botmode)
    {
        m_botMode = botmode;
    }
    #endregion

}
