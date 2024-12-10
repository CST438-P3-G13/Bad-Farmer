using System;
using UnityEngine;
using System.Collections.Generic;
using UnityEngine.Serialization;
using UnityEngine.Tilemaps;
using Random = UnityEngine.Random;

public class AnimalManager : MonoBehaviour
{
    [HideInInspector]
    public static AnimalManager Instance { get; private set; }

    public List<GameObject> animalsPrefabs;
    public List<GameObject> animals;
    public Dictionary<GameObject, HappinessState> animalHappiness = new Dictionary<GameObject, HappinessState>();

    public Transform[] corners;
    public Transform player;
    
    public Transform cowExit;
    public Transform cowPen;

    public Transform pigExit;
    public Transform pigPen;

    public Transform chickenExit;
    public Transform chickenPen;

    public int xLeft = -50;
    public int xRight = 35;
    public int yUp = 30;
    public int yDown = -17;

    public WaveFunctionCollapse _waveFunctionCollapse;
    private GameManager _gameManager;

    private int[,] grid;
    private float happinessUpdateInterval = 30f; 
    private float lastHappinessUpdateTime = 0f;


    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(this);
        }
        // DontDestroyOnLoad(gameObject);
        _gameManager = GameManager.Instance;
        grid = new int[xRight - xLeft, yUp - yDown];
        for (int i = 0; i < xRight - xLeft; i++)
        {
            for (int j = 0; j < yUp - yDown; j++)
            {
                grid[i, j] = 0;
            }
        }
    }

    private void Start()
    {
        // _difficultyManager = DifficultyManager.Instance;

        
        // SpawnAnimals();
    }

    private void Update()
    {
        if (Time.time >= lastHappinessUpdateTime + happinessUpdateInterval)
        {
            List<GameObject> animalsToModify = new List<GameObject>(animalHappiness.Keys);

            foreach (var animal in animalsToModify)
            {
                DecreaseHappiness(animal);
            }

            lastHappinessUpdateTime = Time.time;
        }
    }

    public void SpawnAnimals()
    {
        int cowCount = Random.Range(1, 2);
        int pigCount = Random.Range(1, 3);
        int chickenCount = Random.Range(1, 4);

        for (int i = 0; i < cowCount; i++)
        {
            GameObject newCow = Instantiate(animalsPrefabs[0], GetRandomLocation(), Quaternion.identity);
            Cow cow = newCow.GetComponent<Cow>();
            cow.penArea = cowPen;
            cow.player = player;
            cow.exitPen = cowExit;
            
            PathfindingFunctions pathfindingFunctions = newCow.GetComponent<PathfindingFunctions>();
            pathfindingFunctions.player = player;
            pathfindingFunctions.corners = corners;
            animals.Add(newCow);
            animalHappiness[newCow] = HappinessState.Happy;
            cow.SetHappinessState(HappinessState.Happy);
        }

        for (int i = 0; i < pigCount; i++)
        {
            GameObject newPig = Instantiate(animalsPrefabs[1], GetRandomLocation(), Quaternion.identity);
            Pig pig = newPig.GetComponent<Pig>();
            pig.penArea = pigPen;
            pig.player = player;
            pig.exitPen = pigExit;
            
            PathfindingFunctions pathfindingFunctions = newPig.GetComponent<PathfindingFunctions>();
            pathfindingFunctions.player = player;
            pathfindingFunctions.corners = corners;
            animals.Add(newPig);
            animalHappiness[newPig] = HappinessState.Happy;
            pig.SetHappinessState(HappinessState.Happy);
        }

        for (int i = 0; i < chickenCount; i++)
        {
            GameObject newChicken = Instantiate(animalsPrefabs[2], GetRandomLocation(), Quaternion.identity);
            Chicken chicken = newChicken.GetComponent<Chicken>();
            chicken.penArea = chickenPen;
            chicken.player = player;
            chicken.exitPen = chickenExit;
            
            PathfindingFunctions pathfindingFunctions = newChicken.GetComponent<PathfindingFunctions>();
            pathfindingFunctions.player = player;
            pathfindingFunctions.corners = corners;
            animals.Add(newChicken);
            animalHappiness[newChicken] = HappinessState.Happy;
            chicken.SetHappinessState(HappinessState.Happy);
        }
    }

    private Vector3 GetRandomLocation()
    {
        Tilemap invalidTiles = _waveFunctionCollapse.colliderTileMap;

        int maxChecks = 30;
        
        while (maxChecks > 0)
        {
            int x = Random.Range(xLeft, xRight);
            int y = Random.Range(yDown, yUp);
            if (!invalidTiles.HasTile(new Vector3Int(x, y, 0)) && (grid[x - xLeft, y - yDown] != 1))
            {
                grid[x -xLeft, y - yDown] = 1;
                Debug.Log($"Spawning animal at [{x}, {y}].");
                return new Vector3(x, y, 0);
            }

            maxChecks--;
        }
        
        return Vector3.zero;
    }

    private void DecreaseHappiness(GameObject animal)
    {
        HappinessState currentState = animalHappiness[animal];
        HappinessState newState = currentState switch
        {
            HappinessState.Happy => HappinessState.Neutral,
            HappinessState.Neutral => HappinessState.Agitated,
            HappinessState.Agitated => HappinessState.Suicidal,
            HappinessState.Suicidal => HappinessState.Suicidal, // Stay suicidal
            _ => HappinessState.Neutral
        };

        // Update happiness state
        animalHappiness[animal] = newState;
        if (animal.TryGetComponent(out Cow cow))
        {
            cow.SetHappinessState(newState);
        }
        else if (animal.TryGetComponent(out Pig pig))
        {
            pig.SetHappinessState(newState);
        }
        else if (animal.TryGetComponent(out Chicken chicken))
        {
            chicken.SetHappinessState(newState);
        }
    }
}

// Enum to define happiness states
public enum HappinessState
{
    Happy,
    Neutral,
    Agitated,
    Suicidal
}
