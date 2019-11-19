#pragma warning disable CS0618 // type deprecated
#region Author
/////////////////////////////////////////
//   Yannig Smagghe 
//   https://gitlab.com/YannigSmagghe
/////////////////////////////////////////
#endregion
using UnityEngine;

public class WarriorUnit : CombatUnit
{

    #region Function
    public override UnitType GetUnitType()
    {
        return UnitType.Warrior;
    }
   // protected bool m_diyingLater;
    //public override void TakeDamage(int damageTaken, Constant.Player enemyId)
    //{
    //    m_currentHealth -= damageTaken;

    //    if (m_currentHealth <= 0 && !m_diyingLater)
    //    {
    //        DestroyUnitKilled();
    //    }
    //}

    //public void DyingLater(bool active, int time)
    //{
    //    m_diyingLater = active;
    //    Invoke("RemoveDyingLater", time);
    //}

    //private void RemoveDyingLater()
    //{
    //    if (m_currentHealth <= 0)
    //    {
    //        Destroy(gameObject);
    //    }
    //    else
    //    {
    //        m_diyingLater = !m_diyingLater;
    //    }
    //}
    #endregion

}
