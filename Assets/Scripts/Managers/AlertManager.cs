#region Author
/////////////////////////////////////////
//   Michel Bigourd --> AlertManager
//   https://linkedin.com/in/michel-bigourd-a8a05b100
/////////////////////////////////////////
#endregion

using UnityEngine;

public class AlertManager : MonoBehaviour
{
    #region Variables
    public static AlertManager Instance;
    [SerializeField] private AudioSource m_fxAudioSource = null;
    [SerializeField] private SoundManager m_soundManager = null;
    private float m_masterVolume;
    private float m_fxVolume;
    private float m_lastAlertTimer = 0;

    //Capture Generator
    [SerializeField] private float m_recoverTimePICaptureGenerator = 0;
    [Range(0.0f, 1.0f)]
    [SerializeField] private float m_volumeCapturePIGenerator = 0;
    [SerializeField] private AudioClip AC_capturePIGenerator = null;

    //Capture Tower
    [SerializeField] private float m_recoverTimeCapturePITower = 0;
    [Range(0.0f, 1.0f)]
    [SerializeField] private float m_volumeCapturePITower = 0;
    [SerializeField] private AudioClip AC_capturePITower = null;

    //Capture Barrack
    [SerializeField] private float m_recoverTimeCapturePIBarrack = 0;
    [Range(0.0f, 1.0f)]
    [SerializeField] private float m_volumeCapturePIBarrack = 0;
    [SerializeField] private AudioClip AC_capturePIBarrack = null;

    //Zone Discover
    [SerializeField] private float m_recoverTimeZoneDiscovered = 0;
    [Range(0.0f, 1.0f)]
    [SerializeField] private float m_volumeZoneDiscovered = 0;
    [SerializeField] private AudioClip AC_zoneDiscovered = null;

    //Base in Danger
    [SerializeField] private float m_recoverTimeBaseInDanger = 0;
    [Range(0.0f, 1.0f)]
    [SerializeField] private float m_volumeBaseInDanger = 0;
    [SerializeField] private AudioClip AC_baseInDanger = null;

    //PI Attacked by the ennemi
    [SerializeField] private float m_recoverTimeAttackedPI = 0;
    [Range(0.0f, 1.0f)]
    [SerializeField] private float m_volumeAttackedPI = 0;
    [SerializeField] private AudioClip AC_attackedPI = null;
    #endregion

    #region Functions
    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(Instance.gameObject);
        }
        Instance = this;

        m_masterVolume = m_soundManager.GetVolumeMaster();
        m_fxVolume = m_soundManager.GetVolumeFX();
    }
    #endregion

    // Call TimersVerification[...] to play sound on an other script
    #region TimersVerification
    public void TimerVerificationCapturePIGenerator()
    {
        if (Time.time - m_lastAlertTimer >= m_recoverTimePICaptureGenerator)
        {
            PlayCapturePIGenerator();
            m_lastAlertTimer = Time.time;
        }
    }

    public void TimerVerificationCapturePITower()
    {
        if (Time.time - m_lastAlertTimer >= m_recoverTimeCapturePITower)
        {
            PlayCapturePITower();
            m_lastAlertTimer = Time.time;
        }
    }

    public void TimerVerificationCapturePIBarrack()
    {
        if (Time.time - m_lastAlertTimer >= m_recoverTimeCapturePIBarrack)
        {
            PlayCapturePIBarrack();
            m_lastAlertTimer = Time.time;
        }
    }

    public void TimerVerificationZoneDiscovered()
    {
        if (Time.time - m_lastAlertTimer >= m_recoverTimeZoneDiscovered)
        {
            PlayZoneDiscovered();
            m_lastAlertTimer = Time.time;
        }
    }

    public void TimerVerificationBaseInDanger()
    {
        if (Time.time - m_lastAlertTimer >= m_recoverTimeBaseInDanger)
        {
            PlayBaseInDanger();
            m_lastAlertTimer = Time.time;
        }
    }

    public void TimerVerificationAttackedPI()
    {
        if (Time.time - m_lastAlertTimer >= m_recoverTimeAttackedPI)
        {
            PlayAttackedPI();
            m_lastAlertTimer = Time.time;
        }
    }
    #endregion

    #region PlaySound
    private void PlayCapturePIGenerator()
    {
        m_fxAudioSource.clip = AC_capturePIGenerator;
        m_fxAudioSource.volume = m_volumeCapturePIGenerator * m_fxVolume * m_masterVolume;
        m_fxAudioSource.Play();
    }

    private void PlayCapturePITower()
    {
        m_fxAudioSource.clip = AC_capturePITower;
        m_fxAudioSource.volume = m_volumeCapturePITower * m_fxVolume * m_masterVolume;
        m_fxAudioSource.Play();
    }

    private void PlayCapturePIBarrack()
    {
        m_fxAudioSource.clip = AC_capturePIBarrack;
        m_fxAudioSource.volume = m_volumeCapturePIBarrack * m_fxVolume * m_masterVolume;
        m_fxAudioSource.Play();
    }

    private void PlayZoneDiscovered()
    {
        m_fxAudioSource.clip = AC_zoneDiscovered;
        m_fxAudioSource.volume = m_volumeZoneDiscovered * m_fxVolume * m_masterVolume;
        m_fxAudioSource.Play();
    }

    private void PlayBaseInDanger()
    {
        m_fxAudioSource.clip = AC_baseInDanger;
        m_fxAudioSource.volume = m_volumeBaseInDanger * m_fxVolume * m_masterVolume;
        m_fxAudioSource.Play();
    }

    private void PlayAttackedPI()
    {
        m_fxAudioSource.clip = AC_attackedPI;
        m_fxAudioSource.volume = m_volumeAttackedPI * m_fxVolume * m_masterVolume;
        m_fxAudioSource.Play();
    }
    #endregion


    #region Accessors
    public void SetFXVolume(float volume)
    {
        m_fxVolume = volume;
    }

    public void SetMasterVolume(float volume)
    {
        m_masterVolume = volume;
    }
    #endregion
}