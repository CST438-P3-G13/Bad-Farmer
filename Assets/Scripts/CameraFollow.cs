using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform player;
    public Vector3 offset;
    public float speed = 0.130f;

    // Update is called once per frame
    void LateUpdate()
    {
        Vector3 newPos = player.position + offset;
        Vector3 smoothPos = Vector3.Lerp(transform.position, newPos, speed);
        transform.position = smoothPos;
    }
}
