using UnityEngine;

public class Chicken : MonoBehaviour
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

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        SetHappinessState(HappinessState.Happy);

        if (AnimalManager.Instance != null)
        {
            AnimalManager.Instance.animalHappiness[gameObject] = HappinessState.Happy;
        }
    }

    void Update()
    {
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

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") && !isInteracting)
        {
            isInteracting = true;
            interactionEndTime = Time.time + interactionTime;
            horizontalMove = 0f;
        }
    }

    void FixedUpdate()
    {
        RaycastHit2D horizontalHit = Physics2D.Raycast(transform.position, Vector2.right * Mathf.Sign(horizontalMove), 0.5f, LayerMask.GetMask("Default"));

        if (horizontalHit.collider != null)
        {
            horizontalMove = -horizontalMove;
        }

        RaycastHit2D verticalHit = Physics2D.Raycast(transform.position, Vector2.up * Mathf.Sign(rb.linearVelocity.y), 0.5f, LayerMask.GetMask("Default"));

        if (verticalHit.collider != null)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, 0f);
        }

        rb.linearVelocity = new Vector2(horizontalMove, rb.linearVelocity.y);
    }

    public void SetHappinessState(HappinessState state)
    {
        happinessState = state;

        switch (happinessState)
        {
            case HappinessState.Happy:
                runSpeed = 2f;
                break;
            case HappinessState.Neutral:
                runSpeed = 3f;
                break;
            case HappinessState.Agitated:
                runSpeed = 5f;
                break;
            case HappinessState.Suicidal:
                runSpeed = 7f;
                break;
        }

        Debug.Log($"{gameObject.name} is now {happinessState}, speed set to {runSpeed}");
    }
}
