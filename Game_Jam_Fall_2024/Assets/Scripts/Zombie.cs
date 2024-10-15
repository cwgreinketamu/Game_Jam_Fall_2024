using UnityEngine;

public class Zombie : EnemyBehavior
{
    public float attackDamage = 10f; // Melee attack damage
    public float attackRange = 1.5f; // Range within which the zombie can attack the player
    private Transform player; // Reference to the player's transform
    private float attackCooldown = 1f; // Time between attacks
    private float lastAttackTime; // Time when the last attack was made
    private bool isAttacking = false; // Track if the zombie is currently attacking

    protected override void Start()
    {
        base.Start();
        player = GameObject.FindGameObjectWithTag("Player").transform; // Find the player by tag
    }

    
    protected override void ConfigureMovement()
    {
        base.ConfigureMovement();
        aiPath.maxSpeed = 4.5f; // Set zombie-specific movement parameters
        aiPath.slowdownDistance = 3.0f;
        aiPath.endReachedDistance = 2.0f;
    }
    

    protected override void Update()
    {
        base.GetPosition(transform.position);
        base.Update();
        CheckAttack(); // Check if the zombie can attack the player

        if (!isAttacking)
        {
            // Move towards the player when not attacking
            aiPath.target = player; // Set the player as the target for AIPath
        }
        else
        {
            // Stop moving or prevent AIPath from updating while attacking
            aiPath.target = null; // Stop following the player while attacking
        }
    }


    private void CheckAttack()
    {
        float distanceToPlayer = Vector2.Distance(transform.position, player.position);

        // If within attack range and cooldown period has passed, attack
        if (distanceToPlayer <= attackRange && Time.time >= lastAttackTime + attackCooldown)
        {
            if (!isAttacking) // Only attack if not already attacking
            {
                isAttacking = true; // Set the attacking flag
                AttackPlayer();
            }
        }
        else
        {
            isAttacking = false; // Reset attacking state if out of range
        }
    }

    protected override void AttackPlayer()
    {
        Debug.Log("Zombie attacks the player with melee!");

        Player player1 = player.GetComponent<Player>();
        if (player1 != null)
        {
            player1.TakeDamage(attackDamage);
        }

        lastAttackTime = Time.time; // Update the last attack time
    }

}
