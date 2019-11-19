#pragma warning disable CS0618 // type deprecated
#region Author
/////////////////////////////////////////
//   Yannig Smagghe
//   https://gitlab.com/YannigSmagghe
/////////////////////////////////////////
#endregion
using System.Collections;
using UnityEngine;
using UnityEngine.Networking;

public class ActivePillar : ActiveObstacle
{
    #region Functions
    public override ObstacleType GetObstacleType()
    {
        return ObstacleType.Pillar;
    }

    [Server]
    protected override IEnumerator Construct(PlayerEntity.Player playerNumber)
    {
        yield return base.Construct(playerNumber);
        bool isNavMeshBuilt = GameManager.Instance.GetNavMeshSurface().MyBuildNavMesh();
        yield return new WaitUntil(() => isNavMeshBuilt);
        RecalculatePath();
        GameManager.Instance.GetPlayer(playerNumber).TakeControl(GetComponent<NetworkIdentity>());
    }

    [Server]
    protected override IEnumerator Destruct()
    {
        yield return base.Destruct();
        bool isNavMeshBuilt = GameManager.Instance.GetNavMeshSurface().MyBuildNavMesh();
        yield return new WaitUntil(() => isNavMeshBuilt);
        RecalculatePath();
        NetworkServer.Destroy(gameObject);
    }
    #endregion Functions
}
