#region Author
/********************
 * Corentin Couderc
 ********************/
#endregion

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TowerRange : ObjectSelection
{
    #region Variables
    [SerializeField] private Tower m_tower = null;
    #endregion

    #region Unity's Functions   
    public override void OnMouseEnter()
    {
        if (GameManager.Instance.GetEndGameBool())
        {
            return;
        }
        base.OnMouseEnter();

        switch (m_tower.GetPlayerOwner())
        {
            case PlayerEntity.Player.Player1:
                m_tower.GetProjector().GetComponent<Projector>().material = m_tower.GetProjectorMaterialSyca();
                break;
            case PlayerEntity.Player.Player2:
            case PlayerEntity.Player.Bot:
                m_tower.GetProjector().GetComponent<Projector>().material = m_tower.GetProjectorMaterialArca();
                break;
        }
        m_tower.SetActiveTowerProjector(true);
    }

    public override void OnMouseExit()
    {
        if (!m_isKeepGlow)
        {
            m_rend.material.shader = m_shader1;
            m_tower.SetActiveTowerProjector(false);
        }
    }
    #endregion
}
