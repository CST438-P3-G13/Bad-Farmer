using System;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [HideInInspector]
    public static GameManager Instance { get; private set; }

    public GameObject gameOverScreen; 
    public GameObject pauseScreen;

    private DifficultyManager _difficultyManager;
    // Other managers
    private int _playingState; // 0 for Main Menu, 1 for in game
    private int _day;
    private int _deathsAllowedToday;
    private int _deathsToday;
    
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

    private void Start()
    {
        _difficultyManager = DifficultyManager.Instance;
    }

    private void Update()
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
        _day = 1; // Might change if we allow save data
        // switch to game scene
        StartNewDay();
    }
    
    public void GameOver()
    {
        // The game over screen button will link to a different function to change scene
        gameOverScreen.SetActive(true);
    }
    
    /**
     * <summary>This function starts a new day by calling all the necessary functions from the different managers. </summary>
     * <summary>The difficulty needs to be updated, hazards generated, tasks chosen, animals chosen</summary>
     */
    public void StartNewDay()
    {
        // do stuff here
        if (_day == 1)
        {
            _deathsAllowedToday = _difficultyManager.Initialize();
        }
        else
        {
            _deathsAllowedToday = _difficultyManager.UpdateDifficulty(_deathsToday / (_deathsAllowedToday * 1f), 0f);
        }

        _deathsToday = 0;
        // Procedural generation, tasks, animals
    }

    public void PauseGame()
    {
        pauseScreen.SetActive(!pauseScreen.activeSelf);
        Time.timeScale = pauseScreen.activeSelf ? 0f : 1f;
    }
}
