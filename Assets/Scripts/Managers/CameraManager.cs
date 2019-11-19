#region Author
/////////////////////////////////////////
//   RAPHAËL DAUMAS
//   https://raphdaumas.wixsite.com/portfolio
//   https://github.com/Marsgames
/////////////////////////////////////////
//   VALENTIN CLIMPONT
/////////////////////////////////////////
//  Quiniou Guillaume
/////////////////////////////////////////
#endregion
using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Experimental.Input;
using UnityEngine.Experimental.Input.Controls;

public class CameraManager : MonoBehaviour
{
    #region Variables
    public static CameraManager Instance;

    public enum ECamState { Ortho3D, Iso2D };

    [SerializeField] private InputManager m_inputManager = null;
    [SerializeField] private float m_transitionTime = 0.5f;
    [SerializeField] private Transform m_centerMap = null;
    [SerializeField, Range(1, 150)] private int m_minFov = 15;
    [SerializeField, Range(2, 150)] private int m_maxFov = 45;
    [SerializeField, Range(1, 50)] private int m_sensitivity = 40;
    [SerializeField] private float m_slideSpeed = 1;
    [SerializeField, Range(1, 10)] private int m_timeBetweenTrails = 3;
    [SerializeField, Range(10, 150)] private int m_2DYpoint = 84;
    [SerializeField] private Transform m_leftLimit = null;
    [SerializeField] private Transform m_rightLimit = null;
    [SerializeField] private Transform m_posPlayer1 = null;
    [SerializeField] private Transform m_posPlayer2 = null;

    private ECamState m_cameraState = ECamState.Ortho3D;
    private Camera m_mainCamera;

    private double m_timerDoubleTap = 0.5;
    private double m_timerDoubleTapSpawn1 = 0;
    private double m_timerDoubleTapSpawn2 = 0;

    private GameObject m_spawnUnit1;
    private GameObject m_spawnUnit2;

    private float m_ycamPos;
    private Quaternion m_initRot;
    private Vector3 m_lastPosition;
    private bool m_canUnzoom;

    private Rect m_screenRect;
    private int m_slideZoneX;
    private int m_slideZoneY;

    private MusicManager m_musicManager;
    private bool m_cameraSet;
    
    private GameObject m_plane;
    private PlayerEntity m_localPlayer;
    #endregion

    #region Unity's functions
    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(Instance.gameObject);
        }
        Instance = this;
        m_inputManager.Camera.ChangeView.performed += _ => ChangeView();
        m_inputManager.Camera.FocusSpawn1.performed += ctx => FocusCameraOnSpawnUnit(ctx, Constant.ListOfTag.s_spawnUnit1);
        m_inputManager.Camera.FocusSpawn2.performed += ctx => FocusCameraOnSpawnUnit(ctx, Constant.ListOfTag.s_spawnUnit2);
        m_inputManager.Camera.MouseClick.performed += _ => HandleMouseClick();

        m_inputManager.Camera.ChangeView.Enable();
        m_inputManager.Camera.FocusSpawn1.Enable();
        m_inputManager.Camera.FocusSpawn2.Enable();
        m_inputManager.Camera.MouseClick.Enable();

        m_ycamPos = transform.position.y;
        m_mainCamera = GetComponent<Camera>();
    }

    public IEnumerator Start()
    {
        Cursor.lockState = CursorLockMode.Confined;
        yield return new WaitUntil(() => GameManager.Instance != null);
        yield return new WaitUntil(() => GameManager.Instance.GetAllPlayersReady());
        m_screenRect = new Rect(0, 0, Screen.width, Screen.height);
        m_slideZoneX = 10;
        m_slideZoneY = 10;

        m_mainCamera.fieldOfView = m_maxFov;
        m_initRot = m_mainCamera.transform.rotation;

        m_canUnzoom = true;
        m_musicManager = MusicManager.Instance;

        if (PlayerEntity.Player.Player1 == GameManager.Instance.GetLocalPlayer())
        {
            m_mainCamera.transform.position = m_posPlayer1.position;
        }
        if (PlayerEntity.Player.Player2 == GameManager.Instance.GetLocalPlayer())
        {
            m_mainCamera.transform.position = m_posPlayer2.position;
        }
        m_plane = GameObject.Find("Plane");
        m_plane.SetActive(false);
        m_localPlayer = GameManager.Instance.GetLocalPlayerEntity();
    }

    private void FixedUpdate()
    {
        if (m_cameraState == ECamState.Ortho3D)
        {
            ZoomInPC();
            SlidePC();
        }
    }
    #endregion

    #region Functions
    /// <summary>
    /// If view2D, call function to change to 3DView at the mouse position.
    /// If view3D, if the player click on nothing selectable, deselect the last selectable item.
    /// </summary>
    private void HandleMouseClick()
    {
        if (GameManager.Instance.GetEndGameBool())
        {
            return;
        }
        if (m_cameraState == ECamState.Iso2D && m_canUnzoom)
        {
            ChangeTo3DCameraOnClick();
        }
        else if (m_cameraState == ECamState.Ortho3D)
        {
            PlayerDeselectItem();
        }
    }

    /// <summary>
    /// Handle player click on 3Dview. If click on nothing selectable, deselect the current select item
    /// </summary>
    private void PlayerDeselectItem()
    {
        if (EventSystem.current.IsPointerOverGameObject())
        {
            return;
        }
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            if (!hit.transform.GetComponent<ObjectSelection>() || !hit.transform.GetComponent<UIReleaseUnitButton>())
            {
                m_localPlayer.DeselectLastItem();
            }
        }
        else
        {
            m_localPlayer.DeselectLastItem();
        }

    }

    /// <summary>
    /// Handle change to 3D camera from 2Dview when player click on the map
    /// </summary>
    private void ChangeTo3DCameraOnClick()
    {
        Vector2Control mousePosition = InputSystem.GetDevice<Mouse>().position;
        Vector3 mousePos = new Vector3(mousePosition.x.ReadValue(), mousePosition.y.ReadValue(), 0);

        Ray ray = m_mainCamera.ScreenPointToRay(mousePos);

        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            if (null == hit.transform)
            {
                return;
            }
            var objectHit = hit.transform;
            var parent = objectHit.parent;

            if (null == parent)
            {
                return;
            }
            if (!parent.name.Contains("Territory"))
            {
                return;
            }
        }
        else
        {
            return;
        }

        m_canUnzoom = false;
        mousePos.y -= Screen.height / 7;
        mousePos.x -= Screen.width / 20;
        mousePos = m_mainCamera.ScreenToWorldPoint(mousePos);
        mousePos.y -= 80;
        StartCoroutine(ChangeTo3DCamera(mousePos));
    }

    /// <summary>
    /// Change view between tactical2D and 3D
    /// </summary>
    public void ChangeView()
    {
        if (!m_canUnzoom)
        {
            return;
        }
        m_canUnzoom = false;

        if (m_cameraState == ECamState.Ortho3D)
        {
            StartCoroutine(ChangeTo2DCamera());
        }
        else if (m_cameraState == ECamState.Iso2D)
        {
            StartCoroutine(ChangeTo3DCamera(m_lastPosition));
        }
    }

    /// <summary>
    /// Tactical2D view. Unzoom and center camera, then change to orthographic. 
    /// </summary>
    private IEnumerator ChangeTo2DCamera()
    {
        UIGlobal.Instance.GetUISpawnUnit().HideSpawnUnitUI();
        GameManager.Instance.GetLocalPlayerEntity().HideUISelectedItem();

        m_musicManager.ReverbeChangerCamera2D();
        m_lastPosition = m_mainCamera.transform.position;
        Vector3 endPos = new Vector3(m_centerMap.position.x, m_mainCamera.transform.position.y, m_centerMap.position.z);
        float startFov = m_mainCamera.fieldOfView;

        float lerpTime = 0;
        while (lerpTime <= m_transitionTime)
        {
            lerpTime += Time.deltaTime;
            float t = lerpTime / m_transitionTime;
            t = Mathf.SmoothStep(0, 1, t);
            m_mainCamera.fieldOfView = Mathf.Lerp(startFov, 130f, t);
            m_mainCamera.transform.rotation = Quaternion.Lerp(m_initRot, Quaternion.Euler(90, 0, 0), t);
            m_mainCamera.transform.position = Vector3.Lerp(m_lastPosition, endPos, t);
            yield return null;
        }
        m_cameraState = ECamState.Iso2D;
        m_mainCamera.orthographic = true;
        m_mainCamera.orthographicSize = (float)m_2DYpoint;
        ShowTrails(true, 0);
        m_musicManager.ReverbeChangerCamera2D();
        ShowIcons();

        m_canUnzoom = true;

        m_plane.SetActive(true);

        Vector3 position = m_mainCamera.transform.position;
        position.y += 80;
        m_mainCamera.transform.position = position;
    }

    /// <summary>
    /// Classic 3D view. Change camera to perspective, smooth move to endPos.
    /// </summary>
    /// <param name="endPos">Final position of the camera</param>
    private IEnumerator ChangeTo3DCamera(Vector3 endPos)
    {
        m_plane.SetActive(false);
        m_musicManager.ReverbeChangerCamera3D();
        m_cameraState = ECamState.Ortho3D;
        ShowTrails(false, 0);
        HideIcons();
        m_mainCamera.orthographic = false;
        m_mainCamera.fieldOfView = 110f;

        Quaternion rotStart = m_mainCamera.transform.rotation;
        Vector3 startPos = m_mainCamera.transform.position;
        float fovStart = 110f;
        float fovEnd = 40f;
        float lerpTime = 0;
        while (lerpTime <= m_transitionTime)
        {
            lerpTime += Time.deltaTime;
            float t = lerpTime / m_transitionTime;
            t = Mathf.SmoothStep(0, 1, t);
            m_mainCamera.fieldOfView = Mathf.Lerp(fovStart, fovEnd, t);
            m_mainCamera.transform.rotation = Quaternion.Lerp(rotStart, m_initRot, t);
            m_mainCamera.transform.position = Vector3.Lerp(startPos, endPos, t);
            yield return null;
        }
        m_canUnzoom = true;
        UIGlobal.Instance.GetUISpawnUnit().ShowSpawnUnitUI();
        GameManager.Instance.GetLocalPlayerEntity().ShowUISelectedItem();
    }

    /// <summary>
    /// Call when change to tactical2D view. Show all icons and hide 3Dmodel.
    /// </summary>
    private void ShowIcons()
    {
        foreach (UI2DMapIcon uIcon in FindObjectsOfType<UI2DMapIcon>())
        {
            uIcon.ShowIcon();
        }
    }

    /// <summary>
    /// Call when change to 3D view. Hide all icons and show 3Dmodel.
    /// </summary>
    private void HideIcons()
    {
        foreach (UI2DMapIcon uIcon in FindObjectsOfType<UI2DMapIcon>())
        {
            uIcon.HideIcon();
        }
    }

    /// <summary>
    /// Move camera. If on tactical2D view, change to 3Dview.
    /// </summary>
    /// <param name="pos">New position of the camera</param>
    private IEnumerator MoveCameraToPosition(Vector3 pos)
    {
        if (!m_canUnzoom)
        {
            yield return null;
        }

        m_canUnzoom = false;
        Vector3 endPos;
        endPos = new Vector3(pos.x - 15f, transform.position.y, pos.z - 35f);
        if (m_cameraState == ECamState.Iso2D)
        {
            StartCoroutine(ChangeTo3DCamera(endPos));
        }
        else
        {
            Vector3 startPos = transform.position;
            float lerpTime = 0;
            while (lerpTime <= m_transitionTime)
            {
                lerpTime += Time.deltaTime;
                float t = lerpTime / m_transitionTime;
                t = Mathf.SmoothStep(0, 1, t);
                transform.position = Vector3.Lerp(startPos, endPos, t);
                yield return null;
            }
            m_canUnzoom = true;
        }
        yield return null;
    }

    /// <summary>
    /// Handle input for focus on spawnUnit.
    /// </summary>
    /// <param name="ctx">CallBackContext of input</param>
    /// <param name="spawnUnitTag">tag of the spawnUnit to focus</param>
    private void FocusCameraOnSpawnUnit(InputAction.CallbackContext ctx, string spawnUnitTag)
    {
        if (ECamState.Iso2D == m_cameraState)
        {
            return;
        }

        SpawnUnits spawnToFocus = GameManager.Instance.GetSpawnUnitForPlayer(GameManager.Instance.GetLocalPlayer(), spawnUnitTag);
        spawnToFocus.ShowUISpawnUnit();
        spawnToFocus.GetComponent<ObjectSelection>().OnMouseDown();
        double timer;
        if (spawnUnitTag == Constant.ListOfTag.s_spawnUnit1)
        {
            timer = m_timerDoubleTapSpawn1;
            m_timerDoubleTapSpawn1 = ctx.time;
            m_timerDoubleTapSpawn2 = 0;
        }
        else
        {
            timer = m_timerDoubleTapSpawn2;
            m_timerDoubleTapSpawn2 = ctx.time;
            m_timerDoubleTapSpawn1 = 0;
        }
        if (ctx.time > timer + m_timerDoubleTap)
        {
            return;
        }
        StartCoroutine(MoveCameraToPosition(spawnToFocus.gameObject.transform.position));
    }

    /// <summary>
    /// Handle camera zoom when player use mouse scroll
    /// </summary>
    private void ZoomInPC()
    {
        var fov = m_mainCamera.fieldOfView;
        fov -= Input.GetAxis("Mouse ScrollWheel") * m_sensitivity;
        fov = Mathf.Clamp(fov, m_minFov, m_maxFov);
        m_mainCamera.fieldOfView = fov;
    }

    /// <summary>
    /// Handle slide of the camera when mouse reach the limit of the screen or player use arrow keys
    /// </summary>
    private void SlidePC()
    {
        Vector2 mousePos = InputSystem.GetDevice<Mouse>().position.ReadValue();
        //if (!m_screenRect.Contains(mousePos))
        //{
        //    return;
        //}
        if (m_mainCamera.transform.position.x >= m_leftLimit.position.x)
        {
            if (mousePos.x <= m_slideZoneX || InputSystem.GetDevice<Keyboard>().leftArrowKey.isPressed)
            {
                m_mainCamera.transform.position -= new Vector3(m_slideSpeed, 0, 0);
            }
        }

        if (m_mainCamera.transform.position.x <= m_rightLimit.transform.position.x)
        {
            if (mousePos.x >= Screen.width - m_slideZoneX || InputSystem.GetDevice<Keyboard>().rightArrowKey.isPressed)
            {
                m_mainCamera.transform.position += new Vector3(m_slideSpeed, 0, 0);
            }
        }

        if (m_mainCamera.transform.position.z >= m_leftLimit.transform.position.z)
        {
            if (mousePos.y <= m_slideZoneY || InputSystem.GetDevice<Keyboard>().downArrowKey.isPressed)
            {
                m_mainCamera.transform.position -= new Vector3(0, 0, m_slideSpeed);
            }
        }

        if (m_mainCamera.transform.position.z <= m_rightLimit.transform.position.z)
        {
            if (mousePos.y >= Screen.height - m_slideZoneY || InputSystem.GetDevice<Keyboard>().upArrowKey.isPressed)
            {
                m_mainCamera.transform.position += new Vector3(0, 0, m_slideSpeed);
            }
        }
    }

    /// <summary>
    /// Shows the trails.
    /// </summary>
    /// <param name="value">If set to <c>true</c> value.</param>
    /// <param name="type">If type == 0 show for all units. If type == 1 show only for scouts.</param>
    private void ShowTrails(bool value, int type)
    {
        if (!value)
        {
            StopTrails();
        }
        else
        {
            if (type == 0)
            {
                InvokeRepeating("SendTrailsForUnits", 0, m_timeBetweenTrails);
            }
        }
    }

    /// <summary>
    /// Foreach unit of the localplayer, ask server for path and send trail
    /// </summary>
    private void SendTrailsForUnits()
    {
        foreach (UnitController unit in FindObjectsOfType<UnitController>())
        {
            if (GameManager.Instance.GetLocalPlayer() != unit.GetPlayerNumber())
            {
                continue;
            }

            GameManager.Instance.GetLocalPlayerEntity().CmdAskUnitPath(unit.gameObject);

            unit.SendTrail();
        }
    }

    /// <summary>
    /// Cancel the new trails and delete the existing ones
    /// </summary>
    private void StopTrails()
    {
        CancelInvoke("SendTrailsForUnits");

        foreach (TrailController trail in FindObjectsOfType<TrailController>())
        {
            Destroy(trail.gameObject);
        }
    }
    #endregion

    #region Accessors

    /// <summary>
    /// Gets how many secondes we want to wait between each trail (to be used in UnitController)
    /// </summary>
    /// <returns>The time between trails.</returns>
    public int GetTimeBetweenTrails()
    {
        return m_timeBetweenTrails;
    }

    public ECamState GetCameraState()
    {
        return m_cameraState;
    }

    public Camera GetCamera()
    {
        return m_mainCamera;
    }

    public float GetMinFov()
    {
        return m_minFov;
    }

    public float GetMaxFov()
    {
        return m_maxFov;
    }

    public float GetYCamPos()
    {
        return m_ycamPos;
    }

    #endregion
}