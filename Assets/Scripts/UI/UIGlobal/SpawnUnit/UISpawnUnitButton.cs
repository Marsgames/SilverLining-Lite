#region Author
/////////////////////////////////////////
//  Guillaume Quiniou
/////////////////////////////////////////
#endregion
using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Experimental.Input;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class UISpawnUnitButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    #region Variables
    [SerializeField] private InputManager m_inputManager = null;
    [SerializeField] private GameObject m_tooltip = null;
    [SerializeField] private Constant.SpawnTypeUnit m_unitType = Constant.SpawnTypeUnit.Warrior;

    [SerializeField] private Sprite m_spriteArca = null;
    [SerializeField] private Sprite m_spriteSyca = null;

    private SpawnUnits m_spawnUnits;
    private InputAction m_inputAction;
    private int m_playerGold;
    private int m_unitPrice;
    private Color m_greyImageColor;
    private Image m_greyImage;
    private Text m_costText;
    private Image m_imageUnit;

    #endregion Variables

    #region Unity's function
    private void Awake()
    {
        switch (m_unitType)
        {
            case Constant.SpawnTypeUnit.Warrior:
                m_inputAction = m_inputManager.Spawn.Warrior;
                break;
            case Constant.SpawnTypeUnit.Range:
                m_inputAction = m_inputManager.Spawn.Range;
                break;
            case Constant.SpawnTypeUnit.Tank:
                m_inputAction = m_inputManager.Spawn.Tank;
                break;
            case Constant.SpawnTypeUnit.Scout:
                m_inputAction = m_inputManager.Spawn.Scout;
                break;
        }
        m_inputAction.performed += _ => InputHandler();
    }

    private IEnumerator Start()
    {
        m_imageUnit = transform.Find(Constant.ListOfMisc.s_Image).GetComponent<Image>();
        m_greyImage = transform.Find(Constant.ListOfMisc.s_Image).Find(Constant.ListOfMisc.s_Grey).GetComponent<Image>();
        m_greyImageColor = m_greyImage.color;
        m_costText = transform.Find(Constant.ListOfMisc.s_Image).Find(Constant.ListOfMisc.s_CostText).GetComponent<Text>();
        yield return new WaitUntil(() => GameManager.Instance != null);
        yield return new WaitUntil(() => GameManager.Instance.GetAllPlayersReady());

        m_unitPrice = GameManager.Instance.GetUnitForPlayer(m_unitType).GetComponent<UnitController>().GetUnitCost();
        transform.Find(Constant.ListOfMisc.s_Image).Find(Constant.ListOfMisc.s_CostText).GetComponent<Text>().text = m_unitPrice.ToString();
    }

    private void OnEnable()
    {
        m_inputAction.Enable();
    }

    private void OnDisable()
    {
        m_inputAction.Disable();
    }

    private void Update()
    {
        if (null != GameManager.Instance && null != GameManager.Instance.GetLocalPlayerEntity())
        {
            m_playerGold = GameManager.Instance.GetLocalPlayerEntity().GetUnitGold();
            if (m_playerGold >= m_unitPrice)
            {
                m_greyImageColor.a = .0f;
            }
            else
            {
                m_greyImageColor.a = .68f;
            }

            m_greyImage.color = m_greyImageColor;
        }

    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        m_tooltip.SetActive(true);
        m_tooltip.GetComponent<UITootltipUnit>().ShowTooltip(GameManager.Instance.GetUnitForPlayer(m_unitType).GetComponent<UnitController>());
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (null != m_tooltip)
        {
            m_tooltip.SetActive(false);
        }
    }
    #endregion Unity's function

    #region Functions
    /// <summary>
    /// Handle input to spawn unit
    /// </summary>
    public void InputHandler()
    {
        if (null == transform)
        {
            return;
        }

        if (!isActiveAndEnabled)
        {
            return;
        }
        m_spawnUnits.CmdAddToUnitsQueue(m_unitType);
        StartCoroutine(SelectButton());
    }

    /// <summary>
    /// Visual effect on button when using input
    /// </summary>
    /// <returns></returns>
    private IEnumerator SelectButton()
    {
        GetComponent<Button>().Select();
        yield return new WaitForSeconds(0.1f);
        EventSystem.current.SetSelectedGameObject(null);
    }
    #endregion

    #region Accessors

    public void SetSpawnUnit(SpawnUnits spawnUnit)
    {
        m_spawnUnits = spawnUnit;
        if (GameManager.Instance.GetLocalPlayer() == PlayerEntity.Player.Player1)
        {
            m_imageUnit.sprite = m_spriteSyca;
        }
        else if (GameManager.Instance.GetLocalPlayer() == PlayerEntity.Player.Player2)
        {
            m_imageUnit.sprite = m_spriteArca;
        }
    }
    #endregion Accessors
}
