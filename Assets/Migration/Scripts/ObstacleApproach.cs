using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObstacleApproach : MonoBehaviour
{
    public float moveSpeed = 2;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void FixedUpdate()
    {
        Vector3 pos = transform.position;

        pos.z -= moveSpeed * Time.fixedDeltaTime;

        transform.position = pos;
    }
}
