#region Author
/////////////////////////////////////////
//   Michel Bigourd --> AlertManager
//   https://linkedin.com/in/michel-bigourd-a8a05b100
/////////////////////////////////////////
//   Yannig Smagghe --> #MenuButtonSound#
//   https://gitlab.com/YannigSmagghe
/////////////////////////////////////////
#endregion

using UnityEngine;
using UnityEngine.EventSystems;

public class MenuButtonSound : MonoBehaviour, IPointerEnterHandler
{
    #region Variables
    private SoundManager m_soundManager;
    #endregion

    #region Functions
    private void Start()
    {
        m_soundManager = SoundManager.Instance;
        CheckIfOk();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        m_soundManager.PlaySound(SoundManager.AudioClipList.AC_mouseOver);
    }

    public void ClickButtonPlaySound()
    {
        m_soundManager.PlaySound(SoundManager.AudioClipList.AC_clickBtnMenu);
    }
    #endregion

    private void CheckIfOk()
    {
#if UNITY_EDITOR
        if (null == m_soundManager)
        {
            Debug.LogError("ALLLOOO ALLLOOOO le sound manager ne peut pas être null dans " + name, this);
            UnityEditor.EditorApplication.isPlaying = false;
        }
#endif
    }
}
