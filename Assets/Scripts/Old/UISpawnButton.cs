//#pragma warning disable CS0618 // type deprecated
//#region Author
///////////////////////////////////////////
////   Yannig Smagghe --> #UISpawnUnitButton#
////   https://gitlab.com/YannigSmagghe
///////////////////////////////////////////
//#endregion
//using System.Collections;
//using UnityEngine;
//using UnityEngine.EventSystems;
//using UnityEngine.UI;
//using UnityEngine.Experimental.Input;
//using System;

//[RequireComponent(typeof(Button))]
//public class UISpawnButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
//{
//    #region Variables
//    //[SerializeField] private InputManager m_inputManager = null;
//    [SerializeField] private GameObject m_tooltip = null;
//   // [SerializeField] private Constant.SpawnType m_spawnType = Constant.SpawnType.MediumGolem;
//    [SerializeField] private GameObject m_parentSpawnUnit = null;

//    private Text m_costText;
//    private Image m_greyImage;
//    private int m_unitPrice=0;
//    private Color m_greyImageColor;
//    private int m_playerGold;
//  //  private InputAction m_inputAction;
//    private GameObject m_unitStock;
//    private SoundManager m_soundManager;
//    #endregion

//    #region Unity's Function
//    private void Awake()
//    {
//        //switch (m_spawnType)
//        //{
//        //    case Constant.SpawnType.Warrior:                
//        //        m_inputAction = m_inputManager.Spawn.Warrior;
//        //        break;
//        //    case Constant.SpawnType.Range:
//        //        m_inputAction = m_inputManager.Spawn.Range;
//        //        break;
//        //    case Constant.SpawnType.Tank:
//        //        m_inputAction = m_inputManager.Spawn.Tank;
//        //        break;
//        //    case Constant.SpawnType.Scout:
//        //        m_inputAction = m_inputManager.Spawn.Scout;
//        //        break;
//        //    case Constant.SpawnType.SimpleWall:
//        //        m_inputAction = m_inputManager.Spawn.SimpleWall;
//        //        break;
//        //    case Constant.SpawnType.SlopeLeft:
//        //        m_inputAction = m_inputManager.Spawn.SlopeLeft;
//        //        break;
//        //    case Constant.SpawnType.SlopeRight:
//        //        m_inputAction = m_inputManager.Spawn.SlopeRight;
//        //        break;
//        //    case Constant.SpawnType.Pillar:
//        //        m_inputAction = m_inputManager.Spawn.Pillar;
//        //        break;
//        //    case Constant.SpawnType.Trap:
//        //        m_inputAction = m_inputManager.Spawn.Trap;
//        //        break;
//        //}
//        //if (null != m_inputAction)
//        //{
//        //    m_inputAction.performed += _ => InputHandler();
//        //    if (!m_inputAction.enabled)
//        //    {
//        //        m_inputAction.Enable();
//        //    }
//        //}
     

//    }

//    private void Start()
//    {
//        if (transform.parent.GetComponent<UIStockUnit>())
//        {
//            m_soundManager = SoundManager.Instance;
//            CheckIfOk();
//            return;
//        }
//        m_greyImage = transform.Find(Constant.ListOfMisc.s_Image).Find(Constant.ListOfMisc.s_Grey).GetComponent<Image>();
//        m_greyImageColor = m_greyImage.color;
//        m_costText = transform.Find(Constant.ListOfMisc.s_Image).Find(Constant.ListOfMisc.s_CostText).GetComponent<Text>();
//        if (m_tooltip.GetComponent<UITootltipUnit>())
//        {
//            //m_unitPrice = GameManager.Instance.GetUnitForPlayer(m_spawnType).GetComponent<UnitController>().GetUnitCost();
//            //transform.Find(Constant.ListOfMisc.s_Image).Find(Constant.ListOfMisc.s_CostText).GetComponent<Text>().text = m_unitPrice.ToString();
//        }
//        else if (m_tooltip.GetComponent<UITootltipObstacle>())
//        {
//           // m_unitPrice = m_parentSpawnUnit.GetComponent<ObstacleConstructor>().GetObstacleCost(m_spawnType);
//        }
//        m_costText.text = m_unitPrice.ToString();
//    }

//    public void Update()
//    {
//        if (transform.parent.GetComponent<UIStockUnit>() && null == m_parentSpawnUnit)
//        {
//            UIStockUnit parentTemp = transform.parent.GetComponent<UIStockUnit>();
//            m_parentSpawnUnit = parentTemp.GetParentPrefab();
//        }
//        if (m_parentSpawnUnit.GetComponent<SpawnUnits>())
//        {
//            if (null != GameManager.Instance.GetLocalPlayerEntity())
//            {
//                m_playerGold = GameManager.Instance.GetLocalPlayerEntity().GetUnitGold();
//                if (m_playerGold >= m_unitPrice)
//                {
//                    m_greyImageColor.a = .0f;
//                }
//                else
//                {
//                    m_greyImageColor.a = .68f;
//                }

//                m_greyImage.color = m_greyImageColor;
//            }
//        }
//        // Code for input keyboard wait new input
//        else if (m_parentSpawnUnit.GetComponent<ObstacleConstructor>())
//        {
//            if (null != GameManager.Instance.GetLocalPlayerEntity())
//            {
//                m_playerGold = GameManager.Instance.GetLocalPlayerEntity().GetBuildGold();
//                if (m_playerGold >= m_unitPrice)
//                {
//                    m_greyImageColor.a = .0f;
//                }
//                else
//                {
//                    m_greyImageColor.a = .68f;
//                }

//                m_greyImage.color = m_greyImageColor;
//            }

//            //if (Input.GetButtonDown(Constant.GetSpawnTypeButton(m_spawnType)) && m_parentSpawnUnit.GetComponent<ObstacleConstructor>().GetCurrentState() == ObstacleConstructor.EState.Buildable)
//            //{
//            //    m_parentSpawnUnit.GetComponent<ObstacleConstructor>().GetUISpawnObstacle().gameObject.transform.parent.gameObject.SetActive(false);
//            //    m_parentSpawnUnit.GetComponent<ObstacleConstructor>().DeleteShowObstacle();
//            //    PlayerEntity player = GameManager.Instance.GetLocalPlayerEntity();
//            //    //player.CmdCreateObstacle(m_parentSpawnUnit, m_spawnType);
//            //}
//        }
//    }

//    public void OnPointerEnter(PointerEventData eventData)
//    {
//        if (transform.parent.GetComponent<UIStockUnit>())
//        {
//            return;
//        }
//        m_tooltip.SetActive(true);
//        //if (m_parentSpawnUnit.GetComponent<SpawnUnits>())
//        //{
//        //    m_tooltip.GetComponent<UITootltipUnit>().ShowTooltip(m_parentSpawnUnit.GetComponent<SpawnUnits>().GetPrefabUnit(m_spawnType), m_spawnType);
//        //}
//        //if (m_parentSpawnUnit.GetComponent<ObstacleConstructor>())
//        //{
//        //    //m_tooltip.GetComponent<UITootltipObstacle>().ShowTooltip(m_parentSpawnUnit.GetComponent<ObstacleConstructor>().GetObstacleToSpawn(m_spawnType, Constant.Player.Player1), m_spawnType);
//        //    //m_parentSpawnUnit.GetComponent<ObstacleConstructor>().ShowObstacle(m_spawnType, GameManager.Instance.GetLocalPlayer());
//        //}
//    }

//    public void OnPointerExit(PointerEventData eventData)
//    {
//        if (null != m_tooltip)
//        {
//            m_tooltip.SetActive(false);
//        }
//        //if (m_parentSpawnUnit.GetComponent<ObstacleConstructor>())
//        //{
//        //    m_parentSpawnUnit.GetComponent<ObstacleConstructor>().DeleteShowObstacle();
//        //}
//    }

//    #endregion

//    #region Function

//    public void TaskOnClick()
//    {
//        //if (m_parentSpawnUnit.GetComponent<SpawnUnits>())
//        //{
//        //    m_parentSpawnUnit.GetComponent<SpawnUnits>().CmdAddToUnitsQueue(m_spawnType);

//        //}
//        // else
//        //if (m_parentSpawnUnit.GetComponent<ObstacleConstructor>() && m_parentSpawnUnit.GetComponent<ObstacleConstructor>().GetCurrentState() == ObstacleConstructor.EState.Buildable)
//        //{
//        //    m_parentSpawnUnit.GetComponent<ObstacleConstructor>().GetUISpawnObstacle().gameObject.transform.parent.gameObject.SetActive(false);
//        //    m_parentSpawnUnit.GetComponent<ObstacleConstructor>().DeleteShowObstacle();
//        //    PlayerEntity player = GameManager.Instance.GetLocalPlayerEntity();
//        //  //  player.CmdCreateObstacle(m_parentSpawnUnit, m_spawnType);
//        //}
//        //else
//        if (m_parentSpawnUnit.GetComponent<CheckpointBase>())
//        {
//            ReleaseUnit();
//        }
//        EventSystem.current.SetSelectedGameObject(m_parentSpawnUnit.gameObject);
//    }

//    private void InputHandler()
//    {
//        if (!gameObject.activeInHierarchy)
//        {
//            return;
//        }
//        //if (m_parentSpawnUnit.GetComponent<SpawnUnits>())
//        //{
//        //    m_parentSpawnUnit.GetComponent<SpawnUnits>().CmdAddToUnitsQueue(m_spawnType);
//        //    StartCoroutine(SelectButton());
//        //}
//        //else 
//        //if (m_parentSpawnUnit.GetComponent<ObstacleConstructor>() &&
//        //    m_parentSpawnUnit.GetComponent<ObstacleConstructor>().GetCurrentState() == ObstacleConstructor.EState.Buildable)
//        //{
//        //    m_parentSpawnUnit.GetComponent<ObstacleConstructor>().GetUISpawnObstacle().gameObject.transform.parent.gameObject.SetActive(false);
//        //    m_parentSpawnUnit.GetComponent<ObstacleConstructor>().DeleteShowObstacle();
//        //    PlayerEntity player = GameManager.Instance.GetLocalPlayerEntity();
//        //    //player.CmdCreateObstacle(m_parentSpawnUnit, m_spawnType);
//        //}
       
//    }

//    private IEnumerator SelectButton()
//    {
//        GetComponent<Button>().Select();
//        yield return new WaitForSeconds(0.1f);
//        EventSystem.current.SetSelectedGameObject(null);
//    }

//    public void ReleaseUnit()
//    {
//        if (m_parentSpawnUnit.GetComponent<CheckpointBase>())
//        {
//            //GameManager.Instance.GetLocalPlayerEntity().CmdReleaseUnit(m_parentSpawnUnit,m_unitStock,gameObject);
//        }

//        Destroy(gameObject);
//        m_soundManager.PlaySound(SoundManager.AudioClipList.AC_releaseOneUnit);
//        EventSystem.current.SetSelectedGameObject(m_parentSpawnUnit.gameObject);
//    }
//    #endregion

//    #region Accessors
//    public void SetParentSpawnUnit(GameObject parent)
//    {
//        m_parentSpawnUnit = parent;
//    }
//    public void SetUnitStock(GameObject unitStock)
//    {
//        m_unitStock = unitStock;
//    }

//    public GameObject GetUnitStock()
//    {
//       return m_unitStock;
//    }
//    #endregion

//    private void CheckIfOk()
//    {
//#if UNITY_EDITOR
//        if (null == m_soundManager)
//        {
//            Debug.LogError("ALLLOOO ALLLOOOO le sound manager ne peut pas être null dans " + name, this);
//            UnityEditor.EditorApplication.isPlaying = false;
//        }
//#endif
//    }
//}