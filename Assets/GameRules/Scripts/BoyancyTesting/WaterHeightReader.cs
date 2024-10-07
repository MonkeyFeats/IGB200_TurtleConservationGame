using UnityEngine;

public class WaterHeightReader : MonoBehaviour
{
    public Camera waterCamera; // A camera to render the water surface (it can be offscreen)
    public Material waterMaterial; // Reference to the water shader material
    public RenderTexture renderTexture; // Render Texture to capture the water height map

    private Texture2D texture2D; // This will hold the pixel data

    void Start()
    {
        // Setup RenderTexture to render the water displacement
        renderTexture = new RenderTexture(512, 512, 0, RenderTextureFormat.ARGBFloat);
        waterCamera.targetTexture = renderTexture;

        // Create a Texture2D that we will use to read pixels from the RenderTexture
        texture2D = new Texture2D(renderTexture.width, renderTexture.height, TextureFormat.RGBAFloat, false);

        // Pass the RenderTexture to the water material (optional)
        waterMaterial.SetTexture("_HeightMap", renderTexture);
    }

    void Update()
    {
        // Render the water camera (if it isn't rendering in the main view)
        waterCamera.Render();

        // Read the pixels from the RenderTexture into the Texture2D
        RenderTexture.active = renderTexture;
        texture2D.ReadPixels(new Rect(0, 0, renderTexture.width, renderTexture.height), 0, 0);
        texture2D.Apply();
        RenderTexture.active = null;

        // Example: Get the height at a specific UV coordinate (normalized 0-1 range)
        Vector2 uvCoord = new Vector2(0.5f, 0.5f); // Example UV, adjust accordingly
        Color pixel = GetHeightAtUV(uvCoord);
        float height = pixel.r; // Assuming the height is stored in the red channel

        Debug.Log($"Height at UV (0.5, 0.5): {height}");
    }

    Color GetHeightAtUV(Vector2 uv)
    {
        // Convert UV to pixel coordinates in the Texture2D
        int x = Mathf.FloorToInt(uv.x * texture2D.width);
        int y = Mathf.FloorToInt(uv.y * texture2D.height);

        // Get the color data (height value) from the texture
        return texture2D.GetPixel(x, y);
    }
}
