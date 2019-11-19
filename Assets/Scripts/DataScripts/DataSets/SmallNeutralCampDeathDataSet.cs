#region Author
/////////////////////////////////////////
//   Guillaume Quiniou
/////////////////////////////////////////
#endregion
using UnityEngine;
public class SmallNeutralCampDeathDataSet : DataSet
{
    #region Variables
    [SerializeField] private PlayerEntity.Player m_playerNumber;
    [SerializeField] private int m_goldReward;
    [SerializeField] private float m_time;
    #endregion

    public SmallNeutralCampDeathDataSet()
    {
        eventName = "smallNeutralCampDeath";
    }

    public void CollectData(PlayerEntity.Player playerNumber, int goldReward)
    {
        m_time = Mathf.Round(Time.time);
        m_playerNumber = playerNumber;
        m_goldReward = goldReward;
        SaveData();
    }

}
