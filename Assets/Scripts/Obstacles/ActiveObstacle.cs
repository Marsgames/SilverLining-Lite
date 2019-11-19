#pragma warning disable CS0618 // type deprecated
#region Author
/////////////////////////////////////////
//   RAPHAËL DAUMAS
//   https://raphdaumas.wixsite.com/portfolio
//   https://github.com/Marsgames
/////////////////////////////////////////
//   Guillaume Quiniou 
/////////////////////////////////////////
#endregion
using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Networking;

public abstract class ActiveObstacle : NetworkBehaviour
{
    #region Variables
    public enum ObstacleType
    {
        SimpleWall,
        Pillar,
        SlopeRight,
        SlopeLeft,
        Trap
    }

    [SerializeField, Range(0, 2)] protected int m_buildingCost = 1;
    [SerializeField] private Texture2D m_cursorBomb = null;
    [SerializeField] private Texture2D m_cursorDeconstruction = null;

    [SyncVar] protected PlayerEntity.Player m_playerNumber = PlayerEntity.Player.Neutre;

    protected ObstacleType m_obstacleType;
    protected ObstacleConstructor m_obstacleConstructor;
    protected Vector3 m_initPosition;
    protected ParticleSystem m_bombEffect;
    private SoundManager m_soundManager;
    protected float m_obstacleHeight = 2f;
    #endregion

    #region Unity's functions
    private void Start()
    {
        m_soundManager = SoundManager.Instance;      
    }

    public void OnMouseDown()
    {
        if (!EventSystem.current.IsPointerOverGameObject())
        {

            if (m_obstacleConstructor.GetCurrentState() == ObstacleConstructor.EState.Invulnerable)
            {
                m_soundManager.PlaySound(SoundManager.AudioClipList.AC_obstableImpossibleInteract);
            }

            if (m_playerNumber == GameManager.Instance.GetLocalPlayer())
            {
                if (m_obstacleConstructor.GetCurrentState() == ObstacleConstructor.EState.Built)
                {
                    CmdDestroyObstacle();
                    m_soundManager.PlaySound(SoundManager.AudioClipList.AC_wallDeconstruct);
                }
            }
            else
            {
                PlayerEntity localPlayer = GameManager.Instance.GetLocalPlayerEntity();

                if (m_obstacleConstructor.GetCurrentState() == ObstacleConstructor.EState.Built &&
                    localPlayer.GetComponent<Bomb>().GetBombStack() > 0)
                {
                    localPlayer.GetComponent<Bomb>().UseBombStack();
                    localPlayer.CmdBomb(gameObject);
                }
            }
        }
    }

    public void OnMouseOver()
    {
        if (!EventSystem.current.IsPointerOverGameObject())
        {
            if (m_obstacleConstructor.GetCurrentState() == ObstacleConstructor.EState.Built)
            {
                if (m_playerNumber != GameManager.Instance.GetLocalPlayer())
                {
                    Cursor.SetCursor(m_cursorBomb, new Vector2(32, 32), CursorMode.Auto);
                }
                else
                {
                    Cursor.SetCursor(m_cursorDeconstruction, new Vector2(32, 32), CursorMode.Auto);
                }
            }
            else
            {
                Cursor.SetCursor(null, Vector2.zero, CursorMode.Auto);
            }
        }
    }

    public void OnMouseExit()
    {
        Cursor.SetCursor(null, Vector2.zero, CursorMode.Auto);
    }
    #endregion

    #region Function  
    public abstract ObstacleType GetObstacleType();
    /// <summary>
    /// Start and run animation of construction for this obstacle
    /// </summary>
    /// <param name="playerNumber">The player who construct this obstacle</param>
    [Server]
    protected virtual IEnumerator Construct(PlayerEntity.Player playerNumber)
    {
        m_playerNumber = playerNumber;
        Vector3 destination = new Vector3(transform.localPosition.x, 0, transform.localPosition.z);
        while (transform.localPosition.y < destination.y)
        {
            transform.localPosition = Vector3.MoveTowards(transform.localPosition, destination,
                m_obstacleHeight * Time.deltaTime / m_obstacleConstructor.GetBuildingTime());
            yield return null;
        }
        transform.localPosition = destination;
        yield return null;
    }

    /// <summary>
    /// Start and run animation of destruction for this obstacle
    /// </summary>    
    [Server]
    protected virtual IEnumerator Destruct()
    {
        if (GameManager.Instance.GetLocalPlayer() != PlayerEntity.Player.Bot)
        {
            GetComponent<NetworkIdentity>().RemoveClientAuthority(GetComponent<NetworkIdentity>().clientAuthorityOwner);
        }
        while (transform.position != m_initPosition)
        {
            transform.position = Vector3.MoveTowards(transform.position, m_initPosition,
               m_obstacleHeight * Time.deltaTime / m_obstacleConstructor.GetDeconstructionTime());
            yield return null;
        }
        m_obstacleConstructor.RpcSetCurrentState(ObstacleConstructor.EState.Resting);
        m_obstacleConstructor.RpcEnableObstacleConstructor(true);
        yield return null;
    }

    /// <summary>
    /// Each unit in the scene recalculate her path if needed
    /// </summary>
    [Server]
    protected void RecalculatePath()
    {
        foreach (var unit in FindObjectsOfType<UnitController>())
        {
            if (unit.GetComponent<CombatUnit>().GetCurrentState() == CombatUnit.State.Dead)
            {
                return;
            }

            if (unit.GetIsInSlope())
            {
                return;
            }
            else
            {
                unit.GetAgent().enabled = true;
            }

            if (unit.GetAgent() != null && !unit.GetAgent().isActiveAndEnabled)
            {
                return;
            }

            if (unit.TestIfRecalculatePathUnit(m_obstacleConstructor))
            {
                
                unit.RecalculatePathUnit();
            }
        }
    }

    /// <summary>
    /// Refund player owner and start destruction animation
    /// </summary>
    [Server]
    private void DestroyObstacle()
    {
        if (m_obstacleConstructor.GetCurrentState() == ObstacleConstructor.EState.Built)
        {
            GameManager.Instance.GetPlayer(m_playerNumber).GainBuildGold(m_buildingCost);
            m_obstacleConstructor.RpcSetCurrentState(ObstacleConstructor.EState.BeeingDestruct);
            StartCoroutine(Destruct());
        }
    }

    [Command]
    public void CmdDestroyObstacle()
    {
        DestroyObstacle();
    }

    /// <summary>
    /// Start bomb animation and call DestroyObstacle
    /// </summary>
    [Command]
    public void CmdBomdObstacle()
    {
        DestroyObstacle();
        RpcBombAnimation();
    }

    [ClientRpc]
    private void RpcBombAnimation()
    {
        m_soundManager.PlaySound(SoundManager.AudioClipList.AC_bombUsed);
        m_bombEffect.Play();
    }

    /// <summary>
    /// Setup all the obstacle, then start the animation of construction
    /// </summary>
    /// <param name="parent">GameObject of the obstacle constructor</param>
    /// <param name="playerNumber">The player who build this obstacle</param>
    [ClientRpc]
    public void RpcInitOnConstruction(GameObject parent, PlayerEntity.Player playerNumber)
    {
        m_obstacleConstructor = parent.GetComponent<ObstacleConstructor>();
        transform.SetParent(parent.transform);
        transform.rotation = parent.transform.rotation;
        m_bombEffect = parent.transform.GetComponentInChildren<ParticleSystem>();
        m_obstacleConstructor.SetCurrentState(ObstacleConstructor.EState.Invulnerable);
        GetComponentInChildren<ParticleSystem>().Play();

        InitPosition();

        m_initPosition = transform.position;
        if (isServer)
        {
           StartCoroutine(Construct(playerNumber));
        }
    }

    /// <summary>
    /// Need to be override for slope only
    /// </summary>
    protected virtual void InitPosition()
    {
        transform.localPosition = new Vector3(0, -m_obstacleHeight, 0);
    }

    #endregion

    #region Accessors
    public int GetBuildingCost()
    {
        return m_buildingCost;
    }

    public PlayerEntity.Player GetPlayerNumber()
    {
        return m_playerNumber;
    }
    #endregion

}