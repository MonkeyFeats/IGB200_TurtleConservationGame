using UnityEngine;
using UnityEngine.Audio;

public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance;

    [Header("Audio Mixers")]
    public AudioMixer masterMixer;

    [Header("Volume Settings")]
    [Range(0f, 1f)] public float masterVolume = 1f;
    [Range(0f, 1f)] public float musicVolume = 1f;
    [Range(0f, 1f)] public float sfxVolume = 1f;
    [Range(0f, 1f)] public float ambientVolume = 1f;

    private void Awake()
    {
        // Singleton pattern
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        UpdateAllVolumes();
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

    // Slider control methods
    public void SetMasterVolume(float volume)
    {
        masterVolume = Mathf.Clamp01(volume);
        if (masterVolume == 0)
            masterMixer.SetFloat("MasterVolume", -80); // Mute volume
        else
            masterMixer.SetFloat("MasterVolume", Mathf.Log10(masterVolume) * 20);
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
    }
}
