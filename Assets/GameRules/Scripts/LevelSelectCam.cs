using UnityEngine;
using Cinemachine;
using System.Collections;

public class DollyTrackController : MonoBehaviour
{
    public CinemachineDollyCart dollyCart; // The dolly cart that moves the camera
    public CinemachineSmoothPath dollyPath; // The smooth path the dolly follows
    public float transitionDuration = 2.0f; // Duration for the smooth transition

    private int currentWaypoint = 0;
    private int totalWaypoints;
    private bool isTransitioning = false; // To prevent multiple transitions at the same time

    private void Start()
    {
        // Initialize the total number of waypoints
        totalWaypoints = dollyPath.m_Waypoints.Length;

        // Place the cart at the first waypoint initially
        dollyCart.m_Position = 0f;
    }

    public void MoveToNextWaypoint()
    {
        if (!isTransitioning)
        {
            int nextWaypoint = (currentWaypoint + 1) % totalWaypoints; // Loop to first if last is reached
            StartCoroutine(SmoothTransition(nextWaypoint));
        }
    }

    public void MoveToPreviousWaypoint()
    {
        if (!isTransitioning)
        {
            int previousWaypoint = (currentWaypoint - 1 + totalWaypoints) % totalWaypoints; // Loop to last if first is reached
            StartCoroutine(SmoothTransition(previousWaypoint));
        }
    }

    private IEnumerator SmoothTransition(int targetWaypoint)
    {
        isTransitioning = true;

        // Calculate the start and end positions on the track
        float startPosition = dollyCart.m_Position;
        float endPosition = dollyPath.FromPathNativeUnits(targetWaypoint, CinemachinePathBase.PositionUnits.PathUnits);

        // Time to interpolate
        float elapsedTime = 0f;

        while (elapsedTime < transitionDuration)
        {
            // Interpolate the position
            dollyCart.m_Position = Mathf.Lerp(startPosition, endPosition, elapsedTime / transitionDuration);

            // Update time
            elapsedTime += Time.deltaTime;

            yield return null;
        }

        // Ensure the final position is set exactly
        dollyCart.m_Position = endPosition;

        // Update the current waypoint index
        currentWaypoint = targetWaypoint;

        isTransitioning = false;
    }
}
