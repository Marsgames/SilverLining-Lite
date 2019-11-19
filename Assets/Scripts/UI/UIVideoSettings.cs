#pragma warning disable CS0618 // type deprecated
#region Author
/////////////////////////////////////////
//   MARION WARTELLE-MATHIEU 
/////////////////////////////////////////
#endregion
using UnityEngine;

public class UIVideoSettings : MonoBehaviour
{
    #region Variables
    private int m_fullScreenWidth;
    private int m_fullScreenHeight;
    private int m_windowedWidth = 1280;
    private int m_windowedHeight = 720;

    private bool m_isFullscreen;
    #endregion

    #region Unity's Functions
    private void Awake()
    {
        m_fullScreenWidth = Screen.currentResolution.width;
        m_fullScreenHeight = Screen.currentResolution.height;
        m_isFullscreen = Screen.fullScreen;
    }
    #endregion

    #region Functions

    public void ToggleFullScreen()
    {
        m_isFullscreen = !m_isFullscreen;
        ChangeWindowSize();
    }

    private void ChangeWindowSize()
    {
        if (m_isFullscreen)
        {
            Screen.SetResolution(m_fullScreenWidth, m_fullScreenHeight, true);
        }
        else
        {
            Screen.SetResolution(m_windowedWidth, m_windowedHeight, false);
        }
    }

    public void ChangeResolution(int resolutionMode)
    {
        switch (resolutionMode)
        {
            case 0:
                m_windowedWidth = 1280;
                m_windowedHeight = 720;
                break;
            case 1:
                m_windowedWidth = 1600;
                m_windowedHeight = 900;
                break;
            case 2:
                m_windowedWidth = 1920;
                m_windowedHeight = 1080;
                break;
        }
        ChangeWindowSize();
    }

    public void SetQuality(int qualityIndex)
    {
        QualitySettings.SetQualityLevel(qualityIndex);
    }

    #endregion
}