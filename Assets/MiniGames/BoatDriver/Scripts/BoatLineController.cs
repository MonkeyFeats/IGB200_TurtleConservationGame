using System.Collections.Generic;
using UnityEngine;

public class BoatLineController : MonoBehaviour
{
    public float speed = 5f; // Boat speed
    public float rotationSpeed = 2f; // Speed at which the boat rotates to face the direction
    public float maxTurnAngle = 30f; // Maximum angle the boat can turn in one frame
    public LineRenderer lineRenderer; // LineRenderer component to display the path
    public float tolerance = 0.1f; // Tolerance for boat's proximity to waypoints
    public float minimumDistance = 0.1f; // Minimum distance between points for smooth line
    public float lineHeight = 2f; // Y position where the line will be drawn (above everything else)
    public float fadeDuration = 2f; // Duration over which each line segment fades out
    public float waypointProximityThreshold = 0.5f; // Distance to move to next waypoint if close
    public float overshootThreshold = 1.0f; // Distance to check if boat has overshot the waypoint

    private List<Vector3> lineWaypoints = new List<Vector3>(); // Waypoints used for drawing the line
    private List<Vector3> boatWaypoints = new List<Vector3>(); // Waypoints used for guiding the boat
    private List<float> segmentTimes = new List<float>(); // List to track fade times for each segment
    private bool isDrawing = false; // True when the player is drawing a path
    private int currentWaypointIndex = 0; // Current waypoint the boat is moving to
    private bool drawingEnabled = false; // Ensure drawing starts only after clicking the boat

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
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit))
            {
                if (hit.transform == transform)
                {
                    drawingEnabled = true;
                    isDrawing = true;
                    lineWaypoints.Clear();
                    boatWaypoints.Clear();
                    lineRenderer.positionCount = 0;
                    segmentTimes.Clear();
                }
            }
        }

        if (drawingEnabled && isDrawing && Input.GetMouseButton(0))
        {
            Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
            Plane drawPlane = new Plane(Vector3.up, new Vector3(0, lineHeight, 0));
            float distance;

            if (drawPlane.Raycast(ray, out distance))
            {
                Vector3 mousePosition = ray.GetPoint(distance);

                if (lineWaypoints.Count == 0 || Vector3.Distance(mousePosition, lineWaypoints[lineWaypoints.Count - 1]) > minimumDistance)
                {
                    AddWaypoint(mousePosition);
                }
            }
        }

        if (Input.GetMouseButtonUp(0))
        {
            isDrawing = false;
            currentWaypointIndex = 0;
            drawingEnabled = false;
        }
    }

    // Add a waypoint and update the line renderer
    void AddWaypoint(Vector3 point)
    {
        lineWaypoints.Add(point);
        boatWaypoints.Add(point);
        lineRenderer.positionCount = lineWaypoints.Count;
        lineRenderer.SetPosition(lineWaypoints.Count - 1, point);
        segmentTimes.Add(Time.time);
    }

    // Move the boat along its waypoints
    void MoveAlongPath()
    {
        if (currentWaypointIndex < boatWaypoints.Count)
        {
            Vector3 targetPosition = boatWaypoints[currentWaypointIndex];
            targetPosition.y = transform.position.y; // Ensure boat stays on the same y-plane

            // Move and rotate the boat towards the current waypoint
            Vector3 direction = (targetPosition - transform.position).normalized;
            float angleToTarget = Vector3.SignedAngle(transform.forward, direction, Vector3.up);

            // Smooth turning with dead zone to avoid constant small corrections
            if (Mathf.Abs(angleToTarget) > 2f)
            {
                float clampedAngle = Mathf.Clamp(angleToTarget, -maxTurnAngle, maxTurnAngle);
                Quaternion targetRotation = Quaternion.AngleAxis(clampedAngle, Vector3.up) * transform.rotation;
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
            }

            // Move the boat forward in its current direction
            transform.position += transform.forward * speed * Time.deltaTime;

            // Check if the boat is close enough to the waypoint
            float distanceToWaypoint = Vector3.Distance(transform.position, targetPosition);

            // If the boat is close enough, move to the next waypoint
            if (distanceToWaypoint < waypointProximityThreshold)
            {
                currentWaypointIndex++;
                return;
            }

            // If the boat has overshot the waypoint (passed it), move to the next waypoint
            if (HasOvershotWaypoint(targetPosition))
            {
                currentWaypointIndex++;
                return;
            }
        }
    }

    // Check if the boat has passed the current waypoint
    bool HasOvershotWaypoint(Vector3 targetPosition)
    {
        if (currentWaypointIndex < boatWaypoints.Count - 1)
        {
            Vector3 nextWaypoint = boatWaypoints[currentWaypointIndex + 1];
            Vector3 toNextWaypoint = (nextWaypoint - targetPosition).normalized;
            Vector3 toBoat = (transform.position - targetPosition).normalized;

            // If the dot product is positive, it means the boat is beyond the current waypoint and facing the next one
            return Vector3.Dot(toNextWaypoint, toBoat) > 0 && Vector3.Distance(transform.position, targetPosition) > overshootThreshold;
        }
        return false;
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

                if (i < lineRenderer.positionCount)
                {
                    Color color = lineRenderer.startColor;
                    color.a = alpha;
                    lineRenderer.SetColors(color, color);
                }
            }

            if (segmentTimes.Count > 0 && Time.time - segmentTimes[0] > fadeDuration)
            {
                segmentTimes.RemoveAt(0);
                lineWaypoints.RemoveAt(0);
                lineRenderer.positionCount = lineWaypoints.Count;
                for (int i = 0; i < lineWaypoints.Count; i++)
                {
                    lineRenderer.SetPosition(i, lineWaypoints[i]);
                }
            }
        }
    }
}
