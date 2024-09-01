using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TitleScreenFadeIn : MonoBehaviour
{
    // Array of CanvasGroups to fade in sequentially
    public CanvasGroup[] groupsToFade;

    // Time it takes to fade each group in seconds
    public float fadeDuration = 1f;

    // Time to wait before starting to fade in the next group
    public float delayBetweenFades = 0.5f;

    // Scene to load after fading in all groups
    public string sceneToLoad;

    private void Start()
    {
        // Start the fade-in sequence
        StartCoroutine(FadeInSequence());
    }

    private IEnumerator FadeInSequence()
    {
        foreach (CanvasGroup group in groupsToFade)
        {
            yield return StartCoroutine(FadeIn(group));
            yield return new WaitForSeconds(delayBetweenFades);
        }

        // After fading in all groups, start checking for input
        StartCoroutine(WaitForInput());
    }

    private IEnumerator FadeIn(CanvasGroup group)
    {
        float elapsedTime = 0f;

        while (elapsedTime < fadeDuration)
        {
            elapsedTime += Time.deltaTime;
            group.alpha = Mathf.Clamp01(elapsedTime / fadeDuration);
            yield return null;
        }

        group.alpha = 1f; // Ensure the group is fully visible at the end
    }

    private IEnumerator WaitForInput()
    {
        while (true)
        {
            if (Input.anyKeyDown) // Check for any key press or mouse click
            {
                LoadScene();
                yield break;
            }
            yield return null;
        }
    }

    private void LoadScene()
    {
        // Optionally fade out the UI groups here before loading the scene
        // Example: StartCoroutine(FadeOutAllGroups());

        // Load the specified scene
        SceneManager.LoadScene(sceneToLoad);
    }

    private IEnumerator FadeOutAllGroups()
    {
        foreach (CanvasGroup group in groupsToFade)
        {
            yield return StartCoroutine(FadeOut(group));
        }
    }

    private IEnumerator FadeOut(CanvasGroup group)
    {
        float elapsedTime = 0f;

        while (elapsedTime < fadeDuration)
        {
            elapsedTime += Time.deltaTime;
            group.alpha = Mathf.Clamp01(1 - (elapsedTime / fadeDuration));
            yield return null;
        }

        group.alpha = 0f; // Ensure the group is fully transparent at the end
    }
}
