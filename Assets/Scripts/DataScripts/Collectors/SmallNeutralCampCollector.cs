#pragma warning disable CS0618 // type deprecated
#region Author
/////////////////////////////////////////
//   Guillaume Quiniou
/////////////////////////////////////////
#endregion
using UnityEngine;

[AddComponentMenu("DataCollector/SmallNeutralCampCollector")]
public class SmallNeutralCampCollector : DataCollector
{
    SmallNeutralCampDeathDataSet m_smallNeutralCampDeathDataSet;
    SmallNeutralCamp m_smallNeutralCamp;

    protected override void Start()
    {
        base.Start();
        m_smallNeutralCamp = GetComponent<SmallNeutralCamp>();
        m_smallNeutralCampDeathDataSet = new SmallNeutralCampDeathDataSet();
        m_smallNeutralCamp.DeathEvents.AddListener(OnDeath);
    }
    private void OnDeath(PlayerEntity.Player playerNumber, int goldReward)
    {
        m_smallNeutralCampDeathDataSet.CollectData(playerNumber, goldReward);
    }
}
