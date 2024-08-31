using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingBG : MonoBehaviour
{
    
    private Vector3 startPosition;

    public float magnitude = 2;
    public float frequency = 2;
    public float offset = 2;

    // public bool inverted = false;

    // Start is called before the first frame update
    void Start()
    {
        startPosition = transform.position;

        // sinCenterY = transform.position.y;    
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = startPosition + transform.up * Mathf.Sin(Time.time * frequency + offset) * magnitude;

        // Vector3 pos = transform.position;

        // float sin = Mathf.Sin(Time.deltaTime* frequency) * amplitude;
        // if(inverted)
        // {
        //     sin *= -1;
        // }
        // pos.y = sinCenterY + sin;

        // transform.position = pos;
    }


    private void FixedUpdate()
    {

    }
}
