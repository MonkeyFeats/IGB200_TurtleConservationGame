﻿using System.Collections;
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
    public float movementSpeed = 18;               // Maximum movement speed
    public float returnForceMultiplier = 5f;       // Multiplier for the return force based on distance
    public float maxHorizontalDistance = 5f;       // Maximum distance on the X axis (ellipse)
    public float maxVerticalDistance = 3f;         // Maximum distance on the Y axis (ellipse)
    public float maxRoll = 30f;                    // Maximum roll angle for the turtle
    public float maxPitch = 20f;                   // Maximum pitch angle for the turtle
    public float maxYaw = 20f;                     // Maximum yaw angle for the turtle
    public float rotationSpeed = 5f;               // How fast the turtle rotates towards its velocity direction
    public float zDistanceStrength = 10f;          // How strongly the turtle matches the dolly cart's Z position
    public float alignmentStrength = 2f;           // How strongly the turtle aligns with the dolly cart's orientation
    public float animatorSpeedScalar = 1.5f;       // Scales how fast the animation plays relative to the magnitude of the velocity
    public Vector3 offsetPos;                      // Offset from the dolly cart
    private float wantedCartSpeed = 3f;

    [Space]

    [Header("Public References")]
    public CinemachineDollyCart dolly;      // Dolly cart to follow
    public Transform cameraParent;

    private Vector2 screenCenter;            // The center of the screen for touch input

    void Start()
    {
        playerModel = transform.GetChild(0);
        rb = GetComponent<Rigidbody>();
        animator = GetComponent<Animator>();
        rb.useGravity = false;  // Disable gravity for the turtle

        // Calculate the screen center in screen space (for touch input)
        screenCenter = new Vector2(Screen.width / 2f, Screen.height / 2f);
    }

    void FixedUpdate()
    {
        if (Input.touchCount > 0)
        {
            // Handle mobile touch movement
            HandleTouchMovement();
        }
        else
        {
            // Handle keyboard or joystick movement (e.g., for editor testing)
            float horizontalInput = Input.GetAxis("Horizontal");
            float verticalInput = Input.GetAxis("Vertical");

            MoveOnPlane(horizontalInput, verticalInput);
        }

        MatchDollyZPosition();
        AlignWithDolly();
        UpdateRotation(rb.velocity);

        animator.SetFloat("MovementSpeed", rb.velocity.magnitude * animatorSpeedScalar);
    }

    // Handle touch input for movement
    void HandleTouchMovement()
    {
        Touch touch = Input.GetTouch(0);  // Get the first touch

        // Calculate the direction from the screen center to the touch position
        Vector2 touchPosition = touch.position;
        Vector2 direction = (touchPosition - screenCenter).normalized;

        // Calculate how far the touch is from the center (to control movement magnitude)
        float distanceFromCenter = (touchPosition - screenCenter).magnitude;

        // Scale the movement force based on how far the touch is from the center
        float movementForceMagnitude = Mathf.Clamp(distanceFromCenter / (Screen.width / 2f), 0, 1) * movementSpeed;

        // Convert the direction into local space (relative to the dolly cart)
        Vector3 localRight = dolly.transform.right;
        Vector3 localUp = dolly.transform.up;

        // Apply movement based on the direction and force magnitude
        Vector3 movementForce = (localRight * direction.x + localUp * direction.y) * movementForceMagnitude;

        // Apply the movement force to the Rigidbody
        rb.AddForce(movementForce);

        // Clamp the turtle's position to stay within the ellipse defined by maxHorizontalDistance and maxVerticalDistance
        ClampToEllipse();
    }

    // Movement relative to the dolly cart's local XY plane (keyboard/joystick input)
    void MoveOnPlane(float horizontalInput, float verticalInput)
    {
        // Calculate the local right (X-axis) and up (Y-axis) relative to the dolly cart's rotation
        Vector3 localRight = dolly.transform.right;
        Vector3 localUp = dolly.transform.up;

        // Calculate the movement force based on player input and the dolly cart's local directions
        Vector3 movementForce = (localRight * horizontalInput + localUp * verticalInput) * movementSpeed;

        // Apply the movement force to the Rigidbody
        rb.AddForce(movementForce);

        // Clamp the turtle's position to stay within the ellipse defined by maxHorizontalDistance and maxVerticalDistance
        ClampToEllipse();
    }

    // Clamp the turtle's position to stay within an ellipse around the dolly cart
    void ClampToEllipse()
    {
        Vector3 offset = transform.position - (dolly.transform.position + offsetPos);

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

            // Apply the return force to the Rigidbody
            rb.AddForce(returnDirection);
        }
    }

    // Ensure the turtle follows the dolly cart's Z position based on a configurable strength
    void MatchDollyZPosition()
    {
        // Calculate the difference in Z positions between the turtle and the dolly cart
        float zDifference = dolly.transform.position.z - transform.position.z;

        // Apply a force to the Rigidbody to move the turtle's Z position towards the dolly's Z position
        Vector3 zCorrectionForce = new Vector3(0, 0, zDifference * zDistanceStrength);

        // Apply the force to the Rigidbody to gradually match the dolly's Z position
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
        float targetPitch = Mathf.Clamp(-velocity.y * maxPitch / movementSpeed, -maxPitch, maxPitch);

        // Create a target rotation based on blended direction, but with custom roll and pitch
        Quaternion targetRotation = Quaternion.LookRotation(blendedDirection);
        targetRotation *= Quaternion.Euler(targetPitch, 0, targetRoll); // Add pitch and roll

        transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, Time.deltaTime * rotationSpeed);
    }
}
