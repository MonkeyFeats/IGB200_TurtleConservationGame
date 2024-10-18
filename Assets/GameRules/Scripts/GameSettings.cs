using UnityEngine;

[CreateAssetMenu(fileName = "GameSettings", menuName = "Settings/Game Settings")]
public class GameSettings : ScriptableObject
{
    public int qualityLevel = 2; // ( 0 = low, 1 = medium, 2 = high)
    public float masterVolume = 0.75f;
    public float sfxVolume = 1.0f;
    public float musicVolume = 1.0f;
    public float ambientVolume = 1.0f;
}