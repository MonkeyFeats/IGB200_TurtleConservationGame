using UnityEngine;
using UnityEngine.UI;
using Cinemachine;
using System.Collections;
using TMPro;
using System.Collections.Generic; // Import this for List
using UnityEngine.SceneManagement; // Import for scene management

public class LevelSelectManager : MonoBehaviour
{
    public GameObject infoPanel;
    public GameObject starsGroup;
    public TextMeshProUGUI levelTitle;
    public TextMeshProUGUI levelDescription;
    public Image levelImage;
    public CinemachineDollyCart dollyCart;
    public CinemachineSmoothPath dollyPath;
    public Button levelActionButton;
    public float panelExpandDuration = 0.5f;
    public float transitionDuration = 1.0f;

    public List<LevelData> levels;

    public int currentLevelIndex = 0;
    private bool isTransitioning = false;

    public Image[] starImages; // Array of star Image components in the UI

    void Start()
    {
        dollyCart.m_Position = 0f;
        UpdateInfoPanel(0); // Update info panel to the first level on start
        levelActionButton.onClick.AddListener(LoadLevel); // Add event listener for the button
    }

    public void MoveToNextWaypoint()
    {
        if (!isTransitioning)
        {
            int nextWaypoint = (currentLevelIndex + 1) % levels.Count;
            StartCoroutine(ShrinkInfoPanel());
            StartCoroutine(SmoothTransition(nextWaypoint));
        }
    }

    public void MoveToPreviousWaypoint()
    {
        if (!isTransitioning)
        {
            int previousWaypoint = (currentLevelIndex - 1 + levels.Count) % levels.Count;
            StartCoroutine(ShrinkInfoPanel());
            StartCoroutine(SmoothTransition(previousWaypoint));
        }
    }

    private IEnumerator SmoothTransition(int targetLevelIndex)
    {
        isTransitioning = true;

        float startPosition = dollyCart.m_Position;
        float endPosition = dollyPath.FromPathNativeUnits(targetLevelIndex, CinemachinePathBase.PositionUnits.PathUnits);
        float elapsedTime = 0f;

        while (elapsedTime < transitionDuration)
        {
            dollyCart.m_Position = Mathf.Lerp(startPosition, endPosition, elapsedTime / transitionDuration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        dollyCart.m_Position = endPosition;
        currentLevelIndex = targetLevelIndex;
        isTransitioning = false;

        UpdateInfoPanel(targetLevelIndex);
        StartCoroutine(ExpandInfoPanel());
    }

    void UpdateInfoPanel(int levelIndex = 0)
    {
        LevelData selectedLevel = levels[levelIndex];
        levelTitle.text = selectedLevel.levelName;
        levelDescription.text = selectedLevel.levelDescription;

        if (levelImage != null && selectedLevel.levelImage != null)
        {
            levelImage.sprite = selectedLevel.levelImage;
        }

        // Update star UI based on stars earned for the current level
        UpdateStarDisplay(selectedLevel.starsEarned, selectedLevel.isDialogueScene);

        levelActionButton.onClick.RemoveAllListeners(); // Clear previous listeners
        levelActionButton.onClick.AddListener(LoadLevel); // Add new listener for loading the scene
    }

    void UpdateStarDisplay(int starsEarned, bool isDialogue)
    {
        starsGroup.SetActive(!isDialogue);
        for (int i = 0; i < starImages.Length; i++)
        {
            starImages[i].gameObject.SetActive(i < starsEarned); // Show/hide stars based on earned count
        }
    }

    IEnumerator ExpandInfoPanel()
    {
        infoPanel.SetActive(true);
        Vector3 startScale = Vector3.zero;
        Vector3 endScale = Vector3.one;
        float elapsedTime = 0f;

        while (elapsedTime < panelExpandDuration)
        {
            infoPanel.transform.localScale = Vector3.Lerp(startScale, endScale, elapsedTime / panelExpandDuration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        infoPanel.transform.localScale = endScale;
    }

    IEnumerator ShrinkInfoPanel()
    {
        Vector3 startScale = Vector3.one;
        Vector3 endScale = Vector3.zero;
        float elapsedTime = 0f;

        while (elapsedTime < panelExpandDuration)
        {
            infoPanel.transform.localScale = Vector3.Lerp(startScale, endScale, elapsedTime / panelExpandDuration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        infoPanel.transform.localScale = endScale;
        infoPanel.SetActive(false);
    }

    public void HideInfoPanel()
    {
        StartCoroutine(ShrinkInfoPanel());
    }

    // Method to load the scene corresponding to the selected level index
    void LoadLevel()
    {
        // Ensure the level index is within range
        if (currentLevelIndex >= 0 && currentLevelIndex < levels.Count)
        {
            // Load the scene using the level index
            //SceneManager.LoadScene(levels[currentLevelIndex].levelIndex); // Make sure this matches your scene index
            GetComponent<SceneManagerScript>().LoadSceneByIndex(levels[currentLevelIndex].levelIndex);

        }
    }
}
