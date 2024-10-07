using UnityEngine;

[CreateAssetMenu(fileName = "NewLevelData", menuName = "Level Data")]
public class LevelData : ScriptableObject
{
    public string levelName;            // Name of the level
    public string levelDescription;     // Description of the level
    public bool isDialogueScene;          // Prevents stars from showing up
    public Sprite levelImage;           // Optional image for the level
    public int levelIndex;              // The index of the level
    public bool isUnlocked; 
    public int starsEarned;             // Star count, make hidden private for external access
}