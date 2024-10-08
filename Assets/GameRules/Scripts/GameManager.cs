using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    // List of LevelData Scriptable Objects
    public List<LevelData> levels; // Assign in the Inspector
    private HashSet<string> unlockedLevels = new HashSet<string>();

    // Keys for PlayerPrefs
    private const string CurrentLevelKey = "CurrentLevel";
    private const string TotalStarsKey = "TotalStars";
    private const string UnlockedLevelsKey = "UnlockedLevels";

    public string CurrentLevel { get; private set; }
    public int TotalStars { get; private set; }

    public int lastPlayedLevel = -1; // Used for Making the camera go back to where it was and prevent Title Screen Popup

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            LoadUnlockedLevels();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        if (PlayerPrefs.HasKey(CurrentLevelKey))
        {
            CurrentLevel = PlayerPrefs.GetString(CurrentLevelKey);
        }

        UpdateTotalStars();
    }

    public void SetStarsForLevel(int levelIndex, int stars)
    {
        if (levelIndex >= 0 )
        {
            levels[levelIndex].starsEarned = stars;
            UpdateTotalStars();
            Debug.Log($"Stars for level {levels[levelIndex].levelName} set to: {stars}");
        }
        else
        {
            Debug.LogWarning("Level index is out of range!");
        }
    }

    public int GetStarsForLevel(int levelIndex)
    {
        if (levelIndex >= 0 && levelIndex < levels.Count)
        {
            return levels[levelIndex].starsEarned;
        }
        return 0;
    }

    private void UpdateTotalStars()
    {
        TotalStars = 0;
        foreach (var level in levels)
        {
            TotalStars += level.starsEarned;
        }
        PlayerPrefs.SetInt(TotalStarsKey, TotalStars);
        PlayerPrefs.Save();
    }

    public void SaveCurrentLevel(string levelName)
    {
        CurrentLevel = levelName;
        PlayerPrefs.SetString(CurrentLevelKey, levelName);
        PlayerPrefs.Save();
    }

    public void SetLastPlayedLevel(int index)
    {
        lastPlayedLevel = index;
        //PlayerPrefs.SetInt("LastPlayedLevel", index); // Saves even after game closed
    }


    public void UnlockLevel(string levelName)
    {
        if (!unlockedLevels.Contains(levelName))
        {
            unlockedLevels.Add(levelName);
            SaveUnlockedLevels();
        }
    }

    public bool IsLevelUnlocked(string levelName)
    {
        return unlockedLevels.Contains(levelName);
    }

    private void SaveUnlockedLevels()
    {
        string levelsString = string.Join(",", unlockedLevels);
        PlayerPrefs.SetString(UnlockedLevelsKey, levelsString);
        PlayerPrefs.Save();
    }

    private void LoadUnlockedLevels()
    {
        if (PlayerPrefs.HasKey(UnlockedLevelsKey))
        {
            string levelsString = PlayerPrefs.GetString(UnlockedLevelsKey);
            unlockedLevels = new HashSet<string>(levelsString.Split(','));
        }
    }

    public void ExitGame()
    {
        Application.Quit();
    }
}
