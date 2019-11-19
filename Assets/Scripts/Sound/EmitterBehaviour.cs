#pragma warning disable CS0618 // type deprecated
#region Author
/////////////////////////////////////////
//   Michel Bigourd --> AlertManager
//   https://linkedin.com/in/michel-bigourd-a8a05b100
/////////////////////////////////////////
#endregion

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class EmitterBehaviour : NetworkBehaviour
{
    #region Variables
    private CameraManager m_cameraScript;
    private AudioSource m_thisAudioScource;
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
    }

    void Start()
    {
        transform.position = new Vector3(transform.position.x - 9.7f, m_cameraScript.GetYCamPos(), transform.position.z - 19.5f);
        m_thisAudioScource.volume = 1 - ((m_cameraScript.GetCamera().fieldOfView - m_cameraScript.GetMinFov()) / (m_cameraScript.GetMaxFov() - m_cameraScript.GetMinFov())) + m_minVolume;
    }

    private void Update()
    {
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

    public void PlaySound()
    {
        m_thisAudioScource.Play();
    }

    public void StopSound()
    {

        m_thisAudioScource.Stop();
    }

    [ClientRpc]
    public void RpcPlaySound()
    {
        m_thisAudioScource.Play();
    }

    [TargetRpc]
    public void TargetPlaySound(NetworkConnection target)
    {
        m_thisAudioScource.Play();
    }
    #endregion
}
