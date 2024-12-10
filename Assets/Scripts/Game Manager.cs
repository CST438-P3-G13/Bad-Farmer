using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;

public class GameManager : MonoBehaviour
{
    [HideInInspector]
    public static GameManager Instance { get; private set; }

    public AstarPath AstarPath; 
    public GameObject gameOverScreen;
    public float dayTimer = 300f;

    // public DifficultyManager difficultyManager;
    [HideInInspector]
    public AnimalManager animalManager = null;
    [HideInInspector]
    public SceneManagerScript sceneManagerScript;
    [HideInInspector]
    public WaveFunctionCollapse waveFunctionCollapse = null;
    
    private int _playingState; // 0 for Main Menu, 1 for in game
    private int _day;
    private int _deathsAllowedToday;
    private int _deathsToday;
    
    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else if (Instance != this)
        {
            Debug.Log("test");
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        // difficultyManager = DifficultyManager.Instance;
        sceneManagerScript = GetComponent<SceneManagerScript>();
        _playingState = 0;
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
        sceneManagerScript.LoadScene("Pathfinding Scene");

        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
        
        waveFunctionCollapse = GameObject.Find("PCGManager").GetComponent<WaveFunctionCollapse>();
        animalManager = GameObject.Find("AnimalManager").GetComponent<AnimalManager>();
        
        Time.timeScale = 0;
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
            // _deathsAllowedToday = difficultyManager.Initialize();
        }
        else
        {
            // _deathsAllowedToday = difficultyManager.UpdateDifficulty(_deathsToday / (_deathsAllowedToday * 1f), 0f);
        }

        _deathsToday = 0;
        dayTimer = 300f;
        
        Debug.Log("15% done");
        // Procedural generation
        waveFunctionCollapse.GenerateGrid();
        Debug.Log("55% done");
        AstarPath.Scan(AstarPath.graphs[0]);
        Debug.Log("75% done");
        // Animals
        // Call spawn animals function from animal manager
        animalManager.SpawnAnimals();
        Debug.Log("100% done");
    }
    
    // Method to reassign UI listeners
    // private void ReassignUIListeners()
    // {
    //     // Example: If you have buttons referencing the GameManager
    //     Button[] allButtons = FindObjectsOfType<Button>();
    //     foreach (Button button in allButtons)
    //     {
    //         // Check if the button is referencing the current (duplicate) instance
    //         for (int i = 0; i < button.onClick.GetPersistentEventCount(); i++)
    //         {
    //             var target = button.onClick.GetPersistentTarget(i);
    //             if (target == this)
    //             {
    //                 // Update the listener to point to the original instance
    //                 button.onClick.SetPersistentListenerState(i, Instance.StartGame());
    //             }
    //         }
    //     }
    // }

    public void ToMainMenu()
    {
        _playingState = 0;
    }

    public int GetDay()
    {
        return _day;
    }
}
