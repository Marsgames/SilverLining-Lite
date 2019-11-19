#pragma warning disable CS0618 // type deprecated
#region Author
/////////////////////////////////////////
//   Guillaume Quiniou 
/////////////////////////////////////////
#endregion
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Networking;

[RequireComponent(typeof(Collider))]
public class CheckpointCollider : NetworkBehaviour
{
    #region Variables
    public enum ECollider
    {
        areaCollider,
        stockCollider,
        modelCollider
    };
    [SerializeField] private ECollider m_type = ECollider.areaCollider;
    private CheckpointBase m_checkpointBase;
    private SoundManager m_soundManager;
    #endregion

    #region Unity's function
    private void Start()
    {
        m_checkpointBase = transform.parent.GetComponent<CheckpointBase>();
        m_soundManager = SoundManager.Instance;
    }

    private void OnMouseDown()
    {
        if (EventSystem.current.IsPointerOverGameObject() || GameManager.Instance.GetEndGameBool())
        {
            return;
        }
        if (m_checkpointBase.GetPlayerOwner() != GameManager.Instance.GetLocalPlayer())
        {
            return;
        }
        if (m_checkpointBase.GetComponent<Barrack>())
        {
            m_checkpointBase.GetComponent<SpawnUnits>().ShowUISpawnUnit();
        }
        else if (m_type == ECollider.modelCollider)
        {
            m_checkpointBase.GetUIReleaseUnit().ShowUIRelease();
            m_soundManager.PlaySound(SoundManager.AudioClipList.AC_clickOnCP);
        }
    }

    [ServerCallback]
    private void OnTriggerExit(Collider other)
    {
        if (null == other || null == m_checkpointBase)
        {
            return;
        }

        if (m_type == ECollider.areaCollider)
        {
            m_checkpointBase.TriggerExit(other);
        }
    }
    [ServerCallback]
    private void OnTriggerEnter(Collider other)
    {
        if (m_type == ECollider.areaCollider)
        {
            m_checkpointBase.TriggerEnter(other);
        }
    }

    [ServerCallback]
    private void OnTriggerStay(Collider other)
    {
        if (m_type == ECollider.stockCollider)
        {
            m_checkpointBase.TriggerStay(other);
        }
    }
    #endregion

    #region Accessors
    public ECollider GetColliderType()
    {
        return m_type;
    }

    public CheckpointBase GetCheckpointBase()
    {
        return m_checkpointBase;
    }
    #endregion
}
