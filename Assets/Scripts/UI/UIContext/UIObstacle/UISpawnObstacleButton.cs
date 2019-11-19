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
public class UISpawnObstacleButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    #region Variables
    [SerializeField] private InputManager m_inputManager = null;
    [SerializeField] private GameObject m_tooltip = null;
    [SerializeField] private ActiveObstacle.ObstacleType m_obstacleType = ActiveObstacle.ObstacleType.SimpleWall;

    [SerializeField] private Sprite m_spriteArca = null;
    [SerializeField] private Sprite m_spriteSyca = null;

    private ObstacleConstructor m_obstacleConstructor;
    private InputAction m_inputAction;
    private int m_playerGold;
    private int m_obstaclePrice;
    private Color m_greyImageColor;
    private Image m_greyImage;
    private Text m_costText;
    private Image m_imageUnit;

    #endregion Variables

    #region Unity's function
    protected void Awake()
    {
        switch (m_obstacleType)
        {
            case ActiveObstacle.ObstacleType.SimpleWall:
                m_inputAction = m_inputManager.Spawn.SimpleWall;
                break;
            case ActiveObstacle.ObstacleType.SlopeLeft:
                m_inputAction = m_inputManager.Spawn.SlopeLeft;
                break;
            case ActiveObstacle.ObstacleType.SlopeRight:
                m_inputAction = m_inputManager.Spawn.SlopeRight;
                break;
            case ActiveObstacle.ObstacleType.Pillar:
                m_inputAction = m_inputManager.Spawn.Pillar;
                break;
            case ActiveObstacle.ObstacleType.Trap:
                m_inputAction = m_inputManager.Spawn.Trap;
                break;
        }
        m_inputAction.performed += _ => InputHandler();
        m_inputAction.Enable();
    }

    private void Start()
    {
        m_imageUnit = transform.Find(Constant.ListOfMisc.s_Image).GetComponent<Image>();
        m_greyImage = transform.Find(Constant.ListOfMisc.s_Image).Find(Constant.ListOfMisc.s_Grey).GetComponent<Image>();
        m_greyImageColor = m_greyImage.color;
        m_costText = transform.Find(Constant.ListOfMisc.s_Image).Find(Constant.ListOfMisc.s_CostText).GetComponent<Text>();       
    } 

    private void Update()
    {
        if (null != GameManager.Instance.GetLocalPlayerEntity())
        {
            m_playerGold = GameManager.Instance.GetLocalPlayerEntity().GetBuildGold();
            if (m_playerGold >= m_obstaclePrice)
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

        m_tooltip.GetComponent<UITootltipObstacle>().ShowTooltip(m_obstacleConstructor.GetObstacleToSpawn(m_obstacleType, PlayerEntity.Player.Player1).GetComponent<ActiveObstacle>());
        m_obstacleConstructor.PreviewObstacle(m_obstacleType, GameManager.Instance.GetLocalPlayer());
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (null != m_tooltip)
        {
            m_tooltip.SetActive(false);
        }
        m_obstacleConstructor.DeletePreviewObstacle();
    }
    #endregion

    #region Functions
    /// <summary>
    /// Handle input to spawn unit
    /// </summary>
    public void InputHandler()
    {
        if (!isActiveAndEnabled)
        {
            return;
        }
        if (m_obstacleConstructor.GetCurrentState() == ObstacleConstructor.EState.Buildable)
        {
            m_obstacleConstructor.GetUISpawnObstacle().SetActive(false);
            m_obstacleConstructor.DeletePreviewObstacle();
            PlayerEntity player = GameManager.Instance.GetLocalPlayerEntity();
            player.CmdCreateObstacle(m_obstacleConstructor.gameObject, m_obstacleType);
        }        
    }  
    #endregion

    #region Accessors

    public void SetObstacleConstructor(ObstacleConstructor obstacleConstructor)
    {
        m_obstacleConstructor = obstacleConstructor;
        if (GameManager.Instance.GetLocalPlayer() == PlayerEntity.Player.Player1)
        {
            m_imageUnit.sprite = m_spriteSyca;
        }
        else
        {
            m_imageUnit.sprite = m_spriteArca;
        }
        m_obstaclePrice = m_obstacleConstructor.GetObstacleCost(m_obstacleType);
        transform.Find(Constant.ListOfMisc.s_Image).Find(Constant.ListOfMisc.s_CostText).GetComponent<Text>().text = m_obstaclePrice.ToString();
    }
    #endregion Accessors

}
