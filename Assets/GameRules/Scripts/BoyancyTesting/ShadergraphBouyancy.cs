using UnityEngine;

public class ShaderGraphBuoyancy : MonoBehaviour
{
    public float waveFrequency = 1f;    // Match these with Shader Graph
    public float waveAmplitude = 1f;    // Match these with Shader Graph
    public float waveSpeed = 1f;        // Match with Shader Graph time factor
    public float floatStrength = 2f;    // Adjust to control buoyancy force
    public float damping = 0.1f;        // Damping to prevent excessive bouncing

    private Rigidbody rb;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    void FixedUpdate()
    {
        // Get the time to sync with shader
        float time = Time.time * waveSpeed;

        // Get the water height at the object's position based on the Shader Graph logic
        float waterHeight = CalculateWaterHeight(transform.position, time);

        // Object's current height
        float objectHeight = transform.position.y;

        // Check if object is below water
        if (objectHeight < waterHeight)
        {
            // Calculate buoyancy force proportional to the distance underwater
            float displacement = waterHeight - objectHeight;
            Vector3 buoyancy = Vector3.up * (displacement * floatStrength) - rb.velocity * damping;

            // Apply buoyancy force
            rb.AddForce(buoyancy, ForceMode.Acceleration);
        }
    }

    // Function to calculate water height based on your Shader Graph logic
    float CalculateWaterHeight(Vector3 position, float time)
    {
        // Example: Simple sine waves for x and z axes (replace with your Shader Graph logic)
        float waveHeightX = Mathf.Sin(position.x * waveFrequency + time) * waveAmplitude;
        float waveHeightZ = Mathf.Sin(position.z * waveFrequency + time * 1.3f) * waveAmplitude;

        // Combine the two waves to get the final height
        return waveHeightX + waveHeightZ;
    }

    float GerstnerWave(float x, float z, float time, float wavelength, float amplitude, float speed, Vector2 direction)
    {
        float k = 2 * Mathf.PI / wavelength;
        float f = k * (Vector2.Dot(new Vector2(x, z), direction) - speed * time);
        return amplitude * Mathf.Sin(f);
    }
}
