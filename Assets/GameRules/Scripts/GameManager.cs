using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    // Dictionary to hold stars earned in each minigame
    private Dictionary<string, int> minigameStars = new Dictionary<string, int>();

    // Set to hold unlocked levels
    private HashSet<string> unlockedLevels = new HashSet<string>();

    // Keys for PlayerPrefs
    private const string StarDataKey = "MinigameStars";
    private const string CurrentLevelKey = "CurrentLevel";
    private const string TotalStarsKey = "TotalStars";
    private const string UnlockedLevelsKey = "UnlockedLevels";

    // Tracks the current level
    public string CurrentLevel { get; private set; }

    // Tracks the total stars earned across all minigames
    public int TotalStars { get; private set; }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            LoadStarsData();
            LoadUnlockedLevels();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        // Load the current level if it was previously saved
        if (PlayerPrefs.HasKey(CurrentLevelKey))
        {
            CurrentLevel = PlayerPrefs.GetString(CurrentLevelKey);
        }
    }

    // Method to set stars for a specific minigame
    public void SetStars(string minigameName, int stars)
    {
        if (stars < 0 || stars > 3)
        {
            Debug.LogWarning("Stars must be between 0 and 3.");
            return;
        }

        if (minigameStars.ContainsKey(minigameName))
        {
            if (stars > minigameStars[minigameName])
            {
                minigameStars[minigameName] = stars;
                SaveStarsData();
            }
        }
        else
        {
            minigameStars[minigameName] = stars;
            SaveStarsData();
        }

        // Update the total stars
        UpdateTotalStars();
    }

    // Method to get stars for a specific minigame
    public int GetStars(string minigameName)
    {
        if (minigameStars.TryGetValue(minigameName, out int stars))
        {
            return stars;
        }
        return 0;
    }

    // Method to save the stars data
    private void SaveStarsData()
    {
        foreach (var minigame in minigameStars)
        {
            PlayerPrefs.SetInt(StarDataKey + "_" + minigame.Key, minigame.Value);
        }
        PlayerPrefs.SetInt(TotalStarsKey, TotalStars);
        PlayerPrefs.Save();
    }

    // Method to load the stars data
    private void LoadStarsData()
    {
        foreach (string minigameName in GetMinigameNames())
        {
            if (PlayerPrefs.HasKey(StarDataKey + "_" + minigameName))
            {
                int stars = PlayerPrefs.GetInt(StarDataKey + "_" + minigameName, 0);
                minigameStars[minigameName] = stars;
            }
            else
            {
                minigameStars[minigameName] = 0;
            }
        }

        // Load total stars
        TotalStars = PlayerPrefs.GetInt(TotalStarsKey, 0);
    }

    // Method to reset all stars data (e.g., for a new game or reset)
    public void ResetAllStars()
    {
        minigameStars.Clear();
        foreach (string minigameName in GetMinigameNames())
        {
            PlayerPrefs.DeleteKey(StarDataKey + "_" + minigameName);
        }
        PlayerPrefs.DeleteKey(TotalStarsKey);
        TotalStars = 0;
        PlayerPrefs.Save();
    }

    // Method to reset all unlocked levels data (e.g., for a new game or reset)
    public void ResetAllUnlockedLevels()
    {
        unlockedLevels.Clear();
        PlayerPrefs.DeleteKey(UnlockedLevelsKey);
        PlayerPrefs.Save();
    }

    // This method should return all the minigame names. 
    // You can manually add them here or retrieve them dynamically if stored elsewhere.
    private List<string> GetMinigameNames()
    {
        return new List<string> { "Minigame1", "Minigame2", "Minigame3" }; // Replace with your actual minigame names
    }

    // Helper method to get the total stars earned across all minigames
    public int GetTotalStars()
    {
        return TotalStars;
    }

    // Updates the total stars across all minigames
    private void UpdateTotalStars()
    {
        TotalStars = 0;
        foreach (var stars in minigameStars.Values)
        {
            TotalStars += stars;
        }
        SaveStarsData();
    }

    // Method to save the current level
    public void SaveCurrentLevel(string levelName)
    {
        CurrentLevel = levelName;
        PlayerPrefs.SetString(CurrentLevelKey, levelName);
        PlayerPrefs.Save();
    }

    // Method to load the current level
    public void LoadCurrentLevel()
    {
        if (!string.IsNullOrEmpty(CurrentLevel))
        {
            SceneManager.LoadScene(CurrentLevel);
        }
        else
        {
            Debug.LogWarning("No current level saved.");
        }
    }

    // Method to proceed to the next level
    public void LoadNextLevel(string nextLevelName)
    {
        SaveCurrentLevel(nextLevelName);
        SceneManager.LoadScene(nextLevelName);
    }

    // Method to reload the current level
    public void ReloadCurrentLevel()
    {
        if (!string.IsNullOrEmpty(CurrentLevel))
        {
            SceneManager.LoadScene(CurrentLevel);
        }
        else
        {
            Debug.LogWarning("No current level saved.");
        }
    }

    // Method to unlock a level
    public void UnlockLevel(string levelName)
    {
        if (!unlockedLevels.Contains(levelName))
        {
            unlockedLevels.Add(levelName);
            SaveUnlockedLevels();
        }
    }

    // Method to check if a level is unlocked
    public bool IsLevelUnlocked(string levelName)
    {
        return unlockedLevels.Contains(levelName);
    }

    // Method to save the unlocked levels data
    private void SaveUnlockedLevels()
    {
        string levels = string.Join(",", unlockedLevels);
        PlayerPrefs.SetString(UnlockedLevelsKey, levels);
        PlayerPrefs.Save();
    }

    // Method to load the unlocked levels data
    private void LoadUnlockedLevels()
    {
        if (PlayerPrefs.HasKey(UnlockedLevelsKey))
        {
            string levels = PlayerPrefs.GetString(UnlockedLevelsKey);
            unlockedLevels = new HashSet<string>(levels.Split(','));
        }
    }

    // Method to exit the game (for mobile)
    public void ExitGame()
    {
        Application.Quit();
    }
}
