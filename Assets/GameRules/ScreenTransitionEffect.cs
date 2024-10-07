using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;

public class ScreenTransitionScript : MonoBehaviour
{
    public GameObject imageObject;
    public Material oceanWaveMaterial; // Assign your ocean wave shader material here
    public float fadeSpeed = 1.0f;     // Control the speed of the fade (adjust as needed)

    private Coroutine fadeCoroutine;   // To track the current fade coroutine
    public float lowerRange = -1;
    public float upperRange = 1;

    // UnityEvents for fade in and fade out completion
    public UnityEvent OnFadeInComplete;
    public UnityEvent OnFadeOutComplete;

    private void Start() // always start the fade in on scene start
    {
        FadeIntoScene();
    }

    public void FadeIntoScene()
    {
        oceanWaveMaterial.SetFloat("_CurrentAmount", upperRange);
        imageObject.SetActive(true);
        // Stop any running fade coroutine and start the fade in, into the Game
        if (fadeCoroutine != null) StopCoroutine(fadeCoroutine);
        fadeCoroutine = StartCoroutine(FadeToValue(lowerRange, true));
    }

    // Call this to start fading out
    public void FadeOutOfScene()
    {
        imageObject.SetActive(true);
        // Stop any running fade coroutine and start the fade out
        if (fadeCoroutine != null) StopCoroutine(fadeCoroutine);
        fadeCoroutine = StartCoroutine(FadeToValue(upperRange, false));
    }

    // Coroutine to animate the `currentAmount` property over time
    private IEnumerator FadeToValue(float targetValue, bool isFadingIn)
    {
        float currentAmount = oceanWaveMaterial.GetFloat("_CurrentAmount");

        while (!Mathf.Approximately(currentAmount, targetValue))
        {
            // While the value hasn't reached the target, continue the fade
            // Interpolate the `currentAmount` value towards the target value
            currentAmount = Mathf.MoveTowards(currentAmount, targetValue, fadeSpeed * Time.deltaTime);
            oceanWaveMaterial.SetFloat("_CurrentAmount", currentAmount);

            yield return null;
        }

        oceanWaveMaterial.SetFloat("_CurrentAmount", targetValue);

        // Invoke the appropriate UnityEvent when fade finishes
        if (isFadingIn)
        {
            imageObject.SetActive(false);
            OnFadeInComplete?.Invoke();
        }
        else
        {
            OnFadeOutComplete?.Invoke();
        }
    }
}
