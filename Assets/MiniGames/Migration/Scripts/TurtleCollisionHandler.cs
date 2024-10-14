using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

public class TurtleCollisionHandler : MonoBehaviour
{
    [Header("Settings")]
    public float health = 100f;                 // Turtle's health
    public float damageOnCollision = 20f;       // Damage the turtle takes on collision
    public float flashDuration = 0.1f;          // How often the turtle flashes during cooldown
    public float teleportCooldown = 1.0f;       // Cooldown time before movement re-enables
    public Slider healthSlider;                 // UI Slider to display health
    public UnityEvent onHealthZero;             // Event triggered when health reaches zero

    private Rigidbody rb;                       // Reference to the turtle's Rigidbody
    private Collider turtleCollider;            // Reference to the turtle's Collider
    private MigrationPlayerMovement movementScript;  // Reference to movement script
    private SkinnedMeshRenderer turtleRenderer; // Reference to SkinnedMeshRenderer on the child object
    private bool isTeleporting = false;         // Cooldown flag
    private bool isFlashing = false;            // Flash flag
    private Transform originalPositionParent;   // Parent reference for the rail

    void Start()
    {
        rb = GetComponent<Rigidbody>();         // Get the Rigidbody component
        turtleCollider = GetComponent<Collider>();  // Get the Collider component
        movementScript = GetComponent<MigrationPlayerMovement>();  // Reference to movement script
        originalPositionParent = transform.parent;  // Parent is the rail/cart

        // Assuming the SkinnedMeshRenderer is on a child object
        turtleRenderer = GetComponentInChildren<SkinnedMeshRenderer>();

        UpdateHealthSlider();  // Initialize the slider with current health
    }

    // Detects collisions with objects that cause damage
    void OnCollisionEnter(Collision collision)
    {
        // Check if the object the turtle collided with has the "Obstacle" tag
        if (collision.gameObject.CompareTag("Obstacle"))
        {
            TakeDamage(damageOnCollision);
        }
    }

    // Trigger damage and immediately teleport the turtle back to local position (0,0,0)
    void TakeDamage(float damage)
    {
        if (isTeleporting) return; // Avoid multiple damage during teleport cooldown

        health -= damage;

        // Update the health slider
        UpdateHealthSlider();

        // Trigger the event if health reaches zero
        if (health <= 0)
        {
            onHealthZero.Invoke();
        }

        // Teleport back to the rail over the cooldown period
        StartCoroutine(SmoothMoveBackToRail());
    }

    // Coroutine to move the turtle back to the rail smoothly after taking damage
    IEnumerator SmoothMoveBackToRail()
    {
        isTeleporting = true;

        // Disable the movement script and the turtle's collider
        movementScript.enabled = false;
        turtleCollider.enabled = false;  // Disable the collider

        // Start flashing effect
        if (!isFlashing)
        {
            StartCoroutine(FlashDuringCooldown());
        }

        // Calculate the world position of the local (0, 0, 0) relative to the cart
        Vector3 targetPosition = originalPositionParent.TransformPoint(Vector3.zero);
        Vector3 startPosition = rb.position;  // The current world position

        float elapsedTime = 0f;

        while (elapsedTime < teleportCooldown)
        {
            // Smoothly move the Rigidbody back to the target position (rail) over time
            Vector3 newPosition = Vector3.Lerp(startPosition, targetPosition, elapsedTime / teleportCooldown);
            rb.MovePosition(newPosition);

            elapsedTime += Time.fixedDeltaTime;
            yield return new WaitForFixedUpdate();
        }

        // Ensure the final position is exactly the target position
        rb.MovePosition(targetPosition);

        // Stop any forces acting on the turtle
        rb.velocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;

        // Wait for the cooldown period before re-enabling movement and collider
        yield return new WaitForSeconds(teleportCooldown);

        // Re-enable movement script and collider after cooldown
        movementScript.enabled = true;
        turtleCollider.enabled = true;  // Re-enable the collider

        isTeleporting = false;
    }

    // Coroutine to flash the turtle during the cooldown (targeting SkinnedMeshRenderer)
    IEnumerator FlashDuringCooldown()
    {
        isFlashing = true;

        while (isTeleporting)
        {
            // Toggle the visibility of the SkinnedMeshRenderer by changing its material color
            if (turtleRenderer != null)
            {
                turtleRenderer.material.color = Color.clear;  // Set color to transparent
            }
            yield return new WaitForSeconds(flashDuration);

            if (turtleRenderer != null)
            {
                turtleRenderer.material.color = Color.white;  // Restore original color (or white as default)
            }
            yield return new WaitForSeconds(flashDuration);
        }

        isFlashing = false;
    }

    // Updates the UI slider to reflect the current health
    void UpdateHealthSlider()
    {
        if (healthSlider != null)
        {
            healthSlider.value = health / 100f;  // Normalize health (0 to 1)
        }
    }
}
