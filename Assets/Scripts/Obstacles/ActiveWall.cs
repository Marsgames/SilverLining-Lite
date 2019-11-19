#pragma warning disable CS0618 // type deprecated
#region Author
////////////////////////////////////////////
//   Guillaume Quiniou 
/////////////////////////////////////////
#endregion
using System.Collections;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Networking;

public class ActiveWall : ActiveObstacle
{
    #region Variables
    private bool m_isBridge = false;
    #endregion Variables

    #region Functions
    public override ObstacleType GetObstacleType()
    {
        return ObstacleType.SimpleWall;
    }

    [Server]
    protected override IEnumerator Construct(PlayerEntity.Player playerNumber)
    {
        GetComponentInChildren<NavMeshObstacle>().enabled = true;
        yield return base.Construct(playerNumber);
        BoxCollider[] boxColliders = GetComponentsInChildren<BoxCollider>();
        m_isBridge = false;
        Collider[] hitColliders = Physics.OverlapBox(boxColliders[1].bounds.center, boxColliders[1].bounds.size / 2);
        foreach (Collider col in hitColliders)
        {
            if (col.gameObject.layer == (int)Constant.ELayer.NavMesh && col.name != Constant.ListOfObject.s_wallSideCollider)
            {
                m_isBridge = true;
                break;
            }
        }
        if (m_isBridge)
        {
            hitColliders = Physics.OverlapBox(boxColliders[2].bounds.center, boxColliders[2].bounds.size / 2);
            foreach (Collider col in hitColliders)
            {
                if (col.name != name && col.gameObject.layer == (int)Constant.ELayer.NavMesh)
                {
                    m_isBridge = true;
                    break;
                }
                m_isBridge = false;
            }
        }
        if (m_isBridge)
        {
            bool isNavMeshBuilt = GameManager.Instance.GetNavMeshSurface().MyBuildNavMesh();
            yield return new WaitUntil(() => isNavMeshBuilt);
        }
        else
        {
            Destroy(boxColliders[1].gameObject);
            Destroy(boxColliders[2].gameObject);
        }
        RecalculatePath();
        GameManager.Instance.GetPlayer(playerNumber).TakeControl(GetComponent<NetworkIdentity>());
        yield return null;
    }

    [Server]
    protected override IEnumerator Destruct()
    {
        yield return base.Destruct();
        if (m_isBridge)
        {
            bool isNavMeshBuilt = GameManager.Instance.GetNavMeshSurface().MyBuildNavMesh();
            yield return new WaitUntil(() => isNavMeshBuilt);
        }
        RecalculatePath();
        NetworkServer.Destroy(gameObject);
        yield return null;
    }

    #endregion Functions
}
