#pragma warning disable CS0618 // type deprecated
#region Author
/////////////////////////////////////////
//   Guillaume Quiniou 
/////////////////////////////////////////
#endregion
using System.Collections;
using System.Collections.Generic;
using Prototype.NetworkLobby;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Networking;
using UnityEngine.UI;

public class GameManager : NetworkBehaviour
{
    #region Variables
    public static GameManager Instance;
    public JsonWritter jsonWritter = new JsonWritter();

    [SerializeField] private GameObject m_AI = null;
    [SerializeField] private GameObject m_endPrefab = null;
    [SerializeField] private Sprite m_endScreenWinArca = null;
    [SerializeField] private Sprite m_endScreenWinSyca = null;
    [SerializeField] private GameObject m_trailPrefab = null;
    [SerializeField] private GameObject[] m_sycaPrefabs = new GameObject[4];
    [SerializeField] private GameObject[] m_arcaPrefabs = new GameObject[4];

    [SyncVar] private bool m_allPlayersReady = false;

    private bool m_allPlayersConnected = false;
    private int m_numberOfPlayersReady = 0;
    private Dictionary<KeyValuePair<PlayerEntity.Player, string>, SpawnUnits> m_spawnUnits = new Dictionary<KeyValuePair<PlayerEntity.Player, string>, SpawnUnits>();
    private Dictionary<PlayerEntity.Player, PlayerEntity> m_players = new Dictionary<PlayerEntity.Player, PlayerEntity>();
    private Dictionary<PlayerEntity.Player, Dictionary<Constant.SpawnTypeUnit, GameObject>> m_prefabUnits;
    private Dictionary<string, Barrack> m_barracks = new Dictionary<string, Barrack>();
    private List<ObstacleConstructor> m_listOfObstacles = new List<ObstacleConstructor>();
    private List<CheckpointBase> m_listOfCheckpoint = new List<CheckpointBase>();
    private int m_territory_cost = 1;
    private NavMeshSurface m_navmeshSurface;
    private UIGlobal m_uiGlobal;
    private bool m_endGameBool = false;
    #endregion

    #region Unity's functions
    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(Instance.gameObject);
        }
        Instance = this;
    }

    private IEnumerator Start()
    {
        LobbyManager.s_Singleton.GetComponent<Image>().enabled = false;
        LobbyManager.s_Singleton.countdownPanel.gameObject.SetActive(false);
        m_prefabUnits = new Dictionary<PlayerEntity.Player, Dictionary<Constant.SpawnTypeUnit, GameObject>>();
        Dictionary<Constant.SpawnTypeUnit, GameObject> dicoTemp = new Dictionary<Constant.SpawnTypeUnit, GameObject>();
        foreach (GameObject prefab in m_sycaPrefabs)
        {
            CombatUnit combatUnitPrefab = prefab.GetComponent<CombatUnit>();
            switch (combatUnitPrefab.GetUnitType())
            {
                case CombatUnit.UnitType.Warrior:
                    dicoTemp.Add(Constant.SpawnTypeUnit.Warrior, prefab);
                    break;
                case CombatUnit.UnitType.Range:
                    dicoTemp.Add(Constant.SpawnTypeUnit.Range, prefab);
                    break;
                case CombatUnit.UnitType.Tank:
                    dicoTemp.Add(Constant.SpawnTypeUnit.Tank, prefab);
                    break;
                case CombatUnit.UnitType.Scout:
                    dicoTemp.Add(Constant.SpawnTypeUnit.Scout, prefab);
                    break;
            }
        }
        m_prefabUnits.Add(PlayerEntity.Player.Player1, dicoTemp);

        dicoTemp = new Dictionary<Constant.SpawnTypeUnit, GameObject>();
        foreach (GameObject prefab in m_arcaPrefabs)
        {
            CombatUnit combatUnitPrefab = prefab.GetComponent<CombatUnit>();
            switch (combatUnitPrefab.GetUnitType())
            {
                case CombatUnit.UnitType.Warrior:
                    dicoTemp.Add(Constant.SpawnTypeUnit.Warrior, prefab);
                    break;
                case CombatUnit.UnitType.Range:
                    dicoTemp.Add(Constant.SpawnTypeUnit.Range, prefab);
                    break;
                case CombatUnit.UnitType.Tank:
                    dicoTemp.Add(Constant.SpawnTypeUnit.Tank, prefab);
                    break;
                case CombatUnit.UnitType.Scout:
                    dicoTemp.Add(Constant.SpawnTypeUnit.Scout, prefab);
                    break;
            }
        }
        m_prefabUnits.Add(PlayerEntity.Player.Player2, dicoTemp);
        m_prefabUnits.Add(PlayerEntity.Player.Bot, dicoTemp);
        m_navmeshSurface = GameObject.Find(Constant.ListOfObject.s_navMeshSurface).GetComponent<NavMeshSurface>();
        if (isServer)
        {
            yield return new WaitUntil(() => LobbyManager.s_Singleton.numberOfPlayerConnected == LobbyManager.s_Singleton.numberOfPlayer);
            m_allPlayersConnected = true;
        }
        m_uiGlobal = UIGlobal.Instance;
    }
    #endregion Unity's functions

    #region Functions
    /// <summary>
    /// Pause the game, stop all coroutines running and show the screen of victory/defeat
    /// </summary>
    /// <param name="mPlayer">The player who lose</param>
    public void EndGame(PlayerEntity.Player mPlayer)
    {
        m_endGameBool = true;
        GameObject endCanvas = Instantiate(m_endPrefab);
        Image victoryScreen = endCanvas.transform.Find(Constant.ListOfUI.s_victoryScreen).GetComponent<Image>();
        Text victoryText = victoryScreen.transform.Find(Constant.ListOfUI.s_victoryText).GetComponent<Text>();
        foreach (MonoBehaviour child in FindObjectsOfType<MonoBehaviour>())
        {
            child.StopAllCoroutines();
        }

        if (mPlayer == GetLocalPlayer())
        {
            // DEFEAT
            victoryText.text = Constant.ListOfText.s_defeat;
            FindObjectOfType<DefeatSound>().PlayDefeatSound();
        }
        else
        {
            // VICTORY
            victoryText.text = Constant.ListOfText.s_victory;
            FindObjectOfType<VictorySound>().PlayVictorySound();



        }
        // PLAYER 1 LOSE
        if (mPlayer == PlayerEntity.Player.Player1)
        {
            victoryScreen.sprite = m_endScreenWinArca;
        }
        // PLAYER 2 LOSE
        if (mPlayer == PlayerEntity.Player.Player2)
        {
            victoryScreen.sprite = m_endScreenWinSyca;
        }

        Time.timeScale = 0;
        m_uiGlobal.SetEndGame();
        jsonWritter.WriteJson();
    }

    /// <summary>
    /// Delete all previewObstacle and hide all UIObstacleConstructor
    /// </summary>
    public void CleanUiObstacles()
    {
        foreach (ObstacleConstructor obstacle in GetListObstacleConstructor())
        {
            obstacle.DeletePreviewObstacle();
            obstacle.GetUISpawnObstacle().SetActive(false);
        }
    }

    /// <summary>
    /// Call on start of ObstacleConstructor, register all of them to the game manager
    /// </summary>
    public void AddObstacleConstructorToList(ObstacleConstructor obstacle)
    {
        m_listOfObstacles.Add(obstacle);
    }

    /// <summary>
    /// Call on start of SpawnUnit, register all of them to the game manager
    /// </summary>
    public void AddSpawnUnitToList(PlayerEntity.Player playerNumber, string tag, SpawnUnits spawnUnit)
    {
        m_spawnUnits.Add(new KeyValuePair<PlayerEntity.Player, string>(playerNumber, tag), spawnUnit);
    }

    /// <summary>
    /// Return SpawnUnit depending on the player and the tag
    /// </summary>
    /// <param name="playerNumber">The player who want his SpawnUnit</param>
    /// <param name="tag">The tag of the SpawnUnit</param>
    public SpawnUnits GetSpawnUnitForPlayer(PlayerEntity.Player playerNumber, string tag)
    {
        KeyValuePair<PlayerEntity.Player, string> key = new KeyValuePair<PlayerEntity.Player, string>(playerNumber, tag);
        if (m_spawnUnits.ContainsKey(key))
        {
            return m_spawnUnits[key];
        }
        return null;
    }

    /// <summary>
    /// Call on start of CheckpointBase, register all of them to the game manager
    /// </summary>
    public void AddCheckpointToList(CheckpointBase checkpoint)
    {
        m_listOfCheckpoint.Add(checkpoint);
        if (checkpoint is Barrack)
        {
            m_barracks.Add(checkpoint.gameObject.tag, (Barrack)checkpoint);
        }
    }

    public Barrack GetBarrackFromTag(string tag)
    {
        if (!m_barracks.ContainsKey(tag))
        {
            return null;
        }
        return m_barracks[tag];
    }

    /// <summary>
    /// Register a PlayerEntity depending on the PlayerNumber, then add to this player all the mine references and if solo player, start the bot
    /// </summary>
    public void RegisterPlayer(PlayerEntity.Player _playerNumber, PlayerEntity _player)
    {
        if (!m_players.ContainsKey(_playerNumber))
        {
            m_players.Add(_playerNumber, _player);

            if (isServer)
            {
                foreach (CheckpointBase ckBase in m_listOfCheckpoint)
                {
                    if (ckBase is Mine)
                    {
                        _player.AddMineToDictionary(ckBase.GetComponent<Mine>());
                    }
                }
                if (LobbyManager.s_Singleton.numPlayers == 1)
                {
                    GameObject instantiateBot = Instantiate(m_AI);
                    PlayerEntity bot = instantiateBot.GetComponent<PlayerEntity>();
                    m_players.Add(PlayerEntity.Player.Bot, bot);
                    foreach (CheckpointBase ckBase in m_listOfCheckpoint)
                    {
                        if (ckBase is Mine)
                        {
                            bot.AddMineToDictionary(ckBase.GetComponent<Mine>());
                        }
                    }
                }
                m_numberOfPlayersReady++;
                if (m_numberOfPlayersReady == LobbyManager.s_Singleton.numberOfPlayer)
                {
                    m_allPlayersReady = true;
                }
            }
        }
    }

    /// <summary>
    /// If a PlayerEntity is register under this playerNumber, remove it
    /// </summary>
    public void UnRegisterPlayer(PlayerEntity.Player _playerNumber)
    {
        if (m_players.ContainsKey(_playerNumber))
        {
            m_players.Remove(_playerNumber);
        }
    }

    /// <summary>
    /// If a PlayerEntity is register under this playerNumber, return it
    /// </summary>
    public PlayerEntity GetPlayer(PlayerEntity.Player playerNumber)
    {
        if (!m_players.ContainsKey(playerNumber))
        {
            Debug.Log("cant find " + playerNumber);
            return null;
        }

        return m_players[playerNumber];
    }

    /// <summary>
    /// Return the PlayerNumber of the local player
    /// </summary>
    public PlayerEntity.Player GetLocalPlayer()
    {
        foreach (var item in m_players)
        {
            if (item.Value.hasAuthority && item.Key != PlayerEntity.Player.Bot)
            {
                return item.Key;
            }
        }
        return PlayerEntity.Player.Neutre;
    }

    /// <summary>
    /// Return the PlayerEntity of the local player
    /// </summary>
    public PlayerEntity GetLocalPlayerEntity()
    {
        foreach (var item in m_players)
        {
            if (item.Value.hasAuthority && item.Key != PlayerEntity.Player.Bot)
            {
                return item.Value;
            }
        }
        return null;
    }

    public void InitializeTerritoryCost()
    {
        PlayerEntity.Player player = GetLocalPlayer();

        if (player == PlayerEntity.Player.Player1)
        {
            m_territory_cost = 1; // cost for syca base
        }
        else if (player == PlayerEntity.Player.Player2)
        {
            m_territory_cost = 34; // cost for arca base
        }
    }

    #endregion Functions

    #region Accessors
    /// <summary>
    /// Returns the trail prefab set in the GameManager in the scene
    /// </summary>
    public GameObject GetTrailPrefab()
    {
        return m_trailPrefab;
    }

    public List<ObstacleConstructor> GetListObstacleConstructor()
    {
        return m_listOfObstacles;
    }

    public List<CheckpointBase> GetListCheckpoint()
    {
        return m_listOfCheckpoint;
    }

    /// <summary>
    /// Returns if all the player that were in the lobby are ready to play
    /// </summary>
    public bool GetAllPlayersReady()
    {
        return m_allPlayersReady;
    }

    /// <summary>
    /// Returns if all the player that were in the lobby are connected to the game
    /// </summary>
    public bool GetAllPlayersConnected()
    {
        return m_allPlayersConnected;
    }


    /// <summary>
    /// Returns the gameobject linked to a SpawnType of a player
    /// </summary>
    /// <returns>The unit for player.</returns>
    /// <param name="type">Unit spawn type</param>
    /// <param name="playerNumber">Player1 for syca, other Player for arca. Do not set any value to use LocalPlayer</param>
    public GameObject GetUnitForPlayer(Constant.SpawnTypeUnit type, PlayerEntity.Player playerNumber = PlayerEntity.Player.Neutre)
    {
        playerNumber = playerNumber == PlayerEntity.Player.Neutre ? GetLocalPlayer() : playerNumber;
        return m_prefabUnits[playerNumber][type];
    }

    /// <summary>
    /// Sets territory cost value.
    /// </summary>
    /// <param name="value">New cost to add.</param>
    /// <returns>True if center material has to change.</returns>
    public bool SetTerritoryCost(int value)
    {
        if (value == 0)
        {
            return false;
        }

        m_territory_cost += value;

        return true;
    }

    public int GetTerritoryCost()
    {
        return m_territory_cost;
    }

    public NavMeshSurface GetNavMeshSurface()
    {
        return m_navmeshSurface;
    }

    public bool GetEndGameBool()
    {
        return m_endGameBool;
    }
    #endregion


}