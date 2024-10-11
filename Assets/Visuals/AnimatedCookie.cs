using UnityEngine;

public class AnimateCausticsFlipbook : MonoBehaviour
{
    public Light causticsLight;        // Assign your directional light here
    public int tileX = 4;              // Number of tiles in X direction (columns)
    public int tileY = 4;              // Number of tiles in Y direction (rows)
    public float frameDuration = 0.1f; // Time for each frame

    private int currentFrame = 0;
    private float timer = 0f;

    void Update()
    {
        if (causticsLight.cookie == null)
            return;

        // Update timer
        timer += Time.deltaTime;

        // If enough time has passed, switch to the next frame
        if (timer >= frameDuration)
        {
            currentFrame = (currentFrame + 1) % (tileX * tileY);
            SetFrame(currentFrame);

            // Reset timer
            timer = 0f;
        }
    }

    void SetFrame(int frameIndex)
    {
        // Calculate UV offset based on the frame index
        int frameX = frameIndex % tileX;
        int frameY = frameIndex / tileX;

        Vector2 offset = new Vector2((float)frameX / tileX, 1f - ((float)frameY / tileY));

        //causticsLight.cookie.wrapMode = TextureWrapMode.Repeat; // Ensure it repeats
        //Shader.SetGlobalTextureOffset("_MainTex", offset); // Modify the offset based on the frame index

        // You may need to handle the scale of the texture as well if necessary
        //Shader.SetGlobalTextureScale("_MainTex", new Vector2(1f / tileX, 1f / tileY));
    }
}
