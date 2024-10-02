using UnityEngine;
using Pathfinding; // Reference the Pathfinding namespace if using A* Pathfinding Project

public abstract class EnemyBehavior : MonoBehaviour
{
    // Reference to the AIPath component for movement control
    protected AIPath aiPath;

    // Common properties for all enemies
    public float health;
    public float detectionRange;

    // Abstract methods for specific enemy types to implement
    protected abstract void AttackPlayer();

    // Virtual method to set AI parameters that derived classes can override
    protected virtual void ConfigureMovement()
    {
        // Common configuration can go here
    }

    // Unity methods
    protected virtual void Start()
    {
        aiPath = GetComponent<AIPath>(); // Get the AIPath component
        ConfigureMovement(); // Configure movement based on enemy type
    }

    protected virtual void Update()
    {
        TrackPlayer();
    }

    // Method to track and approach the player based on detection range
    private void TrackPlayer()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null && Vector3.Distance(transform.position, player.transform.position) < detectionRange)
        {
            aiPath.destination = player.transform.position; // Set destination to player
            if (aiPath.reachedDestination)
            {
                AttackPlayer(); // Call the specific enemy's attack behavior
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Debug.Log($"Trigger detected with: {collision.gameObject.name}");

        // Check if the projectile hits the player
        if (collision.gameObject.CompareTag("PlayerProjectile"))
        {
            Debug.Log("Player hit enemy!");
            // Implement player damage logic here (if needed)
            Destroy(gameObject);
        }
    }
}
