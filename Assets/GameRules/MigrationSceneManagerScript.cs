using Cinemachine;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
public class MigrationSceneManagerScript : MonoBehaviour
{
    // UI References
    public GameObject intermissionCanvas;
    public GameObject endGameCanvas;
    public ScreenTransitionScript transitioner;
    public MigrationPlayerMovement playerController;
    public CinemachineDollyCart cart;

    private int lives = 3;
    private int hitsTaken = 0;

    // Enum for game states
    private enum GameState { Intermission, Playing, EndGame };
    private GameState currentState;

    void Start()
    {
        StartIntermission();
    }

    public void StartIntermission()
    {
        currentState = GameState.Intermission;
        if (playerController != null)
        playerController.enabled = false;
        cart.m_Speed = 0f;

        if (intermissionCanvas != null)
        {
            intermissionCanvas.SetActive(true);
        }
    }

    public void EndIntermission()
    {
        currentState = GameState.Playing;
        playerController.enabled = true;
        cart.m_Speed = 3f;

        if (intermissionCanvas != null)
        {
            intermissionCanvas.SetActive(false);
        }
    }

    public void EndGame()
    {
        currentState = GameState.EndGame;
        cart.m_Speed = 0f;

        if (endGameCanvas != null)
        {
            endGameCanvas.SetActive(true);
        }
    }

    public void LoadNextScene()
    {
        StartCoroutine(TransitionAndLoadNextScene());
    }
    public void LoadScene(int sceneIndex)
    {
        StartCoroutine(TransitionAndLoadSceneByIndex(sceneIndex));
    }


    private IEnumerator TransitionAndLoadSceneByIndex(int sceneNum)
    {
        // Start fade-out and wait for it to finish
        transitioner.FadeOutOfScene();

        // Wait until the fade-out is complete
        yield return new WaitUntil(() => transitioner.finishedFadeOut);

        // Load the next scene after the fade-out completes
        if (sceneNum < SceneManager.sceneCountInBuildSettings)
        {
            SceneManager.LoadScene(sceneNum);
        }
        else
        {
            Debug.LogWarning("Scene index does not exist!");
        }
    }

    private IEnumerator TransitionAndLoadNextScene()
    {
        // Start fade-out and wait for it to finish
        transitioner.FadeOutOfScene();

        // Wait until the fade-out is complete
        yield return new WaitUntil(() => transitioner.finishedFadeOut);

        // Load the next scene after the fade-out completes
        int nextSceneIndex = SceneManager.GetActiveScene().buildIndex + 1;
        if (nextSceneIndex < SceneManager.sceneCountInBuildSettings)
        {
            SceneManager.LoadScene(nextSceneIndex);
        }
        else
        {
            Debug.LogWarning("No more scenes to load.");
        }
    }
}
