using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TaskManager : MonoBehaviour
{
    public static TaskManager Instance { get; private set; }

    public GameObject taskUI; 
    public TextMeshProUGUI taskText; 
    public List<FarmTask> tasks; 

    private float taskInterval = 60f; 
    private float taskTimer;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(this);
        }
    }

    private void Start()
    {
        tasks = new List<FarmTask>();
        AddRandomTask();

        taskTimer = taskInterval;
    }

    private void Update()
    {
        taskTimer -= Time.deltaTime;

        if (taskTimer <= 0)
        {
            AddRandomTask();
            taskTimer = taskInterval; 
        }
    }

    public void CompleteTask(String newTaskDescription){
        FarmTask task = tasks.Find(t => t.description == newTaskDescription);

        if(task != null){
            tasks.Remove(task);
            Debug.Log($"Task Completed: {newTaskDescription}");
            UpdateTaskUI();
        }
    }

    private void AddRandomTask()
    {
        List<string> taskPool = new List<string>
        {
            "Milking the cow",
            "Watering the crops",
            "Defeathering a chicken",
            "Cleaning the pig"
        };

        List<string> availableTasks = taskPool.FindAll(task =>
            !tasks.Exists(existingTask => existingTask.description == task));

        if (availableTasks.Count > 0)
        {
            string newTaskDescription = availableTasks[UnityEngine.Random.Range(0, availableTasks.Count)];


            tasks.Add(new FarmTask(newTaskDescription));

            Debug.Log($"New Task Added: {newTaskDescription}"); 

            UpdateTaskUI();
        }
        else
        {
            Debug.Log("No more tasks available to add.");
        }
    }

    private void UpdateTaskUI()
    {
        if (tasks.Count == 0 || taskText == null) return;

        string taskListText = "Tasks:\n";
        foreach (FarmTask task in tasks)
        {
            taskListText += $"- {task.description}\n";
        }

        taskText.text = taskListText;
    }
}
