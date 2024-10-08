using UnityEngine;
using UnityEngine.UI;
using Cinemachine;
using System.Collections;
using TMPro;
using System.Collections.Generic; // Import this for List
using UnityEngine.SceneManagement; // Import for scene management

public class LevelSelectManager : MonoBehaviour
{
    public CanvasGroup infoFadeGroup;
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
    public ScreenTransitionEffect transitionEffect;

    public GameObject blendlistObject;
    public CinemachineVirtualCamera levelSelectCam;
    public GameObject titleCanvas;

    public List<LevelData> levels;

    public int currentLevelIndex = 0;
    private bool isTransitioning = false;

    public Image[] starImages; // Array of star Image components in the UI


    void Start()
    {
        int lastLevelIndex = GameManager.Instance.lastPlayedLevel;

        // Set the dolly cart position based on the last played level
        if (lastLevelIndex != -1)
        {
            titleCanvas.SetActive(false);
            DisableIntroCamAndSwitchToLevelSelect();
            float dollyPosition = dollyPath.FromPathNativeUnits(lastLevelIndex, CinemachinePathBase.PositionUnits.PathUnits);
            dollyCart.m_Position = dollyPosition;
            UpdateInfoPanel(lastLevelIndex);  // Update the info panel to match the last played level
            currentLevelIndex = lastLevelIndex;  // Sync the current index
        }
        else
        {
            // Fallback to default behavior if no level was played
            dollyCart.m_Position = 0f;
            UpdateInfoPanel(0);  // Start with the first level
        }

        levelActionButton.onClick.AddListener(LoadLevel);  // Add event listener for the button
    }

    void DisableIntroCamAndSwitchToLevelSelect()
    {
        // Reparent the LevelSelectCam to the new parent
        if (levelSelectCam != null && dollyCart.transform != null)
        {
            levelSelectCam.transform.SetParent(dollyCart.transform);
            levelSelectCam.Priority = 10; // Set a high priority to make sure it becomes the active camera
            levelSelectCam.enabled = true; // Make sure it's enabled
        }

        // Disable the blend list camera to stop the intro sequence
        if (blendlistObject != null)
        {
            blendlistObject.SetActive(false);
        }
    }


    public void MoveToNextWaypoint()
    {
        if (!isTransitioning)
        {
            int nextWaypoint = (currentLevelIndex + 1) % levels.Count;
            StartCoroutine(ShrinkInfoPanel());
            StartCoroutine(SmoothTransition(nextWaypoint, 1));
        }
    }

    public void MoveToPreviousWaypoint()
    {
        if (!isTransitioning)
        {
            int previousWaypoint = (currentLevelIndex - 1 + levels.Count) % levels.Count;
            StartCoroutine(ShrinkInfoPanel());
            StartCoroutine(SmoothTransition(previousWaypoint, -1));
        }
    }

    private IEnumerator SmoothTransition(int targetLevelIndex, int direction)
    {
        isTransitioning = true;

        // Set the speed to a constant value to start the movement
        dollyCart.m_Speed = 0.8f * direction;

        float targetPosition = dollyPath.FromPathNativeUnits(targetLevelIndex, CinemachinePathBase.PositionUnits.PathUnits);

        // Continue moving the cart until it's within 0.1 units of the target position
        while (Mathf.Abs(dollyCart.m_Position - targetPosition) > 0.1f)
        {
            yield return null; // Wait for the next frame
        }

        // Stop the cart when it reaches the desired position
        dollyCart.m_Speed = 0f;

        // Update the current level index and other UI elements
        currentLevelIndex = targetLevelIndex;
        isTransitioning = false;

        UpdateInfoPanel(targetLevelIndex);
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
        StartCoroutine(ExpandInfoPanel());
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
        if (GameManager.Instance.lastPlayedLevel != -1)
        {
            yield return new WaitUntil(() => transitionEffect.transitionState == 1);
            infoFadeGroup.alpha = 1f;
        }
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
            GameManager gm = FindAnyObjectByType<GameManager>();
            if (gm != null) { gm.SetLastPlayedLevel(currentLevelIndex); }
            // Load the scene using the level index
            //SceneManager.LoadScene(levels[currentLevelIndex].levelIndex); // Make sure this matches your scene index
            GetComponent<SceneManagerScript>().LoadSceneByIndex(levels[currentLevelIndex].levelIndex);


        }
    }
}
