#region Author
///////////////////////////////////////////////
//   Guillaume Quiniou 
/////////////////////////////////////////
#endregion
using UnityEngine;

public abstract class UI2DMapIcon : MonoBehaviour
{
    #region
    [SerializeField] protected Renderer m_parentRenderer = null;
    protected SpriteRenderer m_spriteRenderer;
    protected CameraManager m_camera;
    #endregion

    #region Unity's function

    private void OnEnable()
    {
        m_spriteRenderer = GetComponent<SpriteRenderer>();
        m_camera = Camera.main.GetComponent<CameraManager>();

        CheckIfOk();

        if (m_camera.GetCameraState() == CameraManager.ECamState.Ortho3D)
        {
            HideIcon();
        }
        else
        {
            ShowIcon();
        }
    }

    #endregion

    #region Function

    public virtual void ShowIcon()
    {
        SelectIconToDisplay();
        m_parentRenderer.enabled = false;
    }

    public virtual void HideIcon()
    {
        m_parentRenderer.enabled = true;
    }

    public abstract void SelectIconToDisplay();

    private void CheckIfOk()
    {
#if UNITY_EDITOR
        if (null == m_spriteRenderer)
        {
            Debug.LogError("Il n'y a pas de Sprite Renderer sur " + transform.parent.name, this);
            UnityEditor.EditorApplication.isPlaying = false;
        }

        if (null == m_parentRenderer)
        {
            Debug.LogError("Parent Renderer est null sur " + transform.parent.name, this);
            UnityEditor.EditorApplication.isPlaying = false;
        }

        if (null == m_camera)
        {
            Debug.LogError(transform.parent.name + " n'a pas réussi à trouver la caméra :|", this);
            UnityEditor.EditorApplication.isPlaying = false;
        }
#endif
    }
    #endregion
}
