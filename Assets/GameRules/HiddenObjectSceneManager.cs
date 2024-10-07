using Cinemachine;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
public class HiddenObjectSceneManager : MonoBehaviour
{
    // UI References
    public GameObject starPopup;
    public Animator starsAnimator;
    public ScreenTransitionEffect transitioner;

    // Enum for game states
    private enum GameState { Intermission, Playing, EndGame };
    private GameState currentState;

    void Start()
    {
        StartIntermission();
    }

    public void StartIntermission()
    {
        currentState = GameState.Intermission;
       // if (playerController != null)
       // playerController.enabled = false;
    }

    public void EndIntermission()
    {
        currentState = GameState.Playing;
       // playerController.enabled = true;   
    }

    public void EndGame()
    {
        currentState = GameState.EndGame;
       // cart.m_Speed = 0f;

        int starsEarned = CalculateStarReward();

        starPopup.SetActive(true);
        if (starsAnimator != null)
            starsAnimator.SetInteger("StarsEarned", starsEarned);
        else
        {
            Debug.Log("No Animator");
        }
    }
    
    int CalculateStarReward() 
    {
        //Should calculate how many objects were found in the time limit
        return 2;
    }
    
    public void LoadScene(int sceneIndex)
    {
        SceneManager.LoadScene(sceneIndex);
    }
}
