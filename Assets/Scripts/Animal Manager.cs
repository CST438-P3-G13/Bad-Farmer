using System;
using UnityEngine;
using System.Collections.Generic;
using UnityEngine.Tilemaps;
using Random = UnityEngine.Random;

public class AnimalManager : MonoBehaviour
{
    [HideInInspector]
    public static AnimalManager Instance { get; private set; }

    public List<GameObject> animals; 
    public Dictionary<GameObject, HappinessState> animalHappiness = new Dictionary<GameObject, HappinessState>();

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
    }

    private void Start()
    {
        // _difficultyManager = DifficultyManager.Instance;
        _gameManager = GameManager.Instance;
        _waveFunctionCollapse = GameObject.Find("PCGManager").GetComponent<WaveFunctionCollapse>();

        grid = new int[_waveFunctionCollapse.gridWidth, _waveFunctionCollapse.gridHeight];
        for (int i = 0; i < _waveFunctionCollapse.gridWidth; i++)
        {
            for (int j = 0; j < _waveFunctionCollapse.gridHeight; j++)
            {
                grid[i, j] = 0;
            }
        }
        
        SpawnAnimals();
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
        // Clear existing animals
        foreach (var animal in animals)
        {
            if (animal.activeSelf)
            {
                DestroyImmediate(animal, true);
            }
        }

        int cowCount = Random.Range(1, 2);
        int pigCount = Random.Range(1, 3);
        int chickenCount = Random.Range(1, 4);

        for (int i = 0; i < cowCount; i++)
        {
            GameObject newCow = Instantiate(animals[0], GetRandomLocation(), Quaternion.identity);
            animalHappiness[newCow] = HappinessState.Happy;
            newCow.GetComponent<Cow>().SetHappinessState(HappinessState.Happy);
        }

        for (int i = 0; i < pigCount; i++)
        {
            GameObject newPig = Instantiate(animals[1], GetRandomLocation(), Quaternion.identity);
            animalHappiness[newPig] = HappinessState.Happy;
            newPig.GetComponent<Pig>().SetHappinessState(HappinessState.Happy);
        }

        for (int i = 0; i < chickenCount; i++)
        {
            GameObject newChicken = Instantiate(animals[2], GetRandomLocation(), Quaternion.identity);
            animalHappiness[newChicken] = HappinessState.Happy;
            newChicken.GetComponent<Chicken>().SetHappinessState(HappinessState.Happy);
        }
    }

    private Vector3 GetRandomLocation()
    {
        int width = _gameManager.waveFunctionCollapse.xOffset;
        int height = _gameManager.waveFunctionCollapse.yOffset;
        Tilemap invalidTiles = _gameManager.waveFunctionCollapse.colliderTileMap;

        int maxChecks = 30;
        
        while (maxChecks > 0)
        {
            int x = Random.Range(-width, width);
            int y = Random.Range(-height, height);
            if (!invalidTiles.HasTile(new Vector3Int(x, y, 0)) && (grid[x + width, y + height] != 1))
            {
                grid[x + width, y + height] = 1;
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
