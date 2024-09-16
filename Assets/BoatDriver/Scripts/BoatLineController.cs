using System.Collections.Generic;
using UnityEngine;

public class BoatLineController : MonoBehaviour
{
    public float speed = 5f; // Boat speed
    public float rotationSpeed = 5f; // Speed at which the boat rotates to face the direction
    public LineRenderer lineRenderer; // LineRenderer component to display the path
    public float tolerance = 0.1f; // Tolerance for boat's proximity to waypoints
    public float minimumDistance = 0.1f; // Minimum distance between points for smooth line
    public float lineHeight = 2f; // Y position where the line will be drawn (above everything else)
    public float pathStartThreshold = 0.5f; // Minimum distance for the boat to begin following the path

    private List<Vector3> waypoints = new List<Vector3>(); // Stores the waypoints
    private bool isDrawing = false; // True when the player is drawing a path
    private int currentWaypointIndex = 0; // Current waypoint the boat is moving to

    public Camera mainCamera; // Reference to the camera

    void Update()
    {
        HandleInput(); // Capture player input

        if (!isDrawing && waypoints.Count > 0)
        {
            MoveAlongPath(); // Move the boat along the drawn path
        }
    }

    // Handle mouse/touch input for drawing the path
    void HandleInput()
    {
        // Check for the beginning of input (mouse click or touch)
        if (Input.GetMouseButtonDown(0))
        {
            isDrawing = true;
            waypoints.Clear(); // Clear any existing waypoints
            lineRenderer.positionCount = 0; // Clear the LineRenderer
        }

        // Continue drawing while the input is held
        if (isDrawing && Input.GetMouseButton(0))
        {
            // Convert mouse/touch position to world space using a raycast from the camera
            Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
            Plane drawPlane = new Plane(Vector3.up, new Vector3(0, lineHeight, 0)); // Plane at the desired line height
            float distance;

            // Raycast against a plane at lineHeight (Y-axis level)
            if (drawPlane.Raycast(ray, out distance))
            {
                Vector3 mousePosition = ray.GetPoint(distance);

                // Only add waypoints if they are sufficiently far from the last one for a smooth line
                if (waypoints.Count == 0 || Vector3.Distance(mousePosition, waypoints[waypoints.Count - 1]) > minimumDistance)
                {
                    AddWaypoint(mousePosition);
                }
            }
        }

        // End drawing once the player releases the mouse/touch
        if (Input.GetMouseButtonUp(0))
        {
            isDrawing = false;
            currentWaypointIndex = 0; // Reset to start moving from the first waypoint
        }
    }

    // Add a waypoint and update the line renderer
    void AddWaypoint(Vector3 point)
    {
        waypoints.Add(point);
        lineRenderer.positionCount = waypoints.Count;

        // Set the position for the LineRenderer
        lineRenderer.SetPosition(waypoints.Count - 1, point);
    }

    // Move the boat along the drawn path
    void MoveAlongPath()
    {
        if (currentWaypointIndex < waypoints.Count)
        {
            Vector3 targetPosition = waypoints[currentWaypointIndex];
            targetPosition.y = transform.position.y; // Ensure boat stays on the same y-plane

            // Check if the boat is far enough from the first waypoint to begin movement
            if (currentWaypointIndex == 0 && Vector3.Distance(transform.position, targetPosition) < pathStartThreshold)
            {
                // Skip the first point if it's too close to the boat
                currentWaypointIndex++;
                return;
            }

            // Move the boat towards the current waypoint
            transform.position = Vector3.MoveTowards(transform.position, targetPosition, speed * Time.deltaTime);

            // Rotate the boat to face the target direction
            Vector3 direction = (targetPosition - transform.position).normalized;
            if (direction != Vector3.zero) // Ensure we don't get NaN errors
            {
                Quaternion targetRotation = Quaternion.LookRotation(direction, Vector3.up);
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
            }

            // If the boat reaches the current waypoint, move to the next
            if (Vector3.Distance(transform.position, targetPosition) < tolerance)
            {
                currentWaypointIndex++;
            }
        }
    }
}
