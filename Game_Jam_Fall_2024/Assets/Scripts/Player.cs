using UnityEngine;

public class Player : MonoBehaviour
{
    public float health = 100f; // Player's health
    public float speed = 10f;

    void Update()
    {
        // Handle movement
        float horizontalInput = Input.GetAxis("Horizontal");
        float verticalInput = Input.GetAxis("Vertical");
        Vector3 direction = new Vector3(horizontalInput, verticalInput, 0);
        transform.Translate(direction* speed * Time.deltaTime);
    }

    public void TakeDamage(float damage)
    {
        health -= damage; // Reduce health by damage amount
        Debug.Log($"Player takes {damage} damage. Remaining health: {health}");

        if (health <= 0)
        {
            Die(); // Call a method to handle player death
        }
    }

    private void Die()
    {
        Debug.Log("Player has died!");
        // Implement death logic (e.g., restart game, show game over screen)
    }
}
