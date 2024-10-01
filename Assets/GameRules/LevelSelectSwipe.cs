using UnityEngine;

public class LevelSelectSwipe : MonoBehaviour
{
    public float swipeThreshold = 50f;  // Minimum distance (in pixels) for a swipe to be registered
    private Vector2 startTouchPosition;  // Start position of the touch
    private Vector2 endTouchPosition;    // End position of the touch

    // These are your existing functions
    public void MoveLeft()
    {
        Debug.Log("Moved Left");
        // Implement your logic to move left in the level select screen here
    }

    public void MoveRight()
    {
        Debug.Log("Moved Right");
        // Implement your logic to move right in the level select screen here
    }

    void Update()
    {
        // Detect a single touch on the screen
        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);

            if (touch.phase == TouchPhase.Began)
            {
                // Store the starting touch position when the touch begins
                startTouchPosition = touch.position;
            }
            else if (touch.phase == TouchPhase.Ended)
            {
                // Store the ending touch position when the touch ends
                endTouchPosition = touch.position;

                // Calculate the swipe distance on the x-axis
                float swipeDistanceX = endTouchPosition.x - startTouchPosition.x;

                // Check if the swipe distance exceeds the threshold
                if (Mathf.Abs(swipeDistanceX) > swipeThreshold)
                {
                    // If swipe distance is positive, it means the swipe is to the right
                    if (swipeDistanceX > 0)
                    {
                        MoveRight();  // Call the function to move right
                    }
                    // If swipe distance is negative, it means the swipe is to the left
                    else
                    {
                        MoveLeft();   // Call the function to move left
                    }
                }
            }
        }
    }
}
