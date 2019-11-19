#pragma warning disable CS0618 // type deprecated
#region Author
/////////////////////////////////////////
//   Yannig Smagghe --> #AggroRange#
//   https://gitlab.com/YannigSmagghe
/////////////////////////////////////////
///////////////////////////////////////////////
//   Guillaume Quiniou 
/////////////////////////////////////////
#endregion
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Networking;

public class AggroRange : NetworkBehaviour
{

    #region Variables
    private CombatUnit m_parentCombatUnit;
    #endregion

    #region Unity's functions
    [ServerCallback]
    void Start()
    {
        m_parentCombatUnit = transform.parent.GetComponent<CombatUnit>();
        if(m_parentCombatUnit == null)
        {
            Debug.LogError("AggroRange without Combat unit", this);
        }
        GetComponent<SphereCollider>().radius = m_parentCombatUnit.GetRangeAggro();
    }

    [ServerCallback]
    private void OnTriggerEnter(Collider other)
    {
       
        UnitController otherUC = other.GetComponent<UnitController>();
        if(null == m_parentCombatUnit)
        {
            return;
        }
        UnitController parentUC = m_parentCombatUnit.GetComponent<UnitController>();

        if (null == otherUC || null == parentUC || !other.gameObject.GetComponent<NavMeshAgent>().isActiveAndEnabled)
        {
            return;
        }      

        if (otherUC.GetPlayerNumber() != parentUC.GetPlayerNumber())
        {
            m_parentCombatUnit.UnitEnterAggroRange(other.GetComponent<CombatUnit>());
        }
    }

    [ServerCallback]
    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.GetComponent<UnitController>() && other.gameObject.GetComponent<NavMeshAgent>().isActiveAndEnabled &&
           other.GetComponent<UnitController>().GetPlayerNumber() != m_parentCombatUnit.GetComponent<UnitController>().GetPlayerNumber())
        {
            m_parentCombatUnit.EnemyUnitExitAggroRange(other.GetComponent<CombatUnit>());
        }
    }
    #endregion
}