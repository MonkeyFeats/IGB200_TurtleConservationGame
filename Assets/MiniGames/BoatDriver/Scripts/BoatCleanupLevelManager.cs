using Cinemachine;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
using System.Collections;
using UnityEngine.Events;

public class BoatCleanupLevelManager : MonoBehaviour
{
    public GameObject starPopup; // Star popup UI
    public GameObject gameplayHUD;
    public Animator starsAnimator;
    public ScreenTransitionEffect transitioner;

    public UnityEvent OnStartGameEvent;
    public UnityEvent OnEndGameEvent;

    // Public variables
    public int totalRubbish = 10; // Total rubbish to collect in the level
    public float levelTime = 300f; // Total time in seconds for the level
    public GameObject rubbishPrefab; // Rubbish prefab to spawn
    public float spawnInterval = 5f; // Time interval for spawning rubbish
    public Vector3 spawnArea = new Vector3(50f, 0f, 50f); // Area where rubbish spawns

    // UI elements
    public TMP_Text rubbishCollectedText;
    public TMP_Text timeLeftText;

    // Private variables
    private int rubbishCollected = 0; // Amount of rubbish collected
    private float timeLeft; // Timer for the level
    private float timeSinceLastSpawn = 0f; // Timer for rubbish spawning
    private enum GameState { Intermission, Playing, EndGame }; // Game states
    private GameState currentState;


    // Reef Health Variables
    public float maxReefHealth = 100f;
    private float currentReefHealth = 100f;
    public TextMeshProUGUI reefHealthText;

    public UnityEvent EndGameEvents;

    void Start()
    {
        timeLeft = levelTime;
        rubbishCollected = 0;
        currentReefHealth = maxReefHealth;
        StartIntermission();

        for (int i = 0; i < 50; i++)
        {
            SpawnRubbish();
        }
    }

    void Update()
    {
        if (currentState == GameState.Playing)
        {
            // Update the timer and check for end conditions
            timeLeft -= Time.deltaTime;
            UpdateTimeUI();

            if (timeLeft <= 0f)
            {
                EndLevel(false); // Time's up, game lost
            }

            if (rubbishCollected >= totalRubbish)
            {
                EndLevel(true); // All rubbish collected, game won
            }

            // Spawn rubbish at regular intervals
           //timeSinceLastSpawn += Time.deltaTime;
           //if (timeSinceLastSpawn >= spawnInterval && rubbishCollected < totalRubbish)
           //{
           //    SpawnRubbish();
           //    timeSinceLastSpawn = 0f;
           //}
        }
    }


    // Starts the intermission state
    public void StartIntermission()
    {
        currentState = GameState.Intermission;
        // Optionally disable player controls here
        // if (playerController != null) playerController.enabled = false;
        // Trigger transition animations, etc.
    }

    public void PauseGame()
    {
        Time.timeScale = 0f; // Stop the game by setting time scale to 0
        //isPaused = true;
    }

    public void ResumeGame()
    {
        Time.timeScale = 1f; // Resume the game by setting time scale to 1
        //isPaused = false;
    }

    // Ends the intermission and starts gameplay
    public void EndIntermission()
    {
        currentState = GameState.Playing;
        StartGameplay();
        // Optionally enable player controls here
        // playerController.enabled = true;
    }

    private void StartGameplay()
    {
        //SpawnRubbish();
        UpdateRubbishUI();
        UpdateTimeUI();
        UpdateReefHealthUI();
        OnStartGameEvent?.Invoke();

    }

    public void DamageReef(float damageAmount)
    {
        // Reduce reef health
        currentReefHealth -= damageAmount;

        // Clamp the health to not go below 0
        currentReefHealth = Mathf.Clamp(currentReefHealth, 0, maxReefHealth);

        // Update the UI
        UpdateReefHealthUI();

        // Optionally: check if the reef is fully damaged
        if (currentReefHealth <= 0)
        {
            ReefDestroyed();
        }
    }

    void UpdateReefHealthUI()
    {
        // Set the text to display the current reef health
        reefHealthText.text = "Reef Health: " + currentReefHealth.ToString("F0") + "/" + maxReefHealth.ToString("F0");
    }

    // Optional: Handle what happens when the reef is fully destroyed
    void ReefDestroyed()
    {
        if (currentState != GameState.EndGame)
        {
            currentState = GameState.EndGame;
            gameplayHUD.gameObject.SetActive(false);
            EndGameEvents.Invoke();
        }
    }

    // Spawns rubbish in random positions within the defined spawn area
    private void SpawnRubbish()
    {
        Vector3 randomPosition = new Vector3(
            Random.Range(-spawnArea.x / 2, spawnArea.x / 2),
            spawnArea.y,
            Random.Range(-spawnArea.z / 2, spawnArea.z / 2)
        );
        Instantiate(rubbishPrefab, randomPosition, Quaternion.identity);
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

    // Call this method when rubbish is collected
    public void CollectRubbish(GameObject rubbish)
    {
        rubbishCollected++;
        UpdateRubbishUI();
        rubbish.GetComponent<FloatingRubbish>().CollectRubbish();

        if (rubbishCollected >= totalRubbish)
        {
            EndLevel(true); // Win condition
        }
    }

    // Ends the level send an event to open dialogue
    public void EndLevel(bool won)
    {
        currentState = GameState.EndGame;
        gameplayHUD.gameObject.SetActive(false);               
        OnEndGameEvent?.Invoke();
    }

    public void GiveStarRating()
    {
        int starsEarned = CalculateStarReward();
        starPopup.SetActive(true);
        if (starsAnimator != null)
            starsAnimator.SetInteger("StarsEarned", starsEarned);

    }

    // Calculates the star reward based on performance
    int CalculateStarReward()
    {
        // Calculate the percentage of rubbish collected
        float collectionPercentage = (float)rubbishCollected / totalRubbish;

        // Calculate the reef health penalty as a percentage
        float reefHealthPenalty = (maxReefHealth - currentReefHealth) / maxReefHealth;

        // Adjust the collection percentage by reef damage (penalty)
        float adjustedScore = collectionPercentage * (1f - reefHealthPenalty);

        // Full completion with minimal damage earns 3 stars
        if (adjustedScore >= 0.9f)
            return 3;
        // Two stars for a high completion rate with moderate damage
        else if (adjustedScore >= 0.6f)
            return 2;
        // One star for a low completion rate or high reef damage
        else if (adjustedScore >= 0.3f)
            return 1;
        // Zero stars for very low completion or extensive damage
        else
            return 0;
    }

    // Shows the end game popup with stars earned

    public void LoadScene(int sceneIndex)
    {
        SceneManager.LoadScene(sceneIndex);
    }
}
