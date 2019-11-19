#pragma warning disable CS0618 // type deprecated
/////////////////////////////////////////
//   RAPHAËL DAUMAS 
//   https://raphdaumas.wixsite.com/portfolio
//   https://github.com/Marsgames
/////////////////////////////////////////
using UnityEngine;
using UnityEngine.Networking;

public class Barrack : CheckpointBase
{
    #region Functions
    public override void SetPlayerOwner(PlayerEntity.Player player)
    {
        base.SetPlayerOwner(player);
        SpawnUnits spawnUnitsComponent = GetComponent<SpawnUnits>();
        spawnUnitsComponent.SetPlayerNumber(m_playerOwner);
        spawnUnitsComponent.SetTarget(GetNearestEnemyNexus().gameObject);
        spawnUnitsComponent.GetUISelectSpawnUnit().SetIcons();
}

    protected override void SetCheckpointNeutral()
    {
        return;
    }

    private Nexus GetNearestEnemyNexus()
    {
        float dist = float.MaxValue;
        Nexus nearestNexus = null;
        foreach (Nexus Nexus in FindObjectsOfType<Nexus>())
        {
            if (Nexus.GetPlayerNumber() == m_playerOwner)
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

    [TargetRpc]
    public override void TargetPlaySoundCapture(NetworkConnection target)
    {
        m_alertManager.TimerVerificationCapturePIBarrack();
    }
    #endregion
}
