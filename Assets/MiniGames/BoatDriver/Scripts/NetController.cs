using UnityEngine;

public class NetClothController : MonoBehaviour
{
    public Transform leftAnchor;  // Left side of the boat
    public Transform rightAnchor; // Right side of the boat

    private Cloth cloth;

    // Indices of the corner vertices you want to pin to the boat
    public int leftCornerVertexIndex;
    public int rightCornerVertexIndex;

    // Arrays for cloth constraints
    private ClothSphereColliderPair[] sphereColliders;
    private Vector3[] worldPosConstraints;
    private bool[] pinVertices;

    void Start()
    {
        // Get the Cloth component
        cloth = GetComponent<Cloth>();

        // Set up arrays for vertex constraints
        worldPosConstraints = new Vector3[cloth.vertices.Length];
        pinVertices = new bool[cloth.vertices.Length];

        // Pin the left and right corner vertices
        pinVertices[leftCornerVertexIndex] = true;
        pinVertices[rightCornerVertexIndex] = true;
    }

    void FixedUpdate()
    {
        // Apply the current positions of the anchors to the corresponding cloth vertices
        worldPosConstraints[leftCornerVertexIndex] = leftAnchor.position;
        worldPosConstraints[rightCornerVertexIndex] = rightAnchor.position;

        // Update cloth constraints for the pinned vertices
        UpdateClothConstraints();
    }

    void UpdateClothConstraints()
    {
        for (int i = 0; i < cloth.vertices.Length; i++)
        {
            if (pinVertices[i])
            {
                // Create a sphere collider to constrain the vertex's movement
                sphereColliders = cloth.sphereColliders;

                // Apply the constraint on this vertex by creating a small collider that locks its position
                // (We use this method because Unity's cloth physics doesn't allow us to directly modify vertices)
                sphereColliders[i].first.transform.position = worldPosConstraints[i];
            }
        }

        // Set the updated constraints back to the cloth
        cloth.sphereColliders = sphereColliders;
    }
}
