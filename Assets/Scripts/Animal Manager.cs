using System;
using System.Collections.Generic;
using UnityEngine;

public class AnimalManager : MonoBehaviour
{
    [HideInInspector]
    public static AnimalManager Instance { get; private set; }
    
    public List<GameObject> animals;
    public List<int> happinessThresholds;

    private DifficultyManager _difficultyManager;
    
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
        // Iterate through the animals and update their time outside of pen
        // Update NPC state if time meets threshold
    }

    public void SpawnAnimals()
    {
        foreach (var animal in animals)
        {
            Destroy(animal);
        }

        List<int> animalCounts = _difficultyManager.ProvideAnimalCounts();
        // Iterate through animalCounts and Instantiate that amount of each animal type
        // Get respective pen location from somewhere (most likely from PCG manager)
    }
}
