#region Author
/////////////////////////////////////////
//   Guillaume Quiniou
/////////////////////////////////////////
#endregion
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIReleaseUnit : MonoBehaviour
{
    #region Variables
    [SerializeField] private InputManager m_inputManager = null;
    [SerializeField] private GameObject m_prefabButton = null;
    [SerializeField] private GameObject m_modelCollider = null;
    [SerializeField] private Image m_stockCounter = null;

    private CheckpointBase m_checkpointBase;
    private Dictionary<GameObject, UIReleaseUnitButton> m_buttons = new Dictionary<GameObject, UIReleaseUnitButton>();
    private bool m_isShow = false;
    private int m_nbUnitStocked = 0;

    #endregion

    #region Unity's function
    private void Awake()
    {
        m_inputManager.Checkpoint.ReleaseSlot1.performed += _ => ReleaseUnitFromCheckpoint(0);
        m_inputManager.Checkpoint.ReleaseSlot2.performed += _ => ReleaseUnitFromCheckpoint(1);
        m_inputManager.Checkpoint.ReleaseSlot3.performed += _ => ReleaseUnitFromCheckpoint(2);

        m_inputManager.Checkpoint.ReleaseSlot1.Enable();
        m_inputManager.Checkpoint.ReleaseSlot2.Enable();
        m_inputManager.Checkpoint.ReleaseSlot3.Enable();
    }

    private void Start()
    {
        m_checkpointBase = GetComponentInParent<CheckpointBase>();
        UpdateStockCounter();
    }
    #endregion Unity's function

    #region Functions
    private void ReleaseUnitFromCheckpoint(int index)
    {
        if (transform.childCount > index)
        {
            Transform unitToRelease = transform.GetChild(index);
            if (unitToRelease.gameObject.activeInHierarchy)
            {
                unitToRelease.GetComponent<UIReleaseUnitButton>().ReleaseUnit();
            }
        }
    }

    public void ShowUIRelease()
    {
        if (m_checkpointBase.GetOnFight())
        {
            return;
        }
        GameManager.Instance.GetLocalPlayerEntity().HideUISelectedItem();
        m_modelCollider.layer = 2;
        if (m_buttons.Count <= 0)
        {
            return;
        }
        m_stockCounter.gameObject.SetActive(false);
        foreach (var button in m_buttons)
        {
            button.Value.gameObject.SetActive(true);
        }
        m_isShow = true;
    }

    public void HideUIRelease()
    {
        if (!m_modelCollider.Equals(null))
        {
            m_modelCollider.layer = 0;
        }
        m_stockCounter.gameObject.SetActive(true);
        foreach (var button in m_buttons)
        {
            button.Value.gameObject.SetActive(false);
        }
        m_isShow = false;
        UpdateStockCounter();
    }

    public void AddUnitToStock(GameObject unit, int nbStockUnit)
    {
        GameObject newButton = Instantiate(m_prefabButton, transform);
        m_nbUnitStocked = nbStockUnit;
        newButton.GetComponent<UIReleaseUnitButton>().InitReleaseButton(m_checkpointBase, unit, m_nbUnitStocked);
        if (!m_isShow)
        {
            newButton.SetActive(false);
        }

        if (!m_buttons.ContainsKey(unit))
        {
            m_buttons.Add(unit, newButton.GetComponent<UIReleaseUnitButton>());
        }
        UpdateStockCounter();
    }

    public void RemoveUnitToStock(GameObject unit, int nbStockUnit)
    {
        if (m_buttons.ContainsKey(unit))
        {
            m_nbUnitStocked = nbStockUnit;
            Destroy(m_buttons[unit].gameObject);
            foreach (GameObject unitInStock in m_buttons.Keys)
            {
                if (m_buttons[unitInStock].GetUnitStockID() > m_buttons[unit].GetUnitStockID())
                {
                    m_buttons[unitInStock].SetUnitStockID(m_buttons[unitInStock].GetUnitStockID() - 1);
                }
                m_buttons[unitInStock].UpdateShortcutText();
            }
            m_buttons.Remove(unit);
        }
        if (m_isShow && nbStockUnit == 0)
        {
            HideUIRelease();
        }
        UpdateStockCounter();
    }

    private void UpdateStockCounter()
    {
        m_stockCounter.fillAmount = (float)m_nbUnitStocked / m_checkpointBase.GetNbMaxUnitsStocked();
        if (m_checkpointBase.GetPlayerOwner() == PlayerEntity.Player.Player1)
        {
            m_stockCounter.color = new Color(255, 112, 0);
        }
        else
        {
            m_stockCounter.color = new Color(0, 236, 255);
        }
        if (!m_isShow)
        {
            if (m_nbUnitStocked != 0 && m_checkpointBase.GetPlayerOwner() == GameManager.Instance.GetLocalPlayer())
            {
                m_stockCounter.gameObject.SetActive(true);
            }
            else
            {
                m_stockCounter.gameObject.SetActive(false);
            }
        }
    }
    #endregion Functions
}
