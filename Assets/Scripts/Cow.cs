using UnityEngine;

public class Cow : MonoBehaviour
{
    public Rigidbody2D rb;
    public float runSpeed = 40f;
    public float speedVar = 5f;
    public float horizontalMove = 0f;

    public float directionInterval = 2f;
    private float nextDirectionChange = 0f;
    private float randomDelay = 0f;

    private bool isInteracting = false;
    public float interactionTime = 2f;
    private float interactionEndTime = 0f;

    [Range(0f, 1f)]
    public float idle = 0.25f;

    private HappinessState happinessState = HappinessState.Happy;
    
    private bool followingPlayer = false;
    public Transform player;
    public Transform penArea;

    private bool inPen = false;
    private bool leavePen = false;
    private float penTimer = 0f;
    private bool isTimerActive = false;
    private Transform pen;
    public Transform exitPen;
    private bool isExiting = false;
    
    private bool isDrowning = false;
    private float drowningTimer = 0f;

    private PathfindingFunctions _pathfindingFunctions;
    
    void Start()
    {
        _pathfindingFunctions = GetComponent<PathfindingFunctions>();
        rb = GetComponent<Rigidbody2D>();
        _pathfindingFunctions.enabled = false;
        _pathfindingFunctions.speed = 4f;
        
        SetHappinessState(HappinessState.Happy);

        if (AnimalManager.Instance != null)
        {
            AnimalManager.Instance.animalHappiness[gameObject] = HappinessState.Happy;
        }
    }

    void Update()
    {
        if (isDrowning)
        {
            _pathfindingFunctions.enabled = false;
            if (Time.time >= drowningTimer + 15f)
            {
                isDrowning = false;
                GameManager.Instance.IncrementDeaths();
                Destroy(gameObject);
            }

            return;
        }
        if (isExiting)
        {
            _pathfindingFunctions.enabled = false;
            Vector2 directionToExit = (Vector2)exitPen.position - rb.position;
            if (directionToExit.magnitude > 0.1f)
            {
                rb.linearVelocity = directionToExit.normalized * runSpeed;
                Debug.Log("running towrds exit");
            }
            else
            {
                Debug.Log("Touched ezit");
                isExiting = false;
                inPen = false;
            }

            return;
        }
        if (inPen)
        {
            happinessState = HappinessState.Happy;
            _pathfindingFunctions.enabled = false;
            if (isTimerActive && Time.time >= penTimer + 12f)
            {
                isExiting = true;
                isTimerActive = false;
            }
            else
            {
                
                if (Time.time > nextDirectionChange + randomDelay)
                {
                    randomDelay = Random.Range(0.2f, 1f);

                    if (Random.value < idle)
                    {
                        horizontalMove = 0f;
                        rb.linearVelocity = Vector2.zero;
                    }
                    else
                    {
                        float direction = Random.value < 0.5f ? -1 : 1;
                        float randomSpeed = runSpeed + Random.Range(-speedVar, speedVar);

                        Vector2 movement;
                        if (Random.value < 0.5f)
                        {
                            // horizontalMove = direction * randomSpeed;
                            // rb.linearVelocity = new Vector2(horizontalMove, 0f);
                            movement = new Vector2(horizontalMove, 0f);
                        }
                        else
                        {
                            // float verticalMove = direction * randomSpeed;
                            // rb.linearVelocity = new Vector2(0f, verticalMove);
                            movement = new Vector2(0f, direction * randomSpeed);
                        }
                        Vector2 directionToExit = (Vector2)exitPen.position - rb.position.normalized;
                        if (Vector2.Dot(movement, directionToExit.normalized) < 0)
                        {
                            movement = -movement;
                        }

                        rb.linearVelocity = movement;
                    }
                    nextDirectionChange = Time.time + directionInterval;
                }
            }

            return;
        }
        
        if (Input.GetKeyDown(KeyCode.R))
        {
            if (inPen)
            {
                followingPlayer = false;
                rb.linearVelocity = Vector2.zero;
                SetHappinessState(HappinessState.Happy);
                penTimer = Time.time + 20f;
            }
            else
            {
                followingPlayer = false;
                rb.linearVelocity = Vector2.zero;
            }


        }
        if (followingPlayer)
        {
            _pathfindingFunctions.enabled = false;
            FollowingPlayer();
            if (Vector3.Distance(transform.position, penArea.position) < 0.1f)
            {
                followingPlayer = false;
                rb.linearVelocity = Vector2.zero;
                inPen = true;
            }
            return;
        }
        if (isInteracting)
        {
            if (Time.time < interactionEndTime)
            {
                horizontalMove = 0f;
                return;
            }
            else
            {
                isInteracting = false;
            }
        }

        if (happinessState == HappinessState.Agitated)
        {
            _pathfindingFunctions.enabled = true;
        }
        else if (Time.time > nextDirectionChange + randomDelay)
        {
            _pathfindingFunctions.enabled = false;
            randomDelay = Random.Range(0.2f, 1f);

            if (Random.value < idle)
            {
                horizontalMove = 0f;
                rb.linearVelocity = Vector2.zero;
            }
            else
            {
                float direction = Random.value < 0.5f ? -1 : 1;
                float randomSpeed = runSpeed + Random.Range(-speedVar, speedVar);

                if (Random.value < 0.5f)
                {
                    horizontalMove = direction * randomSpeed;
                    rb.linearVelocity = new Vector2(horizontalMove, 0f);
                }
                else
                {
                    float verticalMove = direction * randomSpeed;
                    rb.linearVelocity = new Vector2(0f, verticalMove);
                }
            }
            nextDirectionChange = Time.time + directionInterval;
        }

        if (rb.linearVelocity.x > 0f)
        {
            transform.localScale = new Vector3(Mathf.Abs(transform.localScale.x) * -1, transform.localScale.y, transform.localScale.z);
        }
        else if (rb.linearVelocity.x < 0f)
        {
            transform.localScale = new Vector3(Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Car"))
        {
            Debug.Log($"{gameObject.name} collided with a car and will be destroyed.");
            GameManager.Instance.IncrementDeaths();
            Destroy(gameObject); // Destroy the chicken on collision with the car
        }

        if (collision.gameObject.CompareTag("Player") && !isInteracting)
        {
            _pathfindingFunctions.enabled = false;
            isInteracting = true;
            interactionEndTime = Time.time + interactionTime;
            horizontalMove = 0f;
            followingPlayer = true;
            isDrowning = false;
        }
        if (!isDrowning && collision.gameObject.CompareTag("Water") && happinessState == HappinessState.Suicidal)
        {
            Debug.Log("Animal is drowning!!!");
            _pathfindingFunctions.enabled = false;
            //add a death timer
            //make animal stay still
            //if death timer runs out, destroy animal object and imcrement deaths in game manager.
            isDrowning = true;
            drowningTimer = Time.time;
            rb.linearVelocity = Vector2.zero;
        }
    }


    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (inPen)
        {
            return;
        }
        if (collision.CompareTag("Pen"))
        {
            inPen = true;
            followingPlayer = false;
            rb.linearVelocity = Vector2.zero;
            penTimer = Time.time;
            isTimerActive = true;

        }
    }
    
    public void SetHappinessState(HappinessState state)
    {
        happinessState = state;

        switch (happinessState)
        {
            case HappinessState.Happy:
                runSpeed = 1f;
                break;
            case HappinessState.Neutral:
                runSpeed = 3f;
                break;
            case HappinessState.Agitated:
                runSpeed = 4f;
                break;
            case HappinessState.Suicidal:
                runSpeed = 5f;
                break;
        }

        Debug.Log($"{gameObject.name} is now {happinessState}, speed set to {runSpeed}");
    }
    private void FollowingPlayer()
    {
        Vector2 direction = player.position - transform.position;
        if (direction.magnitude > 1.5f)
        {
            rb.linearVelocity = Vector2.Lerp(rb.linearVelocity, direction.normalized * runSpeed, Time.deltaTime * 5);

        }
        else
        {
            rb.linearVelocity = Vector2.zero;
        }
    }
    
}
