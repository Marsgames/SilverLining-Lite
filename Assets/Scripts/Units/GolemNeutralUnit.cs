#pragma warning disable CS0618 // type deprecated
#region Author
/////////////////////////////////////////
//  Guillaume Quiniou
/////////////////////////////////////////
#endregion
using UnityEngine;

public class GolemNeutralUnit : NeutralUnit
{
    #region Variables
    [SerializeField] UnitType m_type = UnitType.MediumGolem;
    [SerializeField] GameObject m_prefabGolemArca = null;
    [SerializeField] GameObject m_prefabGolemSyca = null;
    #endregion

    public override UnitType GetUnitType()
    {
        return m_type;
    }

    public GameObject GetPrefabGolemPlayer(PlayerEntity.Player playerNumber)
    {
        if(playerNumber == PlayerEntity.Player.Player1)
        {
            return m_prefabGolemSyca;
        }
        else
        {
            return m_prefabGolemArca;
        }
    }

   
}
