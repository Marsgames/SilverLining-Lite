//#region Author
///////////////////////////////////////////
////   Yannig Smagghe 
////   https://gitlab.com/YannigSmagghe
///////////////////////////////////////////
//#endregion
//using System;
//using System.Collections.Generic;
//using UnityEngine;
//using UnityEngine.EventSystems;

//public abstract class UIBase : MonoBehaviour
//{
//    #region Variables
//    [SerializeField] private List<GameObject> m_listUIActivables = new List<GameObject>();

//    protected PlayerEntity.Player m_playerNumber;
//    protected bool m_showUi;
//    protected GameObject m_parentPrefab;

//    public struct ListOfChild
//    {
//        public static readonly string s_rangeUnit = "Range";
//        public static readonly string s_warriorUnit = "Warrior";
//        public static readonly string s_tankUnit = "Tank";
//        public static readonly string s_scoutUnit = "Scout";
//        public static readonly string s_simpleWall = "SimpleWall";
//        public static readonly string s_pillar = "Pillar";
//        public static readonly string s_slopeWallRight = "SlopeRight";
//        public static readonly string s_slopeWallLeft = "SlopeLeft";
//        public static readonly string s_trap = "Trap";
//    }

//    protected EventSystem m_EventSystem;

//    private GameObject m_currentUi;
//    #endregion
//    #region Unity's functions
//    void Start()
//    {
//        m_EventSystem = EventSystem.current;
//    }

//    void Update()
//    {
//        if (m_EventSystem == null)
//        {
//            m_EventSystem = EventSystem.current;
//        }
//    }
//    #endregion

//    #region Functions
//    public virtual void SetShowUi(GameObject parentPrefab, PlayerEntity.Player playerNumber)
//    {
//        m_showUi = true;
//        m_playerNumber = playerNumber;
//        DeactivateListUIActivable();
//        m_parentPrefab = parentPrefab;
//        gameObject.SetActive(true);
//    }

//    public void DeactivateListUIActivable()
//    {
//        //foreach (GameObject uiGameObject in m_listUIActivables)
//        //{
//        //    uiGameObject.SetActive(false);
//        //}

//        GameManager.Instance.CleanUiObstacles();
//        GameManager.Instance.CleanUiCheckpoint();
//    }

//    public void CleanUI()
//    {
//        foreach (Transform child in transform)
//        {
//            Destroy(child.gameObject);
//        }
//    }

//    public void ToggleChildActive(bool activate)
//    {
//        for (int i = 0; i < transform.childCount; i++)
//        {
//            transform.GetChild(i).gameObject.SetActive(activate);
//        }
//    }

//    public GameObject GetParentPrefab()
//    {
//        return m_parentPrefab;
//    }

//    public void SetParentPrefab(GameObject parentPrefab)
//    {
//        m_parentPrefab = parentPrefab;
//        foreach (Transform child in transform)
//        {
//            if (child.GetComponent<UISpawnButton>())
//            {
//                child.GetComponent<UISpawnButton>().SetParentSpawnUnit(parentPrefab);
//            }
//        }
//    }
//    #endregion
//}
