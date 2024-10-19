using System.Collections.Generic;
using UnityEngine;

public class BoatSwipeController : MonoBehaviour
{
    public float speed = 5f; // Boat speed
    public float rotationSpeed = 100f; // Rotation speed based on swipe
    public float maxTurnAngle = 30f; // Maximum angle the boat can turn per swipe
    public LineRenderer trajectoryRenderer; // LineRenderer for trajectory visualization
    public float trajectoryDuration = 2f; // How far ahead the trajectory is predicted
    public int trajectorySegments = 20; // Number of segments for trajectory

    public Rigidbody rb; // Reference to the Rigidbody component
    private Vector2 swipeStart; // Start point of the swipe
    private bool isSwiping = false; // Whether the player is swiping
    private Vector3 swipeDelta; // Delta vector of swipe to calculate rotation

    void Update()
    {
        HandleInput();

        // Move the boat based on current velocity
        MoveBoat();

        // Update the trajectory line based on the boat's current direction and speed
        UpdateTrajectory();
    }

    // Handle touch or mouse input for swipe-based control
    void HandleInput()
    {
        if (Input.GetMouseButtonDown(0))
        {
            swipeStart = Input.mousePosition;
            isSwiping = true;
        }

        if (Input.GetMouseButton(0) && isSwiping)
        {
            Vector2 swipeEnd = Input.mousePosition;
            swipeDelta = swipeEnd - swipeStart;

            // Calculate the swipe length and direction
            float swipeLength = swipeDelta.magnitude;
            Vector2 swipeDirection = swipeDelta.normalized;

            // Determine the turn angle based on swipe direction and length
            float turnAngle = Mathf.Clamp(swipeLength / Screen.width * maxTurnAngle, -maxTurnAngle, maxTurnAngle);

            // Apply rotation based on the swipe direction and length
            if (swipeDirection.x < 0)
            {
                // Rotate left
                transform.Rotate(Vector3.up, -turnAngle * Time.deltaTime * rotationSpeed);
            }
            else if (swipeDirection.x > 0)
            {
                // Rotate right
                transform.Rotate(Vector3.up, turnAngle * Time.deltaTime * rotationSpeed);
            }

            // Reset swipeStart for continuous swipe control
            swipeStart = swipeEnd;
        }

        if (Input.GetMouseButtonUp(0))
        {
            isSwiping = false;
        }
    }

    // Move the boat forward based on its current facing direction
    void MoveBoat()
    {
        rb.velocity = transform.forward * speed;
    }

    // Update the trajectory visualization
    void UpdateTrajectory()
    {
        // Predict future positions based on boat speed, rotation, and current forward direction
        Vector3[] predictedPositions = new Vector3[trajectorySegments];
        Vector3 currentPosition = transform.position;
        Vector3 currentDirection = transform.forward;

        for (int i = 0; i < trajectorySegments; i++)
        {
            float deltaTime = trajectoryDuration / trajectorySegments;

            // Predict future position based on current velocity and direction
            Vector3 nextPosition = currentPosition + currentDirection * speed * deltaTime;

            // Apply a gradual turn to simulate the boat's turning behavior in the future
            Quaternion rotation = Quaternion.Euler(0, transform.eulerAngles.y * deltaTime, 0);
            currentDirection = rotation * currentDirection;

            // Store the predicted position
            predictedPositions[i] = nextPosition;

            // Move to the next predicted position
            currentPosition = nextPosition;
        }

        // Update the LineRenderer to visualize the trajectory
        trajectoryRenderer.positionCount = trajectorySegments;
        trajectoryRenderer.SetPositions(predictedPositions);
    }
}
