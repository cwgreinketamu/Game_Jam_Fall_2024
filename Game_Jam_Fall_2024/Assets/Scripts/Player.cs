using UnityEngine;

public class Player : MonoBehaviour
{
    public float health = 100f; // Player's health
    public float speed = 10f;
    public int currentXP = 0;
    public int level = 1;
    public int xpToNextLevel = 100;
    private Rigidbody2D rb;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        // Gather input in the Update method for more responsive input handling
        float horizontalInput = Input.GetAxisRaw("Horizontal");
        float verticalInput = Input.GetAxisRaw("Vertical");

        // Calculate movement direction based on WASD input
        Vector2 direction = new Vector2(horizontalInput, verticalInput).normalized;

        // Apply movement
        Move(direction);
    }

    void Move(Vector2 direction)
    {
        // Move the player using Rigidbody2D for smoother and more stable physics updates
        rb.velocity = direction * speed;
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

    // Method to add XP to the player
    public void AddXP(int xpAmount)
    {
        currentXP += xpAmount;

        // Check if the player has enough XP to level up
        if (currentXP >= xpToNextLevel)
        {
            LevelUp();
        }

        Debug.Log($"Current XP: {currentXP} / {xpToNextLevel}");
    }

    // Method to handle player leveling up
    private void LevelUp()
    {
        level++;
        currentXP -= xpToNextLevel;  // Reset XP after level-up
        xpToNextLevel += 50;  // Increase the XP needed for the next level
        Debug.Log($"Player leveled up! New Level: {level}");
        if (level == 2)
        {
            //unlock 2nd spell, trigger tutorial sequence
        }
        else if (level == 3)
        {
            //unlock 3rd spell, trigger tutorial sequence
        }
    }
}
