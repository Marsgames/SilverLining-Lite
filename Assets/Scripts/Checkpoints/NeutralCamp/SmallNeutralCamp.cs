#pragma warning disable CS0618 // type deprecated
#region Author
/////////////////////////////////////////
//   RAPHAËL DAUMAS --> #SCRIPTNAME#
//   https://raphdaumas.wixsite.com/portfolio
//   https://github.com/Marsgames
/////////////////////////////////////////
#endregion

using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.AI;
using UnityEngine.Events;
using System;

public class SmallNeutralCamp : NeutralCamp
{
    #region Variables

    [SerializeField] protected int m_goldReward = 1000;
    [SerializeField] private Canvas m_canvasTextGold = null;

    public class DeathEvent : UnityEvent<PlayerEntity.Player, int> { }

    public DeathEvent DeathEvents { get; } = new DeathEvent();
    #endregion Variables

    #region Functions
    [Server]
    public override void DecrementGolemsNumber(PlayerEntity.Player killer)
    {
        m_nbGolemsToSpawn++;

        if (m_nbGolemsToSpawn >= m_golemsList.Count)
        {
            DeathEvents.Invoke(killer, m_goldReward);
            PlayerEntity playerKiller = GameManager.Instance.GetPlayer(killer);
            playerKiller.GainUnitGold(m_goldReward);
            Invoke("SpawnGolems", m_timeBeforeRespawn);
            RpcActiveTimer();
            if (killer != PlayerEntity.Player.Bot)
            {
                TargetShowTextGold(playerKiller.connectionToClient);
            }
        }
    }

    [TargetRpc]
    private void TargetShowTextGold(NetworkConnection connectionToClient)
    {
        if (CameraManager.Instance.GetCameraState() != CameraManager.ECamState.Iso2D)
        {
            StartCoroutine(ShowText());
        }
    }

    private IEnumerator ShowText()
    {
        if (CameraManager.Instance.GetCameraState() != CameraManager.ECamState.Iso2D)
        {
            GameObject newCanvas = Instantiate(m_canvasTextGold, transform.position + new Vector3(0, 3, -1), Quaternion.identity, transform).gameObject;

            newCanvas.GetComponentInChildren<TextMove>().MoveText(m_goldReward);
            yield return new WaitForSecondsRealtime(2);
            Destroy(newCanvas);
        }
    }
    #endregion

}