#pragma warning disable CS0618 // type deprecated
#region Author
/////////////////////////////////////////
//   Guillaume Quiniou 
/////////////////////////////////////////
#endregion

using UnityEngine;
using UnityEngine.Networking;

public class GolemNeutralCamp : NeutralCamp
{
    [SerializeField] private bool isBigGolem = false;
    [SerializeField] private ParticleSystem m_fxSpawnSyca = null;
    [SerializeField] private ParticleSystem m_fxSpawnArca = null;
    private BossFightSound m_bossSound;

    protected override void Start()
    {
        base.Start();

        if (!isBigGolem)
        {
            return;
        }
        m_bossSound = FindObjectOfType<BossFightSound>();
    }

    [Server]
    public override void DecrementGolemsNumber(PlayerEntity.Player killer)
    {
        m_nbGolemsToSpawn++;

        if (m_nbGolemsToSpawn >= m_golemsList.Count)
        {
            RespawnUnderPlayerControl(killer);
            Invoke("SpawnGolems", m_timeBeforeRespawn);
            RpcActiveTimer();

            if (!isBigGolem)
            {
                return;
            }
            m_bossSound.StopBossMusic();
        }
    }

    [Server]
    private void RespawnUnderPlayerControl(PlayerEntity.Player killer)
    {
        for (int i = 0; i < m_nbGolemsToSpawn; i++)
        {
            GameObject theSpawnPoint = m_golemsSpawnPoints[i].gameObject;
            GameObject prefabGolemPlayer = m_golemsList[i].GetComponent<GolemNeutralUnit>().GetPrefabGolemPlayer(killer);
            GameObject theGolem = Instantiate(prefabGolemPlayer, theSpawnPoint.transform.position, theSpawnPoint.transform.rotation);

            Nexus ennemyNexus = GetNearestEnemyNexus(killer);

            UnitController golemUnitController = theGolem.GetComponent<UnitController>();
            golemUnitController.SetObjective(ennemyNexus.transform.position);
            golemUnitController.SetPlayerNumber(killer);
            golemUnitController.SetNexusEnemy(ennemyNexus.transform.position);
            NetworkServer.Spawn(theGolem);
        }
        RpcRespawnGolemFX(killer);
    }

    [ClientRpc]
    private void RpcRespawnGolemFX(PlayerEntity.Player killer)
    {
        if(killer == PlayerEntity.Player.Player1)
        {
            m_fxSpawnSyca.Play();
        }
        else
        {
            m_fxSpawnArca.Play();
        }
    }

    private Nexus GetNearestEnemyNexus(PlayerEntity.Player killer)
    {
        float dist = float.MaxValue;
        Nexus nearestNexus = null;
        foreach (Nexus Nexus in FindObjectsOfType<Nexus>())
        {
            if (Nexus.GetPlayerNumber() == killer)
            {
                continue;
            }

            float newDist = Vector3.Distance(transform.position, Nexus.transform.position);

            if (newDist < dist)
            {
                nearestNexus = Nexus;
                dist = newDist;
            }
        }

        return nearestNexus;
    }

    protected override void OnTriggerEnter(Collider other)
    {
        base.OnTriggerEnter(other);

        if (!isBigGolem)
        {
            return;
        }

        if (other.name.Contains("GiantGolem"))
        {
            return;
        }

        m_bossSound.PlayBossMusic();
    }

    public override void SetIsInRegen(bool value)
    {
        base.SetIsInRegen(value);

        if (!isBigGolem)
        {
            return;
        }
        m_bossSound.StopBossMusic();
    }
}
