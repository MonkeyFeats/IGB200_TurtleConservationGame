using UnityEngine;
using Cinemachine;
using System.Linq;
using System.Security.Cryptography;

public class TerrainGenerator : MonoBehaviour
{
    [Header("Terrain Height")]
    public Terrain terrain; // Reference to the Terrain object
    public CinemachinePathBase dollyPath; // Reference to the dolly path for carving
    public float terrainNoiseScale = 30f;  // Scale for Perlin noise (adjust for smoother, larger hills)
    public float perlinMultiplier = 20f; // Height multiplier for terrain elevation (increased for hills)
    public float crevassMultiplier = 20f; // Height multiplier for terrain elevation (increased for hills)
    public float lumpMultiplier = 20f; // Height multiplier for terrain elevation (increased for hills)
    public float waterHeight = 0.4f; // Default height for the water level
    public float pathWidth = 10f; // Width of the path to carve out
    public float brushFallOff = 0.3f;          // Falloff factor for the brush
    private float[,] originalTerrainHeights;    // Store original terrain heights
    private TerrainData terrainData;
    private int terrainResolution;

    [Header("Texture Data")]
    public float textureNoiseScale = 1.0f; 
    public float slopeThreshold = 30f; // Slope threshold (in degrees) for texture transition
    public float blendWidth = 5f; // Range around the threshold for blending
    public TerrainLayer baseLayer;              // The base layer texture (e.g., grass)
    public TerrainLayer steepLayer;             // The texture for steep areas (e.g., rock)


    void Start()
    {
        terrainData = terrain.terrainData;
        terrainResolution = terrainData.heightmapResolution;

        // Generate the entire terrain using Perlin noise
        GenerateEnhancedTerrain();
        SaveOriginalTerrainHeights();
        // Apply textures and vegetation
        ApplyTextures();
        //ApplyVegetation();

        //ShapeTerrain();

    }
    private void Update()
    {
       //ShapeTerrain();        
    }

    // Step 1: Generate Perlin Noise across the entire terrain for rolling hills
    void GenerateTerrainWithPerlinNoise()
    {
        float[,] heights = new float[terrainResolution, terrainResolution];

        for (int x = 0; x < terrainResolution; x++)
        {
            for (int z = 0; z < terrainResolution; z++)
            {
                float xCoord = (float)x / terrainResolution * terrainNoiseScale;
                float zCoord = (float)z / terrainResolution * terrainNoiseScale;

                // Generate Perlin noise value for this coordinate and scale it by heightMultiplier
                float heightValue = Mathf.PerlinNoise(xCoord, zCoord) * perlinMultiplier;

                // Raise the default height to the water level, and add Perlin noise for rolling hills
                heights[x, z] = Mathf.Clamp(waterHeight + (heightValue / terrainData.size.y), 0f, 1f);
            }
        }

        // Apply the generated heights to the terrain
        terrainData.SetHeights(0, 0, heights);
    }

    private void SaveOriginalTerrainHeights()
    {
        TerrainData terrainData = terrain.terrainData;
        originalTerrainHeights = terrainData.GetHeights(0, 0, terrainData.heightmapResolution, terrainData.heightmapResolution);
    }

    private void GenerateEnhancedTerrain()
    {
        float[,] heights = new float[terrainResolution, terrainResolution];

        for (int x = 0; x < terrainResolution; x++)
        {
            for (int z = 0; z < terrainResolution; z++)
            {
                float xCoord = (float)x / terrainResolution * terrainNoiseScale;
                float zCoord = (float)z / terrainResolution * terrainNoiseScale;

                // Base terrain using Perlin noise
                float baseHeight = Mathf.PerlinNoise(xCoord, zCoord) * perlinMultiplier;

                // Coral lumps using higher frequency noise
                float lumpHeight = Mathf.PerlinNoise(xCoord * 5f, zCoord * 5f) * lumpMultiplier * 0.3f;

                // Crevasses by subtracting a lower frequency noise
                float crevasseDepth = Mathf.PerlinNoise(xCoord * 0.5f, zCoord * 0.5f) * crevassMultiplier * 0.5f;

                // Combine to create the final height
                float finalHeight = baseHeight + lumpHeight - crevasseDepth;

                // Apply height to the map
                heights[x, z] = Mathf.Clamp(waterHeight + (finalHeight / terrainData.size.y), 0f, 1f);
            }
        }

        terrainData.SetHeights(0, 0, heights);
    }

    private void ShapeTerrain()
    {
        Vector3 terrainPosition = terrain.transform.position;
        TerrainData terrainData = terrain.terrainData;

        // Height range of the terrain
        float terrainMin = terrainPosition.y;
        float terrainMax = terrainPosition.y + terrainData.size.y;
        float totalHeight = terrainMax - terrainMin;

        // Cloning original heights to modify them
        int resolution = terrainData.heightmapResolution;
        float[,] modifiedHeights = originalTerrainHeights.Clone() as float[,];

        // Retrieve the waypoints from the Dolly Track
        Vector3[] pathPoints = GetDollyTrackWaypoints(1.0f);

        // Iterate through each point along the dolly track
        foreach (var point in pathPoints)
        {
            int centerX = Mathf.RoundToInt((point.z - terrainPosition.z) / terrainData.size.z * (resolution - 1));
            int centerY = Mathf.RoundToInt((point.x - terrainPosition.x) / terrainData.size.x * (resolution - 1));

            // Ensure the centerX and centerY are within bounds
            centerX = Mathf.Clamp(centerX, 0, resolution - 1);
            centerY = Mathf.Clamp(centerY, 0, resolution - 1);

            // Adjust the terrain around the path point
            AdjustTerrain(modifiedHeights, centerX, centerY, pathWidth, terrainPosition.y, totalHeight, point.y/1000);
        }

        terrain.terrainData.SetHeights(0, 0, modifiedHeights);
    }
    private Vector3[] GetDollyTrackWaypoints(float samplingInterval)
    {
        // Ensure that the dollyPath is assigned
        if (dollyPath == null)
        {
            Debug.LogWarning("Dolly Path is not assigned.");
            return new Vector3[0]; // Return an empty array if the dolly path is not set
        }

        // Calculate the total length of the path
        float pathLength = dollyPath.PathLength;

        // Calculate the number of waypoints based on the sampling interval
        int numWaypoints = Mathf.CeilToInt(pathLength / samplingInterval);
        Vector3[] waypoints = new Vector3[numWaypoints];

        // Retrieve waypoints along the path
        for (int i = 0; i < numWaypoints; i++)
        {
            // Calculate the distance for the current waypoint
            float distance = i * samplingInterval;

            // Clamp the distance to the path length to avoid exceeding it
            distance = Mathf.Min(distance, pathLength);

            // Get the position at this distance
            Vector3 pathPosition = dollyPath.EvaluatePositionAtUnit(distance, CinemachinePathBase.PositionUnits.Distance);

            // Store the position in the waypoints array
            waypoints[i] = pathPosition;
        }

        return waypoints; // Return the populated array of waypoints
    }

    private void AdjustTerrain(float[,] heightMap, int centerX, int centerY, float width, float terrainBaseHeight, float totalHeight, float waypointY)
    {
        int radius = Mathf.RoundToInt(width / 2f);

        // Target height is the waypointY (desired depth) negated to create a dip
        float targetDepth = waypointY; // Calculate the target depth at the waypoint

        for (int offsetY = -radius; offsetY <= radius; offsetY++)
        {
            for (int offsetX = -radius; offsetX <= radius; offsetX++)
            {
                if (offsetX * offsetX + offsetY * offsetY <= radius * radius)
                {
                    int brushX = centerX + offsetX;
                    int brushY = centerY + offsetY;

                    if (brushX >= 0 && brushY >= 0 && brushX < heightMap.GetLength(0) && brushY < heightMap.GetLength(1))
                    {
                        // Calculate the distance from the center
                        float distance = Mathf.Sqrt(offsetX * offsetX + offsetY * offsetY);

                        // Quadratic falloff calculation
                        float falloff = 1 - (distance / radius);
                        falloff = Mathf.Clamp01(falloff * falloff); // Ensure it's between 0 and 1

                        // Adjust height based on falloff and target depth to create a U-shape
                        // Clamp the new height to ensure it does not exceed the original height
                        heightMap[brushX, brushY] = Mathf.Min(heightMap[brushX, brushY], heightMap[brushX, brushY] + (targetDepth * falloff));
                    }
                }
            }
        }
    }

    void ApplyTextures()
    {
        // Get the terrain data
        TerrainData terrainData = terrain.terrainData;

        // Create a 2D array to store the texture weights
        float[,,] splatmapData = new float[terrainData.alphamapWidth, terrainData.alphamapHeight, terrainData.alphamapLayers];

        // Loop through each point on the terrain
        for (int y = 0; y < terrainData.alphamapHeight; y++)
        {
            for (int x = 0; x < terrainData.alphamapWidth; x++)
            {
                // Calculate the normalized terrain coordinates (flipping x and z)
                float normX = x * 1.0f / (terrainData.alphamapWidth - 1);
                float normY = y * 1.0f / (terrainData.alphamapHeight - 1);

                // Calculate the steepness at this point
                float steepness = terrainData.GetSteepness(normY, normX); // Flipping the arguments

                // Initialize weights for textures
                float[] splatWeights = new float[terrainData.alphamapLayers];

                // Apply the steep texture where steepness exceeds the threshold
                if (steepness > slopeThreshold)
                {
                    // Mask out the base texture and apply the steep texture
                    splatWeights[1] = 1; // Apply the steep texture
                }
                else
                {
                    // Otherwise, apply the base texture
                    splatWeights[0] = 1; // Apply the base texture
                }

                // Normalize weights (they must sum to 1)
                float total = 0;
                for (int i = 0; i < terrainData.alphamapLayers; i++)
                {
                    total += splatWeights[i];
                }

                // Avoid division by zero and normalize weights
                if (total > 0)
                {
                    for (int i = 0; i < terrainData.alphamapLayers; i++)
                    {
                        splatWeights[i] /= total;
                    }
                }

                // Assign the texture weights to the splatmap data
                for (int i = 0; i < terrainData.alphamapLayers; i++)
                {
                    splatmapData[x, y, i] = splatWeights[i];
                }
            }
        }

        // Apply the splatmap data to the terrain
        terrainData.SetAlphamaps(0, 0, splatmapData);
    }


    private void ApplyVegetation()
    {
        TerrainData terrainData = terrain.terrainData;
        int detailRes = terrainData.detailResolution;

        // Example detail layer (e.g., grass)
        int[,] details = new int[detailRes, detailRes];

        for (int y = 0; y < detailRes; y++)
        {
            for (int x = 0; x < detailRes; x++)
            {
                float normX = x / (float)detailRes;
                float normY = y / (float)detailRes;

                // Get the corresponding terrain height
                float height = terrainData.GetHeight(Mathf.RoundToInt(normX * terrainData.heightmapResolution),
                                                     Mathf.RoundToInt(normY * terrainData.heightmapResolution));

                // Example: Only place grass on flat terrain
                float steepness = terrainData.GetSteepness(normX, normY);
                if (height > terrainData.size.y * 0.5f && steepness < 20.0f)
                {
                    details[x, y] = Random.Range(1, 4); // Random density
                }
                else
                {
                    details[x, y] = 0; // No grass
                }
            }
        }

        terrainData.SetDetailLayer(0, 0, 0, details); // Apply the details to the first detail layer
    }


}
