// File: MigrationTurtleController.cs
using Cinemachine;
using System.Collections;
using UnityEngine;

public class MigrationTurtleController : MonoBehaviour
{
    // Movement settings
    public CinemachineDollyCart dollyCart; // Cinemachine dolly cart to move the turtle on the spline path

    // Speed control
    public float defaultSpeed = 5f;
    public float fastSpeed = 10f;
    private float currentSpeed;

    // Movement control
    public float horizontalSpeed = 5f;
    public float verticalSpeed = 3f;

    // Lerp speed for smoother movement
    public float lerpSpeed = 3f;

    // Rolling settings
    public float rollAmount = 5f;  // The amount the turtle rolls based on input
    public float rollLerpSpeed = 5f; // Speed at which to lerp back to normal roll

    private Vector3 targetPosition; // Target position based on player input
    private float currentRoll;       // Current roll angle

    void Start()
    {
        // Set default speed at the start
        currentSpeed = defaultSpeed;
        dollyCart.m_Speed = currentSpeed;

        // Set the target position to the current position
        targetPosition = transform.localPosition;
        currentRoll = 0f; // Initialize current roll
    }

    void Update()
    {
        // Handle the turtle movement and apply the result to dolly cart
        HandleMovement();
        UpdateRoll(); // Update roll based on input
    }

    void HandleMovement()
    {
        // Capture player input for horizontal and vertical movement
        float horizontalInput = Input.GetAxis("Horizontal");
        float verticalInput = Input.GetAxis("Vertical");

        // Calculate desired position based on player input
        Vector3 inputMovement = new Vector3(horizontalInput * horizontalSpeed, verticalInput * verticalSpeed, 0) * Time.deltaTime;

        // Apply the input movement to the target position
        targetPosition += inputMovement;

        // Apply the target position with Lerp for smoother motion
        targetPosition = Vector3.Lerp(transform.localPosition, targetPosition, lerpSpeed * Time.deltaTime);

        // Set the turtle's position in local space, keeping Z fixed to the dolly cart's Z position
        transform.localPosition = new Vector3(targetPosition.x, targetPosition.y, transform.localPosition.z);

        // Optionally, make the turtle face the direction of movement
        if (inputMovement != Vector3.zero)
        {
            Quaternion targetRotation = Quaternion.LookRotation(Vector3.forward, inputMovement);
            transform.localRotation = Quaternion.Slerp(transform.localRotation, targetRotation, lerpSpeed * Time.deltaTime);
        }
    }

    void UpdateRoll()
    {
        // Capture player input for rolling (only horizontal input affects roll)
        float horizontalInput = Input.GetAxis("Horizontal");

        if (horizontalInput != 0)
        {
            // Roll a little based on horizontal input only
            currentRoll = horizontalInput * rollAmount;
        }
        else
        {
            // Lerp back to 0 when no horizontal input is detected
            currentRoll = Mathf.Lerp(currentRoll, 0f, rollLerpSpeed * Time.deltaTime);
        }

        // Apply the roll to the turtle's local rotation on the Z axis only
        transform.localRotation = Quaternion.Euler(0, 0, currentRoll) * Quaternion.Euler(transform.localRotation.eulerAngles.x, transform.localRotation.eulerAngles.y, 0);
    }
}
