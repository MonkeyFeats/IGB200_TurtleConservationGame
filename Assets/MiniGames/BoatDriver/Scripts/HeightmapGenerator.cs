using UnityEngine;

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class PlaneMeshWithHeightMap : MonoBehaviour
{
    [Header("Mesh Settings")]
    public int width = 10;
    public int length = 10;
    public float scale = 1f;

    [Header("Height Settings")]
    public float heightMultiplier = 2f;  // Control the height scale
    public float noiseScale = 0.1f;      // Control the noise frequency for the heightmap

    [Header("Gradient for Fake Lighting")]
    public Gradient heightGradient;      // Gradient to simulate fake lighting

    private MeshFilter meshFilter;
    private Mesh mesh;

    private Vector3[] vertices;
    private int[] triangles;
    private Color[] vertexColors;

    void Awake()
    {
        meshFilter = GetComponent<MeshFilter>();
        GenerateMesh();
    }

    void OnValidate()
    {
        // Regenerate mesh dynamically when values are updated in the inspector
        if (meshFilter != null)
        {
            GenerateMesh();
        }
    }

    void GenerateMesh()
    {
        // Initialize mesh and arrays
        mesh = new Mesh();
        mesh.name = "Generated Plane Mesh";

        vertices = new Vector3[(width + 1) * (length + 1)];
        vertexColors = new Color[vertices.Length];
        triangles = new int[width * length * 6];

        // Create vertices and assign height from Perlin noise
        for (int i = 0, z = 0; z <= length; z++)
        {
            for (int x = 0; x <= width; x++, i++)
            {
                // Use Perlin noise for height, scaled by heightMultiplier
                float y = Mathf.PerlinNoise(x * noiseScale, z * noiseScale) * heightMultiplier;
                vertices[i] = new Vector3(x, y, z);

                // Calculate height percentage to simulate lighting using the gradient
                float heightPercentage = Mathf.InverseLerp(0, heightMultiplier, y);
                vertexColors[i] = heightGradient.Evaluate(heightPercentage);
            }
        }

        // Create triangles
        int vertIndex = 0;
        int triIndex = 0;
        for (int z = 0; z < length; z++)
        {
            for (int x = 0; x < width; x++)
            {
                triangles[triIndex + 0] = vertIndex + 0;
                triangles[triIndex + 1] = vertIndex + width + 1;
                triangles[triIndex + 2] = vertIndex + 1;

                triangles[triIndex + 3] = vertIndex + 1;
                triangles[triIndex + 4] = vertIndex + width + 1;
                triangles[triIndex + 5] = vertIndex + width + 2;

                vertIndex++;
                triIndex += 6;
            }
            vertIndex++;
        }

        // Apply data to the mesh
        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.colors = vertexColors;

        mesh.RecalculateNormals();

        // Assign mesh to the mesh filter
        meshFilter.mesh = mesh;
    }
}
