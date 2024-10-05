using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

public class ScreenTransitionScript : MonoBehaviour
{
    public GameObject imageObject;
    public Material oceanWaveMaterial; // Assign your ocean wave shader material here
    public float fadeSpeed = 1.0f;     // Control the speed of the fade (adjust as needed)

    private Coroutine fadeCoroutine;   // To track the current fade coroutine
    public float lowerRange = -1;
    public float upperRange = 1;
    public bool finishedFadeOut = false;

    private void Start() // always start the fadein on scene start
    {
        FadeIntoScene();
    }

    public void FadeIntoScene()
    {
        oceanWaveMaterial.SetFloat("_CurrentAmount", upperRange);
        imageObject.SetActive(true);
        // Stop any running fade coroutine and start the fade in, into the Game
        if (fadeCoroutine != null) StopCoroutine(fadeCoroutine);
        fadeCoroutine = StartCoroutine(FadeToValue(lowerRange));
    }

    // Call this to start fading out
    public void FadeOutOfScene()
    {
        imageObject.SetActive(true);
        // Stop any running fade coroutine and start the fade out
        if (fadeCoroutine != null) StopCoroutine(fadeCoroutine);
        fadeCoroutine = StartCoroutine(FadeToValue(upperRange));
    }

    // Coroutine to animate the `currentAmount` property over time
    private IEnumerator FadeToValue(float targetValue)
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

        if (currentAmount <= lowerRange)
            imageObject.SetActive(false);

        if (currentAmount >= lowerRange)
            finishedFadeOut = true;

    }
}
