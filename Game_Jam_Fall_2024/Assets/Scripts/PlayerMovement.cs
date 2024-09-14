using UnityEngine;

public class TopDownMovement : MonoBehaviour
{
    public float moveSpeed = 5f; // Movement speed of the player

    private Vector3 moveDirection; // Stores the movement direction

    // Update is called once per frame
    void Update()
    {
        // Get input from WASD or arrow keys
        float moveX = Input.GetAxis("Horizontal"); // A/D or Left/Right arrow keys
        float moveY = Input.GetAxis("Vertical");   // W/S or Up/Down arrow keys

        // Set movement direction
        moveDirection = new Vector3(moveX, moveY, 0f).normalized;

        // Move the player
        transform.position += moveDirection * moveSpeed * Time.deltaTime;
    }
}
