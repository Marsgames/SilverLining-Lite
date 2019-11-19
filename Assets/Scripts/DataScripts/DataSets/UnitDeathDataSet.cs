#pragma warning disable CS0618 // type deprecated
#region Author
/////////////////////////////////////////
//   Guillaume Quiniou 
/////////////////////////////////////////
#endregion
using UnityEngine;

public class UnitDeathDataSet : DataSet
{
    #region Variables
    [SerializeField] private float m_positionX;
    [SerializeField] private float m_positionZ;
    [SerializeField] private PlayerEntity.Player m_playerNumber;
    [SerializeField] private CombatUnit.State m_unitState;
    [SerializeField] private float m_time;
    [SerializeField] private string m_unitType;
    #endregion
    #region Functions
    public UnitDeathDataSet()
    {
        eventName = "unitDeath";
    }

    public void ColledData(GameObject go)
    {
        m_positionX = go.transform.position.x;
        m_positionZ = go.transform.position.z;
        m_playerNumber = go.GetComponent<UnitController>().GetPlayerNumber();
        m_unitState = go.GetComponent<CombatUnit>().GetCurrentState();
        m_time = Mathf.Round(Time.time);
        m_unitType = go.GetComponent<WarriorUnit>() != null ? "normalUnit" :
            go.GetComponent<RangeUnit>() != null ? "rangeUnit" :
            go.GetComponent<TankUnit>() != null ? "tankUnit" : "scoutUnit";
        SaveData();
    }
    #endregion
}
