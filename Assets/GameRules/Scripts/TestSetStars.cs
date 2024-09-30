using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TestSetStars : MonoBehaviour
{
    public GameManager gameManager; // Reference to your GameManager
    public int levelIndex = 0; // Index of the level you want to set stars for
    public int starsToSet = 3; // Number of stars to set (0 to max stars)

    public void SetStars()
    {
        // Check if the gameManager is assigned
        if (gameManager != null)
        {
            // Set the stars for the specified level
            gameManager.SetStarsForLevel(levelIndex, starsToSet);

            // Optional: Log to confirm stars are set
            Debug.Log($"Stars for level {levelIndex} set to: {starsToSet}");
        }
        else
        {
            Debug.LogWarning("GameManager is not assigned!");
        }
    }
}
