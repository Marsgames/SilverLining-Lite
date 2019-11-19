#pragma warning disable CS0618 // type deprecated
#region Author
///////////////////////////////////////////////
//   Guillaume Quiniou 
/////////////////////////////////////////
#endregion
using System.Collections;
using System.Collections.Generic;
using Prototype.NetworkLobby;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Networking;

public class PlayerEntity : NetworkBehaviour {

    #region Variables
    public enum Player {
        Player1,
        Player2,
        Neutre,
        Bot
    }

    [SerializeField, Range (1, 100)] protected int m_regularIncomeUnitGold = 1;
    [SerializeField] private int m_timeInSecondGenerateBuildGold = 80;
    [SerializeField, SyncVar (hook = "OnChangeUnitGold")] protected int m_unitGold = 10;
    [SerializeField] protected int m_hardcapBuildGold = 20;
    [SerializeField, SyncVar (hook = "OnChangeMaxBuildGold")] protected int m_maxBuildGold = 20;
    [SerializeField] protected int m_maxHealthPoint = 100;

    [SyncVar (hook = "OnChangeIncomeUnitGold")] private int m_incomeUnitGold = 0;
    [SyncVar (hook = "OnChangeCurrentBuildGold")] protected int m_currentBuildGold;
    [SyncVar (hook = "OnChangedHealthPoint")] protected int m_healthPoint;
    [SyncVar (hook = "OnChangeNbUnitGame")] private int m_nbUnitInGame = 0;
    [SyncVar (hook = "OnChangeReloadBuildGold")] private int m_reloadBuildGold = 0;

    protected Player m_playerNumber = Player.Neutre;
    private Dictionary<Mine, int> m_unitGoldGenerateByMine = new Dictionary<Mine, int> ();
    private string m_playerName;
    private ObjectSelection m_selectedItem = null;
    protected UIGlobal m_uiGlobal;

    public class TakeDamageEvent : UnityEvent<int> { }
    public TakeDamageEvent TakeDamageEvents { get; } = new TakeDamageEvent ();
    #endregion

    #region Unity's functions
    [ServerCallback]
    public IEnumerator Start () {
        m_healthPoint = m_maxHealthPoint;
        m_currentBuildGold = m_maxBuildGold;
        m_incomeUnitGold = m_regularIncomeUnitGold;

        yield return new WaitUntil (() => GameManager.Instance != null);
        yield return new WaitUntil (() => GameManager.Instance.GetAllPlayersConnected ());

        m_playerNumber = (Player) NetworkServer.connections.IndexOf (GetComponent<NetworkIdentity> ().connectionToClient);

        RpcSetPlayerNumber (m_playerNumber);
        StartCoroutine (GenerateUnitGoldEverySecond ());
        StartCoroutine (GenerateBuildGoldEverySecond ());
        GetComponent<PlayerDataCollector> ().InitAndStartCollect ();
    }

    public override void OnStartClient () {
        m_uiGlobal = UIGlobal.Instance;
    }
    public override void OnStartLocalPlayer () {
        base.OnStartLocalPlayer ();
        CmdPlayerIsConnected ();
    }
    private void OnDisable () {
        GameManager.Instance.UnRegisterPlayer (m_playerNumber);
    }
    #endregion

    #region Functions

    public void DeselectLastItem () {
        if (null == m_selectedItem) {
            return;
        }
        m_selectedItem.Deselect ();
        m_selectedItem = null;
    }

    [Server]
    public void AddMineToDictionary (Mine mine) {
        m_unitGoldGenerateByMine.Add (mine, 0);
    }

    [Server]
    public void ChangeMineIncome (Mine mine, int newIncome) {
        if (!m_unitGoldGenerateByMine.ContainsKey (mine)) {
            return;
        }
        m_unitGoldGenerateByMine[mine] = newIncome;
        UpdateIncome ();
    }

    [Server]
    private void UpdateIncome () {
        m_incomeUnitGold = m_regularIncomeUnitGold;
        foreach (KeyValuePair<Mine, int> tuple in m_unitGoldGenerateByMine) {
            m_incomeUnitGold += tuple.Value;
        }
    }

    [Server]
    public void SubstractUnitGold (int goldAmount) {
        m_unitGold -= goldAmount;
    }

    [Server]
    protected IEnumerator GenerateUnitGoldEverySecond () {
        while (true) {
            yield return new WaitForSecondsRealtime (1f);
            m_unitGold += m_incomeUnitGold;
        }
    }

    [Server]
    protected IEnumerator GenerateBuildGoldEverySecond () {
        while (m_maxBuildGold < m_hardcapBuildGold) {
            yield return new WaitForSecondsRealtime (1f);

            m_reloadBuildGold++;
            if (m_reloadBuildGold >= m_timeInSecondGenerateBuildGold) {
                m_maxBuildGold++;
                m_currentBuildGold++;
                m_reloadBuildGold = 0;
            }
        }
        m_uiGlobal.BuildGoldHardcap ();
    }

    [Server]
    public void TakeDamage (int amount) {
        m_healthPoint -= amount;
        TakeDamageEvents.Invoke (amount);
        if (m_healthPoint <= 0) {
            RpcEndGame (m_playerNumber);
        }
    }

    [ClientRpc]
    private void RpcEndGame (Player m_playerNumber) {
        GameManager.Instance.EndGame (m_playerNumber);
    }

    [Server]
    public void TakeControl (NetworkIdentity networkId) {
        if (null != networkId.clientAuthorityOwner) {
            networkId.RemoveClientAuthority (networkId.clientAuthorityOwner);
        }

        if (m_playerNumber != Player.Bot) {
            networkId.AssignClientAuthority (connectionToClient);
        } else {
            networkId.AssignClientAuthority (GameManager.Instance.GetPlayer (Player.Player1).GetComponent<NetworkIdentity> ().connectionToClient);
        }
    }

    [Server]
    public void GainUnitGold (int amount) {
        m_unitGold += amount;
    }

    [Server]
    public void GainBuildGold (int amount) {
        m_currentBuildGold += amount;
    }

    [Command]
    public void CmdCreateObstacle (GameObject parentGo, ActiveObstacle.ObstacleType spawnType) {
        parentGo.GetComponent<ObstacleConstructor> ().BuildObstacle (spawnType, m_playerNumber);
    }

    [Command]
    private void CmdPlayerIsConnected () {
        LobbyManager.s_Singleton.numberOfPlayerConnected++;
    }

    [Command]
    public void CmdBomb (GameObject activeObstacle) {
        activeObstacle.GetComponent<ActiveObstacle> ().CmdBomdObstacle ();
    }

    [Command]
    public void CmdAskUnitPath (GameObject obj) {
        if (null != obj) {
            UnitController unit = obj.GetComponent<UnitController> ();

            Vector3[] corners = unit.GetAgent ().path.corners;
            if (corners.Length > 1) {
                corners[0] = corners[1];
            }
            NetworkConnection con = GameManager.Instance.GetPlayer (unit.GetPlayerNumber ()).connectionToClient;
            TargetSendUnitPath (con, corners, obj);
        }

    }

    [TargetRpc]
    private void TargetSendUnitPath (NetworkConnection target, Vector3[] corners, GameObject obj) {
        if (null != obj) {
            UnitController unit = obj.GetComponent<UnitController> ();
            unit.SetCorners (corners);
        }
    }

    /// <summary>
    /// If selected item is obstacleConstructor or Checkpoint, show the contextual UI
    /// </summary>
    public void ShowUISelectedItem () {
        if (null == m_selectedItem || GameManager.Instance.GetEndGameBool ()) {
            return;
        }

        if (m_selectedItem.GetComponent<ObstacleConstructor> () && m_selectedItem.GetComponent<ObstacleConstructor> ().GetCurrentState () == ObstacleConstructor.EState.Buildable) {
            m_selectedItem.GetComponent<ObstacleConstructor> ().GetUISpawnObstacle ().SetActive (true);
        } else if (m_selectedItem.GetComponent<CheckpointCollider> ()) {
            m_selectedItem.transform.parent.GetComponent<CheckpointBase> ().GetUIReleaseUnit ().ShowUIRelease ();
        }
    }

    /// <summary>
    /// If selected item is obstacleConstructor or Checkpoint, hide the contextual UI
    /// </summary>
    public void HideUISelectedItem () {
        if (null == m_selectedItem) {
            return;
        }
        if (m_selectedItem.GetComponent<ObstacleConstructor> ()) {
            m_selectedItem.GetComponent<ObstacleConstructor> ().GetUISpawnObstacle ().SetActive (false);
            m_selectedItem.GetComponent<ObstacleConstructor> ().DeletePreviewObstacle ();
        } else if (m_selectedItem.GetComponent<CheckpointCollider> ()) {
            m_selectedItem.transform.parent.GetComponent<CheckpointBase> ().GetUIReleaseUnit ().HideUIRelease ();
        }
    }
    #endregion

    #region HookFunctions
    private void OnChangeCurrentBuildGold (int currentBuildGold) {
        if (isLocalPlayer && m_uiGlobal) {
            m_uiGlobal.SetCurrentBuildGold (currentBuildGold);
        }

        m_currentBuildGold = currentBuildGold;
    }
    private void OnChangeMaxBuildGold (int maxBuildGold) {
        if (isLocalPlayer && m_uiGlobal) {
            m_uiGlobal.SetMaxBuildGold (maxBuildGold);
        }

        m_maxBuildGold = maxBuildGold;
    }
    private void OnChangeReloadBuildGold (int reloadBuildGold) {
        if (isLocalPlayer && m_uiGlobal) {
            m_uiGlobal.SetBuildGoldReload (reloadBuildGold);
        }

        m_reloadBuildGold = reloadBuildGold;
    }

    private void OnChangeNbUnitGame (int nbUnitInGame) {
        if (isLocalPlayer && m_uiGlobal) {
            m_uiGlobal.SetNbUnitInGame (nbUnitInGame);
        }

        m_nbUnitInGame = nbUnitInGame;
    }

    private void OnChangeIncomeUnitGold (int income) {
        if (isLocalPlayer && m_uiGlobal) {
            m_uiGlobal.SetIncomeGold (income);
        }

        m_incomeUnitGold = income;
    }

    private void OnChangeUnitGold (int unitGold) {
        if (isLocalPlayer && m_uiGlobal) {
            m_uiGlobal.SetUnitGold (unitGold);
        }
        m_unitGold = unitGold;
    }

    private void OnChangedHealthPoint (int healthPoint) {
        if (m_uiGlobal) {
            if (isLocalPlayer) {
                m_uiGlobal.SetLocalHealthPoint (healthPoint);
            } else {
                m_uiGlobal.SetEnemyHealthPoint (healthPoint);
            }
        }

        m_healthPoint = healthPoint;
    }
    #endregion

    #region Accessors
    public int GetNbUnitInGame () {
        return m_nbUnitInGame;
    }

    public void IncrementNbUnitInGame () {
        m_nbUnitInGame++;
    }

    public void DecrementNbUnitInGame () {
        if (m_nbUnitInGame > 0) {
            m_nbUnitInGame--;
        }
    }

    public int GetTimeInSecondGenerateBuildGold () {
        return m_timeInSecondGenerateBuildGold;
    }

    public string GetPlayerName () {
        return m_playerName;
    }

    public int GetUnitGold () {
        return m_unitGold;
    }

    public int GetBuildGold () {
        return m_currentBuildGold;
    }

    public int GetMaxBuildGold () {
        return m_maxBuildGold;
    }

    public int GetHealthPoint () {
        return m_healthPoint;
    }

    public int GetMaxHealthPoint () {
        return m_maxHealthPoint;
    }

    public Player GetPlayerNumber () {
        return m_playerNumber;
    }

    public void SetPlayerName (string name) {
        m_playerName = name;
    }

    [ClientRpc]
    private void RpcSetPlayerNumber (Player number) {
        m_playerNumber = number;
        GameManager.Instance.RegisterPlayer (m_playerNumber, this);
        if (isLocalPlayer) {
            m_uiGlobal.InitLocalPlayerUI (this);
        } else {
            m_uiGlobal.InitEnemyPlayerUI (this);
        }
    }

    public ObjectSelection GetSelectedItem () {
        return m_selectedItem;
    }

    public void SetSelectedItem (ObjectSelection selectedItem) {
        if (selectedItem != m_selectedItem) {
            DeselectLastItem ();
            m_selectedItem = selectedItem;
        }
    }

    public int GetIncomeUnitGold () {
        return m_incomeUnitGold;
    }

    public int GetReloadBuildGold () {
        return m_reloadBuildGold;
    }
    #endregion

}