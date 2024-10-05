using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using DG.Tweening;

[RequireComponent(typeof(Rigidbody))]
public class MigrationPlayerMovement : MonoBehaviour
{
    private Transform playerModel;
    private Rigidbody rb;
    Animator animator;

    [Header("Parameters")]
    public float movementSpeed = 18;
    public float returnForceMultiplier = 5f;  // Multiplier for the return force based on distance
    public float maxHorizontalDistance = 5f;  // Maximum distance on the X axis (ellipse)
    public float maxVerticalDistance = 3f;    // Maximum distance on the Y axis (ellipse)
    public float maxRoll = 30f;               // Maximum roll angle for the turtle
    public float maxPitch = 20f;              // Maximum pitch angle for the turtle
    public float rotationSpeed = 5f;          // How fast the turtle rotates towards its velocity direction
    public float zDistanceStrength = 10f;     // How strongly the turtle matches the dolly cart's Z position
    public float alignmentStrength = 2f;      // How strongly the turtle aligns with the dolly cart's orientation
    public float animatorSpeedScalar = 1.5f;    // Scales how fast the animation plays relative to the magnitude of the velocity

    [Space]

    [Header("Public References")]
    public CinemachineDollyCart dolly;      // Dolly cart to follow
    public Transform cameraParent;

    void Start()
    {
        playerModel = transform.GetChild(0);
        rb = GetComponent<Rigidbody>();
        animator = GetComponent<Animator>();
        rb.useGravity = false;  // Disable gravity for the turtle
    }

    void FixedUpdate()
    {
        // Handle movement input (joystick or keyboard)
        float horizontalInput = Input.GetAxis("Horizontal");
        float verticalInput = Input.GetAxis("Vertical");

        MoveOnPlane(horizontalInput, verticalInput);
        MatchDollyZPosition();
        AlignWithDolly();
        UpdateRotation(rb.velocity);

        animator.SetFloat("MovementSpeed", rb.velocity.magnitude * animatorSpeedScalar);
    }

    // Movement relative to the dolly cart's local 2D plane
    void MoveOnPlane(float horizontalInput, float verticalInput)
    {
        Vector3 localRight = dolly.transform.right;
        Vector3 localUp = dolly.transform.up;

        // Calculate the movement force based on player input and local directions
        Vector3 movementForce = (localRight * horizontalInput + localUp * verticalInput) * movementSpeed;

        // Apply the movement force to the Rigidbody
        rb.AddForce(movementForce);

        // Clamp the turtle's position to stay within the ellipse defined by maxHorizontalDistance and maxVerticalDistance
        ClampToEllipse();
    }

    // Clamp the turtle's position to stay within an ellipse around the dolly cart
    void ClampToEllipse()
    {
        Vector3 offset = transform.position - dolly.transform.position;

        // Handle clamping for X (horizontal) and Y (vertical) distances
        float horizontalDistance = Mathf.Abs(offset.x) / maxHorizontalDistance;
        float verticalDistance = Mathf.Abs(offset.y) / maxVerticalDistance;

        if (horizontalDistance > 1f || verticalDistance > 1f)
        {
            // If the turtle is outside the ellipse, calculate the return force
            float returnForce = returnForceMultiplier * Mathf.Pow(Mathf.Max(horizontalDistance, verticalDistance), 2);

            // Normalize the offset and apply a force to return to the ellipse
            Vector3 returnDirection = new Vector3(
                -offset.x / maxHorizontalDistance,
                -offset.y / maxVerticalDistance,
                0
            ).normalized * returnForce;

            // Apply the return force to the rigidbody
            rb.AddForce(returnDirection);
        }
    }

    // Ensure the turtle follows the dolly cart's Z position based on a configurable strength
    void MatchDollyZPosition()
    {
        // Calculate the difference in Z positions between the turtle and the dolly cart
        float zDifference = dolly.transform.position.z - transform.position.z;

        // Apply a force to the rigidbody to move the turtle's Z position towards the dolly's Z position
        Vector3 zCorrectionForce = new Vector3(0, 0, zDifference * zDistanceStrength);

        // Apply the force to the rigidbody to gradually match the dolly's Z position
        rb.AddForce(zCorrectionForce);
    }

    // Align the turtle's rotation with the dolly cart, influenced by a configurable strength
    void AlignWithDolly()
    {
        // Get the target forward direction from the dolly cart
        Vector3 targetForward = dolly.transform.forward;

        // Calculate the current turtle's forward direction
        Vector3 currentForward = transform.forward;

        // Blend the rotation to slowly match the dolly's forward direction (aligns with dolly over time)
        Vector3 blendedDirection = Vector3.Slerp(currentForward, targetForward, Time.deltaTime * alignmentStrength);

        // Apply the blended rotation back to the turtle, but don't affect pitch and roll
        Quaternion targetRotation = Quaternion.LookRotation(blendedDirection);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * alignmentStrength);
    }

    // Update the turtle's rotation based on a blend between the cart's orientation and the velocity direction
    void UpdateRotation(Vector3 velocity)
    {
        Vector3 cartForward = dolly.transform.forward;
        Vector3 velocityDirection = velocity.normalized;

        // Blend between cart forward and velocity direction (the more the turtle moves, the more the velocity direction overrides)
        Vector3 blendedDirection = Vector3.Slerp(cartForward, velocityDirection, Mathf.Clamp01(velocity.magnitude / movementSpeed));

        float targetRoll = Mathf.Clamp(-velocity.x * maxRoll / movementSpeed, -maxRoll, maxRoll);
        float targetPitch = Mathf.Clamp(velocity.y * maxPitch / movementSpeed, -maxPitch, maxPitch);

        // Create a target rotation based on blended direction, but with custom roll and pitch
        Quaternion targetRotation = Quaternion.LookRotation(blendedDirection);
        targetRotation *= Quaternion.Euler(targetPitch, 0, targetRoll); // Add pitch and roll

        transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, Time.deltaTime * rotationSpeed);
    }

    public void QuickSpin(int dir)
    {
        playerModel.DOLocalRotate(new Vector3(playerModel.localEulerAngles.x, playerModel.localEulerAngles.y, 360 * -dir), .4f, RotateMode.LocalAxisAdd).SetEase(Ease.OutSine);
    }

    // Function to set the speed of the dolly cart
    void SetSpeed(float x)
    {
        dolly.m_Speed = x;
    }

    // Function to adjust the camera zoom with a smooth transition
    void SetCameraZoom(float zoom, float duration)
    {
        cameraParent.DOLocalMove(new Vector3(0, 0, zoom), duration);
    }

    // OnDrawGizmos is used to visualize elements in the Unity Editor
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, Mathf.Max(maxHorizontalDistance, maxVerticalDistance));  // Visualize the max radius around the dolly cart
    }
}
