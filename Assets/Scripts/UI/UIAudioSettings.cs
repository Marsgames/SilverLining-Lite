#pragma warning disable CS0618 // type deprecated
#region Author
/////////////////////////////////////////
//   MARION WARTELLE-MATHIEU 
/////////////////////////////////////////
#endregion
using UnityEngine;
using UnityEngine.Audio;

public class UIAudioSettings : MonoBehaviour
{
    #region Variables
    public AudioMixer audioMixer;
    #endregion

    #region Functions
    public void SetGlobalVolume(float GlobalVolume)
    {
        audioMixer.SetFloat("GlobalVolume", GlobalVolume);
    }

    public void SetMusicVolume(float MusicVolume)
    {
        audioMixer.SetFloat("MusicVolume", MusicVolume);
    }

    public void SetSFXVolume(float SFXVolume)
    {
        audioMixer.SetFloat("SFXVolume", SFXVolume);
    }

    #endregion
}