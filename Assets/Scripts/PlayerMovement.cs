using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public float speed;

    private void Update()
    {
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");
        // Vector3 movement = new Vector3(horizontal, 0, vertical);
        // transform.Translate(movement * speed * Time.deltaTime);
        Vector3 movement = new Vector3(horizontal,vertical);
        transform.position += movement * speed * Time.deltaTime;
    }
}
