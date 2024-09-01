using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneManagerScript : MonoBehaviour
{

    public GameObject endGameCanvas;
    private void DisablePlayerControls()
    {
        Time.timeScale = 0f; // Pauses the game
        // Disable your player control script here, e.g.,
        // player.GetComponent<PlayerControlScript>().enabled = false;
    }

    public void EndGame()
    {
        DisablePlayerControls();

        // Display the game over UI
        if (endGameCanvas != null)
        {
            endGameCanvas.SetActive(true);
        }
    }

    // Loads a scene by its name
    public void LoadSceneByName(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }

    // Loads a scene by its index in the build settings
    public void LoadSceneByIndex(int sceneIndex)
    {
        SceneManager.LoadScene(sceneIndex);
    }

    // Reloads the current active scene
    public void ReloadCurrentScene()
    {
        Scene currentScene = SceneManager.GetActiveScene();
        SceneManager.LoadScene(currentScene.name);
    }

    // Loads the next scene in the build index
    public void LoadNextScene()
    {
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

    // Loads the previous scene in the build index
    public void LoadPreviousScene()
    {
        int previousSceneIndex = SceneManager.GetActiveScene().buildIndex - 1;
        if (previousSceneIndex >= 0)
        {
            SceneManager.LoadScene(previousSceneIndex);
        }
        else
        {
            Debug.LogWarning("Already at the first scene.");
        }
    }
}
