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
    private void Update()
    {
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");
        // Vector3 movement = new Vector3(horizontal, 0, vertical);
        // transform.Translate(movement * speed * Time.deltaTime);
        
        DoublePressed();
        float speed;
        if (currentlyRunning)
        {
            speed = walkingSpeed * runningSpeed;
        }
        else
        {
            speed = walkingSpeed;
        }
        
        
        Vector3 movement = new Vector3(horizontal,vertical);
        transform.position += movement * speed * Time.deltaTime;

        if (horizontal == 0 && vertical == 0)
        {
            currentlyRunning = false;
        }
        StaminaFunc();
        UpdateStaminaBar();
        
    }


    string TrackingKey()
    {
        if (Input.GetKey(KeyCode.LeftArrow))
        {
            return "Left";
        }
        if (Input.GetKey(KeyCode.RightArrow))
        {
            return "Right";
        }

        if (Input.GetKey(KeyCode.UpArrow))
        {
            return "Up";
        }

        if (Input.GetKey(KeyCode.DownArrow))
        {
            return "Down";
        }

        return "";
    }

    void DoublePressed()
    {
        if (Input.GetKeyDown(KeyCode.LeftArrow) || Input.GetKeyDown(KeyCode.RightArrow) || Input.GetKeyDown(KeyCode.UpArrow) ||
            Input.GetKeyDown(KeyCode.DownArrow))
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
                stamina -= Time.deltaTime * 10f; // 20 is the drainrate, could make it into a variable
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

    void UpdateStaminaBar()
    {
        StaminaBar.value = stamina;
    }
}
