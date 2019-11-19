#pragma warning disable CS0618 // type deprecated
#region Author
/////////////////////////////////////////
//   MARION WARTELLE-MATHIEU 
/////////////////////////////////////////
//   Yannig Smagghe
//   https://gitlab.com/YannigSmagghe
/////////////////////////////////////////
#endregion
using UnityEngine;
using UnityEngine.SceneManagement;

public class UIMainMenu : MonoBehaviour
{
    #region Variables
    private SoundManager m_soundManager;
    #endregion

    #region Functions
    private void Start()
    {
        m_soundManager = SoundManager.Instance;
    }

    public void PlayGame()
    {
        m_soundManager.PlaySound(SoundManager.AudioClipList.AC_clickBtnMenu);
        SceneManager.LoadScene(Constant.ListOfScene.s_networkLobby);
    }

    public void QuitGame()
    {
        Application.Quit();
    }
    #endregion
}