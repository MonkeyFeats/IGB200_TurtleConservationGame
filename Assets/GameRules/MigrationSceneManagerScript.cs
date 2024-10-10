using Cinemachine;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using HeneGames.DialogueSystem;
public class MigrationSceneManagerScript : MonoBehaviour
{
    // UI References
    public GameObject starPopup;
    public Animator starsAnimator;
    public ScreenTransitionEffect transitioner;
    public MigrationPlayerMovement playerController;
    public CinemachineDollyCart cart;

    private int lives = 3;
    private int hitsTaken = 0;

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
        if (playerController != null)
        playerController.enabled = false;
        cart.m_Speed = 0f;
    }

    public void EndIntermission()
    {
        currentState = GameState.Playing;
        playerController.enabled = true;
        cart.m_Speed = 10f;        
    }

    public void EndGame()
    {
        currentState = GameState.EndGame;
        cart.m_Speed = 0f;
        playerController.enabled = false;
    }

    public void GiveStarRating()
    {
        int starsEarned = CalculateStarReward();
        starPopup.SetActive(true);
        if (starsAnimator != null)
            starsAnimator.SetInteger("StarsEarned", starsEarned);
    }

    int CalculateStarReward() 
    {
        return 2;
    }
    
    public void LoadScene(int sceneIndex)
    {
        SceneManager.LoadScene(sceneIndex);
    }


}
