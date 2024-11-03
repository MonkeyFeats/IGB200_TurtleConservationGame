using Cinemachine;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using HeneGames.DialogueSystem;
using UnityEngine.Events;

public class MigrationSceneManagerScript : MonoBehaviour
{
    // UI References
    public GameObject starPopup;
    public Animator starsAnimator;
    public ScreenTransitionEffect transitioner;
    public MigrationPlayerMovement playerController;
    public CinemachineDollyCart cart;
    public UnityEvent EndGameEvents;
    public TurtleCollisionHandler turtleStatus;

    private int lives = 3;
    private int hitsTaken = 0;

    // Enum for game states
    private enum GameState { Intermission, Playing, EndGame };
    private GameState currentState;

    private float targetSpeed = 0f;
    private float speedChangeDuration = 2f; // Time taken to reach target speed

    void Start()
    {
        StartIntermission();
    }

    public void StartIntermission()
    {
        currentState = GameState.Intermission;
        if (playerController != null)
            playerController.enabled = false;
        StartCoroutine(ChangeCartSpeed(0f));
    }

    public void EndIntermission()
    {
        currentState = GameState.Playing;
        playerController.enabled = true;
        StartCoroutine(ChangeCartSpeed(3f)); // Set to desired speed
    }

    public void EndGame()
    {
        currentState = GameState.EndGame;
        playerController.enabled = false;
        StartCoroutine(ChangeCartSpeed(0f)); // Gradually stop
        if (EndGameEvents != null)
        {
            EndGameEvents.Invoke();
        }
    }

    public void ChangeSpeed(float speed)
    {
        StartCoroutine(ChangeCartSpeed(speed)); // Gradually stop
    }

    IEnumerator ChangeCartSpeed(float desiredSpeed)
    {
        float initialSpeed = cart.m_Speed;
        float elapsedTime = 0f;

        while (elapsedTime < speedChangeDuration)
        {
            cart.m_Speed = Mathf.Lerp(initialSpeed, desiredSpeed, elapsedTime / speedChangeDuration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        cart.m_Speed = desiredSpeed; // Ensure exact target speed at the end
    }

    public void GiveStarRating()
    {
        int starsEarned = CalculateStarReward();
        starPopup.SetActive(true);
        if (starsAnimator != null)
            starsAnimator.SetInteger("StarsEarned", starsEarned);
    }
    int CalculateStarReward()
    {
        float healthPercentage = turtleStatus.health / 100;

        if (healthPercentage >= 0.75f)
            return 3; // 3 stars for 80% or more health
        else if (healthPercentage >= 0.5f)
            return 2; // 2 stars for 50% or more health
        else if (healthPercentage > 0)
            return 1; // 1 star for any remaining health
        else
            return 0; // 0 stars if no health left
    }

    public void LoadScene(int sceneIndex)
    {
        SceneManager.LoadScene(sceneIndex);
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

}
