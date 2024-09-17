using System.Collections;
using UnityEngine;

public class FloatingRubbish : MonoBehaviour
{
    public float floatSpeed = 0.5f; // Speed of floating movement
    public float floatRange = 0.5f; // Vertical range of floating

    private Vector3 initialPosition;
    private bool collected = false;

    void Start()
    {
        initialPosition = transform.position;
    }

    void Update()
    {
        // Simple floating movement using Sin wave
        if (!collected)
        {
            float newY = Mathf.Sin(Time.time * floatSpeed) * floatRange;
            transform.position = new Vector3(initialPosition.x, initialPosition.y + newY, initialPosition.z);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        // Check if the boat or net collides with the rubbish
        if (other.CompareTag("Boat") || other.CompareTag("Net"))
        {
            CollectRubbish();
        }
    }

    private void CollectRubbish()
    {
        collected = true;
        // Perform collection logic, like destroying or disabling the rubbish object
        Destroy(gameObject);
    }
}
