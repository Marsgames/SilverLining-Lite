#region Author

/////////////////////////////////////////
//  Guillaume Quiniou
/////////////////////////////////////////
#endregion

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UISpawnObstacle : MonoBehaviour
{
    #region Variables
    [SerializeField] private List<UISpawnObstacleButton> m_buttons = new List<UISpawnObstacleButton>();
    private ObstacleConstructor m_obstacleConstructor;
    #endregion Variables

    #region Unity's function
    private void Start()
    {
        m_obstacleConstructor = GetComponentInParent<ObstacleConstructor>();
        foreach (UISpawnObstacleButton button in m_buttons)
        {
            button.SetObstacleConstructor(m_obstacleConstructor);
        }        
    }
    #endregion

    #region Functions
    public void SetActive(bool value)
    {
        if (null == m_obstacleConstructor)
        {
            m_obstacleConstructor = GetComponentInParent<ObstacleConstructor>();
        }      
        if (!m_obstacleConstructor.GetTerritory().GetIsAvailable())
        {
            return;
        }
        gameObject.SetActive(value);
    }
    #endregion
}
