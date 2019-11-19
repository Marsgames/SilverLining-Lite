#region Author
/********************
 * Corentin Couderc
 ********************/
#endregion

using System;
using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;

public class ObjectSelection : MonoBehaviour
{
    #region Variables
    [SerializeField] protected Renderer m_rend = null;
    [SerializeField] private Color m_color = new Color(1, 1, 1, 1);
    [SerializeField] private float m_pingPongCheckpointGlowSpeed = 2f;
    [SerializeField] private float m_pingPongObstacleGlowSpeed = 2f;
    [SerializeField] protected Category m_category = Category.Checkpoint;

    protected Shader m_shader1;
    private Shader m_shader2;
    private bool m_isMaterialSet = false;
    protected bool m_isKeepGlow = false;
    private bool m_isSelected = false;
    private Color m_emissionColor;
    private PlayerEntity player;

    protected enum Category
    {
        Checkpoint,
        Obstacle,
        SpawnUnit,
        ObstacleConstructor
    };
    #endregion

    #region Unity's Functions
    protected virtual IEnumerator Start()
    {
        yield return new WaitUntil(() => GameManager.Instance != null);
        yield return new WaitUntil(() => GameManager.Instance.GetAllPlayersReady());
        m_shader1 = Shader.Find("Standard");
        m_shader2 = Shader.Find("Custom/GlowEffect");
        m_rend.material.shader = m_shader1;
        m_emissionColor = m_rend.material.GetColor("_EmissionColor");
        player = GameManager.Instance.GetLocalPlayerEntity();
    }

    protected void Update()
    {
        if (m_isSelected && m_rend.material.shader == m_shader2 && Category.Obstacle != m_category && Category.ObstacleConstructor != m_category)
        {
            float speed = m_pingPongCheckpointGlowSpeed;
            float rimPower = Mathf.PingPong(Time.time * speed, 1.5f) + 0.5f;
            m_rend.material.SetFloat("_RimPower", rimPower);
            m_rend.material.SetColor("_RimColor", m_color);
        }

        if (Category.ObstacleConstructor == m_category)
        {
            float speed = m_pingPongObstacleGlowSpeed;
            float intensity = Mathf.PingPong(Time.time * speed, 1.0f) + 0.5f;
            Color new_emissionColor = m_emissionColor * intensity;
            m_rend.material.SetColor("_EmissionColor", new_emissionColor);
        }
    }

    public void OnMouseDown()
    {
        if (EventSystem.current.IsPointerOverGameObject())
        {
            return;
        }
        Select();
    }

    public virtual void OnMouseEnter()
    {
        if (GameManager.Instance.GetEndGameBool())
        {
            return;
        }
        if (!EventSystem.current.IsPointerOverGameObject())
        {
            if (!m_isMaterialSet)
            {
                m_rend.material.shader = m_shader1;
                m_rend.material.shader = m_shader2;
                m_isMaterialSet = true;
            }

            m_rend.material.shader = m_shader2;
        }
    }

    public virtual void OnMouseExit()
    {
        if (!m_isKeepGlow)
        {
            m_rend.material.shader = m_shader1;
        }
    }
    #endregion

    #region Functions
    public void Select()
    {
        if (GameManager.Instance.GetEndGameBool())
        {
            return;
        }
        switch (m_category)
        {
            case (Category.Checkpoint):
                if (GameManager.Instance.GetLocalPlayer() != GetComponentInParent<CheckpointBase>().GetPlayerOwner())
                {
                    return;
                }
                break;
            case (Category.SpawnUnit):
                if (GetComponent<SpawnUnits>().GetPlayerNumber() != GameManager.Instance.GetLocalPlayer())
                {
                    return;
                }
                break;
        }
        m_rend.material.shader = m_shader2;
        m_isKeepGlow = true;
        player.SetSelectedItem(this);
        SetIsSelected(true);
    }
    public void Deselect()
    {
        SetKeepGlow(false);
        OnMouseExit();
        SetIsSelected(false);
        switch (m_category)
        {
            case (Category.Checkpoint):
                GetComponentInParent<CheckpointBase>().GetUIReleaseUnit().HideUIRelease();
                break;
            case (Category.ObstacleConstructor):
                GetComponent<ObstacleConstructor>().DeletePreviewObstacle();
                GetComponent<ObstacleConstructor>().GetUISpawnObstacle().SetActive(false);
                break;
        }
    }
    #endregion Functions

    #region Accessors
    public bool GetKeepGlow()
    {
        return m_isKeepGlow;
    }

    public void SetKeepGlow(bool keepGlow)
    {
        m_isKeepGlow = keepGlow;
    }

    public void SetIsSelected(bool selected)
    {
        m_isSelected = selected;
    }

    #endregion
}
