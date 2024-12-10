using UnityEngine;
// using UnityEngine.UIElements;
using UnityEngine.UI;

public class PlayerMovement : MonoBehaviour
{
    // public float speed;
    public float walkingSpeed = 1f;
    public float runningSpeed = 1.5f;
    public string previousKey = "";
    public float previousKeyTime = 0f;
    public bool currentlyRunning = false;
    public float stamina = 100f;
    public Slider StaminaBar;
    public float interactionRadius = 2f; 
    public LayerMask interactableLayer; 

    private void Update()
{
    // Get input values for movement
    float horizontal = Input.GetAxisRaw("Horizontal"); // Use GetAxisRaw for instant response
    float vertical = Input.GetAxisRaw("Vertical");

    // Calculate movement direction
    Vector3 movement = new Vector3(horizontal, vertical, 0f).normalized; // Normalize for consistent speed in diagonal movement

    // Determine speed
    DoublePressed(); // Check for double-tap to sprint
    float speed = currentlyRunning ? walkingSpeed + runningSpeed : walkingSpeed;

    // Apply movement
    transform.position += speed * Time.deltaTime * movement;

    // If no input, stop sprinting
    if (movement == Vector3.zero)
    {
        currentlyRunning = false;
    }

    // Handle stamina logic
    StaminaFunc();
    UpdateStaminaBar();

    // Interaction logic (complete tasks)
    if (Input.GetKeyDown(KeyCode.E))
    {
        TryCompleteTask();
    }
}



    string TrackingKey()
    {
        if (Input.GetKey(KeyCode.LeftArrow) || Input.GetKeyDown(KeyCode.A))
        {
            return "Left";
        }
        if (Input.GetKey(KeyCode.RightArrow) || Input.GetKeyDown(KeyCode.D))
        {
            return "Right";
        }

        if (Input.GetKey(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.W))
        {
            return "Up";
        }

        if (Input.GetKey(KeyCode.DownArrow) || Input.GetKeyDown(KeyCode.S))
        {
            return "Down";
        }

        return "";
    }

    void DoublePressed()
    {
        if (Input.GetKeyDown(KeyCode.LeftArrow) || Input.GetKeyDown(KeyCode.RightArrow) || Input.GetKeyDown(KeyCode.UpArrow) ||
            Input.GetKeyDown(KeyCode.DownArrow) || Input.GetKeyDown(KeyCode.W)|| Input.GetKeyDown(KeyCode.A)|| Input.GetKeyDown(KeyCode.S)|| Input.GetKeyDown(KeyCode.D))
        {
            string activeKey = TrackingKey();
            if (activeKey == previousKey && (Time.time - previousKeyTime) <= 1f) //0.5 is the time which it takes to consider if its a double press or not
            {
                currentlyRunning = true;
            }
            else
            {
                currentlyRunning = false;
            }
            previousKeyTime = Time.time;
            previousKey = activeKey;
        }
    }

    void StaminaFunc()
    {
        if (currentlyRunning)
        {
            if (stamina > 0)
            {
                stamina -= Time.deltaTime * 30f; // 20 is the drainrate, could make it into a variable
                if (stamina <= 0)
                {
                   
                    stamina = 0;
                    currentlyRunning = false;
                }
            }
        }
        else
        {
            if (stamina < 100)
            {
                stamina += Time.deltaTime * 20f; //this time 20 would be regen
            }
            // stamina += Time.deltaTime * 20f; //this time 20 would be regen
        }
    }

    void TryCompleteTask() {
        Collider2D[] nearbyAnimals = Physics2D.OverlapCircleAll(transform.position, interactionRadius, interactableLayer);

        foreach(var obj in nearbyAnimals){
            if(obj.CompareTag("Cow") && TaskManager.Instance.tasks.Exists(t => t.description == "Milking the cow")) {
                TaskManager.Instance.CompleteTask("Milking the cow");
                return;
            }
            else if(obj.CompareTag("Pig") && TaskManager.Instance.tasks.Exists(t => t.description == "Cleaning the pig")) {
                TaskManager.Instance.CompleteTask("Cleaning the pig");
                return;
            }
            else if(obj.CompareTag("Chicken") && TaskManager.Instance.tasks.Exists(t => t.description == "Defeathering a chicken")) {
                TaskManager.Instance.CompleteTask("Defeathering a chicken");
                return;
            }
            else if (obj.gameObject.CompareTag("Crops") && TaskManager.Instance.tasks.Exists(t => t.description == "Watering the crops"))
            {
                TaskManager.Instance.CompleteTask("Watering the crops");
                return;
            }
        }
        Debug.Log("No tasks to complete nearby.");
    }

    void UpdateStaminaBar()
{
    if (StaminaBar == null)
    {
        Debug.LogWarning("StaminaBar is not assigned in the Inspector.");
        return;
    }
    StaminaBar.value = stamina;
}

    
    private void OnDrawGizmos(){
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position,interactionRadius);
    }
}
