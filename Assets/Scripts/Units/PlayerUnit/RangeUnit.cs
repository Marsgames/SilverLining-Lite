#pragma warning disable CS0618 // type deprecated
#region Author
/////////////////////////////////////////
//   Yannig Smagghe 
//   https://gitlab.com/YannigSmagghe
/////////////////////////////////////////
#endregion
using UnityEngine;
using UnityEngine.Networking;

public class RangeUnit : CombatUnit
{
    public ParticleSystem m_projectilePrefab;
    public Transform m_firePoint;


    #region Function

    public override UnitType GetUnitType()
    {
        return UnitType.Range;
    }

    [Server]
    protected override void Attack(CombatUnit enemyUnit)
    {
        if (enemyUnit.GetCurrentState() != State.Dead && Time.time > m_nextAttack)
        {
            m_nextAttack = Time.time + m_attackSpeed;
            enemyUnit.TakeDamage(m_attackDamage, m_unitController.GetPlayerNumber());
            RpcShootProjectile(enemyUnit.gameObject);
        }
    }

    [ClientRpc]
    private void RpcShootProjectile(GameObject enemyUnit)
    {
        if (enemyUnit != null)
        {
            ParticleSystem projectileTemp = Instantiate(m_projectilePrefab, m_firePoint.position, m_firePoint.rotation);
            UnitProjectiles projectile = projectileTemp.GetComponent<UnitProjectiles>();

            projectile.SeekTarget(enemyUnit.transform);
        }
    }

    //public void BoostRange(int value, int time)
    //{
    //    m_boostRange = value;
    //    SetRangeInWorldSpace(m_boostRange);

    //    Invoke("RemoveBoostRange", time);
    //}

    //private void RemoveBoostRange()
    //{
    //    SetRangeInWorldSpace(-m_boostRange);
    //}
    #endregion
}
