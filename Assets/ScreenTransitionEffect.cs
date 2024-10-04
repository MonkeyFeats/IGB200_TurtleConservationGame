using System.Collections;
using UnityEngine;

public class OceanWaveFadeScript : MonoBehaviour
{
    public Material oceanWaveMaterial; // Assign your ocean wave shader material here
    public float fadeSpeed = 1.0f;     // Control the speed of the fade (adjust as needed)

    private Coroutine fadeCoroutine;   // To track the current fade coroutine

    // Call this to start fading in
    public void FadeIn()
    {
        // Stop any running fade coroutine and start the fade in
        if (fadeCoroutine != null) StopCoroutine(fadeCoroutine);
        fadeCoroutine = StartCoroutine(FadeToValue(3.0f));
    }

    // Call this to start fading out
    public void FadeOut()
    {
        // Stop any running fade coroutine and start the fade out
        if (fadeCoroutine != null) StopCoroutine(fadeCoroutine);
        fadeCoroutine = StartCoroutine(FadeToValue(-2.0f));
    }

    // Coroutine to animate the `currentAmount` property over time
    private IEnumerator FadeToValue(float targetValue)
    {
        // Get the current value of `currentAmount`
        float currentAmount = oceanWaveMaterial.GetFloat("_currentAmount");

        // While the value hasn't reached the target, continue the fade
        while (!Mathf.Approximately(currentAmount, targetValue))
        {
            // Interpolate the `currentAmount` value towards the target value
            currentAmount = Mathf.MoveTowards(currentAmount, targetValue, fadeSpeed * Time.deltaTime);

            // Set the interpolated value back to the shader property
            oceanWaveMaterial.SetFloat("_currentAmount", currentAmount);

            // Wait for the next frame before continuing the loop
            yield return null;
        }

        // Make sure the target value is set exactly at the end
        oceanWaveMaterial.SetFloat("_currentAmount", targetValue);
    }
}
