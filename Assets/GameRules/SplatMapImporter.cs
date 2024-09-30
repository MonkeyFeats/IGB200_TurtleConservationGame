using UnityEngine;
using UnityEditor;
using static UnityEditor.IMGUI.Controls.PrimitiveBoundsHandle;

[ExecuteInEditMode]
public class SplatMapImporterEditor : MonoBehaviour
{
    public Terrain terrain;      // Reference to your terrain
    public Texture2D splatMap;   // The splat map texture from Gaea

    // Add tunable variables for fine-tuning the splat map application
    [Range(0f, 2f)] public float redChannelStrength = 1.0f;    // Control strength of the red channel (Texture Layer 1)
    [Range(0f, 2f)] public float greenChannelStrength = 1.0f;  // Control strength of the green channel (Texture Layer 2)
    [Range(0f, 2f)] public float blueChannelStrength = 1.0f;   // Control strength of the blue channel (Texture Layer 3)
    [Range(0f, 2f)] public float alphaChannelStrength = 1.0f;  // Control strength of the alpha channel (Texture Layer 4)

    // Add thresholds to control how strongly each channel affects the terrain
    [Range(0f, 1f)] public float redThreshold = 0.0f;    // Red channel application threshold
    [Range(0f, 1f)] public float greenThreshold = 0.0f;  // Green channel application threshold
    [Range(0f, 1f)] public float blueThreshold = 0.0f;   // Blue channel application threshold
    [Range(0f, 1f)] public float alphaThreshold = 0.0f;  // Alpha channel application threshold

    [ContextMenu("Apply Splat Map")] // Right-click context menu in the editor
    public void ApplySplatMapInEditor()
    {
        if (terrain == null || splatMap == null)
        {
            Debug.LogError("Please assign both the terrain and splat map in the inspector.");
            return;
        }

        // Get the terrain data
        TerrainData terrainData = terrain.terrainData;

        // Get the alpha map resolution (control texture size)
        int mapSize = terrainData.alphamapResolution;

        // Prepare the splat map data (RGBA -> 4 layers)
        float[,,] splatData = new float[mapSize, mapSize, 4];

        for (int y = 0; y < mapSize; y++)
        {
            for (int x = 0; x < mapSize; x++)
            {
                // Flip both X and Y axes to adjust rotation and orientation
                Color splatColor = splatMap.GetPixelBilinear((float)(y) / mapSize, (float)(mapSize - x) / mapSize);

                // Adjust channels with strengths and thresholds
                float redValue = Mathf.Clamp((splatColor.r - redThreshold) * redChannelStrength, 0f, 1f);
                float greenValue = Mathf.Clamp((splatColor.g - greenThreshold) * greenChannelStrength, 0f, 1f);
                float blueValue = Mathf.Clamp((splatColor.b - blueThreshold) * blueChannelStrength, 0f, 1f);
                float alphaValue = Mathf.Clamp((splatColor.a - alphaThreshold) * alphaChannelStrength, 0f, 1f);

                // Assign splat data for each texture layer
                splatData[x, y, 0] = redValue;   // Red channel -> Texture Layer 1
                splatData[x, y, 1] = greenValue; // Green channel -> Texture Layer 2
                splatData[x, y, 2] = blueValue;  // Blue channel -> Texture Layer 3
                splatData[x, y, 3] = alphaValue; // Alpha channel -> Texture Layer 4
            }
        }

        // Apply the splat data to the terrain
        terrainData.SetAlphamaps(0, 0, splatData);

        // Mark the terrain as dirty to save changes
        EditorUtility.SetDirty(terrain);
        Debug.Log("Splat map applied correctly!");
    }
}
