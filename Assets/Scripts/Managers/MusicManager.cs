#region Author
/////////////////////////////////////////
//   Michel Bigourd
//   https://linkedin.com/in/michel-bigourd-a8a05b100
/////////////////////////////////////////
#endregion

using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MusicManager : MonoBehaviour
{
    #region Variables
    public static MusicManager Instance;

    [SerializeField] private AudioClip m_lobbyMusic = null;
    [SerializeField] private AudioClip m_mainMusic = null;
    private AudioSource m_musicAudioSource;
    private SoundManager m_SoundManager;
    private float m_masterVolume;
    private float m_musicVolume;
    private AudioReverbFilter m_reverbeFilter;
    #endregion

    #region Functions
    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(Instance.gameObject);
        }
        Instance = this;    
    }

    private void Start()
    {
        m_SoundManager = SoundManager.Instance;
        m_musicAudioSource = GetComponent<AudioSource>();
        m_reverbeFilter = GetComponent<AudioReverbFilter>();
        m_masterVolume = m_SoundManager.GetVolumeMaster();
        m_musicVolume = m_SoundManager.GetVolumeMusic();
        PlayLobbyMusic();
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.buildIndex == 2)
        {
            PlayMainMusic();
        }
        else if (scene.buildIndex == 0)
        {
            PlayLobbyMusic();
        }
        else if (scene.buildIndex == 1 && m_musicAudioSource.clip != m_lobbyMusic)
        {
            PlayLobbyMusic();
        }
    }

    private void PlayLobbyMusic()
    {

        m_musicAudioSource.clip = m_lobbyMusic;
        m_musicAudioSource.loop = true;
        m_musicAudioSource.volume = m_masterVolume * m_musicVolume;
        m_musicAudioSource.Play();
    }

    private void PlayMainMusic()
    {
        m_musicAudioSource.clip = m_mainMusic;
        m_musicAudioSource.loop = true;
        m_musicAudioSource.volume = m_masterVolume * m_musicVolume;
        m_musicAudioSource.Play();
    }

    public void ChangeVolumeMusic()
    {
        m_musicAudioSource.volume = m_masterVolume * m_musicVolume;
    }

    public IEnumerator GameStartedCountdown()
    {
        while (m_musicAudioSource.volume >= (m_masterVolume * m_musicVolume) / 50)
        {
            m_musicAudioSource.volume -= Time.deltaTime;
            yield return null;
        }
    }


    public void ReverbeChangerCamera2D()
    {
        m_reverbeFilter.dryLevel = -2000;
        m_reverbeFilter.dryLevel = -500;
        m_reverbeFilter.roomHF = -1500;
    }

    public void ReverbeChangerCamera3D()
    {
        m_reverbeFilter.dryLevel = 0;
        m_reverbeFilter.dryLevel = 0;
        m_reverbeFilter.roomHF = 0;
    }


    public IEnumerator BossFightStartedCountdown()
    {
        while (m_musicAudioSource.volume >= (m_masterVolume * m_musicVolume) / 4)
        {
            m_musicAudioSource.volume -= Time.deltaTime / 25;
            yield return null;
        }
    }

    public IEnumerator BossFightEndedCountdown()
    {
        while (m_musicAudioSource.volume <= (m_masterVolume * m_musicVolume))
        {
            m_musicAudioSource.volume += Time.deltaTime / 25;
            yield return null;
        }
    }
    #endregion

    #region Accessors
    public void SetMasterVolume(float volume)
    {
        m_masterVolume = volume;
        ChangeVolumeMusic();
    }

    public void SetMusicVolume(float volume)
    {
        m_musicVolume = volume;
        ChangeVolumeMusic();
    }

    public float GetMusicVolume()
    {
        return m_musicVolume;
    }
    #endregion
}
