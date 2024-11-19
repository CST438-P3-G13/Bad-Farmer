using UnityEngine;
using System.Collections.Generic;

public class AnimalManager : MonoBehaviour
{
    [HideInInspector]
    public static AnimalManager Instance { get; private set; }

    public List<GameObject> animals; // Prefabs of animals
    public Dictionary<GameObject, HappinessState> animalHappiness = new Dictionary<GameObject, HappinessState>();

    private DifficultyManager _difficultyManager;
    private float happinessUpdateInterval = 5f; // Time in seconds to reduce happiness
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
        DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        _difficultyManager = DifficultyManager.Instance;
    }

    private void Update()
    {
        // Check if it's time to update happiness
        if (Time.time >= lastHappinessUpdateTime + happinessUpdateInterval)
        {
            foreach (var animal in animalHappiness.Keys)
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
            Destroy(animal);
        }

        List<int> animalCounts = _difficultyManager.ProvideAnimalCounts();
        foreach (var animalType in animals)
        {
            GameObject newAnimal = Instantiate(animalType);
            animalHappiness[newAnimal] = HappinessState.Happy; // Start with Happy state
            newAnimal.GetComponent<Cow_Animal>().SetHappinessState(HappinessState.Happy);
        }
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
        animal.GetComponent<Cow_Animal>().SetHappinessState(newState);
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
