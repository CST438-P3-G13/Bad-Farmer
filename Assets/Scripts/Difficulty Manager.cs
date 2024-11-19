using System;
using System.Collections.Generic;
using UnityEngine;

public class DifficultyManager : MonoBehaviour
{
    [HideInInspector]
    public static DifficultyManager Instance { get; private set; }

    public int[] deathLimits;
    public int difficultyLvl;

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

    public int Initialize()
    {
        difficultyLvl = 0;
        return deathLimits[0];
    }

    /**
     * <param name="deathRatio">The ratio of: # of animal deaths / # of animal deaths allowed</param>
     * <param name="taskRatio">The ratio of : # of tasks completed / # of tasks assigned</param>
     */
    public int UpdateDifficulty(double deathRatio, double taskRatio)
    {
        // Use these values to gauge how well the player is doing
        // The lower the deathRatio, and the higher the taskRatio, the better they are doing
        
        // Make some if statements that will check if the ratios are in certain ranges, and then increment the difficulty level accordingly

        return 0;
    }

    public List<int> ProvideAnimalCounts()
    {
        // Make/Find some algorithm to turn the difficulty level into a seed to randomly generate different numbers of animals
        return new List<int>() { 0, 0, 0 };
    }

    public int ProvideTaskCount()
    {
        // Similar to ProvideAnimalCounts function, but just to generate one value
        return 0;
    }
}
