using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TitleScreenFadeIn : MonoBehaviour
{
    // Array of CanvasGroups to fade in sequentially
    public CanvasGroup[] groupsToFade;

    // CanvasGroup to fade out after a certain time
    public CanvasGroup groupToFadeOut;

    // CanvasGroup to fade in after the fade-out of `groupToFadeOut`
    public CanvasGroup groupToFadeInAfterFadeOut;

    // Time it takes to fade each group in seconds
    public float fadeDuration = 1f;

    // Time to wait before starting to fade in the next group
    public float delayBetweenFades = 0.5f;

    // Time to wait before starting to fade out `groupToFadeOut`
    public float delayBeforeFadeOut = 2f;

    // Scene to load after fading in all groups
    public string sceneToLoad;

    private void Start()
    {
        // Start the fade-in sequence
        StartCoroutine(FadeInSequence());
    }

    private IEnumerator FadeInSequence()
    {
        // Fade in each group in `groupsToFade`
        foreach (CanvasGroup group in groupsToFade)
        {
            yield return StartCoroutine(FadeIn(group));
            yield return new WaitForSeconds(delayBetweenFades);
        }

        // After fading in all groups, start the additional fade-out/in process
        StartCoroutine(FadeOutAndFadeInGroup());

    }

    private IEnumerator FadeOutAndFadeInGroup()
    {
        // Wait for a delay before starting the fade-out of `groupToFadeOut`
        yield return new WaitForSeconds(delayBeforeFadeOut);

        // Fade out the specified group
        if (groupToFadeOut != null)
        {
            yield return StartCoroutine(FadeOut(groupToFadeOut));
        }

        // Fade in the next specified group after the fade-out
        if (groupToFadeInAfterFadeOut != null)
        {
            yield return StartCoroutine(FadeIn(groupToFadeInAfterFadeOut));
        }
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
