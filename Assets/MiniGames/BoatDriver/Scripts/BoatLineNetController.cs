using System.Collections.Generic;
using UnityEngine;

public class NetController3D : MonoBehaviour
{
    public GameObject boat;                  // Reference to the boat
    public LayerMask objectLayer;            // Layer for objects that can be collected
    public float segmentDistance = 1.0f;     // Distance between net segments
    public float maxNetLength = 50.0f;       // Max allowed net length
    public float flashDuration = 1.0f;       // Duration for net flash when closing
    public LineRenderer lineRenderer;        // Reference to the LineRenderer component
    public float closeDistance = 1.0f;       // Distance to check for closing the net
    public int segmentThreshold = 8;         // Number of segments to skip for checking

    private List<Vector3> netSegments = new List<Vector3>(); // Stores the net's path in 3D
    private bool netClosed = false;          // Flag for net closure

    public BoatCleanupLevelManager levelManager;

    void Start()
    {
        // Make sure the LineRenderer is attached to the same object
        lineRenderer.positionCount = 0; // Initialize with no points
    }

    void Update()
    {
        if (netClosed)
        {
            // Flashing and resetting logic
            FlashNet();
        }
        else
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
            if (IsBoatCloseToAnyPoint())
            {
                netClosed = true;
                TrimNetSegments();
                CollectObjectsInsideNet();
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
    bool IsBoatCloseToAnyPoint()
    {
        Vector3 boatPosition = boat.transform.position;
        for (int i = segmentThreshold; i < netSegments.Count; i++)
        {
            if (Vector3.Distance(boatPosition, netSegments[i]) < closeDistance)
            {
                return true;
            }
        }
        return false;
    }

    // Trims the net segments to only include the closed shape
    void TrimNetSegments()
    {
        Vector3 boatPosition = boat.transform.position;
        for (int i = segmentThreshold; i < netSegments.Count; i++)
        {
            if (Vector3.Distance(boatPosition, netSegments[i]) < closeDistance)
            {
                netSegments = netSegments.GetRange(0, i + 1);
                break;
            }
        }
    }

    // Flash the net when closed
    void FlashNet()
    {
        // Flashing logic here (e.g., change material or mesh visibility)
        flashDuration -= Time.deltaTime;
        if (flashDuration <= 0)
        {
            ResetNet();
        }
    }

    // Resets the net segments and prepares for the next path
    void ResetNet()
    {
        netSegments.Clear();
        netClosed = false;
        flashDuration = 1.0f; // Reset flash duration
        UpdateLineRenderer();
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

        // Reset the net after collecting objects
        ResetNet();
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
