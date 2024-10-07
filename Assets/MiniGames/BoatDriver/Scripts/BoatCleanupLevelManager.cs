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

    public BoatLineController boatController;

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

    void Start()
    {
        boatController.enabled = false;
        timeLeft = levelTime;
        rubbishCollected = 0;
        StartIntermission();
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
            timeSinceLastSpawn += Time.deltaTime;
            if (timeSinceLastSpawn >= spawnInterval && rubbishCollected < totalRubbish)
            {
                SpawnRubbish();
                timeSinceLastSpawn = 0f;
            }
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
        boatController.enabled = true;
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
        Destroy(rubbish);

        if (rubbishCollected >= totalRubbish)
        {
            EndLevel(true); // Win condition
        }
    }

    // Ends the level send an event to open dialogue
    public void EndLevel(bool won)
    {
        currentState = GameState.EndGame;
        boatController.enabled = false;
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
        float percentageFound = 0.55f;//(float)objectsFound / totalObjects;
        //float timePercentage = currentTime / gameTime;

        // Full completion bonus (3 stars) if all objects are found and time is still left
        if (percentageFound >= 1f)// && timePercentage > 0)
            return 3;

        // 2 stars for completing 60% or more of objects
        else if (percentageFound >= 0.6f)
            return 2;

        // 1 stars for completing 30-65%
        else if (percentageFound >= 0.3f)
            return 1;

        // 0 stars for less than 30% found
        return 0;
    }

    // Shows the end game popup with stars earned

    public void LoadScene(int sceneIndex)
    {
        SceneManager.LoadScene(sceneIndex);
    }
}
