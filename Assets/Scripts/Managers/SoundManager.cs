#pragma warning disable CS0649 // useless variable
#region Author
/////////////////////////////////////////
//   Michel Bigourd --> AlertManager
//   https://linkedin.com/in/michel-bigourd-a8a05b100
/////////////////////////////////////////
#endregion
using System;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    #region Variables
    public static SoundManager Instance;

    public enum AudioClipList
    {
        AC_clickBtnMenu,
        AC_mouseOver,
        AC_cdEndedAbilities,
        AC_obstableImpossibleInteract,
        AC_unitBuy,
        AC_notEnoughMoney,
        AC_releaseOneUnit,
        AC_generatorClick,
        AC_clickOnCP,
        AC_clickOnWall,
        AC_wallDeconstruct,
        AC_bombUsed,
        AC_construction
    }

    [Range(0.0f, 1.0f)]
    [SerializeField] private float m_masterVolume = 0;
    [Range(0.0f, 1.0f)]
    [SerializeField] private float m_fxVolume = 0;
    [Range(0.0f, 1.0f)]
    [SerializeField] private float m_musicVolume = 0;

    private AudioSource m_MyAudioSource;
    [Header("Menu")]
    [Range(0.0f, 1.0f)]
    [SerializeField] private float m_volumeMenuBtn = 0;
    [SerializeField] private AudioClip AC_clickBtnMenu = null;
    [Range(0.0f, 1.0f)]
    [SerializeField] private float m_volumeMouseOver = 0;
    [SerializeField] private AudioClip AC_mouseOver = null;
    [Header("UI")]
    [Range(0.0f, 1.0f)]
    [SerializeField] private float m_volumeCDAbilitiesEnded = 0;
    [SerializeField] private AudioClip AC_cdEndedAbilities = null;
    [Range(0.0f, 1.0f)]
    [SerializeField] private float m_volumeImpossibleInteract = 0;
    [SerializeField] private AudioClip AC_obstableImpossibleInteract = null;
    [Range(0.0f, 1.0f)]
    [SerializeField] private float m_volumeUnitBuy = 0;
    [SerializeField] private AudioClip AC_unitBuy = null;
    [Range(0.0f, 1.0f)]
    [SerializeField] private float m_volumeNotEnoughMoney = 0;
    [SerializeField] private AudioClip AC_notEnoughMoney = null;
    [Range(0.0f, 1.0f)]
    [SerializeField] private float m_volumeRelaseOneUnit = 0;
    [SerializeField] private AudioClip AC_releaseOneUnit = null;
    [Range(0.0f, 1.0f)]
    [SerializeField] private float m_volumeGeneratorClick = 0;
    [SerializeField] private AudioClip AC_generatorClick = null;
    [Range(0.0f, 1.0f)]
    [SerializeField] private float m_volumeClickOnCP = 0;
    [SerializeField] private AudioClip AC_clickOnCP = null;
    [Range(0.0f, 1.0f)]
    [SerializeField] private float m_volumeClickWall = 0;
    [SerializeField] private AudioClip AC_clickOnWall = null;
    [Range(0.0f, 1.0f)]
    [SerializeField] private float m_volumeWallDeconstruct = 0;
    [SerializeField] private AudioClip AC_wallDeconstruct = null;
    [Range(0.0f, 1.0f)]
    [SerializeField] private float m_volumeBombUsed = 0;
    [SerializeField] private AudioClip AC_bombUsed = null;
    [Range(0.0f, 1.0f)]
    [SerializeField] private float m_volumeConstruction = 0;
    [SerializeField] private AudioClip AC_construction = null;

    private Dictionary<AudioClipList, Tuple<AudioClip, float>> m_audioDico = new Dictionary<AudioClipList, Tuple<AudioClip, float>>();
    #endregion

    #region Unity's function
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
        DontDestroyOnLoad(gameObject);
        m_MyAudioSource = GetComponent<AudioSource>();

        m_audioDico = new Dictionary<AudioClipList, Tuple<AudioClip, float>>()
        {
            {AudioClipList.AC_clickBtnMenu, new Tuple<AudioClip, float>(AC_clickBtnMenu, m_volumeMenuBtn) },
            {AudioClipList.AC_mouseOver, new Tuple<AudioClip, float>(AC_mouseOver, m_volumeMouseOver)},
            {AudioClipList.AC_cdEndedAbilities, new Tuple<AudioClip, float>(AC_cdEndedAbilities,m_volumeCDAbilitiesEnded)},
            {AudioClipList.AC_obstableImpossibleInteract, new Tuple<AudioClip, float>(AC_obstableImpossibleInteract, m_volumeImpossibleInteract)},
            {AudioClipList.AC_unitBuy, new Tuple<AudioClip, float>(AC_unitBuy, m_volumeUnitBuy)},
            {AudioClipList.AC_notEnoughMoney, new Tuple<AudioClip, float>(AC_notEnoughMoney, m_volumeNotEnoughMoney)},
            {AudioClipList.AC_releaseOneUnit, new Tuple<AudioClip, float>(AC_releaseOneUnit, m_volumeRelaseOneUnit)},
            {AudioClipList.AC_generatorClick, new Tuple<AudioClip, float>(AC_generatorClick, m_volumeGeneratorClick)},
            {AudioClipList.AC_clickOnCP, new Tuple<AudioClip, float>(AC_clickOnCP, m_volumeClickOnCP)},
            {AudioClipList.AC_clickOnWall,new Tuple<AudioClip, float>(AC_clickOnWall, m_volumeClickWall)},
            {AudioClipList.AC_wallDeconstruct, new Tuple<AudioClip, float>(AC_wallDeconstruct, m_volumeWallDeconstruct)},
            {AudioClipList.AC_bombUsed, new Tuple<AudioClip, float>(AC_bombUsed, m_volumeBombUsed)},
            {AudioClipList.AC_construction,new Tuple<AudioClip, float>(AC_construction, m_volumeConstruction)}
        };
    }
    #endregion

    #region Function
    public void PlaySound(AudioClipList audioClip)
    {
        AudioClip ac = m_audioDico[audioClip].Item1;
        m_MyAudioSource.clip = ac;
        m_MyAudioSource.volume = m_audioDico[audioClip].Item2 * m_fxVolume * m_masterVolume;
        m_MyAudioSource.Play();
    }
    #endregion

    #region Accessors
    public float GetVolumeMaster()
    {
        return m_masterVolume;
    }

    public void SetVolumeMaster(float _masterVolume)
    {
        m_masterVolume = _masterVolume;
    }
    public float GetVolumeFX()
    {
        return m_fxVolume;
    }

    public void SetVolumeFX(float _fxVolume)
    {
        m_fxVolume = _fxVolume;
    }

    public float GetVolumeMusic()
    {
        return m_musicVolume;
    }

    public void SetVolumeMusic(float _musicVolume)
    {
        m_musicVolume = _musicVolume;
    }
    #endregion
}
