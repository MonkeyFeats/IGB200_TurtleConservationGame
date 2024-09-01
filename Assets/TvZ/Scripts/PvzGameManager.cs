using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class PvzGameManager : MonoBehaviour
{
    public static string coinPrefsName = "Coins_Player";

    public TMP_Text sunDisp;
    public int startingSunAmnt;
    public int SunAmount = 0;

    public GameObject decorativeZombies;

    public Transform cardSlotsHolder;
    public GameObject endGameCanvas;

    public ZombieManager zombieManager;

    public Animator cameraPan;

    public static int currentAmount;
    public int preCurrentAmount = 4;

    //public TMP_Text coinDisplay;

    private void Start()
    {
        currentAmount = PlayerPrefs.GetInt(coinPrefsName);

        //coinDisplay.SetText(currentAmount + "");

        CardManager.isGameStart = false;
        AddSun(startingSunAmnt);

        StartMatch();
    }

	public void Update()
	{
		if (preCurrentAmount != currentAmount)
		{
            preCurrentAmount = currentAmount;
            //coinDisplay.SetText(currentAmount + "");
        }

        if (Time.frameCount % 60 == 0)
        {
            AddSun(5);
        }
	}

	public void StartMatch()
	{
        //cameraPan.SetTrigger("PanToGulls");
        CardManager.isGameStart = true;
        RefreshAllPlantCards();
        zombieManager.SpawnZombies();
	}

    public void AddSun(int amnt)
    {
        SunAmount += amnt;
        sunDisp.text = "" + SunAmount;
    }

    public void DeductSun(int amnt)
    {
        SunAmount -= amnt;
        sunDisp.text = "" + SunAmount;
    }

    public void RefreshAllPlantCards()
	{
        foreach (Transform card in cardSlotsHolder)
        {
            try
            {
                card.GetComponent<CardManager>().StartRefresh();
            }
            catch (System.Exception)
            {
                Debug.LogError("Card does not contain CardManager script!");
            }
        }
    }

    public static void IncrementCoins(int value)
	{
        currentAmount += value;
    }


    private void DisablePlayerControls()
    {
        Time.timeScale = 0f; // Pauses the game
        // Disable your player control script here, e.g.,
        // player.GetComponent<PlayerControlScript>().enabled = false;
    }

    public void EndGame()
    {
        DisablePlayerControls();

        // Display the game over UI
        if (endGameCanvas != null)
        {
            endGameCanvas.SetActive(true);
        }
    }

    public void OnApplicationQuit()
	{
        PlayerPrefs.SetInt(coinPrefsName, currentAmount);
	}
}
