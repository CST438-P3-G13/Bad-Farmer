using UnityEngine;

public class Car : MonoBehaviour
{
    public Transform targetA; // First target
    public Transform targetB; // Second target
    public float speed = 3f; // Speed of the car
    private Transform currentTarget; // The current target the car is moving toward
    private float thresholdDistance = 0.5f; // Adjusted distance threshold to avoid getting stuck

    void Start()
    {
        // Set the initial target to targetA
        currentTarget = targetA;
    }

    void Update()
    {
        // Move towards the current target
        if (currentTarget != null)
        {
            MoveToTarget();
        }
    }

    private void MoveToTarget()
    {
        // Calculate the direction to the target
        Vector3 direction = (currentTarget.position - transform.position).normalized;

        // Move the car toward the target
        transform.position += direction * speed * Time.deltaTime;

        // Check if the car is close enough to the target
        if (Vector3.Distance(transform.position, currentTarget.position) <= thresholdDistance)
        {
            SwitchTarget();
        }
    }

    private void SwitchTarget()
    {
        // Change the current target to the other one
        if (currentTarget == targetA)
        {
            currentTarget = targetB;
        }
        else
        {
            currentTarget = targetA;
        }

        Debug.Log("Switched target to: " + currentTarget.name);
    }
}
