using UnityEngine;
using System.Collections.Generic;

public class AnimalManager : MonoBehaviour
{
    [HideInInspector]
    public static AnimalManager Instance { get; private set; }

    public List<GameObject> animals; 
    public Dictionary<GameObject, HappinessState> animalHappiness = new Dictionary<GameObject, HappinessState>();

    private DifficultyManager _difficultyManager;
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
        DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        _difficultyManager = DifficultyManager.Instance;
        // SpawnAnimals(); Uncomment if needed during initialization
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
            Destroy(animal);
        }

        int cowCount = Random.Range(1, 6);  
        int pigCount = Random.Range(1, 6);  
        int chickenCount = Random.Range(1, 6);  

        for (int i = 0; i < cowCount; i++)
        {
            GameObject newCow = Instantiate(animals[0]); 
            animalHappiness[newCow] = HappinessState.Happy; 
            newCow.GetComponent<Cow>().SetHappinessState(HappinessState.Happy);
        }

        for (int i = 0; i < pigCount; i++)
        {
            GameObject newPig = Instantiate(animals[1]);  
            animalHappiness[newPig] = HappinessState.Happy;
            newPig.GetComponent<Pig>().SetHappinessState(HappinessState.Happy);
        }

        for (int i = 0; i < chickenCount; i++)
        {
            GameObject newChicken = Instantiate(animals[2]);  
            animalHappiness[newChicken] = HappinessState.Happy; 
            newChicken.GetComponent<Chicken>().SetHappinessState(HappinessState.Happy);
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
