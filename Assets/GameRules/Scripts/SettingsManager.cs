using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;
using System.Collections;
using UnityEngine.Rendering;

public class SettingsManager : MonoBehaviour
{
    public GameSettings gameSettings;
    public AudioMixer masterMixer;
    public AudioMixerSnapshot mutedSnapshot;
    public AudioMixerSnapshot normalSnapshot;
    public AudioSource musicPlayer;

    public Toggle lowQualityToggle;
    public Toggle mediumQualityToggle;
    public Toggle highQualityToggle;

    public Slider masterSlider;
    public Slider sfxSlider;
    public Slider musicSlider;
    public Slider ambienceSlider;

    [Header("Volume Settings")]
    [Range(0f, 1f)] public float masterVolume = 1f;
    [Range(0f, 1f)] public float musicVolume = 1f;
    [Range(0f, 1f)] public float sfxVolume = 1f;
    [Range(0f, 1f)] public float ambientVolume = 1f;

    private Coroutine fadeCoroutine;

    private void OnEnable()
    {
    }
    private void Start()
    {
        LoadUI();
        StartWithFadeIn();
    }
    private void Awake()
    {
        StartWithFadeIn();
    }
    private void LoadUI()
    {
        // Load quality settings
        lowQualityToggle.isOn = (gameSettings.qualityLevel == 0);
        mediumQualityToggle.isOn = (gameSettings.qualityLevel == 1);
        highQualityToggle.isOn = (gameSettings.qualityLevel == 2);

        // Load audio settings into sliders
        masterSlider.value = gameSettings.masterVolume;
        sfxSlider.value = gameSettings.sfxVolume;
        musicSlider.value = gameSettings.musicVolume;
        ambienceSlider.value = gameSettings.ambientVolume;
    }

    private void StartWithFadeIn()
    {
        // Start fade in at scene start
        FadeInAudio(0.7f);
    }

    public void SetMasterVolumeFromSlider()
    {
        SetMasterVolume(masterSlider.value);
    }

    public void SetMusicVolumeFromSlider()
    {
        SetMusicVolume(musicSlider.value);
    }

    public void SetSFXVolumeFromSlider()
    {
        SetSFXVolume(sfxSlider.value);
    }

    public void SetAmbientVolumeFromSlider()
    {
        SetAmbientVolume(ambienceSlider.value);
    }

    private void Update()
    {
        //UpdateAllVolumes();
    }

    public void UpdateAllVolumes()
    {
        SetMasterVolume(masterVolume);
        SetMusicVolume(musicVolume);
        SetSFXVolume(sfxVolume);
        SetAmbientVolume(ambientVolume);
    }

    public void SetQualityLow() { ApplyQualitySetting(0); }
    public void SetQualityMedium() { ApplyQualitySetting(1); }
    public void SetQualityHigh() { ApplyQualitySetting(2); }

    public void ApplyQualitySetting(int level)
    {
        QualitySettings.SetQualityLevel(level);
        gameSettings.qualityLevel = level;
    }

    public void SetMasterVolume(float volume)
    {
        masterVolume = Mathf.Clamp01(volume);
        masterMixer.SetFloat("MasterVolume", Mathf.Log10(masterVolume) * 20);
        gameSettings.masterVolume = masterVolume;
    }

    public void SetMusicVolume(float volume)
    {
        musicVolume = Mathf.Clamp01(volume);
        masterMixer.SetFloat("MusicVolume", Mathf.Log10(musicVolume) * 20);
        gameSettings.musicVolume = musicVolume;
    }

    public void SetSFXVolume(float volume)
    {
        sfxVolume = Mathf.Clamp01(volume);
        masterMixer.SetFloat("SFXVolume", Mathf.Log10(sfxVolume) * 20);
        gameSettings.sfxVolume = sfxVolume;
    }

    public void SetAmbientVolume(float volume)
    {
        ambientVolume = Mathf.Clamp01(volume);
        masterMixer.SetFloat("AmbientVolume", Mathf.Log10(ambientVolume) * 20);
        gameSettings.ambientVolume = ambientVolume;
    }

    public void FadeInAudio(float fadeDuration = 0.3f)
    {
        if (fadeCoroutine != null) StopCoroutine(fadeCoroutine);
        fadeCoroutine = StartCoroutine(FadeToSnapshot(normalSnapshot, fadeDuration));
    }

    public void FadeOutAudio(float fadeDuration = 0.3f)
    {
        if (fadeCoroutine != null) StopCoroutine(fadeCoroutine);
        fadeCoroutine = StartCoroutine(FadeToSnapshot(mutedSnapshot, fadeDuration));
    }

    // Coroutine to handle snapshot transitions
    private IEnumerator FadeToSnapshot(AudioMixerSnapshot targetSnapshot, float duration)
    {
        if (targetSnapshot == normalSnapshot && musicPlayer != null)
            musicPlayer.UnPause();

        targetSnapshot.TransitionTo(duration); // Transition to the target snapshot over 'duration' seconds
        yield return new WaitForSeconds(duration); // Wait for the transition to complete

        if (targetSnapshot == mutedSnapshot && musicPlayer != null)
            musicPlayer.Pause(); // Pause music if fading out
    }
}
