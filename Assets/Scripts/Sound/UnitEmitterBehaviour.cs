#region Author
/////////////////////////////////////////
//   Michel Bigourd --> AlertManager
//   https://linkedin.com/in/michel-bigourd-a8a05b100
/////////////////////////////////////////
#endregion

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitEmitterBehaviour : MonoBehaviour
{
    #region Variables
    private Transform m_emitterTransform;
    private CameraManager m_cameraScript;
    private AudioSource m_thisAudioScource;
    private Transform m_myParent;
    private Vector3 m_emitterPlacer;
    [Range(0.0f, 1.0f)]
    [SerializeField] private float m_minVolume = 0;
    [Range(0.0f, 1.0f)]
    [SerializeField] private float m_maxVolume = 0;
    [SerializeField] private float m_minRange = 0;
    [SerializeField] private float m_maxRange = 0;
    #endregion

    #region Unity's Functions
    private void OnEnable()
    {
        m_thisAudioScource = GetComponent<AudioSource>();
        m_cameraScript = CameraManager.Instance;
        m_myParent = GetComponentInParent<Transform>();
    }

    void Start()
    {
        m_emitterPlacer = new Vector3(-9.7f, m_cameraScript.GetYCamPos() - 14.6f, -19.5f);
        m_thisAudioScource.volume = 1 - ((m_cameraScript.GetCamera().fieldOfView - m_cameraScript.GetMinFov()) / (m_cameraScript.GetMaxFov() - m_cameraScript.GetMinFov())) + m_minVolume;
    }

    private void Update()
    {
        transform.position = transform.parent.position + m_emitterPlacer;
        if (Input.GetAxis("Mouse ScrollWheel") != 0)
        {
            StartCoroutine(SetVolumeRangeWithZoom());
        }
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            DisableEmitter();
        }
    }

    public IEnumerator SetVolumeRangeWithZoom()
    {
        float vol;
        float rang;
        while (Input.GetAxis("Mouse ScrollWheel") != 0)
        {
            vol = 1 - ((m_cameraScript.GetCamera().fieldOfView - m_cameraScript.GetMinFov()) / (m_cameraScript.GetMaxFov() - m_cameraScript.GetMinFov()));
            if (vol > m_maxVolume)
                vol = m_maxVolume;
            else if (vol < m_minVolume)
                vol = m_minVolume;
            m_thisAudioScource.volume = vol;

            rang = ((m_cameraScript.GetCamera().fieldOfView - m_cameraScript.GetMinFov()) / (m_cameraScript.GetMaxFov() - m_cameraScript.GetMinFov()));
            m_thisAudioScource.maxDistance = m_minRange + ((m_maxRange - m_minRange) * rang);
            yield return null;
        }
        yield return null;
    }

    private void DisableEmitter()
    {
        if (m_cameraScript.GetCameraState() == CameraManager.ECamState.Iso2D)
        {
            m_thisAudioScource.mute = true;
        }
        else
        {
            m_thisAudioScource.mute = false;
        }
    }

    public void PlayAttackSound()
    {
        m_thisAudioScource.Play();

    }

    public void StopAttackSound()
    {
        m_thisAudioScource.Stop();
    }
    #endregion
}