using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurtleHealth : MonoBehaviour
{
    int hits = 4;
    bool invincible = false;
    float invincibleTimer = 0;
    float invincibleTime = 1;
    MeshRenderer meshRenderer;

    void Start()
    {
        meshRenderer = GetComponent<MeshRenderer>();
    }

    void Update()
    {
        if (invincible)
        {
            if (invincibleTimer >= invincibleTime)
            {
                invincibleTimer = 0;
                invincible = false;
                meshRenderer.enabled = true;
            }
            else
            {
                invincibleTimer += Time.deltaTime;
                meshRenderer.enabled = !meshRenderer.enabled;
            }
        }
    }

    public void TakeDamage()
    {
        if (!invincible)
        {
            hits--;
            if (hits <= 0)
            {
                Destroy(gameObject);
            }
            else
            {
                invincible = true;
                invincibleTimer = 0;
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Enemy"))
        {
            TakeDamage();
        }
    }
}
