using UnityEngine;

public class Projectile : MonoBehaviour
{
    public float speed = 10f; // Speed of the projectile
    public float lifetime = 3f; // Time before the projectile despawns
    private Rigidbody2D rb; // Reference to Rigidbody2D component

    private void Start()
    {
        // Get the Rigidbody2D component attached to the projectile
        rb = GetComponent<Rigidbody2D>();

        // Check if the Rigidbody2D is found and apply velocity
        if (rb != null)
        {
            // Calculate the direction using the right of the transform (assuming right is forward for 2D projectiles)
            Vector2 moveDirection = transform.right; // This assumes the projectile's "forward" is set to the right direction
            rb.velocity = moveDirection * speed;

            Debug.Log($"Projectile Velocity Set: {rb.velocity}");
        }
        else
        {
            Debug.LogWarning("Rigidbody2D component missing on the projectile!");
        }

        // Destroy the projectile after `lifetime` seconds
        Destroy(gameObject, lifetime);
    }

    private void FixedUpdate()
    {
        // Fallback for manual movement if Rigidbody2D is not assigned or if velocity is not set correctly
        if (rb == null || rb.velocity == Vector2.zero)
        {
            transform.Translate(Vector2.right * speed * Time.fixedDeltaTime);
            Debug.Log("Fallback movement used for the projectile.");
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Debug.Log($"Trigger detected with: {collision.gameObject.name}");

        Player player = collision.gameObject.GetComponent<Player>();

        // Check if the projectile hits the player
        if (collision.gameObject.CompareTag("Player"))
        {
            Debug.Log("Projectile triggered with the player!");
            player.TakeDamage(5f);
            Destroy(gameObject);
        }
        else if (collision.gameObject.CompareTag("Environment"))
        {
            // Destroy the projectile if it hits any other object
            Destroy(gameObject);
        }
    }
}
