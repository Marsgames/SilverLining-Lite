#region Author
/////////////////////////////////////////
//   Yannig Smagghe --> #UITootltipUnit#
//   https://gitlab.com/YannigSmagghe
/////////////////////////////////////////
#endregion
using UnityEngine;
using UnityEngine.UI;

public class UITootltipObstacle : MonoBehaviour
{
    #region Variables
    [SerializeField] private Text m_cost = null;
    #endregion

    #region Function
    public void ShowTooltip(ActiveObstacle obstacle)
    {
        gameObject.SetActive(true);
      
        int cost = obstacle.GetBuildingCost();
        m_cost.text = cost.ToString();
    }
    #endregion
}
