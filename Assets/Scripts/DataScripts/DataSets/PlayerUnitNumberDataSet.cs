#pragma warning disable CS0618 // type deprecated
#region Author
/////////////////////////////////////////
//   Guillaume Quiniou 
/////////////////////////////////////////
#endregion
using System.Collections.Generic;
using UnityEngine;


public class PlayerUnitNumberDataSet : DataSet
{
    #region Variables
    [SerializeField] private PlayerEntity.Player m_playerNumber;
    [SerializeField] private int m_count = 0;
    [SerializeField] private float m_time;
    #endregion

    #region Functions
    public PlayerUnitNumberDataSet(PlayerEntity.Player _playerNumber)
    {
        eventName = "playerUnitNumber";
        m_playerNumber = _playerNumber;
    }
    public void ColledData(int _count)
    {
        m_count = _count;
        m_time = Mathf.Round(Time.time);
        SaveData();
    }
    #endregion
}
