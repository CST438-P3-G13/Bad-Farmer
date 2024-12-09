using System;
using UnityEngine;
using Pathfinding;

public class GameManager : MonoBehaviour
{
    [HideInInspector]
    public static GameManager Instance { get; private set; }

    public AstarPath AstarPath; 
    public GameObject gameOverScreen;
    public float dayTimer = 300f;

    private DifficultyManager _difficultyManager;
    private AnimalManager _animalManager;
    private SceneManagerScript _sceneManagerScript;
    private WaveFunctionCollapse _waveFunctionCollapse;
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
        // Update time for day timer
        dayTimer -= Time.deltaTime;
        // Check for game over
        if (dayTimer <= 0f)
        {
            GameOver();
        }
    }

    // New Game/Play/Continue button will link to this function
    public void StartGame()
    {
        _playingState = 1;
        _day = 1; // Might change if we allow save data
        _sceneManagerScript.LoadScene("MainScene");
        Time.timeScale = 0;
        _animalManager = GameObject.Find("AnimalManager").GetComponent<AnimalManager>();
        // Start loading thing
        
        StartNewDay();
        
        // Finish loading thing
        Time.timeScale = 1;
    }
    
    public void GameOver()
    {
        // The game over screen button will link to a different function to change scene
        gameOverScreen.SetActive(true);
        Time.timeScale = 0;
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
        dayTimer = 300f;
        // Procedural generation
        _waveFunctionCollapse.GenerateGrid();
        AstarPath.Scan(AstarPath.graphs[0]);
        
        // Animals
        // Call spawn animals function from animal manager
        _animalManager.SpawnAnimals();
    }

    public void ToMainMenu()
    {
        _playingState = 0;
    }

    public int GetDay()
    {
        return _day;
    }
}
