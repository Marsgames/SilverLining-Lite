#pragma warning disable CS0618 // type deprecated
#region Author
/////////////////////////////////////////
//   Guillaume Quiniou
/////////////////////////////////////////
#endregion
using UnityEngine;

public class PlayerTakeDamageDataSet : DataSet
{
    #region Variables
    [SerializeField] private PlayerEntity.Player m_playerNumber;
    [SerializeField] private int m_amount;
    [SerializeField] private float m_time;
    #endregion
    #region Functions
    public PlayerTakeDamageDataSet(PlayerEntity.Player _playerNumber)
    {
        eventName = "playerTakeDamage";
        m_playerNumber = _playerNumber;
    }
    public void ColledData(int _dmg)
    {     
        m_amount = _dmg;
        m_time = Mathf.Round(Time.time);
        SaveData();
    }
    #endregion
}
