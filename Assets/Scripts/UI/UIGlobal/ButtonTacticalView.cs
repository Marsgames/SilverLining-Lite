#region Author
/////////////////////////////////////////
//   Yannig Smagghe 
//   https://gitlab.com/YannigSmagghe
/////////////////////////////////////////
#endregion

using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class ButtonTacticalView : MonoBehaviour
{
    #region Variables
    private CameraManager m_cameraManager;
    #endregion Variables

    #region Unity's function
    private void Start()
    {
        m_cameraManager = CameraManager.Instance;
    }
    #endregion

    #region Functions
    public void TaskOnClick()
    {
        m_cameraManager.ChangeView();
    }
    #endregion
}
