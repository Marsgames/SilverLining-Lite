using UnityEngine;

public class VictorySound : MonoBehaviour
{

    [SerializeField] private AudioClip m_victory = null;
    private AudioSource m_musicAudioSource;
    private SoundManager m_SoundManager;
    private float m_masterVolume;
    private float m_musicVolume;
    private MusicManager m_musicManager;

    void Start()
    {
        m_SoundManager = SoundManager.Instance;
        m_musicAudioSource = GetComponent<AudioSource>();
        m_masterVolume = m_SoundManager.GetVolumeMaster();
        m_musicVolume = m_SoundManager.GetVolumeMusic();
        m_musicManager = GetComponentInParent<MusicManager>();
    }
    
    public void PlayVictorySound()
    {
        m_musicAudioSource.clip = m_victory;
        m_musicAudioSource.volume = m_masterVolume * m_musicVolume;
        m_musicAudioSource.Play();
        StartCoroutine(m_musicManager.GameStartedCountdown());
    }
}
