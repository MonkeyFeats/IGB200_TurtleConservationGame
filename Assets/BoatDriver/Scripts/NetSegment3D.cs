using UnityEngine;

public class NetSegment3D : MonoBehaviour
{
    public Rigidbody connectedSegment; // Next segment in the chain (or the boat)
    public float waterDrag = 2f;

    private void Start()
    {
        HingeJoint hinge = GetComponent<HingeJoint>();

        // Connect this segment to the next one in the chain or the boat
        if (connectedSegment != null)
        {
            hinge.connectedBody = connectedSegment;
            // Adjust the connected anchor position to simulate a chain
            hinge.anchor = Vector3.zero;
            hinge.autoConfigureConnectedAnchor = false;
            hinge.connectedAnchor = new Vector3(0, 0, -0.5f); // This depends on the size of the segments
        }

        // Apply constraints (Freeze only Y position and XZ rotation for 3D chain-like movement)
        Rigidbody rb = GetComponent<Rigidbody>();
        rb.constraints = RigidbodyConstraints.FreezePositionY | RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;
    }

    void FixedUpdate()
    {
        // Apply custom water drag for realistic water resistance
        Rigidbody rb = GetComponent<Rigidbody>();
        rb.velocity *= (1f - Time.fixedDeltaTime * waterDrag);
    }
}
