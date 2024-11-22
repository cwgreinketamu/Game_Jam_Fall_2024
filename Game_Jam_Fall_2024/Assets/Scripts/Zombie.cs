using UnityEngine;

public class Zombie : EnemyBehavior
{
    public float attackDamage = 10f; // Melee attack damage
    public float attackRange = 1.5f; // Range within which the zombie can attack the player
    private Transform player; // Reference to the player's transform
    private float attackCooldown = 1f; // Time between attacks
    private float lastAttackTime; // Time when the last attack was made
    private bool isAttacking = false; // Track if the zombie is currently attacking
    public Vector3 position;

    //variables used for animation
    public Animator anim; //animator component on skeleton

    protected override void Start()
    {
        base.Start();
        player = GameObject.FindGameObjectWithTag("Player").transform; // Find the player by tag
    }

    
    protected override void ConfigureMovement()
    {
        base.ConfigureMovement();
    }
    

    protected override void Update()
    {
        base.GetPosition(transform.position);
        base.Update();
        CheckAttack(); // Check if the zombie can attack the player
        //animator direction float management
        //Rigidbody rb = GetComponent<Rigidbody>();
        //float movementx = rb.velocity.x;
        float movementx = GetComponent<Pathfinding.AIPath>().desiredVelocity.x;
        anim.SetFloat("Horizontal", movementx);
        float speedx = Mathf.Abs(movementx);
        anim.SetFloat("Speed", speedx);
    }


    private void CheckAttack()
    {
        Debug.Log("Zombie attacks the player with melee!");
        float distanceToPlayer = Vector2.Distance(transform.position, player.position);
        
        // If within attack range and cooldown period has passed, attack
        if (distanceToPlayer <= attackRange && Time.time >= lastAttackTime + attackCooldown)
        {
            if (!isAttacking && !isDead) // Only attack if not already attacking
            {
                isAttacking = true; // Set the attacking flag
                AttackPlayer();
            }
        }
        else
        {
            Debug.Log($"{distanceToPlayer},{attackRange},{lastAttackTime},{attackCooldown}");
            isAttacking = false; // Reset attacking state if out of range
        }
    }

    protected override void AttackPlayer()
    {

        Player player1 = player.GetComponent<Player>();
        if (player1 != null)
        {
            Debug.Log("Zombie attacks the player with melee!");
            //Play Attack Animation
            anim.SetTrigger("Attack");
            anim.SetFloat("Speed", 0);
            player1.TakeDamage(attackDamage);
        }
        lastAttackTime = Time.time; // Update the last attack time
    }

}
