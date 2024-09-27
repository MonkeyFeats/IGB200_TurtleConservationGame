// File: TurtleController.cs
using System.Collections;
using UnityEngine;
using Cinemachine;

public class MigrationTurtleController : MonoBehaviour
{
    // Movement settings
    public CinemachineDollyCart dollyCart; // Cinemachine dolly cart to move the turtle on the spline path
    public float defaultSpeed = 5f;
    public float fastSpeed = 10f;
    public float slowSpeed = 2f;

    // Speed control
    private float currentSpeed;

    // Movement control
    public float horizontalSpeed = 5f;
    public float verticalSpeed = 3f;

    // Circular boundary
    public float boundaryRadius = 5f;  // Max radius for the turtle's movement from the center
    public float lerpSpeed = 5f;       // Speed at which the turtle is lerped back into the boundary

    private Vector3 centerPoint;        // Center of the circular boundary (relative to turtle's local space)

    void Start()
    {
        // Set default speed at the start
        currentSpeed = defaultSpeed;
        dollyCart.m_Speed = currentSpeed;

        // Set the center of the circular boundary as the starting position of the turtle
        centerPoint = transform.localPosition;
    }

    void Update()
    {
        // Update the turtle's position along the spline path
        HandleMovement();
        dollyCart.m_Speed = currentSpeed;
    }

    void HandleMovement()
    {
        // Capture player input for horizontal and vertical movement
        float horizontalInput = Input.GetAxis("Horizontal");
        float verticalInput = Input.GetAxis("Vertical");

        // Update turtle's position based on player input
        Vector3 newPosition = transform.localPosition;
        newPosition.x += horizontalInput * horizontalSpeed * Time.deltaTime;
        newPosition.y += verticalInput * verticalSpeed * Time.deltaTime;

        // Calculate the displacement from the center point
        Vector3 displacementFromCenter = newPosition - centerPoint;
        float distanceFromCenter = displacementFromCenter.magnitude;

        // Check if turtle is outside the circular boundary
        if (distanceFromCenter > boundaryRadius)
        {
            // Lerp the turtle back into the boundary
            Vector3 targetPosition = centerPoint + displacementFromCenter.normalized * boundaryRadius;
            newPosition = Vector3.Lerp(newPosition, targetPosition, lerpSpeed * Time.deltaTime);
        }

        // Apply the new position to the turtle
        transform.localPosition = newPosition;

        // Ensure the turtle's Z position matches the dolly cart's position
        transform.localPosition = new Vector3(newPosition.x, newPosition.y, centerPoint.z);
    }

    void OnTriggerEnter(Collider other)
    {
        // Handle pickups
        if (other.CompareTag("SpeedBoost"))
        {
            StartCoroutine(SpeedBoost());
            Destroy(other.gameObject); // Remove the pickup after collection
        }
        else if (other.CompareTag("SlowZone"))
        {
            StartCoroutine(SlowZone());
        }
    }

    IEnumerator SpeedBoost()
    {
        // Temporary speed boost
        currentSpeed = fastSpeed;
        yield return new WaitForSeconds(5f); // Boost lasts for 5 seconds
        currentSpeed = defaultSpeed;
    }

    IEnumerator SlowZone()
    {
        // Temporary slow down
        currentSpeed = slowSpeed;
        yield return new WaitForSeconds(5f); // Slow lasts for 5 seconds
        currentSpeed = defaultSpeed;
    }

    void OnTriggerExit(Collider other)
    {
        // Reset speed when leaving slow zones
        if (other.CompareTag("SlowZone"))
        {
            currentSpeed = defaultSpeed;
        }
    }
}
