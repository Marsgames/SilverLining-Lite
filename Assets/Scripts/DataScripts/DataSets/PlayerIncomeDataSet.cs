#pragma warning disable CS0618 // type deprecated
#region Author
/////////////////////////////////////////
//   Guillaume Quiniou
/////////////////////////////////////////
#endregion
using UnityEngine;

public class PlayerIncomeDataSet : DataSet
{
    #region Variables
    [SerializeField] private PlayerEntity.Player m_playerNumber;
    [SerializeField] private int m_unitGold;
    [SerializeField] private int m_buildGold;
    [SerializeField] private float m_time;
    #endregion
    #region Functions

    public PlayerIncomeDataSet(PlayerEntity.Player _playerNumber)
    {
        eventName = "playerIncome";
        m_playerNumber = _playerNumber;
    }
    
    public void CollectData(int _gold, int _mana)
    {
        m_time = Mathf.Round(Time.time);
        m_unitGold = _gold;
        m_buildGold = _mana;
        SaveData();
    }
    #endregion

}
