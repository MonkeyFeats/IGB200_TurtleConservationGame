using UnityEngine;
using UnityEngine.UI;
using Cinemachine;
using System.Collections;
using TMPro;

public class LevelSelectManager : MonoBehaviour
{
    public GameObject infoPanel;        // The panel displaying level information
    public TextMeshProUGUI levelTitle;             // Text component to display level title
    public TextMeshProUGUI levelDescription;       // Text component to display level description
    public Image levelImage;            // Image component to display level image (optional)
    public CinemachineDollyCart dollyCart; // The dolly cart controlling the camera
    public CinemachineSmoothPath dollyPath; // The smooth path the dolly follows
    public float panelExpandDuration = 0.5f; // Duration to expand/shrink the info panel
    public float transitionDuration = 1.0f;  // Duration for the smooth camera transition from one level to next

    public LevelSelectData[] levels;          // Array to hold data for all levels

    public int currentLevelIndex = 0;
    private bool isTransitioning = false; // To prevent multiple transitions at the same time

    void Start()
    {
        //infoPanel.SetActive(false); // Hide info panel initially

        // Set the initial dolly position and panel info
        dollyCart.m_Position = 0f;
        UpdateInfoPanel(0); // Optional: Update info panel to first level on start
    }

    public void MoveToNextWaypoint()
    {
        if (!isTransitioning)
        {
            int nextWaypoint = (currentLevelIndex + 1) % levels.Length; // Loop to first if last is reached
            StartCoroutine(ShrinkInfoPanel());
            StartCoroutine(SmoothTransition(nextWaypoint));
        }
    }

    public void MoveToPreviousWaypoint()
    {
        if (!isTransitioning)
        {
            int previousWaypoint = (currentLevelIndex - 1 + levels.Length) % levels.Length; // Loop to last if first is reached
            StartCoroutine(ShrinkInfoPanel());
            StartCoroutine(SmoothTransition(previousWaypoint));
        }
    }
    private IEnumerator SmoothTransition(int targetLevelIndex)
    {
        isTransitioning = true;

        // Calculate the start and end positions on the track
        float startPosition = dollyCart.m_Position;
        float endPosition = dollyPath.FromPathNativeUnits(targetLevelIndex, CinemachinePathBase.PositionUnits.PathUnits);
        float elapsedTime = 0f;

        while (elapsedTime < transitionDuration)
        {
            dollyCart.m_Position = Mathf.Lerp(startPosition, endPosition, elapsedTime / transitionDuration);
            elapsedTime += Time.deltaTime;

            yield return null;
        }

        // Ensure the final position is set exactly
        dollyCart.m_Position = endPosition;
        currentLevelIndex = targetLevelIndex;
        isTransitioning = false;

        // Update the level information and show the info panel
        UpdateInfoPanel(targetLevelIndex);
        StartCoroutine(ExpandInfoPanel());
    }

    void UpdateInfoPanel(int levelIndex = 1)
    {
        LevelSelectData selectedLevel = levels[levelIndex];
        Debug.Log(levelIndex);
        Debug.Log(selectedLevel.levelName);
        levelTitle.text = selectedLevel.levelName;
        levelDescription.text = selectedLevel.levelDescription;

        if (levelImage != null && selectedLevel.levelImage != null)
        {
            levelImage.sprite = selectedLevel.levelImage;
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

        infoPanel.transform.localScale = endScale; // Ensure final scale is exact
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

        infoPanel.transform.localScale = endScale; // Ensure final scale is exact
        infoPanel.SetActive(false);
    }

    public void HideInfoPanel()
    {
        StartCoroutine(ShrinkInfoPanel());
    }
}
