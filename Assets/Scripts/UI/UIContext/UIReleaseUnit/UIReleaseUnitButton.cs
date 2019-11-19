#region Author
/////////////////////////////////////////
//   Guillaume Quiniou
/////////////////////////////////////////
#endregion
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class UIReleaseUnitButton : MonoBehaviour, IPointerEnterHandler
{
    #region Variables
    [SerializeField] private Image m_unitIcon = null;
    [SerializeField] private Sprite m_warriorSycaIcon = null;
    [SerializeField] private Sprite m_rangeSycaIcon = null;
    [SerializeField] private Sprite m_tankSycaIcon = null;
    [SerializeField] private Sprite m_scoutSycaIcon = null;
    [SerializeField] private Sprite m_warriorArcaIcon = null;
    [SerializeField] private Sprite m_rangeArcaIcon = null;
    [SerializeField] private Sprite m_tankArcaIcon = null;
    [SerializeField] private Sprite m_scoutArcaIcon = null;

    private GameObject m_unitStock;
    private CheckpointBase m_checkpointBase;
    private SoundManager m_soundManager;
    private int m_unitStockID;
    private Text m_shortcutText;
    #endregion

    #region Unity's function
    private void OnEnable()
    {
        m_soundManager = SoundManager.Instance;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        Cursor.SetCursor(null, Vector2.zero, CursorMode.Auto);
    }
    #endregion Unity's function

    #region Functions
    public void ReleaseUnit()
    {
        m_soundManager.PlaySound(SoundManager.AudioClipList.AC_releaseOneUnit);
        m_checkpointBase.CmdReleaseUnit(m_unitStock);
    }

    public void InitReleaseButton(CheckpointBase checkpointBase, GameObject unit, int unitStockID)
    {
        m_checkpointBase = checkpointBase;
        m_unitStock = unit;
        m_unitStockID = unitStockID;
        m_shortcutText = transform.GetComponentInChildren<Text>();
        CombatUnit combatUnit = unit.GetComponent<CombatUnit>();
        UnitController unitController = unit.GetComponent<UnitController>();
        switch (combatUnit.GetUnitType())
        {
            case CombatUnit.UnitType.Warrior:
                if (unitController.GetPlayerNumber() == PlayerEntity.Player.Player1)
                {
                    m_unitIcon.sprite = m_warriorSycaIcon;
                }
                else
                {
                    m_unitIcon.sprite = m_warriorArcaIcon;
                }
                break;
            case CombatUnit.UnitType.Range:
                if (unitController.GetPlayerNumber() == PlayerEntity.Player.Player1)
                {
                    m_unitIcon.sprite = m_rangeSycaIcon;
                }
                else
                {
                    m_unitIcon.sprite = m_rangeArcaIcon;
                }
                break;
            case CombatUnit.UnitType.Tank:
                if (unitController.GetPlayerNumber() == PlayerEntity.Player.Player1)
                {
                    m_unitIcon.sprite = m_tankSycaIcon;
                }
                else
                {
                    m_unitIcon.sprite = m_tankArcaIcon;
                }
                break;
            case CombatUnit.UnitType.Scout:
                if (unitController.GetPlayerNumber() == PlayerEntity.Player.Player1)
                {
                    m_unitIcon.sprite = m_scoutSycaIcon;
                }
                else
                {
                    m_unitIcon.sprite = m_scoutArcaIcon;
                }
                break;
        }
        
        m_shortcutText.text = unitStockID.ToString();
    }

    public void UpdateShortcutText()
    {
        m_shortcutText.text = m_unitStockID.ToString();
    }
    #endregion Functions

    #region Accessors
    public void SetUnitStockID(int unitStockID)
    {
        m_unitStockID = unitStockID;
    }

    public int GetUnitStockID()
    {
        return m_unitStockID;
    }
    #endregion
}
