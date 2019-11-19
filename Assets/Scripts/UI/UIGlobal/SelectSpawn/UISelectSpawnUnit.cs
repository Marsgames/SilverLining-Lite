#region Author
/////////////////////////////////////////
//   Guillaume Quiniou
/////////////////////////////////////////
#endregion
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class UISelectSpawnUnit : MonoBehaviour
{

    #region Variables
    [SerializeField] protected ESpawnUnit m_type = ESpawnUnit.Top;
    [SerializeField] protected Sprite m_iconSyca = null;
    [SerializeField] protected Sprite m_iconArca = null;

    private UIGlobal m_uiGlobal;
    protected SpawnUnits m_spawnUnits;

    protected enum ESpawnUnit
    {
        Top,
        Bot
    }
    #endregion Variables

    #region Unity's functions

    private IEnumerator Start()
    {
        yield return new WaitUntil(() => GameManager.Instance != null);
        yield return new WaitUntil(GameManager.Instance.GetAllPlayersReady);

        m_uiGlobal = UIGlobal.Instance;
        StartCoroutine(SetSpawnUnit());       
    }
    #endregion Unity's functions

    #region Functions
    /// <summary>
    /// Handle click on this button
    /// </summary>
    public virtual void TaskOnClick()
    {
        m_spawnUnits.GetComponent<ObjectSelection>().Select();
        GameManager.Instance.GetLocalPlayerEntity().SetSelectedItem(m_spawnUnits.GetComponent<ObjectSelection>());
        m_spawnUnits.ShowUISpawnUnit();
        OnSelection();
    }


    /// <summary>
    /// Set spawnunit depending on the localplayer and the ESpawnUnit of this
    /// </summary>
    protected virtual IEnumerator SetSpawnUnit()
    {
        string tag;
        if (m_type == ESpawnUnit.Top)
        {
            tag = Constant.ListOfTag.s_spawnUnit1;
        }
        else
        {
            tag = Constant.ListOfTag.s_spawnUnit2;
        }
        yield return new WaitWhile(() => GameManager.Instance.GetSpawnUnitForPlayer(GameManager.Instance.GetLocalPlayer(), tag) == null);
        m_spawnUnits = GameManager.Instance.GetSpawnUnitForPlayer(GameManager.Instance.GetLocalPlayer(), tag);
        m_spawnUnits.SetUISelectSpawnUnit(this);
        SetIcons();

    } 
    /// <summary>
    /// Set icons
    /// </summary>
    public virtual void SetIcons()
    {
        Sprite sprite = GetComponent<Image>().sprite;
        PlayerEntity.Player player = GameManager.Instance.GetLocalPlayer();

        if (player == PlayerEntity.Player.Player1)
        {
            sprite = m_iconSyca;
        }
        else
        {
            sprite = m_iconArca;
        }
        GetComponent<Image>().sprite = sprite;
        if (m_type == ESpawnUnit.Top)
        {
            OnSelection();
        }
    }

    public void OnSelection()
    {
        if (CameraManager.Instance.GetCameraState() == CameraManager.ECamState.Ortho3D)
        {
            foreach (Transform selectSpawn in m_uiGlobal.GetUISelectSpawnUnit().transform)
            {
                selectSpawn.Find(Constant.ListOfMisc.s_Grey).gameObject.SetActive(false);
            }
            transform.Find(Constant.ListOfMisc.s_Grey).gameObject.SetActive(true);
        }
    }
    #endregion Functions
}
