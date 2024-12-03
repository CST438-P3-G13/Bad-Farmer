using UnityEngine;

public class Cow_Animal : MonoBehaviour
{
    public Rigidbody2D rb;
    public Animator anim;
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

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        SetHappinessState(HappinessState.Happy); // Initialize happiness

        if (AnimalManager.Instance != null)
    {
        AnimalManager.Instance.animalHappiness[gameObject] = HappinessState.Happy;
    }
    }

    // Update is called once per frame
    void Update()
{
    if (isInteracting)
    {
        if (Time.time < interactionEndTime)
        {
            horizontalMove = 0f;
            anim.SetBool("isRunning", false);
            return;
        }
        else
        {
            isInteracting = false;
        }
    }

    if (Time.time > nextDirectionChange + randomDelay)
    {
        randomDelay = Random.Range(0.2f, 1f);

        if (Random.value < idle)
        {
            horizontalMove = 0f;
            rb.linearVelocity = Vector2.zero; // Stop movement
            IdleBehavior();
        }
        else
        {
            float direction = Random.value < 0.5f ? -1 : 1; // Randomly choose direction
            float randomSpeed = runSpeed + Random.Range(-speedVar, speedVar);

            if (Random.value < 0.5f) // 50% chance for horizontal or vertical movement
            {
                horizontalMove = direction * randomSpeed; // Horizontal movement
                rb.linearVelocity = new Vector2(horizontalMove, 0f); // Apply horizontal velocity
            }
            else
            {
                float verticalMove = direction * randomSpeed; // Vertical movement
                rb.linearVelocity = new Vector2(0f, verticalMove); // Apply vertical velocity
            }
        }
        nextDirectionChange = Time.time + directionInterval;
    }

    // Update animation state
    anim.SetBool("isRunning", rb.linearVelocity != Vector2.zero);

    // Flip sprite for horizontal movement
    if (rb.linearVelocity.x < 0f)
    {
        transform.localScale = new Vector3(-1, 1, 1);
    }
    else if (rb.linearVelocity.x > 0f)
    {
        transform.localScale = new Vector3(1, 1, 1);
    }
}


    private void IdleBehavior()
    {
        anim.SetTrigger("Idle");
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") && !isInteracting)
        {
            isInteracting = true;
            interactionEndTime = Time.time + interactionTime;
            horizontalMove = 0f;
            anim.SetBool("isRunning", false);
        }
    }

    void FixedUpdate()
    {
        RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector2.right * Mathf.Sign(horizontalMove), 1f);

        if (hit)
        {
            horizontalMove = -horizontalMove;
        }
        rb.linearVelocity = new Vector2(horizontalMove, rb.linearVelocity.y);
    }

    public void SetHappinessState(HappinessState state)
    {
        happinessState = state;

        switch (happinessState)
        {
            case HappinessState.Happy:
                runSpeed =5f;
                break;
            case HappinessState.Neutral:
                runSpeed = 40f;
                break;
            case HappinessState.Agitated:
                runSpeed = 60f;
                break;
            case HappinessState.Suicidal:
                runSpeed = 80f;
                break;
        }

        Debug.Log($"{gameObject.name} is now {happinessState}, speed set to {runSpeed}");
    }

}
