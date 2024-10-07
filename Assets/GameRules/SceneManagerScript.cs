using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneManagerScript : MonoBehaviour
{

    public GameObject endGameCanvas;
    private void DisablePlayerControls()
    {
        //Time.timeScale = 0f; // Pauses the game
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

    // Loads a scene by its index in the build settings
    public void LoadSceneByIndex(int sceneIndex)
    {
        GameManager gm = FindAnyObjectByType<GameManager>();
        if (gm != null) { gm.SetLastPlayedLevel(sceneIndex); }

        SceneManager.LoadScene(sceneIndex);
    }

    // Reloads the current active scene
    public void ReloadCurrentScene()
    {
        Scene currentScene = SceneManager.GetActiveScene();
        SceneManager.LoadScene(currentScene.name);
    }
}
