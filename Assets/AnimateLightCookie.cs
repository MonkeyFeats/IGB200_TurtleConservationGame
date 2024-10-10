using UnityEngine;

public class AnimateCaustics : MonoBehaviour
{
    public Light causticsLight;        // Assign your directional light here
    public Texture[] causticFrames;    // Array of caustic textures (sequence of images)
    public float frameDuration = 0.1f; // Time each frame stays visible

    private int currentFrame = 0;
    private float timer = 0f;

    void Update()
    {
        if (causticFrames.Length == 0 || causticsLight == null)
            return;

        // Update the timer
        timer += Time.deltaTime;

        // If enough time has passed, switch to the next frame
        if (timer >= frameDuration)
        {
            // Move to the next frame, looping back if necessary
            currentFrame = (currentFrame + 1) % causticFrames.Length;

            // Apply the new frame as the light's cookie
            causticsLight.cookie = causticFrames[currentFrame];

            // Reset the timer
            timer = 0f;
        }
    }
}
