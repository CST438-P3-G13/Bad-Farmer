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
                IdleBehavior();
            }
            else
            {
                float direction = Random.Range(-1, 2);
                float randomSpeed = runSpeed + Random.Range(-speedVar, speedVar);
                horizontalMove = direction * randomSpeed;
            }
            nextDirectionChange = Time.time + directionInterval;
        }

        anim.SetBool("isRunning", horizontalMove != 0f);

        if (horizontalMove < 0f)
        {
            transform.localScale = new Vector3(-1, 1, 1);
        }
        else if (horizontalMove > 0f)
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
        rb.velocity = new Vector2(horizontalMove, rb.velocity.y);
    }

    public void SetHappinessState(HappinessState state)
    {
        happinessState = state;

        switch (happinessState)
        {
            case HappinessState.Happy:
                runSpeed = 20f; // Slow speed
                break;
            case HappinessState.Neutral:
                runSpeed = 40f; // Normal speed
                break;
            case HappinessState.Agitated:
                runSpeed = 60f; // Faster speed
                break;
            case HappinessState.Suicidal:
                runSpeed = 80f; // Sprint speed
                break;
        }
    }
}
