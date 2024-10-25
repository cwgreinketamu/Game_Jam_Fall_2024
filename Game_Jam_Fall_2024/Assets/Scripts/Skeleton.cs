using UnityEngine;

public class Skeleton : EnemyBehavior
{
    public GameObject projectilePrefab; // Reference to the projectile prefab
    public float projectileSpeed = 10f; // Speed of the projectile
    public float attackCooldown = 2f; // Time between attacks
    private float lastAttackTime; // Tracks time since last attack

    public float movementSpeed = 3.0f; // Movement speed of the skeleton

    
    protected override void ConfigureMovement()
    {
        base.ConfigureMovement();
        // Set skeleton-specific movement parameters
    }
    

    protected override void AttackPlayer()
    {
        // Check if the cooldown period has passed since the last attack
        if (Time.time > lastAttackTime + attackCooldown && !isDead)
        {
            Debug.Log("Skeleton fires a projectile at the player!");
            // Implement projectile firing logic
            FireProjectile();
            lastAttackTime = Time.time; // Update the last attack time
        }
    }

    private void FireProjectile()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            // Calculate the direction to the player (2D vector)
            Vector2 direction = (player.transform.position - transform.position).normalized;

            // Instantiate the projectile at the enemy's position
            GameObject projectile = Instantiate(projectilePrefab, transform.position, Quaternion.identity);

            // Set the projectile's rotation to point towards the player in 2D
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            projectile.transform.rotation = Quaternion.Euler(0, 0, angle);  // Rotate only on the Z-axis for 2D

            // Set the projectile's velocity
            Rigidbody2D rb = projectile.GetComponent<Rigidbody2D>();
            if (rb != null)
            {
                rb.velocity = direction * projectileSpeed; // Use Rigidbody2D for movement in 2D space
            }
        }
    }




    protected override void Update()
    {
        base.GetPosition(transform.position);
        base.Update();
        HandleRangedAttack();
    }

    // Additional logic for ranged attack behavior
    private void HandleRangedAttack()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            float distanceToPlayer = Vector3.Distance(transform.position, player.transform.position);

            // Check if the enemy is within attack range but not too close
            if (distanceToPlayer <= aiPath.endReachedDistance && !aiPath.pathPending)
            {
                AttackPlayer();
            }
        }
    }
}
