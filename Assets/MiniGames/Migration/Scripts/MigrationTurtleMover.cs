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
    private bool isSpeedBoosted = false;
    private bool isSlowed = false;

    // Pickup and game object references
    public GameObject speedBoostPickup;
    public GameObject slowPickup;

    // Horizontal and vertical control
    public float horizontalControlSpeed = 5f;
    public float verticalControlSpeed = 3f;
    private float horizontalInput;
    private float verticalInput;

    // Turtle's boundaries
    public float horizontalBound = 5f;
    public float verticalBound = 3f;

    void Start()
    {
        // Set default speed at the start
        currentSpeed = defaultSpeed;
        dollyCart.m_Speed = currentSpeed;
    }

    void Update()
    {
        // Update the turtle's position along the spline path
        HandleMovement();
        dollyCart.m_Speed = currentSpeed;
    }

    void HandleMovement()
    {
        // Movement on the spline is handled by CinemachineDollyCart
        // Use player input to control horizontal/vertical movement within bounds

        horizontalInput = Input.GetAxis("Horizontal");
        verticalInput = Input.GetAxis("Vertical");

        // Apply input to turtle's position with clamping to stay within bounds
        Vector3 newPosition = transform.position;
        newPosition.x += horizontalInput * horizontalControlSpeed * Time.deltaTime;
        newPosition.y += verticalInput * verticalControlSpeed * Time.deltaTime;
        newPosition.x = Mathf.Clamp(newPosition.x, dollyCart.transform.localPosition.x - horizontalBound, dollyCart.transform.localPosition.x + horizontalBound);
        newPosition.y = Mathf.Clamp(newPosition.y, dollyCart.transform.localPosition.y - verticalBound, dollyCart.transform.localPosition.y + verticalBound);

        transform.position = newPosition;
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
        isSpeedBoosted = true;
        currentSpeed = fastSpeed;
        yield return new WaitForSeconds(5f); // Boost lasts for 5 seconds
        currentSpeed = defaultSpeed;
        isSpeedBoosted = false;
    }

    IEnumerator SlowZone()
    {
        // Temporary slow down
        isSlowed = true;
        currentSpeed = slowSpeed;
        yield return new WaitForSeconds(5f); // Slow lasts for 5 seconds
        currentSpeed = defaultSpeed;
        isSlowed = false;
    }

    void OnTriggerExit(Collider other)
    {
        // Reset speed when leaving slow zones
        if (other.CompareTag("SlowZone") && isSlowed)
        {
            currentSpeed = defaultSpeed;
        }
    }
}
