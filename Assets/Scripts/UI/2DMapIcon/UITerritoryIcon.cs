using System.Collections;
using UnityEngine;

public class UITerritoryIcon : UI2DMapIcon
{
    #region Functions      

    public override void SelectIconToDisplay()
    {
        return;
    }

    public override void ShowIcon()
    {
        m_spriteRenderer.enabled = true;
    }

    public override void HideIcon()
    {
        m_spriteRenderer.enabled = false;
    }
    #endregion

}
