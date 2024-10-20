using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
using UnityEngine.Events;

public class HiddenObjectGameManager : MonoBehaviour
{
    // UI References
    public GameObject starPopup;
    public Animator starsAnimator;
    public ScreenTransitionEffect transitioner;

    public UnityEvent OnEndGameEvent;

    [SerializeField] private List<GameObject> trashPrefabs; // For trash items
    [SerializeField] private List<GameObject> jellyfishPrefabs; // For jellyfish
    [SerializeField] private List<GameObject> debrisPrefabs; // For debris
    [SerializeField] private List<Transform> spawnLocations;
    [SerializeField] private List<Transform> JellyLocations;
    [SerializeField] private List<Transform> DebrisLocations;
    [SerializeField] private int numberOfObjectsToSpawn;
    [SerializeField] private TextMeshProUGUI remainingItemsText;
    [SerializeField] private TextMeshProUGUI timerText;
    [SerializeField] private List<UiObjectSlot> objectSlots;
    [SerializeField] private float gameTime = 120f;

    private enum GameState { Intermission, Playing, EndGame }
    private GameState currentState;

    private List<GameObject> activeObjects = new List<GameObject>();
    private Dictionary<string, int> objectCounters = new Dictionary<string, int>();
    private float currentTime;
    private int totalObjects;
    private int objectsFound;

    void Start()
    {
        StartIntermission();
    }

    void Update()
    {
        if (currentState == GameState.Playing)
        {
            currentTime -= Time.deltaTime;
            if (currentTime <= 0)
            {
                currentTime = 0;
                EndGame(false); // Lose condition
            }

            UpdateTimerUI();
        }
    }

    public void StartIntermission()
    {
        currentState = GameState.Intermission;
        // Optional: Disable player controls during intermission if necessary
    }

    public void EndIntermission()
    {
        currentState = GameState.Playing;
        objectsFound = 0;
        currentTime = gameTime;

        SpawnObjects();
        totalObjects = activeObjects.Count;
        InitializeObjectCounters();
        UpdateUI();
    }

    public void EndGame(bool won)
    {
        currentState = GameState.EndGame;
        remainingItemsText.gameObject.SetActive(false);
        OnEndGameEvent?.Invoke();
    }

    public void GiveStarRating()
    {
        int starsEarned = CalculateStarReward();
        starPopup.SetActive(true);
        if (starsAnimator != null)
            starsAnimator.SetInteger("StarsEarned", starsEarned);

        remainingItemsText.gameObject.SetActive(false);
    }

    int CalculateStarReward()
    {
        float percentageFound = (float)objectsFound / (totalObjects - 17);
        float timePercentage = currentTime / gameTime;

        // Full completion bonus (3 stars) if all objects are found and time is still left
        if (percentageFound == 1f && timePercentage > 0)
            return 3;

        // 2 stars for completing 60% or more of objects
        if (percentageFound >= 0.6f)
            return 2;

        // 1 star for completing 30-65%
        if (percentageFound >= 0.3f)
            return 1;

        // 0 stars for less than 30% found
        return 0;
    }

    public void LoadScene(int sceneIndex)
    {
        SceneManager.LoadScene(sceneIndex);
    }

    void SpawnObjects()
    {
        List<Transform> shuffledLocations = new List<Transform>(spawnLocations);
        ShuffleList(shuffledLocations);

        // Spawn trash objects
        for (int i = 2; i < 14; i++)
        {
            GameObject randomTrash = trashPrefabs[Random.Range(0, trashPrefabs.Count)]; // Trash items
            Transform spawnPoint = shuffledLocations[i];
            GameObject spawnedTrash = Instantiate(randomTrash, spawnPoint.position, spawnPoint.rotation);
            activeObjects.Add(spawnedTrash);
        }

        // Spawn jellyfish
        List<Transform> shuffledJellyLocations = new List<Transform>(JellyLocations);
        ShuffleList(shuffledJellyLocations);
        for (int i = 0; i < 3; i++)
        {
            GameObject randomJellyfish = jellyfishPrefabs[0]; // Assuming there's only one jellyfish prefab
            Transform spawnPoint = shuffledJellyLocations[i];
            GameObject spawnedJellyfish = Instantiate(randomJellyfish, spawnPoint.position, spawnPoint.rotation);
            activeObjects.Add(spawnedJellyfish);
        }

        // Spawn debris without tracking them in the object counters
        List<Transform> shuffledDebrisLocations = new List<Transform>(DebrisLocations);
        ShuffleList(shuffledDebrisLocations);
        for (int i = 0; i < 14; i++)
        {
            GameObject randomDebris = debrisPrefabs[Random.Range(0, debrisPrefabs.Count)]; // Debris
            Transform spawnPoint = shuffledDebrisLocations[i];
            GameObject spawnedDebris = Instantiate(randomDebris, spawnPoint.position, spawnPoint.rotation);
            // No need to track debris in activeObjects
        }
    }

    public void FindObject(GameObject foundObject, string objectName)
    {
        foundObject.SetActive(false); // Disable the object
        objectsFound++; // Increment found objects count
        activeObjects.Remove(foundObject); // Remove from active objects

        objectCounters[objectName]--; // Decrease object counter

        if (objectCounters[objectName] == 0)
        {
            RemoveObjectFromUI(objectName); // Remove from UI if no more objects
        }
        else
        {
            UpdateObjectSlotUI(objectName); // Update UI
        }

        UpdateUI();

        if (objectsFound == (totalObjects - 17))
        {
            EndGame(true); // Win condition when all objects are found
        }
    }

    void RemoveObjectFromUI(string objectName)
    {
        for (int i = 0; i < trashPrefabs.Count; i++)
        {
            if (trashPrefabs[i].name == objectName)
            {
                objectSlots[i].gameObject.SetActive(false); // Remove slot from UI
                break;
            }
        }

        for (int i = 0; i < jellyfishPrefabs.Count; i++)
        {
            if (jellyfishPrefabs[i].name == objectName)
            {
                objectSlots[trashPrefabs.Count + i].gameObject.SetActive(false); // Remove slot from UI
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
        // Initialize counters for trash items
        foreach (GameObject obj in trashPrefabs)
        {
            string objectName = obj.name;
            objectCounters[objectName] = 0;
        }

        // Initialize counters for jellyfish
        foreach (GameObject obj in jellyfishPrefabs)
        {
            string objectName = obj.name;
            objectCounters[objectName] = 0;
        }

        foreach (GameObject obj in activeObjects)
        {
            string objectName = obj.name.Replace("(Clone)", "").Trim();
            objectCounters[objectName]++;
        }

        // Set UI slots for trash and jellyfish
        for (int i = 0; i < objectSlots.Count; i++)
        {
            if (i < trashPrefabs.Count)
            {
                string objectName = trashPrefabs[i].name;
                objectSlots[i].SetSlot(trashPrefabs[i].GetComponent<SpriteRenderer>().sprite, objectCounters[objectName]);
            }
            else if (i < trashPrefabs.Count + jellyfishPrefabs.Count)
            {
                string objectName = jellyfishPrefabs[i - trashPrefabs.Count].name;
                objectSlots[i].SetSlot(jellyfishPrefabs[i - trashPrefabs.Count].GetComponent<SpriteRenderer>().sprite, objectCounters[objectName]);
            }
            else
            {
                objectSlots[i].gameObject.SetActive(false);
            }
        }
    }

    void UpdateObjectSlotUI(string objectName)
    {
        for (int i = 0; i < jellyfishPrefabs.Count; i++)
        {
            if (jellyfishPrefabs[i].name == objectName)
            {
                objectSlots[trashPrefabs.Count + i].UpdateCounter(objectCounters[objectName]);
                break;
            }
        }

        for (int i = 0; i < trashPrefabs.Count; i++)
        {
            if (trashPrefabs[i].name == objectName)
            {
                objectSlots[i].UpdateCounter(objectCounters[objectName]);
                break;
            }
        }        
    }

    void ShuffleList<T>(List<T> list)
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
