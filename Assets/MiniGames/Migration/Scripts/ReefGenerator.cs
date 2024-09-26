using System.Collections.Generic;
using UnityEngine;

public class ReefGenerator : MonoBehaviour
{
    public GameObject terrainPrefab;
    public Transform player;
    public float terrainLength = 100f; // Length of each segment
    public int maxSegments = 5; // Keep a limited number of segments active
    public float generationDistance = 50f; // Distance from player to generate next segment

    private Queue<GameObject> terrainSegments = new Queue<GameObject>();
    private Vector3 nextSpawnPosition;

    void Start()
    {
        nextSpawnPosition = Vector3.zero;
        for (int i = 0; i < maxSegments; i++)
        {
            GenerateNextSegment();
        }
    }

    void Update()
    {
        if (Vector3.Distance(player.position, nextSpawnPosition) < generationDistance)
        {
            GenerateNextSegment();
        }
    }

    void GenerateNextSegment()
    {
        GameObject newSegment = Instantiate(terrainPrefab, nextSpawnPosition, Quaternion.identity);
        nextSpawnPosition += Vector3.forward * terrainLength;
        terrainSegments.Enqueue(newSegment);

        if (terrainSegments.Count > maxSegments)
        {
            Destroy(terrainSegments.Dequeue());
        }
    }
}
