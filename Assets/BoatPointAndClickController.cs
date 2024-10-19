using System.Collections.Generic;
using UnityEngine;

public class BoatPointAndClickController : MonoBehaviour
{
    public float speed = 5f; // Boat forward speed
    public float rotationSpeed = 30f; // Degrees per second the boat can rotate
    public LineRenderer trajectoryRenderer; // LineRenderer for trajectory visualization
    public int trajectorySegments = 20; // Number of segments for trajectory

    public Rigidbody rb; // Reference to the Rigidbody component
    private Vector3 targetPosition; // The point where the player clicked
    private bool hasTarget = false; // Whether the player has selected a target point

    void Update()
    {
        // Handle player input for selecting a target
        HandleInput();

        // Move the boat towards the target position
        if (hasTarget)
        {
            MoveBoat();
        }

        // Update the trajectory line to reflect boat's actual movement path
        if (hasTarget)
        {
            UpdateTrajectory();
        }
    }

    // Handle click or touch input to set the target position
    void HandleInput()
    {
        if (Input.GetMouseButtonDown(0))
        {
            // Convert the mouse position to a world position on the XZ plane (ignoring Y-axis)
            Plane plane = new Plane(Vector3.up, 0); // XZ plane at Y = 0
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            float distance;
            if (plane.Raycast(ray, out distance))
            {
                // Get the point on the XZ plane where the player clicked
                targetPosition = ray.GetPoint(distance);
                hasTarget = true;
            }
        }
    }

    // Move the boat towards the target position with tank-like movement
    void MoveBoat()
    {
        // Direction towards target
        Vector3 directionToTarget = (targetPosition - transform.position).normalized;

        // Calculate the angle between the boat's forward direction and the direction to the target
        float angleToTarget = Vector3.SignedAngle(transform.forward, directionToTarget, Vector3.up);

        // Rotate the boat gradually towards the target
        float rotationAmount = Mathf.Clamp(angleToTarget, -rotationSpeed * Time.deltaTime, rotationSpeed * Time.deltaTime);
        transform.Rotate(Vector3.up, rotationAmount);

        // Move the boat forward in its current direction
        rb.velocity = transform.forward * speed;

        // Check if the boat has reached the target
        if (Vector3.Distance(transform.position, targetPosition) < 0.5f)
        {
            rb.velocity = Vector3.zero; // Stop the boat when near the target
            hasTarget = false;
        }
    }

    // Update the trajectory visualization based on the boat's actual movement pattern
    void UpdateTrajectory()
    {
        Vector3[] predictedPositions = new Vector3[trajectorySegments];
        Vector3 currentPosition = transform.position;
        Vector3 currentDirection = transform.forward;
        float currentAngle = transform.eulerAngles.y;

        for (int i = 0; i < trajectorySegments; i++)
        {
            float deltaTime = (i + 1) / (float)trajectorySegments;

            // Predict future rotation based on boat's rotation speed
            Vector3 targetDir = (targetPosition - currentPosition).normalized;
            float angleToTarget = Vector3.SignedAngle(currentDirection, targetDir, Vector3.up);
            float rotationAmount = Mathf.Clamp(angleToTarget, -rotationSpeed * deltaTime, rotationSpeed * deltaTime);
            currentAngle += rotationAmount;

            // Calculate the future position and direction
            currentDirection = Quaternion.Euler(0, currentAngle, 0) * Vector3.forward;
            currentPosition += currentDirection * speed * deltaTime;

            predictedPositions[i] = currentPosition;
        }

        // Update the LineRenderer to show the predicted trajectory
        trajectoryRenderer.positionCount = trajectorySegments;
        trajectoryRenderer.SetPositions(predictedPositions);
    }
}
