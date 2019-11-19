#region Author
/////////////////////////////////////////
//   Guillaume Quiniou 
/////////////////////////////////////////
//   RAPHAËL DAUMAS 
//   https://raphdaumas.wixsite.com/portfolio
//   https://github.com/Marsgames
/////////////////////////////////////////
#endregion
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class Territory : MonoBehaviour
{
    #region Variables

    public bool terterOKPlayer1;
    public bool terterOKPlayer2;
    public bool terterOKPlayerBot;

    [SerializeField] private PlayerEntity.Player m_ownerNumber = PlayerEntity.Player.Neutre;
    [SerializeField] private GameObject m_area = null;
    [SerializeField] private GameManager m_gameManager = null;
    [SerializeField] private MeshRenderer m_centerIsletRenderer = null;
    [SerializeField] private int m_cost = 0;
    [SerializeField] private Material[] m_centerMaterials = new Material[6];
    [SerializeField] private SpriteRenderer m_iconRenderer = null;
    [SerializeField] private CheckpointBase m_barrack = null;

    private Dictionary<int, Material> m_materialDic = new Dictionary<int, Material>();

    private Collider m_collider = null;
    private bool m_isAvailable = false;
    private AlertManager m_alertManager;
    private bool m_init;
    private float m_stockTime = 0;
    private PlayerEntity.Player m_localPlayer;

    protected int m_unitsPlayer1 = 0;
    protected int m_unitsPlayer2 = 0;
    #endregion

    #region Unity's function
    private void OnEnable()
    {
        m_alertManager = AlertManager.Instance;
    }

    private IEnumerator Start()
    {
        m_collider = m_area.GetComponent<BoxCollider>();
        if (PlayerEntity.Player.Player2 == m_ownerNumber && 1 == NetworkServer.connections.Count)
        {
            m_ownerNumber = PlayerEntity.Player.Bot;
        }

        CheckIfOk();
        yield return new WaitUntil(() => GameManager.Instance != null);
        yield return new WaitUntil(m_gameManager.GetAllPlayersReady);

        m_materialDic = new Dictionary<int, Material> { { 1, m_centerMaterials[0] },
            { 34, m_centerMaterials[1] },
            { 35, m_centerMaterials[2] },
            { 46, m_centerMaterials[3] },
            { 12, m_centerMaterials[4] },
            { 45, m_centerMaterials[5] }
        };

        m_localPlayer = GameManager.Instance.GetLocalPlayer();
        InitializeTerritory();

        terterOKPlayer1 = IsAvailable(m_localPlayer);
        terterOKPlayer2 = IsAvailable(PlayerEntity.Player.Player2);
        terterOKPlayerBot = IsAvailable(PlayerEntity.Player.Bot);
    }

    public void TriggerEnter(Collider other)
    {
        UnitController unit = other.GetComponent<UnitController>();

        if (!unit)
        {
            return;
        }

        unit.AddToTerritories(this);
        unit.SetCurrentTerritory(this);

        SetUnitNumberInTerritory(unit, 1);
        AllowVisionAndConstruction(unit);

        PlaySoundBaseInDanger(unit);

        terterOKPlayer1 = IsAvailable(m_localPlayer);
        terterOKPlayer2 = IsAvailable(PlayerEntity.Player.Player2);
        terterOKPlayerBot = IsAvailable(PlayerEntity.Player.Bot);
    }

    public void TriggerExit(Collider other)
    {
        UnitController unit = other.GetComponent<UnitController>();

        if (!unit)
        {
            return;
        }
        SetUnitNumberInTerritory(unit, -1);
        unit.RemoveFromTerritories(this);
        DenyVisionAndConstruction(unit);

        terterOKPlayer1 = IsAvailable(m_localPlayer);
        terterOKPlayer2 = IsAvailable(PlayerEntity.Player.Player2);
        terterOKPlayerBot = IsAvailable(PlayerEntity.Player.Bot);
    }
    #endregion Unity 's function

    #region Function
    public void OnUnitDeath(UnitController unit)
    {
        IncrementUnitNumber(unit, -1);
        unit.RemoveFromTerritories(this);
        DenyVisionAndConstruction(unit);
    }

    public void SetUnitNumberInTerritory(UnitController unit, int increment)
    {
        if (unit.GetCheckpointState() == UnitController.EUnitCheckpointState.fightingForCheckpoint)
        {
            return;
        }
        IncrementUnitNumber(unit, increment);
    }

    public void IncrementUnitNumber(UnitController unit, int increment)
    {
        switch (unit.GetPlayerNumber())
        {
            case PlayerEntity.Player.Player1:
                m_unitsPlayer1 += increment;
                return;
            case PlayerEntity.Player.Player2:
            case PlayerEntity.Player.Bot:
                m_unitsPlayer2 += increment;
                return;
        }
    }

    private void InitializeCenterMaterial(PlayerEntity.Player playerNumber)
    {
        if (playerNumber == PlayerEntity.Player.Player1)
        {
            m_centerIsletRenderer.material = m_centerMaterials[0];
        }
        else if (playerNumber == PlayerEntity.Player.Player2)
        {
            m_centerIsletRenderer.material = m_centerMaterials[1];
        }
    }

    private void CheckAndSetCenterTerritoryValue(bool changed)
    {
        if (changed)
        {
            int centerValue = m_gameManager.GetTerritoryCost();
            if (m_materialDic.ContainsKey(centerValue))
            {
                m_centerIsletRenderer.material = m_materialDic[centerValue];
            }
        }
    }

    private void InitializeTerritory()
    {
        if (m_localPlayer == m_ownerNumber)
        {
            float colorValue = 1f;
            SetIsAivailable(true);
            ChangeIconVision(colorValue);
            ChangeObstacleConstructorVision(colorValue, m_isAvailable);
            ChangeRendererVision(colorValue);
        }
        else
        {
            float colorValue = 0.3f;
            SetIsAivailable(false);
            ChangeIconVision(colorValue);
            ChangeObstacleConstructorVision(colorValue, m_isAvailable);
            ChangeRendererVision(colorValue);
        }

        m_gameManager.InitializeTerritoryCost();
        CheckAndSetCenterTerritoryValue(true);
    }

    private void AllowVisionAndConstruction(UnitController unit)
    {
        if (unit.GetPlayerNumber() != m_localPlayer)
        {
            return;
        }
        if (m_isAvailable) // if territory is already available
        {
            return;
        }

        SetIsAivailable(IsAvailable(m_localPlayer));

        // If territory is not available after the check
        if (!m_isAvailable)
        {
            return;
        }

        float colorValue = 1f;

        CheckAndSetCenterTerritoryValue(m_gameManager.SetTerritoryCost(m_cost));

        ChangeIconVision(colorValue);
        ChangeObstacleConstructorVision(colorValue, m_isAvailable);
        ChangeRendererVision(colorValue);
    }

    private void DenyVisionAndConstruction(UnitController unit)
    {
        if (unit.GetPlayerNumber() != m_localPlayer)
        {
            return;
        }

        SetIsAivailable(IsAvailable(m_localPlayer));
        if (!m_isAvailable)
        {
            float colorValue = 0.3f;

            CheckAndSetCenterTerritoryValue(m_gameManager.SetTerritoryCost(m_cost * -1));

            ChangeIconVision(colorValue);
            ChangeObstacleConstructorVision(colorValue, m_isAvailable);
            ChangeRendererVision(colorValue);
        }
    }

    private void ChangeIconVision(float colorValue)
    {
        m_iconRenderer.material.color = new Color(colorValue, colorValue, colorValue);
    }

    private void ChangeObstacleConstructorVision(float colorValue, bool canConstruct)
    {
        foreach (ObstacleConstructor obsContruct in GetComponentsInChildren<ObstacleConstructor>())
        {
            obsContruct.ChangeMaterial(canConstruct);
            obsContruct.GetComponentInChildren<SpriteRenderer>().material.color = new Color(colorValue, colorValue, colorValue);
        }
    }

    private void ChangeRendererVision(float colorValue)
    {
        foreach (Renderer rend in GetComponentsInChildren<Renderer>())
        {
            if (!rend.gameObject.CompareTag(Constant.ListOfTag.s_unit) && !rend.gameObject.CompareTag(Constant.ListOfTag.s_obstacle) && !rend.name.Equals("SM_Islet_Mid") && !rend.name.Equals("CheckpointZone_Arca") && !rend.name.Equals("CheckpointZone_Syca"))
            {
                rend.material.color = new Color(colorValue, colorValue, colorValue);
            }
        }
    }

    public bool IsAvailable(PlayerEntity.Player playerNumber)
    {
        if (PlayerEntity.Player.Bot == playerNumber)
        {
            playerNumber = PlayerEntity.Player.Player2;
        }

        if (m_ownerNumber == playerNumber)
        {
            return true;
        }

        if (playerNumber == PlayerEntity.Player.Player1 && m_unitsPlayer1 > 0)
        {
            return true;
        }
        else if ((playerNumber == PlayerEntity.Player.Player2 || playerNumber == PlayerEntity.Player.Bot) && m_unitsPlayer2 > 0)
        {
            return true;
        }

        if (null != m_barrack && m_barrack.GetPlayerOwner() == playerNumber)
        {
            return true;
        }

        return false;
    }

    /// <summary>
    /// Checks if all required conditions are filled, otherwise crash
    /// </summary>
    private void CheckIfOk()
    {
#if UNITY_EDITOR
        if (null == m_collider)
        {
            Debug.LogError("Il doit y avoir un collider dans " + name + " de " + transform.parent.name);
            UnityEditor.EditorApplication.isPlaying = false;
        }

        if (null == m_gameManager)
        {
            Debug.LogError("Le GameManager n'est pas set dans " + name + "de " + transform.parent.name);
            UnityEditor.EditorApplication.isPlaying = false;
        }

        if (null == m_area)
        {
            Debug.LogError("Area n 'est pas set dans " + name + " de " + transform.parent.name);
            UnityEditor.EditorApplication.isPlaying = false;
        }
#endif
    }

    private void PlaySoundBaseInDanger(UnitController unit)
    {
        PlayerEntity.Player unitPlayer = unit.GetPlayerNumber();

        // Si le territoire n'appartient pas au joueur
        if (m_ownerNumber != m_localPlayer || m_ownerNumber == unitPlayer)
        {
            return;
        }
        if (Time.time - m_stockTime >= 15)
        {
            m_alertManager.TimerVerificationBaseInDanger();
            m_stockTime = Time.time;
        }
    }

    #endregion Functions

    #region Accessors
    /// <summary>
    /// Gets the m_isAvailable property.
    /// </summary>
    /// <returns><c>true</c>, if the local player can see through the FoW, <c>false</c> otherwise.</returns>
    public bool GetIsAvailable()
    {
        return m_isAvailable;
    }

    private void SetIsAivailable(bool value)
    {
        if (value == m_isAvailable)
        {
            return;
        }

        m_isAvailable = value;

        if (!value)
        {
            return;
        }

        if (m_init)
        {
            m_alertManager.TimerVerificationZoneDiscovered();
        }
        else
        {
            m_init = true;
        }

    }

    public int GetUnitsPlayer2()
    {
        return m_unitsPlayer2;
    }
    #endregion Accessors
}