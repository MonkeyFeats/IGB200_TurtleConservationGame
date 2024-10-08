using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneManagerScript : MonoBehaviour
{
    public GameObject transitionObject;
    public GameObject endGameCanvas;
    int sceneWantedToLoad = 0;
    private void DisablePlayerControls()
    {
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
        transitionObject.SetActive(true);

        ScreenTransitionEffect transitionEffect = transitionObject.GetComponent<ScreenTransitionEffect>();
        if (transitionEffect != null) 
        {
            Debug.Log("Made it here");
            sceneWantedToLoad = sceneIndex;
            transitionObject.GetComponent<ScreenTransitionEffect>().FadeOutOfScene();
        }
    }

    public void ActuallyLoadNewScene()
    {
        SceneManager.LoadScene(sceneWantedToLoad);
    }

    public void ActuallyLoadNewSceneByIndex(int sceneIndex)
    {
        SceneManager.LoadScene(sceneIndex);
    }

    // Reloads the current active scene
    public void ReloadCurrentScene()
    {
        Scene currentScene = SceneManager.GetActiveScene();
        SceneManager.LoadScene(currentScene.name);
    }
}
