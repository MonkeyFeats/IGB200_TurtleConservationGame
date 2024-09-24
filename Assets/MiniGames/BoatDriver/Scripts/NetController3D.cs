using System.Collections.Generic;
using UnityEngine;

public class NetController3D : MonoBehaviour
{
    public GameObject netSegmentPrefab; // Prefab for each net segment
    public int segmentCount = 10; // Number of segments
    public Rigidbody boat; // Boat's Rigidbody

    private List<GameObject> netSegments = new List<GameObject>();

    void Start()
    {
        Vector3 spawnPosition = boat.position;

        // Create the first segment and attach it to the boat
        GameObject firstSegment = Instantiate(netSegmentPrefab, spawnPosition, Quaternion.identity);
        HingeJoint firstHinge = firstSegment.GetComponent<HingeJoint>();
        firstHinge.connectedBody = boat; // First segment is attached to the boat
        netSegments.Add(firstSegment);

        // Instantiate the remaining segments and attach them to the previous one
        for (int i = 1; i < segmentCount; i++)
        {
            GameObject segment = Instantiate(netSegmentPrefab, spawnPosition - new Vector3(0, 0, i * 1f), Quaternion.identity); // Adjust z-offset for proper spacing
            HingeJoint hinge = segment.GetComponent<HingeJoint>();
            hinge.connectedBody = netSegments[i - 1].GetComponent<Rigidbody>(); // Connect to the previous segment
            netSegments.Add(segment);
        }
    }
}
