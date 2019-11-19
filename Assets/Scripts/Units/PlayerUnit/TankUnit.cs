#pragma warning disable CS0618 // type deprecated
#region Author
/////////////////////////////////////////
//   Yannig Smagghe
//   https://gitlab.com/YannigSmagghe
/////////////////////////////////////////
#endregion
using UnityEngine;

public class TankUnit : CombatUnit
{
    #region Function
    public override UnitType GetUnitType()
    {
        return UnitType.Tank;
    }

    // protected bool m_invulnerability;
    //public override void TakeDamage(int damageTaken, PlayerEntity.Player enemyId)
    //{
    //    if (m_invulnerability)
    //    {
    //        return;
    //    }
    //    base.TakeDamage(damageTaken, enemyId);
    //}

    //public void Invulnerability(bool active, int time)
    //{
    //    m_invulnerability = active;
    //    Invoke("RemoveInvulnerability", time);
    //}

    //private void RemoveInvulnerability()
    //{
    //    m_invulnerability = !m_invulnerability;
    //}
    #endregion
}
