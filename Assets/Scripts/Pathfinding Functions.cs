using System;
using UnityEngine;
using Pathfinding;

public class PathfindingFunctions : MonoBehaviour
{
    [Header("Targets")]
    public Transform[] corners;

    public Transform player;
    
    public float speed;
    public float nextWaypointDistance = 5f;

    private Path path = null;
    private int currWaypoint = 0;
    private HappinessState happinessState = HappinessState.Agitated;
    private float delay = 1f;
    private float time = 0f;

    private Seeker seeker;
    private Rigidbody2D rb;

    private void Start()
    {
        seeker = GetComponent<Seeker>();
        rb = GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
        if (happinessState != HappinessState.Agitated)
        {
            return;
        }

        if (path == null)
        {
            StartPathfinding();
        }

        time += Time.deltaTime;
        if (time >= delay)
        {
            StartPathfinding();
            time = 0f;
        }

        if (currWaypoint >= path.vectorPath.Count)
        {
            return;
        }

        Vector2 dir = ((Vector2)path.vectorPath[currWaypoint] - rb.position).normalized;
        Vector2 force = speed * Time.deltaTime * dir;
        rb.AddForce(force);

        float distance = Vector2.Distance(rb.position, path.vectorPath[currWaypoint]);
        if (distance < nextWaypointDistance)
        {
            currWaypoint++;
        }
    }

    public void StartPathfinding()
    {
        if (seeker.IsDone())
        {
            seeker.StartPath(rb.position, corners[GetTarget()].position, OnPathComplete);
        }
    }

    private void OnPathComplete(Path p)
    {
        if (!p.error)
        {
            path = p;
            currWaypoint = 0;
        }
    }

    public void SetHappiness(HappinessState state)
    {
        happinessState = state;
    }

    private int GetTarget()
    {
        int furthestCorner = 0;
        float maxDist = -1f;

        for (int i = 0; i < corners.Length; i++)
        {
            float currDist = Vector3.Distance(player.position, corners[i].position);
            if (currDist > maxDist)
            {
                maxDist = currDist;
                furthestCorner = i;
            }
        }

        return furthestCorner;
    }
}
