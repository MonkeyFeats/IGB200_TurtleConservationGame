using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NetController3D : MonoBehaviour
{
    public GameObject boat;                  // Reference to the boat
    public float segmentDistance = 1.0f;     // Distance between net segments
    public float maxNetLength = 50.0f;       // Max allowed net length
    public Material netMaterial; // Reference to the net material
    public float flashDuration = 5.0f;       // Duration for net flash when closing
    public LineRenderer lineRenderer;        // Reference to the LineRenderer component
    public float closeDistance = 1.0f;       // Distance to check for closing the net
    public int segmentThreshold = 8;         // Number of segments to skip for checking

    private List<Vector3> netSegments = new List<Vector3>(); // Stores the net's path in 3D
    private bool netClosed = false;          // Flag for net closure

    public BoatCleanupLevelManager levelManager;

    void Start()
    {
        flashDuration = 5.0f;
        // Make sure the LineRenderer is attached to the same object
        lineRenderer.positionCount = 0; // Initialize with no points
    }

    void Update()
    {
        if (!netClosed)
        {
            // Update net position based on boat movement
            UpdateNetPath();
        }
    }

    // Updates the net path with the boat's position and draws the line
    void UpdateNetPath()
    {
        Vector3 boatPosition = boat.transform.position;

        // Only add a new point if it's far enough from the last point
        if (netSegments.Count == 0 || Vector3.Distance(netSegments[0], boatPosition) >= segmentDistance)
        {
            netSegments.Insert(0, boatPosition);

            // Remove old points if net gets too long
            if (GetNetLength() > maxNetLength)
            {
                netSegments.RemoveAt(netSegments.Count - 1);
            }

            // Update the LineRenderer with the new net points
            UpdateLineRenderer();

            // Check if the net has looped back (the boat is close to any previous point)
            int closePoint;
            if (IsBoatCloseToAnyPoint(out closePoint))
            {
                netClosed = true;
                TrimNetSegments(closePoint);
                CollectObjectsInsideNet();
                StartCoroutine(FlashNetCoroutine()); // Start the flashing coroutine
            }
        }
    }

    // Calculates the total length of the net
    float GetNetLength()
    {
        float length = 0;
        for (int i = 1; i < netSegments.Count; i++)
        {
            length += Vector3.Distance(netSegments[i - 1], netSegments[i]);
        }
        return length;
    }

    // Updates the LineRenderer component with the current net segments
    void UpdateLineRenderer()
    {
        lineRenderer.positionCount = netSegments.Count; // Set the number of points in the LineRenderer
        lineRenderer.SetPositions(netSegments.ToArray()); // Update the positions
    }

    // Checks if the boat is close to any previous point in the net
    bool IsBoatCloseToAnyPoint(out int pointIndex)
    {
        pointIndex = -1;
        Vector3 boatPosition = boat.transform.position;
        for (int i = segmentThreshold; i < netSegments.Count; i++)
        {
            if (Vector3.Distance(boatPosition, netSegments[i]) < closeDistance)
            {
                pointIndex = i;
                return true;
            }
        }
        return false;
    }

    // Trims the net segments to only include the closed shape
    void TrimNetSegments(int closepoint)
    {
        netSegments = netSegments.GetRange(0, closepoint + 1);
        UpdateLineRenderer(); // Ensure LineRenderer reflects the trimmed net
    }

    // Coroutine for flashing the net
    private IEnumerator FlashNetCoroutine()
    {
        netClosed = true;
        float flashDuration = 1.0f; // Set the total duration for flashing
        float timer = 0.0f;

        // Flashing effect
        while (timer < flashDuration)
        {
            float t = Mathf.PingPong(timer * 2.0f, 1.0f); // Create a pulsing effect
            netMaterial.SetInt("_isFlashing", 1); // Enable flashing

            // Update the flash timer
            timer += Time.deltaTime;
            yield return null; // Wait for the next frame
        }

        // Reset the net after flashing
        ResetNet();
    }

    void ResetNet()
    {
        netSegments.Clear();
        netClosed = false;
        netMaterial.SetInt("_isFlashing", 0); // Stop flashing
        UpdateLineRenderer(); // Update LineRenderer to clear the visual
    }


    // Check and collect objects inside the net once the loop is closed
    void CollectObjectsInsideNet()
    {
        // Create a mesh from the net segments
        Mesh netMesh = CreateMeshFromPoints(netSegments);

        // Create a new GameObject for the mesh collider
        GameObject netColliderObject = new GameObject("NetCollider");
        netColliderObject.transform.position = Vector3.zero;
        netColliderObject.transform.rotation = Quaternion.identity;

        // Add a mesh collider to the new GameObject
        AddMeshCollider(netColliderObject, netMesh);

        // Check for collisions with objects inside the net
        Collider[] hitColliders = Physics.OverlapBox(netMesh.bounds.center, netMesh.bounds.extents, Quaternion.identity);
        foreach (var hitCollider in hitColliders)
        {
            if (hitCollider.CompareTag("Trash"))
            {
                levelManager.CollectRubbish(hitCollider.gameObject);
            }
           // else if (hitCollider.CompareTag("Freindly"))
           // {
           //     //levelManager.CollectRubbish(hitCollider.gameObject);
           // }
        }

        // Destroy the net collider object and its mesh immediately after checking for collisions
        Destroy(netColliderObject);
    }

    Mesh CreateMeshFromPoints(List<Vector3> points)
    {
        Mesh mesh = new Mesh();
        Vector3[] vertices = points.ToArray();
        int[] triangles = new int[(points.Count - 2) * 3];

        for (int i = 0; i < points.Count - 2; i++)
        {
            triangles[i * 3] = 0;
            triangles[i * 3 + 1] = i + 1;
            triangles[i * 3 + 2] = i + 2;
        }

        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.RecalculateNormals();
        mesh.RecalculateBounds();

        return mesh;
    }

    void AddMeshCollider(GameObject obj, Mesh mesh)
    {
        MeshCollider meshCollider = obj.AddComponent<MeshCollider>();
        meshCollider.sharedMesh = mesh;
        meshCollider.convex = false; // Set to false for accurate collision detection
    }
}
