using UnityEngine;
using Cinemachine;

public class HiddenObjectCameraMover : MonoBehaviour
{
    public CinemachineDollyCart cameraCart;      // The Cinemachine cart for the camera
    public CinemachineDollyCart aimCart;         // The Cinemachine cart for the aim target
    public CinemachineVirtualCamera virtualCamera; // The Cinemachine virtual camera for adjusting FOV
    public float speed = 0.5f;                   // Speed of movement along the track
    public float startPosition = 0f;             // Starting position percentage along the track (0 to 1)
    public float minFOV = 30f;                   // Minimum field of view (zoom in)
    public float maxFOV = 60f;                   // Maximum field of view (zoom out)
    public float zoomSpeed = 10f;                // Speed of zooming
    private float trackPosition = 0f;            // Current position percentage (0 to 1)

    void Start()
    {
        // Initialize both carts to the start position
        trackPosition = Mathf.Clamp01(startPosition);
        cameraCart.m_Position = trackPosition * cameraCart.m_Path.PathLength;
        aimCart.m_Position = trackPosition * aimCart.m_Path.PathLength;
    }

    void Update()
    {
        HandleMovementInput();
        HandleZoomInput();
    }

    // Handle movement of the carts along the track based on player input
    private void HandleMovementInput()
    {
        float input = 0f;

        // Handle keyboard input
        input = Input.GetAxis("Horizontal")*0.25f;

        // Handle touch swipe input
        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);
            if (touch.phase == TouchPhase.Moved)
            {
                input = -touch.deltaPosition.x / Screen.width; // Normalize to screen width
            }
        }

        // Update track position based on input and speed
        trackPosition = Mathf.Clamp01(trackPosition + input * speed * Time.deltaTime);

        // Move both the camera and aim carts along their respective tracks
        cameraCart.m_Position = trackPosition * cameraCart.m_Path.PathLength;
        aimCart.m_Position = trackPosition * aimCart.m_Path.PathLength;
    }

    // Handle pinch-to-zoom and scroll wheel zoom for adjusting the camera's field of view
    private void HandleZoomInput()
    {
        float zoomChange = 0f;

        // Handle mouse scroll wheel zoom input
        zoomChange -= Input.GetAxis("Mouse ScrollWheel") * zoomSpeed;

        // Handle pinch-to-zoom on mobile
        if (Input.touchCount == 2)
        {
            Touch touch0 = Input.GetTouch(0);
            Touch touch1 = Input.GetTouch(1);

            // Find the position in the previous frame of each touch
            Vector2 touch0PrevPos = touch0.position - touch0.deltaPosition;
            Vector2 touch1PrevPos = touch1.position - touch1.deltaPosition;

            // Calculate the distance between the touches in the current and previous frames
            float prevTouchDeltaMag = (touch0PrevPos - touch1PrevPos).magnitude;
            float touchDeltaMag = (touch0.position - touch1.position).magnitude;

            // Find the difference in the distances between frames
            zoomChange = (prevTouchDeltaMag - touchDeltaMag) * zoomSpeed * 0.01f;
        }

        // Adjust the FOV of the virtual camera, clamped between minFOV and maxFOV
        virtualCamera.m_Lens.FieldOfView = Mathf.Clamp(virtualCamera.m_Lens.FieldOfView + zoomChange, minFOV, maxFOV);
    }
}
