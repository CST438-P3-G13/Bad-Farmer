using System;
using UnityEngine;
using Pathfinding;
using Random = UnityEngine.Random;

public class PathfindingFunctions : MonoBehaviour
{
    [Header("Targets")]
    public Transform[] corners;

    public Transform player;
    
    public float speed;
    public float nextWaypointDistance = 5f;

    private Path path = null;
    private int currWaypoint = 0;
    // private HappinessState happinessState = HappinessState.Happy;
    private float delay = .5f;
    private float time = 0f;

    private Seeker seeker;
    private Rigidbody2D rb;

    private void Start()
    {
        seeker = GetComponent<Seeker>();
        rb = GetComponent<Rigidbody2D>();
        GameManager.Instance.AstarPath.Scan(GameManager.Instance.AstarPath.graphs[0]);
    }

    private void Update()
    {
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
        Vector2 force = speed * 10f * Time.deltaTime * dir;
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
            Debug.Log("Calculating path now");
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

    // public void SetHappiness(HappinessState state)
    // {
    //     happinessState = state;
    // }

    private int GetTarget()
    {
        int furthestCorner = 0;
        float maxDist = -1f;

        for (int i = 0; i < corners.Length; i++)
        {
            float currDist = Vector3.Distance(player.position, corners[i].position);
            if (currDist > maxDist)
            {
                Vector2 dirToCorner = ((Vector2)corners[i].position - rb.position).normalized;
                Vector2 dirToPlayer = ((Vector2)player.position - rb.position).normalized;
                if (Vector2.Angle(dirToCorner, dirToPlayer) >= 90f)
                {
                    maxDist = currDist;
                    furthestCorner = i;
                }
            }
        }

        if (maxDist < 0)
        {
            furthestCorner = Random.Range(0, 3);
        }
        return furthestCorner;
    }
}
