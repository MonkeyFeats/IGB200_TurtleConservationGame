using UnityEngine;
using UnityEngine.Events;

[System.Serializable]
public class LevelSelectData
{
    public string levelName;            // Name of the level
    public string levelDescription;     // Description of the level
    public Sprite levelImage;           // Optional image for the level
    public int levelIndex;              // The index of the level
    public int topScore;                // Star count, possibly should be privated
    public UnityEvent onButtonPressed; // Action to perform when the button is pressed
}
