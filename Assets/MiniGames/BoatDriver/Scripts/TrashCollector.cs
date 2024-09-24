using UnityEngine;

public class TrashCollector : MonoBehaviour
{
    public BoatCleanupLevelManager manager; // Reference to the other script
    public float overlapRadius = 1.0f; // The radius of the overlap sphere

    void Update()
    {
        CheckForTrash();
    }

    void CheckForTrash()
    {
        // Create an array of colliders within the overlap sphere
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, overlapRadius);

        // Loop through all the colliders and check if any have the "Trash" tag
        foreach (Collider hitCollider in hitColliders)
        {
            if (hitCollider.CompareTag("Trash"))
            {
                // Call the function in the other script if "Trash" is detected
                manager.CollectRubbish(hitCollider.gameObject);
                break; // Exit the loop once we find the first "Trash" object
            }
        }
    }

    // Optional: Visualize the overlap sphere in the editor
    void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, overlapRadius);
    }
}
