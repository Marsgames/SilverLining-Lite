using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.Networking;

namespace Prototype.NetworkLobby
{
    //Main menu, mainly only a bunch of callback called by the UI (setup throught the Inspector)
    public class LobbyMainMenu : MonoBehaviour
    {
        public LobbyManager lobbyManager;

        public RectTransform lobbyServerList;
        public RectTransform lobbyPanel;

        public InputField ipInput;
        public InputField matchNameInput;

        private SoundManager m_soundManager;

        private void Start()
        {
            m_soundManager = FindObjectOfType<SoundManager>();
            CheckIfOk();
        }

        public void OnEnable()
        {
            Time.timeScale = 1;

            lobbyManager.GetComponent<Image>().enabled = true;
            lobbyManager.topPanel.ToggleVisibility(true);
            ipInput.onEndEdit.RemoveAllListeners();
            ipInput.onEndEdit.AddListener(onEndEditIP);

            matchNameInput.onEndEdit.RemoveAllListeners();
            matchNameInput.onEndEdit.AddListener(onEndEditGameName);

        }

        public void OnClickHost()
        {
            lobbyManager.StartHost();
            m_soundManager.PlaySound(SoundManager.AudioClipList.AC_clickBtnMenu);
        }

        public void OnClickJoin()
        {
            lobbyManager.ChangeTo(lobbyPanel);

            lobbyManager.networkAddress = ipInput.text;
            lobbyManager.StartClient();

            lobbyManager.backDelegate = lobbyManager.StopClientClbk;
            lobbyManager.DisplayIsConnecting();

            lobbyManager.SetServerInfo("Connecting...", lobbyManager.networkAddress);
            m_soundManager.PlaySound(SoundManager.AudioClipList.AC_clickBtnMenu);
        }

        public void OnClickDedicated()
        {
            lobbyManager.ChangeTo(null);
            lobbyManager.StartServer();

            lobbyManager.backDelegate = lobbyManager.StopServerClbk;

            lobbyManager.SetServerInfo("Dedicated Server", lobbyManager.networkAddress);
            m_soundManager.PlaySound(SoundManager.AudioClipList.AC_clickBtnMenu);
        }

        public void OnClickCreateMatchmakingGame()
        {
            lobbyManager.StartMatchMaker();
            lobbyManager.matchMaker.CreateMatch(
                matchNameInput.text != "" ? matchNameInput.text : "Server " + SystemInfo.deviceName,
                (uint)lobbyManager.maxPlayers,
                true,
                "", "", "", 0, 0,
                lobbyManager.OnMatchCreate);

            lobbyManager.backDelegate = lobbyManager.StopHost;
            lobbyManager._isMatchmaking = true;
            lobbyManager.DisplayIsConnecting();

            lobbyManager.SetServerInfo("Matchmaker Host", lobbyManager.matchHost);
            m_soundManager.PlaySound(SoundManager.AudioClipList.AC_clickBtnMenu);
        }

        public void OnClickOpenServerList()
        {
            lobbyManager.StartMatchMaker();
            lobbyManager.backDelegate = lobbyManager.SimpleBackClbk;
            lobbyManager.ChangeTo(lobbyServerList);
            m_soundManager.PlaySound(SoundManager.AudioClipList.AC_clickBtnMenu);
        }

        void onEndEditIP(string text)
        {
            if (Input.GetKeyDown(KeyCode.Return))
            {
                OnClickJoin();
            }
        }

        void onEndEditGameName(string text)
        {
            if (Input.GetKeyDown(KeyCode.Return))
            {
                OnClickCreateMatchmakingGame();
            }
        }
        private void CheckIfOk()
        {
#if UNITY_EDITOR
            if (null == m_soundManager)
            {
                Debug.LogError("<color=red>Error: </color>ALLLOOO ALLLOOOO le sound manager ne peut pas ¨ºre null dans " + name, this);
                UnityEditor.EditorApplication.isPlaying = false;
            }
#endif
        }
    }


}
