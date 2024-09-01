using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ScoreManager : MonoBehaviour
{
    public TextMeshProUGUI scoreText;
    public float distancePerSecond = 2f;
    public float distanceToWin = 100f;

    private float score = 0f;
    private bool gameIsActive = true;

    void Start()
    {
        StartCoroutine(UpdateScore());
    }

    IEnumerator UpdateScore()
    {
        while (gameIsActive)
        {
            score += distancePerSecond;
            UpdateScoreText();
            CheckWinCondition();

            yield return new WaitForSeconds(1f);
        }
    }

    void UpdateScoreText()
    {
        scoreText.text = $"Distance: {score:F2} m";
    }

    void CheckWinCondition()
    {
        if (score >= distanceToWin)
        {
            Debug.Log("You reached Level 1 distance!");
        }
    }

    public void StopGame()
    {
        gameIsActive = false;
    }
}
