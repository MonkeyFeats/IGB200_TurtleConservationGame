using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeathManager : MonoBehaviour
{
    public PvzGameManager gameManager;
    public void OnTriggerEnter2D(Collider2D collision)
    {
        Debug.Log("Collided death!");

        if (collision.tag == "Zombie")
        {
            //Ending Code here
            Debug.Log("Died!");
            gameManager.EndGame();
        }
    }

}
