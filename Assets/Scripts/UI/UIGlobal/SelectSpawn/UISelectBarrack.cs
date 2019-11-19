using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UISelectBarrack : UISelectSpawnUnit
{
    #region Variables
    [SerializeField] private Sprite m_iconNeutral = null;
    #endregion Variables

    #region Functions
    protected override IEnumerator SetSpawnUnit()
    {
        string tag;
        if (m_type == ESpawnUnit.Top)
        {
            tag = Constant.ListOfTag.s_spawnUnit1;
        }
        else
        {
            tag = Constant.ListOfTag.s_spawnUnit2;
        }
        m_spawnUnits = GameManager.Instance.GetBarrackFromTag(tag).GetComponent<SpawnUnits>();
        m_spawnUnits.SetUISelectSpawnUnit(this);
        SetIcons();
        yield return null;
        
    }
    public override void SetIcons()
    {
        Sprite sprite = GetComponent<Image>().sprite;
        PlayerEntity.Player player = m_spawnUnits.GetPlayerNumber();

        switch (player)
        {
            case PlayerEntity.Player.Player1:
                sprite = m_iconSyca;
                break;
            case PlayerEntity.Player.Player2:
                sprite = m_iconArca;
                break;
            case PlayerEntity.Player.Neutre:
                sprite = m_iconNeutral;
                break;
        }     
        GetComponent<Image>().sprite = sprite;     
    }

    public override void TaskOnClick()
    {
        if (m_spawnUnits.GetPlayerNumber() == GameManager.Instance.GetLocalPlayer())
        {
            base.TaskOnClick();
        }
    }
    #endregion Functions
}
