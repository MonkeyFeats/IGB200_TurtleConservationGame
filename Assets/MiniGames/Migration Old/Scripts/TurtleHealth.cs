using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurtleHealth : MonoBehaviour
{
    int hits = 3; // Current health
    int maxHits = 3; // Maximum health
    bool invincible = false;
    float invincibleTimer = 0;
    float invincibleTime = 1;
    MeshRenderer meshRenderer;
    [SerializeField] private GameObject[] livesImages;
    public SceneManagerScript SceneManager;

    private int jellyfishCount = 0; // Counter for collected Jellyfish

    void Start()
    {
        meshRenderer = GetComponent<MeshRenderer>();
        UpdateHealthUI(); // Initialize UI based on current health
    }

    void Update()
    {
        if (invincible)
        {
            if (invincibleTimer >= invincibleTime)
            {
                invincibleTimer = 0;
                invincible = false;
                meshRenderer.enabled = true;
            }
            else
            {
                invincibleTimer += Time.deltaTime;
                meshRenderer.enabled = !meshRenderer.enabled;
            }
        }
    }

    public void TakeDamage()
    {
        if (!invincible)
        {
            hits--;
            Debug.Log("Hits: " + hits); // Debug log to show current hits
            UpdateHealthUI();
            if (hits <= 0)
            {
                Destroy(gameObject);
                SceneManager.EndGame();
            }
            else
            {
                invincible = true;
                invincibleTimer = 0;
            }
        }
    }

    public void CollectJellyfish()
    {
        jellyfishCount++;
        Debug.Log("Jellyfish Count: " + jellyfishCount); // Debug log to show jellyfish count

        // Restore health if 3 Jellyfish have been collected
        if (jellyfishCount >= 3)
        {
            Debug.Log("Restoring Health"); // Debug log to confirm health restoration
            RestoreHealth(1); // Restore 1 health
            jellyfishCount = 0; // Reset the counter
        }
    }

    private void RestoreHealth(int amount)
    {
        // Increase health up to the maximum
        if (hits < maxHits)
        {
            hits += amount;
            // Ensure health does not exceed the maximum
            if (hits > maxHits)
            {
                hits = maxHits;
            }
            UpdateHealthUI(); // Update UI to reflect restored health

            Debug.Log("Health Restored. Current Hits: " + hits); // Debug log to show restored health
        }
    }

    private void UpdateHealthUI()
    {
        // Update the health UI images based on current health
        for (int i = 0; i < livesImages.Length; i++)
        {
            livesImages[i].SetActive(i < hits); // Activate images up to current health
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Enemy"))
        {
            TakeDamage();
        }
        else if (other.gameObject.CompareTag("Jellyfish"))
        {
            Debug.Log("Jellyfish Collected"); // Debug log to confirm Jellyfish collection
            CollectJellyfish();
            Destroy(other.gameObject); // Destroy the Jellyfish after collecting it
        }
    }
}
