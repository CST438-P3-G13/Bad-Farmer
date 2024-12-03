using System;

[Serializable]
public class FarmTask
{
    public string description; // The task description

    public FarmTask(string description)
    {
        this.description = description;
    }
}
