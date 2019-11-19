#pragma warning disable CS0618 // type obsolete
#region Author
/////////////////////////////////////////
//   RAPHAËL DAUMAS
//   https://raphdaumas.wixsite.com/portfolio
//   https://github.com/Marsgames
/////////////////////////////////////////
#endregion

using System.Collections;
using UnityEngine;
using UnityEngine.Networking;

public class ActiveTrap : ActiveObstacle
{
    #region Variables
    [SerializeField] private int m_dommages = 10;
    [SerializeField] private bool m_bidirictionnalDamages = true;
    
    #endregion

    #region Unity's functions
    private void OnTriggerEnter(Collider other)
    {
        if (!enabled)
        {
            return;
        }

        if (!other.tag.Equals("Unit"))
        {
            return;
        }


        if (m_bidirictionnalDamages || m_playerNumber != other.GetComponent<UnitController>().GetPlayerNumber())
        {
            other.GetComponent<CombatUnit>().TakeDamage(m_dommages, m_playerNumber);
        }
    }
    #endregion

    #region Functions
    public override ObstacleType GetObstacleType()
    {
        return ObstacleType.Trap;
    }
    
    [Server]
    protected override IEnumerator Construct(PlayerEntity.Player playerNumber)
    {
        yield return base.Construct(playerNumber);
        GameManager.Instance.GetPlayer(playerNumber).TakeControl(GetComponent<NetworkIdentity>());
    }

    [Server]
    protected override IEnumerator Destruct()
    {
        yield return base.Destruct();
        NetworkServer.Destroy(gameObject);
        yield return null;
    }  
    #endregion
}
