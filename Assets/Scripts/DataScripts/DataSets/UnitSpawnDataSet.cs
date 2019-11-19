#pragma warning disable CS0618 // type deprecated
#region Author
/////////////////////////////////////////
//   Guillaume Quiniou 
/////////////////////////////////////////
#endregion
using UnityEngine;

public class UnitSpawnDataSet : DataSet
{
    #region Variables
    [SerializeField] private PlayerEntity.Player m_playerNumber;
    [SerializeField] private string m_unitType;
    [SerializeField] private float m_time;
    #endregion
    #region Functions
    public UnitSpawnDataSet()
    {
        eventName = "unitSpawn";
    }

    public void ColledData(GameObject go)
    {
        m_playerNumber = go.GetComponent<UnitController>().GetPlayerNumber();
        m_time = Mathf.Round(Time.time);
        m_unitType = go.GetComponent<WarriorUnit>() != null ? "normalUnit" :
           go.GetComponent<RangeUnit>() != null ? "rangeUnit" :
           go.GetComponent<TankUnit>() != null ? "tankUnit" : "scoutUnit";
        SaveData();
    }
    #endregion
}
