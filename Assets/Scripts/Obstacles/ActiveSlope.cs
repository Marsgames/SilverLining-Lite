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

public class ActiveSlope : ActiveObstacle
{
    #region Variables   
    private enum SlopeType
    {
        Right,
        Left
    }
    [SerializeField] private SlopeType m_type = SlopeType.Left;
    private float m_obstacleLength = 3f;
    #endregion

    #region Unity's Functions

    [ServerCallback]
    private void OnTriggerStay(Collider other)
    {
        UnitController unit = other.GetComponent<UnitController>();

        if (!unit || m_obstacleConstructor.GetCurrentState() != ObstacleConstructor.EState.Invulnerable)
        {
            return;
        }

        if (transform.localPosition.y < 0)
        {
            Ray ray = new Ray(unit.transform.position + Vector3.up * 5f, Vector3.down);
            RaycastHit hit;
            if (transform.GetComponent<Collider>().Raycast(ray, out hit, 10f))
            {
                unit.GetAgent().enabled = false;
                unit.transform.position = hit.point;
            }
            unit.SetIsInSlope(true);
        }
        else
        {
            unit.SetIsInSlope(false);
        }
    }
    #endregion

    #region Functions    
    public override ObstacleType GetObstacleType()
    {
        switch (m_type)
        {
            case SlopeType.Right:
                return ObstacleType.SlopeRight;
            case SlopeType.Left:
                return ObstacleType.SlopeLeft;
            default:
                return ObstacleType.SlopeLeft;
        }
    } 

    protected override void InitPosition()
    {
        switch (m_type)
        {
            case SlopeType.Right:
                transform.localPosition = new Vector3(0, -m_obstacleHeight, -m_obstacleLength/2);
                break;
            case SlopeType.Left:
                transform.localPosition = new Vector3(0, -m_obstacleHeight, m_obstacleLength / 2);
                transform.Rotate(Vector3.up, 180);
                break;
        }
    }

    [Server]
    protected override IEnumerator Construct(PlayerEntity.Player playerNumber)
    {
        yield return base.Construct(playerNumber);
        bool isNavMeshBuilt = GameManager.Instance.GetNavMeshSurface().MyBuildNavMesh();
        yield return new WaitUntil(() => isNavMeshBuilt);
        RecalculatePath();
        GameManager.Instance.GetPlayer(playerNumber).TakeControl(GetComponent<NetworkIdentity>());
        yield return null;
    }

    [Server]
    protected override IEnumerator Destruct()
    {
        gameObject.layer = (int)Constant.ELayer.Default;
        bool isNavMeshBuilt = GameManager.Instance.GetNavMeshSurface().MyBuildNavMesh();
        yield return new WaitUntil(() => isNavMeshBuilt);
        yield return base.Destruct();
        RecalculatePath();
        NetworkServer.Destroy(gameObject);
        yield return null;
    }

    #endregion Functions
}
