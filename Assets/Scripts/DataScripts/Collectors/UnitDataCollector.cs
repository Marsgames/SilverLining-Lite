#pragma warning disable CS0618 // type deprecated
#region Author
/////////////////////////////////////////
//   Guillaume Quiniou 
/////////////////////////////////////////
#endregion
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

[AddComponentMenu("DataCollector/UnitDataCollector")]
public class UnitDataCollector : DataCollector
{
    #region Variables
    private UnitDeathDataSet m_deathData;
    private UnitSpawnDataSet m_spawnData;
    #endregion
    #region Unity's functions
    protected override void Start()
    {
        base.Start();
        m_deathData = new UnitDeathDataSet();
        m_spawnData = new UnitSpawnDataSet();
        m_spawnData.ColledData(gameObject);
    }
    
    [ServerCallback]
    private void OnDestroy()
    {
        if (GetComponent<CombatUnit>().GetCurrentHealth() <= 0) 
        {
            m_deathData.ColledData(gameObject);
        }
    }
    #endregion
}
