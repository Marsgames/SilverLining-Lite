#pragma warning disable CS0618 // type deprecated
#region Author
///////////////////////////////////////////////
//   Guillaume Quiniou 
/////////////////////////////////////////
#endregion

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using Prototype.NetworkLobby;

[RequireComponent(typeof(Collider))]
public class Nexus : NetworkBehaviour
{
    #region Variables
    [Header("Player infos")]
    [SerializeField] private PlayerEntity.Player m_playerNumber = PlayerEntity.Player.Neutre;
    [SerializeField] private ParticleSystem m_nexusDamage = null;
    [SerializeField] public List<GameObject> m_objectToControl;
    private PlayerEntity m_player;
    #endregion
    #region Unity's functions

    [ServerCallback]
    public IEnumerator Start()
    {
        yield return new WaitUntil(() => GameManager.Instance != null);
        yield return new WaitUntil(GameManager.Instance.GetAllPlayersReady);

        if (NetworkServer.connections.Count == 1 && m_playerNumber == PlayerEntity.Player.Player2)
        {
            yield return new WaitWhile(() => null == FindObjectOfType<AIControl>());
            m_playerNumber = PlayerEntity.Player.Bot;
            m_player = FindObjectOfType<AIControl>();
            yield return new WaitUntil(() =>
            NetworkServer.SpawnWithClientAuthority(m_player.gameObject, GameManager.Instance.GetPlayer(PlayerEntity.Player.Player1).GetComponent<NetworkIdentity>().connectionToClient));
        }
        else
        {
            m_player = NetworkServer.connections[(int)m_playerNumber].playerControllers[0].gameObject.GetComponent<PlayerEntity>();
        }

        m_player.TakeControl(GetComponent<NetworkIdentity>());

        foreach (GameObject item in m_objectToControl)
        {
            m_player.TakeControl(item.GetComponent<NetworkIdentity>());
            if (item.GetComponent<SpawnUnits>())
            {
                SpawnUnits spawnUnits = item.GetComponent<SpawnUnits>();
                spawnUnits.RpcInitPlayerNumber(m_playerNumber);
                spawnUnits.SetTarget(GetNearestNexus().gameObject);
            }
        }
    }

    [ServerCallback]
    private void OnTriggerEnter(Collider other)
    {
        if (m_player)
        {
            if (other.gameObject.CompareTag(Constant.ListOfTag.s_unit)
                && other.gameObject.GetComponent<UnitController>().GetPlayerNumber() != m_playerNumber)
            {
                m_player.TakeDamage(other.gameObject.GetComponent<UnitController>().GetDamageToNexus());
                Destroy(other.gameObject);
                RpcDamageNexus();
            }
        }
    }
    #endregion

    #region Functions

    private Nexus GetNearestNexus()
    {
        Nexus nearestNexus = FindObjectOfType<Nexus>();
        foreach (Nexus Nexus in FindObjectsOfType<Nexus>())
        {
            if (this == Nexus)
            {
                continue;
            }

            if (Vector3.Distance(transform.position, Nexus.transform.position) < Vector3.Distance(transform.position, nearestNexus.transform.position) || Vector3.Distance(transform.position, nearestNexus.transform.position) == 0)
            {
                nearestNexus = Nexus;
            }
        }
        return nearestNexus;
    }

    [ClientRpc]
    public void RpcDamageNexus()
    {

        if (m_nexusDamage != null)
        {
            m_nexusDamage.Play();
        }
    }
    #endregion
    #region Accessors
    public PlayerEntity.Player GetPlayerNumber()
    {
        return m_playerNumber;
    }
    #endregion
}

