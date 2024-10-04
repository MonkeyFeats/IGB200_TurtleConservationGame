using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomObstacleSpawner : MonoBehaviour
{
    public GameObject[] obstaclePrefabs; // Array of obstacle prefabs
    public Transform player; // Reference to the player's position
    public float spawnDistance = 50f; // Distance ahead of the player where obstacles are spawned
    public float spawnInterval = 3f; // Time interval between spawns
    public float spawnHeightOffset = 0.4f; // Additional height offset for obstacles

    private float nextSpawnTime;
    private int lastPrefabIndex = -1; // Index of the last spawned prefab

    void Start()
    {
        nextSpawnTime = Time.time + spawnInterval;
    }

    void Update()
    {
        if (Time.time >= nextSpawnTime)
        {
            SpawnObstacle();
            nextSpawnTime = Time.time + spawnInterval; // Schedule the next spawn
        }
    }

    void SpawnObstacle()
    {
        int prefabIndex;

        // Ensure the new prefab index is different from the last one
        do
        {
            prefabIndex = Random.Range(0, obstaclePrefabs.Length);
        } while (prefabIndex == lastPrefabIndex);

        // Update the lastPrefabIndex to the current one
        lastPrefabIndex = prefabIndex;

        // Get the selected prefab
        GameObject obstaclePrefab = obstaclePrefabs[prefabIndex];

        // Get the X position from the prefab's original position
        float prefabX = obstaclePrefab.transform.position.x;

        // Set the spawn position, preserving the prefab's X position and applying a height offset
        Vector3 spawnPosition = new Vector3(prefabX, obstaclePrefab.transform.position.y + spawnHeightOffset, player.position.z + spawnDistance);

        // Instantiate the obstacle at the calculated position with its original rotation
        Instantiate(obstaclePrefab, spawnPosition, obstaclePrefab.transform.rotation);
    }
}
