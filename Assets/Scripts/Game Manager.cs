using System;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [HideInInspector]
    public static GameManager Instance { get; private set; }

    public GameObject gameOverScreen; 
    public GameObject pauseScreen;

    // 0 for Main Menu, 1 for in game
    private int _playingState;
    
    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(this);
        }
        DontDestroyOnLoad(gameObject);
    }

    void Update()
    {
        // Currently in Main Menu, so don't allow pausing
        if (_playingState == 0)
        {
            return;
        }
        if (Input.GetKeyDown(KeyCode.Escape) || Input.GetKeyDown(KeyCode.P))
        {
            PauseGame();
        }
        // Update time for day timer
        // Check for game over
    }

    // New Game/Play/Continue button will link to this function
    public void StartGame()
    {
        _playingState = 1;
        // switch to game scene
        StartNewDay();
    }
    
    public void GameOver()
    {
        // The game over screen button will link to a different function to change scene
        gameOverScreen.SetActive(true);
    }
    
    public void StartNewDay()
    {
        // do stuff here
    }

    public void PauseGame()
    {
        pauseScreen.SetActive(!pauseScreen.activeSelf);
        Time.timeScale = pauseScreen.activeSelf ? 0f : 1f;
    }
}
