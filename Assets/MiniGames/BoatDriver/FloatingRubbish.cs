using UnityEngine;
using System.Collections;

public class FloatingRubbish : MonoBehaviour
{
    public float currentStrength = 2f; // Speed at which the rubbish moves with the current
    public float floatSpeed = 0.5f; // Speed of the bobbing effect
    public float floatRange = 0.5f; // Vertical range of bobbing
    public Sprite[] trashSprites; // Array of different trash sprites
    public GameObject collectionEffectPrefab; // The prefab with AudioSource and ParticleSystem
    public float flashDuration = 0.1f; // Duration for each flash
    public int flashCount = 3; // Number of flashes
    public float currentChangeInterval = 10f; // Time interval for changing current direction

    private SpriteRenderer spriteRenderer;
    private AudioSource audioSource;
    private Vector3 initialPosition;
    private bool collected = false;
    private Vector3 currentDirection; // Direction of the current
    private float currentChangeTimer = 0f; // Timer for current change

    void Start()
    {
        initialPosition = transform.position;
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        audioSource = GetComponent<AudioSource>();

        // Randomly assign a sprite from the array of trash sprites
        AssignRandomSprite();

        // Set initial current direction
        ChangeCurrentDirection();
    }

    void Update()
    {
        if (!collected)
        {
            // Bobbing effect
            float newY = Mathf.Sin(Time.time * floatSpeed) * floatRange;
            transform.position = new Vector3(transform.position.x, initialPosition.y + newY, transform.position.z);

            // Move rubbish along the current direction
            transform.Translate(currentDirection * currentStrength * Time.deltaTime);

            // Update the timer and change current direction if needed
            currentChangeTimer += Time.deltaTime;
            if (currentChangeTimer >= currentChangeInterval)
            {
                ChangeCurrentDirection();
                currentChangeTimer = 0f;
            }
        }
    }

    private void AssignRandomSprite()
    {
        // Randomly pick a sprite from the array of trash sprites
        if (trashSprites.Length > 0)
        {
            int randomIndex = Random.Range(0, trashSprites.Length);
            spriteRenderer.sprite = trashSprites[randomIndex];
        }
    }

    private void ChangeCurrentDirection()
    {
        // Randomly change the current direction (X, Z axes only)
        currentDirection = new Vector3(Random.Range(-1f, 1f), 0, Random.Range(-1f, 1f)).normalized;
    }

    public void CollectRubbish()
    {
        if (!collected)
        {
            collected = true;

            // Start flashing, particle effect, and sound before destruction
            StartCoroutine(FlashSprite());
        }
    }

    private IEnumerator FlashSprite()
    {
        for (int i = 0; i < flashCount; i++)
        {
            spriteRenderer.enabled = false; // Turn off the sprite
            yield return new WaitForSeconds(flashDuration);
            spriteRenderer.enabled = true; // Turn the sprite back on
            yield return new WaitForSeconds(flashDuration);
        }

        // Instantiate the prefab with sound and particle effect
        if (collectionEffectPrefab != null)
        {
            Instantiate(collectionEffectPrefab, transform.position, Quaternion.identity);
        }

        Destroy(gameObject); // Destroy the rubbish object after the effect starts
    }
}
