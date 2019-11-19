//#region Author
///////////////////////////////////////////
////   Yannig Smagghe --> #UIStockUnit#
////   https://gitlab.com/YannigSmagghe
///////////////////////////////////////////
//#endregion
//using UnityEngine;
//using UnityEngine.UI;

//public class UIStockUnit : UIBaseIcon
//{
//    #region Variables
//    [SerializeField] private InputManager m_inputManager = null;
//    [SerializeField] private UISpawnButton m_stockUnitButton = null;
//    //private GameObject m_checkpointParent;
//    #endregion

//    #region Unity's function

//    private void Awake()
//    {
//        m_inputManager.Checkpoint.ReleaseSlot1.performed += _ => ReleaseUnitFromCheckpoint(0);
//        m_inputManager.Checkpoint.ReleaseSlot2.performed += _ => ReleaseUnitFromCheckpoint(1);
//        m_inputManager.Checkpoint.ReleaseSlot3.performed += _ => ReleaseUnitFromCheckpoint(2);

//        m_inputManager.Checkpoint.ReleaseSlot1.Enable();
//        m_inputManager.Checkpoint.ReleaseSlot2.Enable();
//        m_inputManager.Checkpoint.ReleaseSlot3.Enable();
//    }

//    #endregion

//    #region Functions

//    public override void SetShowUi(GameObject parentPrefab, PlayerEntity.Player playerNumber)
//    {
//        base.SetShowUi(parentPrefab, playerNumber);
//        if (null == parentPrefab.GetComponent<CheckpointBase>())
//        {
//            if (parentPrefab.transform.parent.GetComponent<CheckpointBase>())
//            {
//                m_parentPrefab = parentPrefab.transform.parent.gameObject;
//            }
//        }
//    }

//    private void ReleaseUnitFromCheckpoint(int index)
//    {

//        if (transform.childCount > index)
//        {
//            Transform unitToRelease = transform.GetChild(index);
//            unitToRelease.GetComponent<UISpawnButton>().ReleaseUnit();
//        }
//    }

//    public void AddUIStockUnit(GameObject parentPrefab, PlayerEntity.Player playerNumber, GameObject unitGameObject, bool activate)
//    {
//        if (parentPrefab.GetComponent<CheckpointBase>())
//        {
//            m_parentPrefab = parentPrefab;
//            AddChildrenButton(unitGameObject, playerNumber);
//        }
//    }

//    private void AddChildrenButton(GameObject unitGameObject, PlayerEntity.Player playerNumber)
//    {
//        UISpawnButton stockUnitButton = Instantiate(m_stockUnitButton);
//        stockUnitButton.transform.SetParent(transform, false);
//        stockUnitButton.SetUnitStock(unitGameObject);
//        stockUnitButton.SetParentSpawnUnit(m_parentPrefab);
//        CombatUnit.UnitType untiSpawnType = unitGameObject.GetComponent<CombatUnit>().GetUnitType();
//        // Select image 1 to dont take your image
//        Image childImage = stockUnitButton.transform.GetComponentsInChildren<Image>()[1];
//        if (playerNumber == PlayerEntity.Player.Player2)
//        {
//            switch (untiSpawnType)
//            {
//                case CombatUnit.UnitType.Range:
//                    childImage.sprite = m_arcaRangeIcon;
//                    break;
//                case CombatUnit.UnitType.Warrior:
//                    childImage.sprite = m_arcaNormalIcon;
//                    break;
//                case CombatUnit.UnitType.Tank:
//                    childImage.sprite = m_arcaTankIcon;
//                    break;
//                case CombatUnit.UnitType.Scout:
//                    childImage.sprite = m_arcaScoutIcon;
//                    break;
//            }
//        }

//        if (playerNumber == PlayerEntity.Player.Player1)
//        {
//            switch (untiSpawnType)
//            {
//                case CombatUnit.UnitType.Range:
//                    childImage.sprite = m_forestRangeIcon;
//                    break;
//                case CombatUnit.UnitType.Warrior:
//                    childImage.sprite = m_forestNormalIcon;
//                    break;
//                case CombatUnit.UnitType.Tank:
//                    childImage.sprite = m_forestTankIcon;
//                    break;
//                case CombatUnit.UnitType.Scout:
//                    childImage.sprite = m_forestScoutIcon;
//                    break;
//            }
//        }
//    }

//    //public void CreateChildButton()
//    //{
//    //    Debug.Log("recreate button" + m_parentPrefab.GetComponent<CheckpointBase>().GetStockUnits().transform.childCount);
//    //    foreach (Transform child in transform)
//    //    {
//    //        Debug.Log("name chuld "+ child.name);
//    //        Destroy(child.gameObject);
//    //    }

//    //    foreach (Transform child in m_parentPrefab.GetComponent<CheckpointBase>().GetStockUnits().transform)
//    //    {
//    //        AddChildrenButton(child.gameObject, m_parentPrefab.transform.GetComponent<CheckpointBase>().GetPlayerOwner());
//    //    }
//    //}

//    #endregion
//}
