using UnityEngine;
using TMPro; // For TextMeshPro components

public class BoatCleanupLevelManager : MonoBehaviour
{
    // Public variables
    public int totalRubbish = 10; // Total rubbish to collect in the level
    public float levelTime = 300f; // Total time in seconds for the level (e.g., 5 minutes)
    public GameObject rubbishPrefab; // Rubbish prefab to spawn
    public float spawnInterval = 5f; // Time interval for spawning rubbish
    public Vector3 spawnArea = new Vector3(50f, 0f, 50f); // Defines the area where rubbish will spawn

    // UI elements
    public TMP_Text rubbishCollectedText; // TextMeshPro UI text for rubbish collected
    public TMP_Text timeLeftText; // TextMeshPro UI text for time remaining

    // Private variables
    private int rubbishCollected = 0; // Amount of rubbish collected
    private float timeLeft; // Timer for the level
    private float timeSinceLastSpawn = 0f; // Timer for rubbish spawning

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

        // Spawn rubbish at regular intervals
        timeSinceLastSpawn += Time.deltaTime;
        if (timeSinceLastSpawn >= spawnInterval && rubbishCollected < totalRubbish)
        {
            SpawnRubbish();
            timeSinceLastSpawn = 0f;
        }
    }

    // Spawns rubbish in a random position within the defined spawn area
    private void SpawnRubbish()
    {
        // Generate a random position within the spawn area
        Vector3 randomPosition = new Vector3(
            Random.Range(-spawnArea.x / 2, spawnArea.x / 2),
            0f, // Adjust for 2D top-down view, keep Y consistent
            Random.Range(-spawnArea.z / 2, spawnArea.z / 2)
        );

        // Instantiate the rubbish prefab at the random position
        Instantiate(rubbishPrefab, randomPosition, Quaternion.identity);
    }

    // Call this method whenever the player collects rubbish
    public void CollectRubbish(GameObject rubbish)
    {
        rubbishCollected++;
        UpdateRubbishUI();
        Destroy(rubbish);

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
