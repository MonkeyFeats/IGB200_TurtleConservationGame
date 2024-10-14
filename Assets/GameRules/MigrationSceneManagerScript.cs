using Cinemachine;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using HeneGames.DialogueSystem;

public class MigrationSceneManagerScript : MonoBehaviour
{
    // UI References
    public GameObject starPopup;
    public Animator starsAnimator;
    public ScreenTransitionEffect transitioner;
    public MigrationPlayerMovement playerController;
    public CinemachineDollyCart cart;

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
        return 2;
    }

    public void LoadScene(int sceneIndex)
    {
        SceneManager.LoadScene(sceneIndex);
    }
}
