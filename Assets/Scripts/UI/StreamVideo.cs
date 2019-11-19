using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.Video;

public class StreamVideo : MonoBehaviour
{
    private string m_videoAdress = "";
    private RawImage m_rawImage;
    private VideoPlayer m_videoPlayer;
    private AudioSource m_musicAudioSource;
    private MusicManager m_musicManager;
    private float m_masterVolume = 1;
    private float m_musicVolume;

    public void PlayVideo()
    {
        m_rawImage = GetComponent<RawImage>();
        m_musicManager = FindObjectOfType<MusicManager>();
        m_musicAudioSource = m_musicManager.GetComponent<AudioSource>();
        m_musicVolume = m_musicManager.GetMusicVolume();
        m_musicManager.SetMusicVolume(0.1f);

        StartCoroutine(PrivatePlayVideo());
    }

    private IEnumerator PrivatePlayVideo()
    {

        m_videoPlayer = gameObject.AddComponent<VideoPlayer>();
        m_videoPlayer.playOnAwake = false;
        m_videoPlayer.source = VideoSource.Url;
        m_videoPlayer.url = m_videoAdress;
        m_videoPlayer.Prepare();

        while (!m_videoPlayer.isPrepared)
        {
            yield return null;
        }

        m_rawImage.texture = m_videoPlayer.texture;
        if (m_masterVolume < 0.05f)
        {
            m_videoPlayer.SetDirectAudioVolume((ushort)0, 0.1f);
        }
        else
        {
            m_videoPlayer.SetDirectAudioVolume((ushort)0, m_masterVolume);
        }
        m_videoPlayer.Play();

        while (m_videoPlayer.isPlaying)
        {
            yield return null;
        }

        m_videoPlayer.Stop();
        m_musicManager.SetMusicVolume(m_musicVolume);
        gameObject.SetActive(false);
    }

    public void StopVideo()
    {
        m_videoPlayer.Stop();
        m_musicManager.SetMusicVolume(m_musicVolume);
        gameObject.SetActive(false);
    }

    public void SetAdress(string adress)
    {
        m_videoAdress = adress;
    }

    public void SetMasterVolume(float volume)
    {
        m_masterVolume = volume;
    }
}
