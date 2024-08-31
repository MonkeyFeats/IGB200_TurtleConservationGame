using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObstacleDeletion : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.name == "Turtle")
        {
            Destroy(gameObject);
        }
    }
}
