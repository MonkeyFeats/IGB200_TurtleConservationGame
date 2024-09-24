using UnityEngine;

public class DynamicRopeController : MonoBehaviour
{
    public Transform boat;  // Reference to the boat
    public Transform anchor;  // Anchor point where the rope attaches
    public GameObject ropeSegmentPrefab;  // Prefab for the rope segments
    public int segmentCount = 10;  // Number of segments in the rope
    public float segmentLength = 1f;  // Length of each segment
    public float ropeForce = 10f;  // Force applied to simulate rope tension

    private Rigidbody boatRb;
    private Rigidbody[] ropeSegments;

    void Start()
    {
        boatRb = boat.GetComponent<Rigidbody>();
        CreateRope();
    }

    void Update()
    {
        UpdateRope();
    }

    void CreateRope()
    {
        ropeSegments = new Rigidbody[segmentCount];

        // Create rope segments and set up physics
        for (int i = 0; i < segmentCount; i++)
        {
            GameObject segment = Instantiate(ropeSegmentPrefab, anchor.position - (i * segmentLength * Vector3.up), Quaternion.identity);
            Rigidbody rb = segment.GetComponent<Rigidbody>();
            rb.mass = 0.1f;  // Adjust mass for realistic simulation
            if (i > 0)
            {
                // Connect segments with spring joints
                SpringJoint sj = segment.AddComponent<SpringJoint>();
                sj.connectedBody = ropeSegments[i - 1];
                sj.spring = ropeForce;
                sj.damper = 1f;
                sj.maxDistance = segmentLength;
            }
            ropeSegments[i] = rb;
        }

        // Connect the first segment to the anchor
        SpringJoint firstSegmentJoint = ropeSegments[0].gameObject.AddComponent<SpringJoint>();
        firstSegmentJoint.connectedBody = anchor.GetComponent<Rigidbody>();
        firstSegmentJoint.spring = ropeForce;
        firstSegmentJoint.damper = 1f;
        firstSegmentJoint.maxDistance = segmentLength;
    }

    void UpdateRope()
    {
        // Update the position of the last rope segment to follow the boat
        if (ropeSegments.Length > 0)
        {
            ropeSegments[ropeSegments.Length - 1].position = boat.position - (segmentLength * Vector3.up);
        }
    }
}
