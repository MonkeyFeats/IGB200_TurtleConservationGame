using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;
using System.Collections;
using UnityEngine.Rendering;

public class SettingsManager : MonoBehaviour
{
    public GameSettings gameSettings; // Reference to the ScriptableObject
    public AudioMixer masterMixer;
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

    [Header("Fade Settings")]
    [Range(0f, 1f)] public float fadeVolume = 0f;

    private Coroutine fadeCoroutine;

    private void OnEnable()
    {
    }
    private void Start()
    {
        LoadUI();
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
        // Start with Fade volume at a lower level (fadeToVolume) and then fade in
        fadeVolume = 0f; //Start Quiet
        masterMixer.SetFloat("FadeAmount", -80); // Ensure it's exactly at the target
        FadeInVolume(1.3f);
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
        UpdateAllVolumes();
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

    public void FadeInVolume(float time = 0.3f)
    {
        if (fadeCoroutine != null) StopCoroutine(fadeCoroutine);
        fadeCoroutine = StartCoroutine(FadeInAudio(time));
    }
    public void FadeOutVolume(float time = 0.3f)
    {
        if (fadeCoroutine != null) StopCoroutine(fadeCoroutine);
        fadeCoroutine = StartCoroutine(FadeOutAudio(time));
    }

    // Coroutine to handle fade over time
    public IEnumerator FadeInAudio(float fadeDuration)
    {
        if (musicPlayer != null)
            musicPlayer.UnPause();

        float currentTime = 0f;
        float startVolume = Mathf.Pow(10f, fadeVolume / 20f); // Convert current dB volume to linear scale
        float targetVolume = 1f; // Target volume (1 is full volume in linear scale)

        while (currentTime < fadeDuration)
        {
            currentTime += Time.deltaTime;
            fadeVolume = Mathf.Lerp(startVolume, targetVolume, currentTime / fadeDuration);
            fadeVolume = Mathf.Clamp(fadeVolume, 0f, 1f); // Clamp the value to prevent going above full volume
            masterMixer.SetFloat("FadeAmount", Mathf.Log10(fadeVolume) * 20); // Convert back to dB
            yield return null;
        }

        masterMixer.SetFloat("FadeAmount", Mathf.Log10(targetVolume) * 20); // Ensure it's exactly at the target
    }

    public IEnumerator FadeOutAudio(float fadeDuration)
    {
        float currentTime = 0f;
        float startVolume = Mathf.Pow(10f, fadeVolume / 20f); // Convert current dB volume to linear scale
        float targetVolume = 0f; // Mute

        while (currentTime < fadeDuration)
        {
            currentTime += Time.deltaTime;
            fadeVolume = Mathf.Lerp(startVolume, targetVolume, currentTime / fadeDuration);
            fadeVolume = Mathf.Clamp(fadeVolume, 0.0001f, 1f); // Clamp to avoid logarithmic issues (no negative infinity)
            masterMixer.SetFloat("FadeAmount", Mathf.Log10(fadeVolume) * 20); // Convert back to dB
            yield return null;
        }

        masterMixer.SetFloat("FadeAmount", Mathf.Log10(0.0001f) * 20); // Set to a very low value at the end
        if (musicPlayer != null)
            musicPlayer.Pause();
    }

}
