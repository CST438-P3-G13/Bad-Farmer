using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

// The Task class will hold most of the logic for the Farm Task mechanic
public class TaskManager : MonoBehaviour
{
    [HideInInspector]
    public static TaskManager Instance { get; private set; }
    
    // public List<FarmTask> tasks; Uncomment when the Task class is created
    public GameObject taskUI;

    private DifficultyManager _difficultyManager;
    // private List<FarmTask> _currTaskList; Uncomment when the Task class is created
    private int _numTasks;
    
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
        // Iterate through the tasks and check if any of them are fulfilled or expired
        // Generate a number of timestamps within the day's time limit to reveal tasks
    }

    /**
     * <returns>Two ints. Num tasks and </returns>
     */
    public void GenerateTasks()
    {
        int taskNum = _difficultyManager.ProvideTaskCount();
        
        // Randomly choose this amount of tasks?
        // If tasks each have difficulty ratings, then generate different combinations of tasks
        // with the ratings sums less than or equal to this amount?

    }
}
