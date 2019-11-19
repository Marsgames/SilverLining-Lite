#region Author
/////////////////////////////////////////
//   Guillaume Quiniou 
/////////////////////////////////////////
#endregion
using Prototype.NetworkLobby;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class UIBackToStartGame : MonoBehaviour
{
    #region Functions
    public void TaskOnClick()
    {
        StartCoroutine(LoadStartGameScene());
    }

    private IEnumerator LoadStartGameScene()
    {
        AsyncOperation async = SceneManager.LoadSceneAsync(0);
        yield return new WaitUntil(() => async.isDone);
        LobbyManager.s_Singleton.dontDestroyOnLoad = false;
        LobbyManager.s_Singleton.StopServer();
        Destroy(LobbyManager.s_Singleton.gameObject);
    }
    #endregion
}