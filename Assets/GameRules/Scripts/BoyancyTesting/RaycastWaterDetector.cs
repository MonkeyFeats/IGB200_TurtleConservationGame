using UnityEditor.ShaderGraph.Internal;
using UnityEngine;

public class WaterHeightDetector : MonoBehaviour
{
    public LayerMask waterLayer;  // Assign your water layer in the inspector
    private Rigidbody rb;
    public float minWaterHeight = -1.0f;
    public float maxWaterHeight = -1.0f;
    public Transform offsetRayPos;
    public float floatStrength = 1f;
    public float rayDist = 5f;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    void FixedUpdate()
    {
        // Object's current height
        float objectHeight = transform.position.y;
        RaycastHit hit;

        // Cast a ray downward from a high point to find where it hits the water mesh
        Vector3 rayOrigin = offsetRayPos.position;  // Starting point of the ray
        Vector3 rayDirection = Vector3.down;         // Direction of the ray

        // Draw the ray in the Scene view for debugging purposes
        Debug.DrawRay(rayOrigin, rayDirection * rayDist, Color.red);

        if (Physics.Raycast(rayOrigin, rayDirection, out hit, rayDist, waterLayer))
        {
            // Calculate buoyancy force proportional to the distance underwater
            float displacement = hit.point.y - objectHeight;
            Vector3 buoyancy = Vector3.up * (displacement * floatStrength) - rb.velocity;

            // Apply buoyancy force
            rb.AddForce(buoyancy, ForceMode.Acceleration);
            Debug.Log("Adding Force");

            // Draw the hit point in the Scene view for debugging
            Debug.DrawLine(rayOrigin, hit.point, Color.green); // Line from ray origin to hit point
        }
    }

}