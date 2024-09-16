using UnityEngine;

public class BoatController : MonoBehaviour
{
    // Public variables for tuning boat behavior
    public float maxSpeed = 10f; // Maximum speed the boat can reach
    public float acceleration = 5f; // How fast the boat accelerates
    public float steeringSpeed = 2f; // How fast the boat steers
    public float drag = 0.99f; // Water resistance
    public float angularDrag = 0.95f; // Angular water resistance (for realistic turning)

    private float currentSpeed = 0f; // Current speed of the boat
    private float throttleInput = 0f; // Input from throttle (Vertical axis)
    private float steeringInput = 0f; // Input from steering wheel (Horizontal axis)

    void Update()
    {
        // Get player input for throttle (Vertical) and steering (Horizontal)
        throttleInput = Input.GetAxis("Vertical");  // Forward/Backward (W/S or Up/Down Arrow)
        steeringInput = Input.GetAxis("Horizontal"); // Left/Right (A/D or Left/Right Arrow)

        // Calculate current speed based on throttle input and acceleration
        currentSpeed += throttleInput * acceleration * Time.deltaTime;
        currentSpeed = Mathf.Clamp(currentSpeed, -maxSpeed, maxSpeed); // Clamp speed between negative and positive maxSpeed

        // Apply drag (friction from water) to gradually slow down the boat
        currentSpeed *= drag;

        // Apply steering based on current speed and steering input
        float turnAngle = steeringInput * steeringSpeed * Time.deltaTime;
        transform.Rotate(0, turnAngle, 0);

        // Move the boat forward based on its current speed
        Vector3 movement = transform.forward * currentSpeed * Time.deltaTime;
        transform.Translate(movement, Space.World);
    }
}
