using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;

public class SettingsManager : MonoBehaviour
{
    public GameSettings gameSettings; // Reference to the ScriptableObject
    public AudioMixer masterMixer;

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

    private void OnEnable()
    {
        LoadUI();
        UpdateAllVolumes();
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

    public void SetMasterVolumeFromSlider()
    {
        SetMasterVolume(masterSlider.value); // Scale from 0-100 to 0-1
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

    // Update the mixer volumes for all categories
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
        if (masterVolume == 0)
            masterMixer.SetFloat("MasterVolume", -80); // Mute volume
        else
            masterMixer.SetFloat("MasterVolume", Mathf.Log10(masterVolume) * 20);

        gameSettings.masterVolume = masterVolume;  
    }

    public void SetMusicVolume(float volume)
    {
        musicVolume = Mathf.Clamp01(volume);
        if (musicVolume == 0)
        {
            masterMixer.SetFloat("MusicVolume", -80);  // Mute volume
        }
        else
        {
            masterMixer.SetFloat("MusicVolume", Mathf.Log10(musicVolume) * 20);
        }
        gameSettings.musicVolume = musicVolume;
    }


    public void SetSFXVolume(float volume)
    {
        sfxVolume = Mathf.Clamp01(volume);
        if (sfxVolume == 0)
        {
            masterMixer.SetFloat("SFXVolume", -80);  // Mute volume
        }
        else
        {
            masterMixer.SetFloat("SFXVolume", Mathf.Log10(sfxVolume) * 20);
        }
        gameSettings.musicVolume = sfxVolume;
    }

    public void SetAmbientVolume(float volume)
    {
        ambientVolume = Mathf.Clamp01(volume);
        if (ambientVolume == 0)
        {
            masterMixer.SetFloat("AmbientVolume", -80);  // Mute volume
        }
        else
        {
            masterMixer.SetFloat("AmbientVolume", Mathf.Log10(ambientVolume) * 20);
        }
        gameSettings.musicVolume = ambientVolume;
    }
    
}
