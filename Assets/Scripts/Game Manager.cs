using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro; // Import TextMeshPro namespace

public class GameManager : MonoBehaviour
{
    [HideInInspector]
    public static GameManager Instance { get; private set; }

    public AstarPath AstarPath;
    public GameObject gameOverScreen;

    [Header("Day Timer Settings")]
    public float dayTimer = 300f; // 5 minutes in seconds
    public TextMeshProUGUI timerDisplay; // Use TextMeshProUGUI for the timer display
    public TextMeshProUGUI deathDisplay;

    [HideInInspector]
    public AnimalManager animalManager = null;
    [HideInInspector]
    public SceneManagerScript sceneManagerScript;
    [HideInInspector]
    public WaveFunctionCollapse waveFunctionCollapse = null;

    private int _playingState; // 0 for Main Menu, 1 for in-game
    private int _day;
    private int _deathsAllowedToday = 3;
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
        sceneManagerScript = GetComponent<SceneManagerScript>();
        _playingState = 0;
    }

    private void Update()
    {
        if (_playingState == 1)
        {
            UpdateDayTimer();
        }
    }

    private void UpdateDayTimer()
    {
        if (Time.timeScale == 0)
        {
            Debug.LogWarning("Time is paused. Timer won't update.");
            return;
        }

        dayTimer -= Time.deltaTime;

        if (timerDisplay != null)
        {
            TimeSpan time = TimeSpan.FromSeconds(dayTimer);
            timerDisplay.text = $"{time.Minutes:D2}:{time.Seconds:D2}";
            // Debug.Log($"Timer Updated: {timerDisplay.text}"); // Log timer updates
        }

        if (dayTimer <= 0f)
        {
            dayTimer = 0f;
            EndDay();
        }
    }

    private void EndDay()
    {
        Debug.Log($"Day {_day} ended!");
        ToMainMenu(); // Go to main menu after the day ends
    }

    public void StartGame()
    {
        // _playingState = 1;
        _day = 1;
        sceneManagerScript.LoadScene("Pathfinding Scene");
        SceneManager.sceneLoaded += OnSceneLoaded;
        Debug.Log("Game Started.");

        dayTimer = 300f; // Reset day timer
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;

        waveFunctionCollapse = GameObject.Find("PCGManager")?.GetComponent<WaveFunctionCollapse>();
        animalManager = GameObject.Find("AnimalManager")?.GetComponent<AnimalManager>();
        timerDisplay = GameObject.Find("Timer").GetComponent<TextMeshProUGUI>();
        deathDisplay = GameObject.Find("Death Count").GetComponent<TextMeshProUGUI>();
        deathDisplay.text = $"Deaths: {_deathsAllowedToday}";

        if (waveFunctionCollapse == null)
        {
            Debug.LogError("WaveFunctionCollapse not found!");
        }
        if (animalManager == null)
        {
            Debug.LogError("AnimalManager not found!");
        }
        if (timerDisplay == null)
        {
            Debug.LogError("Timer not found!");
        }

        
        Time.timeScale = 0;
        _playingState = 1;

        StartNewDay();
        Time.timeScale = 1;
    }

    public void StartNewDay()
    {
        _day++;
        Debug.Log($"Starting Day {_day}");

        _deathsToday = 0;
        dayTimer = 300f; // Reset timer for 5 minutes

        if (waveFunctionCollapse == null || AstarPath == null || animalManager == null)
        {
            Debug.LogError("One or more dependencies are missing! Cannot proceed with StartNewDay.");
            return;
        }

        // Procedural generation
        Debug.Log("Starting procedural generation...");
        waveFunctionCollapse.GenerateGrid();
        // AstarPath.Scan(AstarPath.graphs[0]);

        // Spawn animals
        animalManager.SpawnAnimals();
    }

    public void GameOver()
    {
        _playingState = 0;
        Time.timeScale = 1; // Reset time scale in case the game was paused
        Debug.Log("Returning to Main Menu...");
        SceneManager.LoadScene("MainMenu"); // Load the Main Menu scene
    }

    public void ToMainMenu()
    {
        _playingState = 0;
        Time.timeScale = 1; // Reset time scale
        Debug.Log("Returning to Main Menu...");
        SceneManager.LoadScene("MainMenu"); // Load the Main Menu scene
    }
    
    public void IncrementDeaths()
    {
        _deathsToday++;
        deathDisplay.text = $"Deaths: {_deathsAllowedToday - _deathsToday}";
        if (_deathsAllowedToday <= _deathsToday)
        {
            GameOver();
        }
        
    }

    public int GetDay()
    {
        return _day;
    }
}
