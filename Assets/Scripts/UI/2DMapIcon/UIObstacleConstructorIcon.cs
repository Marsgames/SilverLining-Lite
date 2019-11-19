using System.Collections;
using UnityEngine;

public class UIObstacleConstructorIcon : UI2DMapIcon
{

    #region Functions      

    public override void SelectIconToDisplay()
    {
        return;
    }

    public void ShowConstructorIcon()
    {
        m_spriteRenderer.enabled = true;
    }

    public void HideConstructorIcon()
    {
        m_spriteRenderer.enabled = false;
    }

    public override void ShowIcon()
    {
        return;
    }

    public override void HideIcon()
    {
        return;
    }
    #endregion

}
