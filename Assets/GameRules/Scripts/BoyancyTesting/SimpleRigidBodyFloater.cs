using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimpleRigidBodyFloater : MonoBehaviour
{
    private Rigidbody rb;
    public float minWaterHeight = -1.0f;
    public float floatStrength = 1f;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    void FixedUpdate()
    {
        // Object's current height
        float objectHeight = transform.position.y;

        if (objectHeight < minWaterHeight)
        {
            Vector3 buoyancy = Vector3.up * floatStrength;
            rb.AddForce(buoyancy, ForceMode.Acceleration);
            Debug.Log("Adding Force");
        }
    }
}
