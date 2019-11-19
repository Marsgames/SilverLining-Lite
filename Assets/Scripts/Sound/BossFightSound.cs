using System.Collections;
using UnityEngine;

public class BossFightSound : MonoBehaviour
{
    [SerializeField] private AudioClip m_bossFight = null;
    private AudioSource m_musicAudioSource;
    private SoundManager m_SoundManager;
    private float m_masterVolume;
    private float m_musicVolume;
    private MusicManager m_musicManager;
    //private bool m_isPlaying;

    void Start()
    {
        m_SoundManager = SoundManager.Instance;
        m_musicAudioSource = GetComponent<AudioSource>();
        m_masterVolume = m_SoundManager.GetVolumeMaster();
        m_musicVolume = m_SoundManager.GetVolumeMusic();
        m_musicManager = GetComponentInParent<MusicManager>();
    }

    public void PlayBossMusic()
    {
        if (m_musicAudioSource.isPlaying)
        {
            return;
        }

        m_musicAudioSource.clip = m_bossFight;
        m_musicAudioSource.volume = m_masterVolume * m_musicVolume;
        m_musicAudioSource.Play();
        StartCoroutine(m_musicManager.BossFightStartedCountdown());
    }

    public void StopBossMusic()
    {
        StartCoroutine(m_musicManager.BossFightEndedCountdown());
        StartCoroutine(BossMusicFade());
        m_musicAudioSource.Stop();
    }

    public IEnumerator BossMusicFade()
    {
        while (m_musicAudioSource.volume > 0)
        {
            m_musicAudioSource.volume -= Time.deltaTime / 25;
            yield return null;
        }
    }
}
