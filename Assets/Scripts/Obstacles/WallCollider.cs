#region Author
////////////////////////////////////////////
//   Guillaume Quiniou 
/////////////////////////////////////////
#endregion
using UnityEngine;

public class WallCollider : MonoBehaviour
{
    #region Variables
    private ActiveWall m_activeWall;
    private ObjectSelection m_objetSelection;
    #endregion Variables

    #region Function
    private void Start()
    {
        m_activeWall = GetComponentInParent<ActiveWall>();
        m_objetSelection = GetComponentInParent<ObjectSelection>();
    }

    private void OnMouseDown()
    {
        m_activeWall.OnMouseDown();
    }

    private void OnMouseEnter()
    {
        m_activeWall.OnMouseOver();
        m_objetSelection.OnMouseEnter();
    }

    private void OnMouseExit()
    {
        m_activeWall.OnMouseExit();
        m_objetSelection.OnMouseExit();
    }
    #endregion
}
