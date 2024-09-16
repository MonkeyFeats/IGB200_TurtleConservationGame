using UnityEngine;
using UnityEngine.UI; // For UI components

public class BoatCleanupLevelManager : MonoBehaviour
{
    // Public variables
    public int totalRubbish = 10; // Total rubbish in the level
    public float levelTime = 300f; // Total time in seconds for the level (e.g., 5 minutes)

    // UI elements (optional)
    public Text rubbishCollectedText; // UI text for rubbish collected
    public Text timeLeftText; // UI text for time remaining

    // Private variables
    private int rubbishCollected = 0; // Amount of rubbish collected
    private float timeLeft; // Timer for the level

    void Start()
    {
        // Initialize rubbish count and time
        rubbishCollected = 0;
        timeLeft = levelTime;

        // Update UI at start
        UpdateRubbishUI();
        UpdateTimeUI();
    }

    void Update()
    {
        // Update the timer every frame
        timeLeft -= Time.deltaTime;
        UpdateTimeUI();

        // Check if the level has ended due to time running out
        if (timeLeft <= 0f)
        {
            EndLevel(false); // Lose the game
        }

        // Check win condition (all rubbish collected)
        if (rubbishCollected >= totalRubbish)
        {
            EndLevel(true); // Win the game
        }
    }

    // Call this method whenever the player collects rubbish
    public void CollectRubbish()
    {
        rubbishCollected++;
        UpdateRubbishUI();

        // Check if this is the last piece of rubbish
        if (rubbishCollected >= totalRubbish)
        {
            EndLevel(true); // Win the game
        }
    }

    // Updates the rubbish collected UI
    private void UpdateRubbishUI()
    {
        if (rubbishCollectedText != null)
        {
            rubbishCollectedText.text = "Rubbish Collected: " + rubbishCollected + " / " + totalRubbish;
        }
    }

    // Updates the time left UI
    private void UpdateTimeUI()
    {
        if (timeLeftText != null)
        {
            timeLeftText.text = "Time Left: " + Mathf.Max(0, Mathf.FloorToInt(timeLeft)) + "s";
        }
    }

    // End the level (win or lose)
    private void EndLevel(bool hasWon)
    {
        // Stop the game
        Time.timeScale = 0; // Freeze game when the level ends

        if (hasWon)
        {
            Debug.Log("You've collected all the rubbish! You win!");
        }
        else
        {
            Debug.Log("Time's up! You lose!");
        }

        // Here you could load a new scene, display a game over screen, etc.
        // For now, just restart the level
        RestartLevel();
    }

    // Restart the level (for simplicity, just reload the current scene)
    private void RestartLevel()
    {
        Time.timeScale = 1; // Reset time scale
        UnityEngine.SceneManagement.SceneManager.LoadScene(UnityEngine.SceneManagement.SceneManager.GetActiveScene().name);
    }
}
