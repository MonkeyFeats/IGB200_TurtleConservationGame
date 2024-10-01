using UnityEngine;
using Cinemachine;
using UnityEngine.UI;

public class CameraSwitcher : MonoBehaviour
{
    public CinemachineVirtualCamera[] cameras; // Array to hold your virtual cameras
    public Button leftButton;  // Button to go to the previous camera
    public Button rightButton; // Button to go to the next camera

    private int currentIndex = 0; // Track which camera is currently active

    void Start()
    {
        // Initialize buttons and listeners
        leftButton.onClick.AddListener(SwitchLeft);
        rightButton.onClick.AddListener(SwitchRight);

        // Enable only the first camera
        SetActiveCamera(currentIndex);
    }

    // Switch to the previous camera in the array
    void SwitchLeft()
    {
        currentIndex--;
        if (currentIndex < 0)
            currentIndex = cameras.Length - 1; // Loop back to the last camera

        SetActiveCamera(currentIndex);
    }

    // Switch to the next camera in the array
    void SwitchRight()
    {
        currentIndex++;
        if (currentIndex >= cameras.Length)
            currentIndex = 0; // Loop back to the first camera

        SetActiveCamera(currentIndex);
    }

    // Enable the active camera and disable all others
    void SetActiveCamera(int index)
    {
        for (int i = 0; i < cameras.Length; i++)
        {
            cameras[i].Priority = (i == index) ? 10 : 0; // Active camera gets higher priority
        }
    }
}
