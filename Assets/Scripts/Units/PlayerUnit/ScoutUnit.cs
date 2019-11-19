using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScoutUnit : CombatUnit
{
    #region Function
    public override UnitType GetUnitType()
    {
        return UnitType.Scout;
    }

    //public void BoostAttack(int value, int time)
    //{
    //    m_boostAttack = value;
    //    m_attackDamage += value;

    //    Invoke("RemoveBonusAttack", time);
    //}

    //private void RemoveBonusAttack()
    //{
    //    m_attackDamage -= m_boostAttack;
    //}
    #endregion
}
