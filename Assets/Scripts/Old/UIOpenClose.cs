//#region Author
///////////////////////////////////////////
////   Yannig Smagghe --> #UIOpenCloseCheckpoint#
////   https://gitlab.com/YannigSmagghe
///////////////////////////////////////////
//#endregion
//using UnityEngine;
//using UnityEngine.EventSystems;

//public class UIOpenClose : UIBase
//{
//    #region Variables
//    [SerializeField] protected Sprite m_iconOpen;
//    [SerializeField] protected Sprite m_iconClose;
//    PlayerEntity.Player player;
//    #endregion

//    #region Unity's function
//    void Start()
//    {
//        PlayerEntity.Player player = GameManager.Instance.GetLocalPlayer();        
//    }
//    #endregion

//    #region Function
//    public override void SetShowUi(GameObject parentPrefab, PlayerEntity.Player playerNumber)
//    {
//        if (parentPrefab.transform.parent.GetComponent<TowerPlayer>() && playerNumber != GameManager.Instance.GetLocalPlayer())
//        {
//            //DeactivateListUIActivable();
//            return;
//        }

//        base.SetShowUi(parentPrefab, playerNumber);
//        CheckpointBase parentCheckpoint = m_parentPrefab.transform.parent.GetComponent<CheckpointBase>();
//        if (parentCheckpoint)
//        {
//            //parentCheckpoint.GetUIStockUnit().gameObject.SetActive(true);
//            if (parentCheckpoint.GetPlayerOwner() == GameManager.Instance.GetLocalPlayer())
//            {
//                GameObject stocksUnits = parentCheckpoint.GetStockUnits();
//                parentCheckpoint.GetUIStockUnit().CleanUI();
//                foreach (Transform child in stocksUnits.transform)
//                {
//                    parentCheckpoint.GetUIStockUnit().AddUIStockUnit(m_parentPrefab.transform.parent.gameObject, playerNumber, child.gameObject, true);
//                }
//                if (parentCheckpoint.GetComponent<SpawnUnits>())
//                {
//                //    UISpawn uiSpawn = parentCheckpoint.GetComponent<SpawnUnits>().GetUiSpawnUnit();
//                //    uiSpawn.SetParentPrefab(parentCheckpoint.gameObject);
//                //    uiSpawn.gameObject.SetActive(true);
//                }
//            }
//            else
//            {
//                parentCheckpoint.GetUIStockUnit().CleanUI();
//            }
//        }
//    }


//    //public void TaskOnClick()
//    //{
//    //    OpenCloseCheckpoint();
//    //}

//    //public void Update()
//    //{
//    //    if (Input.GetButtonDown(Constant.ListOfShortcut.s_openCloseCheckpoint))
//    //    {
//    //        OpenCloseCheckpoint();
//    //    }
//    //}

//    //void OpenCloseCheckpoint()
//    //{
//    //    if (m_parentPrefab.GetComponent<CheckpointBase>())
//    //    {
//    //        GameManager.Instance.GetLocalPlayerEntity().CmdSetCheckpointOpen(m_parentPrefab);
//    //    }
//    //    else if (m_parentPrefab.transform.parent.GetComponent<CheckpointBase>())
//    //    {
//    //        GameManager.Instance.GetLocalPlayerEntity().CmdSetCheckpointOpen(m_parentPrefab.transform.parent.gameObject);
//    //    }
//    //    EventSystem.current.SetSelectedGameObject(m_parentPrefab.gameObject);
//    //}
//    #endregion

//    #region Accessors

//    public Sprite GetIconOpen()
//    {
//        return m_iconOpen;
//    }

//    public Sprite GetIconClose()
//    {
//        return m_iconClose;
//    }
//    #endregion
//}
