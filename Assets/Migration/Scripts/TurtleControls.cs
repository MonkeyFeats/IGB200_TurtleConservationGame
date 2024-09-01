using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurtleControls : MonoBehaviour
{
    private Vector2 startTouchPos, endTouchPos;
    private Touch touch;

    private IEnumerator goCoroutine;
    private bool coroutineAllowed;

    private void Start()
    {
        coroutineAllowed = true;
    }

    private void Update()
    {
        if (Input.touchCount > 0)
        {
            touch = Input.GetTouch(0);
        }

        if (touch.phase == TouchPhase.Began)
        {
            startTouchPos = touch.position;
        }

        if (Input.touchCount > 0 && touch.phase == TouchPhase.Ended && coroutineAllowed)
        {
            endTouchPos = touch.position;

            if ((endTouchPos.x < startTouchPos.x)
                && (Mathf.Abs(touch.deltaPosition.x) > Mathf.Abs(touch.deltaPosition.y)))
            {
                goCoroutine = Go(new Vector3(-0.15f, 0f, 0f));
                StartCoroutine(goCoroutine);
            }

            else if ((endTouchPos.x > startTouchPos.x)
                && (Mathf.Abs(touch.deltaPosition.x) > Mathf.Abs(touch.deltaPosition.y)))
            {
                goCoroutine = Go(new Vector3(0.15f, 0f, 0f));
                StartCoroutine(goCoroutine);
            }
        }
    }

    private IEnumerator Go(Vector3 direction)
    {
        coroutineAllowed = false;

        for (int i = 0; i <= 2; i++)
        {
            transform.Translate(direction);
            yield return new WaitForSeconds(0.01f);
        }

        for (int i = 0; i <= 2; i++)
        {
            transform.Translate(direction);
            yield return new WaitForSeconds(0.01F);
        }

        transform.Translate(direction);

        coroutineAllowed = true;
    }   
}
