#pragma warning disable CS0618 // type deprecated
#region Author
/////////////////////////////////////////
//   Yannig Smagghe
//   https://gitlab.com/YannigSmagghe
/////////////////////////////////////////
#endregion

using System;
using UnityEngine;
using UnityEngine.UI;

public class UITootltipUnit : MonoBehaviour
{
    #region Variables
    [SerializeField] private Text m_cost = null;
    [SerializeField] private Text m_damage = null;
    [SerializeField] private Text m_health = null;  
    #endregion

    #region Function
    public void ShowTooltip(UnitController unit)
    {    
        m_cost.text = unit.GetUnitCost().ToString();
        CombatUnit combatUnit = unit.GetComponent<CombatUnit>();
        float dps = combatUnit.GetAttackDamage() / combatUnit.GetAttackSpeed();
        float dpsTruncated = (float)(Math.Truncate(dps * 100.0) / 100.0);
        m_damage.text = dpsTruncated.ToString();
        m_health.text = combatUnit.GetMaxHealth().ToString();
    }
    #endregion Function
       
}
