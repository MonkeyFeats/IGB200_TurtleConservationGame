using System.Collections;
using UnityEngine;

public class TurtleControls : MonoBehaviour
{
    private Vector2 startTouchPos, endTouchPos;
    private Touch touch;

    private IEnumerator goCoroutine;
    private bool coroutineAllowed;

    // Define lane positions and current lane index
    private int currentLane = 0; // Start in the center lane (0)
   public float laneDistance = 0.40f; // Distance between lanes

    private void Start()
    {
        coroutineAllowed = true;
        transform.position = new Vector3(0f, transform.position.y, transform.position.z); // Start at center lane
    }

    private void Update()
    {
        HandleTouchInput();
        HandleKeyboardInput();
    }

    private void HandleTouchInput()
    {
        if (Input.touchCount > 0)
        {
            touch = Input.GetTouch(0);

            if (touch.phase == TouchPhase.Began)
            {
                startTouchPos = touch.position;
            }

            if (touch.phase == TouchPhase.Ended && coroutineAllowed)
            {
                endTouchPos = touch.position;

                if ((endTouchPos.x < startTouchPos.x)
                    && (Mathf.Abs(touch.deltaPosition.x) > Mathf.Abs(touch.deltaPosition.y)))
                {
                    MoveLeft();
                }
                else if ((endTouchPos.x > startTouchPos.x)
                    && (Mathf.Abs(touch.deltaPosition.x) > Mathf.Abs(touch.deltaPosition.y)))
                {
                    MoveRight();
                }
            }
        }
    }

    private void HandleKeyboardInput()
    {
        if (coroutineAllowed)
        {
            if (Input.GetKeyDown(KeyCode.LeftArrow) || Input.GetKeyDown(KeyCode.A))
            {
                MoveLeft();
            }
            else if (Input.GetKeyDown(KeyCode.RightArrow) || Input.GetKeyDown(KeyCode.D))
            {
                MoveRight();
            }
        }
    }

    private void MoveLeft()
    {
        if (currentLane > -1) // Only move left if not already in the leftmost lane
        {
            currentLane--;
            goCoroutine = Go(new Vector3(-laneDistance, 0f, 0f));
            StartCoroutine(goCoroutine);
        }
    }

    private void MoveRight()
    {
        if (currentLane < 1) // Only move right if not already in the rightmost lane
        {
            currentLane++;
            goCoroutine = Go(new Vector3(laneDistance, 0f, 0f));
            StartCoroutine(goCoroutine);
        }
    }

    private IEnumerator Go(Vector3 direction)
    {
        coroutineAllowed = false;

        for (int i = 0; i < 3; i++)
        {
            transform.Translate(direction);
            yield return new WaitForSeconds(0.01f);
        }

        coroutineAllowed = true;
    }
}
