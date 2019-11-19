#pragma warning disable CS0618 // type deprecated
#region Author
/////////////////////////////////////////
//   Guillaume Quiniou 
/////////////////////////////////////////
#endregion
using Prototype.NetworkLobby;
using System.Collections;
using UnityEngine;
using UnityEngine.Networking;

[RequireComponent(typeof(CapsuleCollider))]
public class TowerPlayer : Tower
{
    #region Variables
    [SerializeField] private PlayerEntity.Player m_playerNumber = PlayerEntity.Player.Neutre;
    #endregion

    #region Unity's function

    protected override IEnumerator Start()
    {
        yield return base.Start();
        if (m_playerNumber == PlayerEntity.Player.Player1)
        {
            m_lineRenderer.material = m_laserSyca;
        }
        else
        {
            m_lineRenderer.material = m_laserArca;
        }
        if (isServer)
        {
            if (m_playerNumber == PlayerEntity.Player.Player2 && LobbyManager.s_Singleton.numPlayers == 1)
            {
                m_playerOwner = PlayerEntity.Player.Bot;
            }
            else
            {
                m_playerOwner = m_playerNumber;
            }
        }
        
    }
    #endregion

    #region Functions
    protected override void CaptureCheckpoint(UnitController unit)
    {
        return;
    }

    protected override void SetCheckpointNeutral()
    {
        return;
    }
    #endregion

    protected override void CheckIfOk()
    {
#if UNITY_EDITOR
        if (null == m_stockUnits)
        {
            Debug.LogError("Stock Units n'est pas set sur " + name, this);
            UnityEditor.EditorApplication.isPlaying = false;
        }

        if (null == m_alertManager)
        {
            Debug.LogError("Alert Manager n'est pas set sur " + name, this);
            UnityEditor.EditorApplication.isPlaying = false;
        }

        if (null == m_area)
        {
            Debug.LogError("Area n'est pas set sur " + name, this);
            UnityEditor.EditorApplication.isPlaying = false;
        }

        if (null == m_friendsUnits)
        {
            Debug.LogError("Friends Units n'est pas set sur " + name, this);
            UnityEditor.EditorApplication.isPlaying = false;
        }

        if (null == m_enemiesUnits)
        {
            Debug.LogError("Enemies Units n'est pas set sur " + name, this);
            UnityEditor.EditorApplication.isPlaying = false;
        }

        if (null == m_territory)
        {
            Debug.LogError(name + " NOT IN A TERRITORY", this);
            UnityEditor.EditorApplication.isPlaying = false;
        }
#endif
    }
}
