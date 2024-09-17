using UnityEngine;

public class FloatingRubbish : MonoBehaviour
{
    public Texture2D flowMap; // The flow map texture for movement
    public float flowStrength = 2f; // Speed at which the rubbish moves with the flow
    public float floatSpeed = 0.5f; // Speed of the bobbing effect
    public float floatRange = 0.5f; // Vertical range of bobbing
    public Sprite[] trashSprites; // Array of different trash sprites

    private SpriteRenderer spriteRenderer;
    private Vector3 initialPosition;
    private bool collected = false;

    void Start()
    {
        initialPosition = transform.position;
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();

        // Randomly assign a sprite from the array of trash sprites
        AssignRandomSprite();
    }

    void Update()
    {
        if (!collected)
        {
            // Bobbing effect
            //float newY = Mathf.Sin(Time.time * floatSpeed) * floatRange;
            //transform.position = new Vector3(transform.position.x, initialPosition.y + newY, transform.position.z);

            // Get flow direction from the flow map
            Vector3 flowDirection = SampleFlowMap(transform.position);
            // Move rubbish along the flow direction
            transform.Translate(flowDirection * flowStrength * Time.deltaTime);
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

    private Vector3 SampleFlowMap(Vector3 worldPosition)
    {
        // Convert world position to UV coordinates based on the flow map
        float u = Mathf.InverseLerp(0, 100, worldPosition.x); // Adjust 100 to your world size
        float v = Mathf.InverseLerp(0, 100, worldPosition.z); // Adjust 100 to your world size

        // Sample the texture using UV coordinates
        Color flowColor = flowMap.GetPixelBilinear(u, v);

        // Convert the red and green channels to directional vectors (X, Z axes)
        Vector3 flowDirection = new Vector3(flowColor.r * 2 - 1, 0, flowColor.g * 2 - 1); // R & G channels to [-1, 1] range

        return flowDirection.normalized; // Return a normalized vector to get only the direction
    }


    private void CollectRubbish()
    {
        collected = true;
        Destroy(gameObject); // Replace with collection logic if needed
    }
}
