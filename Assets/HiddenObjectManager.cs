using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class HiddenObjectGame : MonoBehaviour
{
    [SerializeField] private List<GameObject> objectPrefabs;
    [SerializeField] private List<Transform> spawnLocations;
    [SerializeField] private int numberOfObjectsToSpawn;

    [SerializeField] private TextMeshProUGUI remainingItemsText;
    [SerializeField] private TextMeshProUGUI winText;
    [SerializeField] private TextMeshProUGUI timerText;

    [SerializeField] private List<UiObjectSlot> objectSlots;

    [SerializeField] private float gameTime = 120f;

    private List<GameObject> activeObjects = new List<GameObject>();
    private Dictionary<string, int> objectCounters = new Dictionary<string, int>();
    private float currentTime;
    private int totalObjects;
    private int objectsFound;
    private bool gameOver;

    void Start()
    {
        winText.gameObject.SetActive(false);
        objectsFound = 0;
        currentTime = gameTime;
        gameOver = false;

        SpawnObjects();
        totalObjects = activeObjects.Count;
        InitializeObjectCounters();
        UpdateUI();
    }

    void Update()
    {
        if (gameOver) return;

        currentTime -= Time.deltaTime;
        if (currentTime <= 0)
        {
            currentTime = 0;
            EndGame(false);
        }

        UpdateTimerUI();

    }

    void SpawnObjects()
    {
        List<Transform> shuffledLocations = new List<Transform>(spawnLocations);
        ShuffleList(shuffledLocations);

        for (int i = 0; i < numberOfObjectsToSpawn; i++)
        {
            GameObject randomObject = objectPrefabs[Random.Range(0, objectPrefabs.Count)];
            Transform spawnPoint = shuffledLocations[i];
            GameObject spawnedObject = Instantiate(randomObject, spawnPoint.position, spawnPoint.rotation);
            activeObjects.Add(spawnedObject);
        }
    }


    public void FindObject(GameObject foundObject, string objectName)
    {
        foundObject.SetActive(false);  // Disable the object
        objectsFound++;                // Increment the found objects count
        activeObjects.Remove(foundObject); // Remove from active objects list

        objectCounters[objectName]--;   // Decrease object counter

        if (objectCounters[objectName] == 0)
        {
            RemoveObjectFromUI(objectName); // Remove from UI if no more of that object type
        }
        else
        {
            UpdateObjectSlotUI(objectName); // Update counter in UI
        }

        UpdateUI();

        if (objectsFound == totalObjects)
        {
            EndGame(true); // End game when all objects are found
        }
    }

    void RemoveObjectFromUI(string objectName)
    {
        for (int i = 0; i < objectPrefabs.Count; i++)
        {
            if (objectPrefabs[i].name == objectName)
            {
                objectSlots[i].gameObject.SetActive(false); // Remove the slot from UI
                break;
            }
        }
    }

    void UpdateUI()
    {
        remainingItemsText.text = "Remaining Items: " + (totalObjects - objectsFound).ToString();
    }

    void UpdateTimerUI()
    {
        timerText.text = "Time Left: " + Mathf.Ceil(currentTime).ToString() + "s";
    }

    void InitializeObjectCounters()
    {
        foreach (GameObject obj in objectPrefabs)
        {
            string objectName = obj.name;
            objectCounters[objectName] = 0;
        }

        foreach (GameObject obj in activeObjects)
        {
            string objectName = obj.name.Replace("(Clone)", "").Trim();
            objectCounters[objectName]++;
        }

        for (int i = 0; i < objectSlots.Count; i++)
        {
            if (i < objectPrefabs.Count)
            {
                string objectName = objectPrefabs[i].name;
                objectSlots[i].SetSlot(objectPrefabs[i].GetComponent<SpriteRenderer>().sprite, objectCounters[objectName]);
            }
            else
            {
                objectSlots[i].gameObject.SetActive(false);
            }
        }
    }

    void UpdateObjectSlotUI(string objectName)
    {
        for (int i = 0; i < objectPrefabs.Count; i++)
        {
            if (objectPrefabs[i].name == objectName)
            {
                objectSlots[i].UpdateCounter(objectCounters[objectName]);
                break;
            }
        }
    }

    void EndGame(bool won)
    {
        gameOver = true;
        if (won)
        {
            winText.gameObject.SetActive(true);
        }
        remainingItemsText.gameObject.SetActive(false);
    }

    private void ShuffleList<T>(List<T> list)
    {
        for (int i = list.Count - 1; i > 0; i--)
        {
            int randomIndex = Random.Range(0, i + 1);
            T temp = list[i];
            list[i] = list[randomIndex];
            list[randomIndex] = temp;
        }
    }
}
