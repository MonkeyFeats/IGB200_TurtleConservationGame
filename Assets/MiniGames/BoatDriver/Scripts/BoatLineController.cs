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
    public float fadeDuration = 2f; // Duration over which each line segment fades out

    private List<Vector3> lineWaypoints = new List<Vector3>(); // Waypoints used for drawing the line
    private List<Vector3> boatWaypoints = new List<Vector3>(); // Waypoints used for guiding the boat
    private List<float> segmentTimes = new List<float>(); // List to track fade times for each segment
    private bool isDrawing = false; // True when the player is drawing a path
    private int currentWaypointIndex = 0; // Current waypoint the boat is moving to

    public Camera mainCamera; // Reference to the camera

    void Update()
    {
        HandleInput(); // Capture player input

        if (!isDrawing && boatWaypoints.Count > 0)
        {
            MoveAlongPath(); // Move the boat along its waypoints
        }

        UpdateLineFade(); // Update the fading effect of the line
    }

    // Handle mouse/touch input for drawing the path
    void HandleInput()
    {
        // Check for the beginning of input (mouse click or touch)
        if (Input.GetMouseButtonDown(0))
        {
            isDrawing = true;
            lineWaypoints.Clear(); // Clear any existing waypoints for the line
            boatWaypoints.Clear(); // Clear the boat's waypoints (if desired)
            lineRenderer.positionCount = 0; // Clear the LineRenderer
            segmentTimes.Clear(); // Clear segment fade times
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
                if (lineWaypoints.Count == 0 || Vector3.Distance(mousePosition, lineWaypoints[lineWaypoints.Count - 1]) > minimumDistance)
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
        lineWaypoints.Add(point);
        boatWaypoints.Add(point); // Also add the waypoint to the boat's path
        lineRenderer.positionCount = lineWaypoints.Count;
        lineRenderer.SetPosition(lineWaypoints.Count - 1, point);

        // Add a new fade time entry for the new segment
        segmentTimes.Add(Time.time);
    }

    // Move the boat along its waypoints
    void MoveAlongPath()
    {
        if (currentWaypointIndex < boatWaypoints.Count)
        {
            Vector3 targetPosition = boatWaypoints[currentWaypointIndex];
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

    // Update the fading effect of the line
    void UpdateLineFade()
    {
        if (lineWaypoints.Count > 0)
        {
            for (int i = 0; i < segmentTimes.Count; i++)
            {
                float elapsed = Time.time - segmentTimes[i];
                float alpha = Mathf.Clamp01(1 - (elapsed / fadeDuration));

                // Update the color of the line segment
                if (i < lineRenderer.positionCount)
                {
                    Color color = lineRenderer.startColor;
                    color.a = alpha;
                    lineRenderer.SetColors(color, color);
                }
            }

            // Remove faded segments from the line renderer
            if (segmentTimes.Count > 0 && Time.time - segmentTimes[0] > fadeDuration)
            {
                segmentTimes.RemoveAt(0);
                List<Vector3> newLinePoints = new List<Vector3>();
                for (int i = 1; i < lineWaypoints.Count; i++)
                {
                    newLinePoints.Add(lineWaypoints[i]);
                }
                lineWaypoints = newLinePoints;
                lineRenderer.positionCount = lineWaypoints.Count;
                for (int i = 0; i < lineWaypoints.Count; i++)
                {
                    lineRenderer.SetPosition(i, lineWaypoints[i]);
                }
            }
        }
    }
}
