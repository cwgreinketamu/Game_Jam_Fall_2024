using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding; // Reference the Pathfinding namespace if using A* Pathfinding Project
using TMPro;


public abstract class EnemyBehavior : MonoBehaviour
{
    // Reference to the AIPath component for movement control
    protected AIPath aiPath;

    // Common properties for all enemies
    public float health;
    public float detectionRange;

    // Popup prefab
    public GameObject damagePopupPrefab;

    private Vector3 enemyPosition;

    public GameObject xpOrbPrefab;

    [SerializeField] private GameObject particlePrefab;

    public Canvas uiCanvas; // Reference to the canvas where the popup should be shown

    private Vector3 worldEnemyPosition;

    // List to track active damage popups
    private List<GameObject> activePopups = new List<GameObject>();

    private SpriteRenderer spriteRenderer;
    private Seeker seeker;
    private BoxCollider2D boxCollider2D;

    public bool isDead = false;

    private GameObject progressionManager;
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

        // Get references to the components to disable upon death
        spriteRenderer = GetComponent<SpriteRenderer>();
        seeker = GetComponent<Seeker>();
        boxCollider2D = GetComponent<BoxCollider2D>();
        progressionManager = GameObject.FindWithTag("Progression");
    }

    protected virtual void Update()
    {
        enemyPosition = transform.position;
        TrackPlayer();

        if (health <= 0 && !isDead)
        {
            // Only call once when the enemy dies
            isDead = true;
            if (progressionManager != null)
            {
                progressionManager.GetComponent<ProgressionScript>().EnemyDeath();
            }
            StartCoroutine(HandleDeath());
        }

        worldEnemyPosition = Camera.main.WorldToScreenPoint(enemyPosition);
    }

    protected virtual void GetPosition(Vector3 position)
    {
        enemyPosition = position;
    }

    // Method to track and approach the player based on detection range
    private void TrackPlayer()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null && Vector3.Distance(transform.position, player.transform.position) < detectionRange)
        {
            aiPath.destination = player.transform.position; // Set destination to player
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Debug.Log($"Trigger detected with: {collision.gameObject.name}");

        // Check if the projectile hits the player
        if (collision.gameObject.CompareTag("PlayerProjectile") || collision.gameObject.CompareTag("Fire") || collision.gameObject.CompareTag("Ice"))
        {
            Debug.Log("Player hit enemy!");
            collision.gameObject.GetComponent<Collider2D>().enabled = false;

            if (collision.gameObject.CompareTag("Fire"))
            {
                if (particlePrefab != null)
                {
                    GameObject particle = Instantiate(particlePrefab, transform.position, Quaternion.identity);
                    var main = particle.GetComponent<ParticleSystem>().main;
                    main.startColor = Color.red;
                }
                TakeDamage(10, "Fire");
            }
            else if (collision.gameObject.CompareTag("Ice"))
            {
                if (particlePrefab != null)
                {
                    GameObject particle = Instantiate(particlePrefab, transform.position, Quaternion.identity);
                    var main = particle.GetComponent<ParticleSystem>().main;
                    main.startColor = Color.cyan;
                }
                TakeDamage(30, "Ice");
            }
            else
            {
                if (particlePrefab != null)
                {
                    GameObject particle = Instantiate(particlePrefab, transform.position, Quaternion.identity);
                    var main = particle.GetComponent<ParticleSystem>().main;
                    main.startColor = Color.blue;
                }
                TakeDamage(100, "Lightning");
            }

            CoroutineManager.Instance.StartManagedCoroutine(DestroyAfterDelay(collision.gameObject, 0.5f));
        }
    }

    private IEnumerator DestroyAfterDelay(GameObject obj, float seconds)
    {
        yield return new WaitForSeconds(seconds);
        Destroy(obj);
        Debug.Log("Player projectile destroyed after delay.");
    }

    private void TakeDamage(int damage, string type)
    {
        if (isDead) return; // Don't take more damage if already dead

        Debug.Log("Enemy Position at TakeDamage: " + transform.position);

        if (type == "Fire")
        {
            StartCoroutine(DamageOverTime(damage, 5.0f, 1.0f));
            Debug.Log("Fire damage over time");
        }
        else if (type == "Ice")
        {
            health -= damage;
            ShowDamagePopup(damage);
            aiPath.maxSpeed = 0.3f * aiPath.maxSpeed;
            Debug.Log("Slowed by ice");
        }
        else
        {
            health -= damage;
            ShowDamagePopup(damage);
        }
    }

    private void ShowDamagePopup(int damage)
    {
        if (damagePopupPrefab == null)
        {
            Debug.LogWarning("Damage Popup Prefab is not assigned.");
            return;
        }

        // Instantiate the damage popup prefab at the enemy's position
        GameObject damagePopup = Instantiate(damagePopupPrefab, transform.position, Quaternion.identity);
        activePopups.Add(damagePopup); // Track the instantiated popup

        // Get the TMP_Text component from the instantiated popup
        TMP_Text textMesh = damagePopup.GetComponentInChildren<TMP_Text>();

        if (textMesh != null)
        {
            textMesh.text = damage.ToString();
        }
        else
        {
            Debug.LogWarning("No TMP_Text component found in the damage popup prefab.");
        }

        // Optionally, if you want the popup to appear on the UI canvas:
        if (uiCanvas != null)
        {
            damagePopup.transform.SetParent(uiCanvas.transform, false);

            // Set the position of the popup in screen space, since it's on a canvas
            Vector3 screenPosition = Camera.main.WorldToScreenPoint(transform.position);
            damagePopup.transform.position = screenPosition;
        }

        // Start coroutine to animate and destroy the popup after a delay
        StartCoroutine(AnimateAndDestroyPopup(damagePopup, 1.0f));
    }

    private IEnumerator AnimateAndDestroyPopup(GameObject popup, float seconds)
    {
        TMP_Text textMesh = popup.GetComponentInChildren<TMP_Text>();
        if (textMesh == null)
        {
            yield break;
        }

        Vector3 originalPosition = textMesh.transform.position;
        Color originalColor = textMesh.color;

        float elapsed = 0f;
        while (elapsed < seconds)
        {
            float t = elapsed / seconds;

            // Fade out the text
            textMesh.color = new Color(originalColor.r, originalColor.g, originalColor.b, Mathf.Lerp(1f, 0f, t));

            elapsed += Time.deltaTime;
            yield return null;
        }

        // Ensure the popup is fully faded out and then destroy it
        textMesh.color = new Color(originalColor.r, originalColor.g, originalColor.b, 0f);
        Destroy(popup);

        // Remove from the list of active popups
        activePopups.Remove(popup);

        // If no more popups are active and enemy is dead, destroy the enemy
        if (isDead && activePopups.Count == 0)
        {
            Destroy(gameObject);
        }
    }

    private IEnumerator DamageOverTime(float damage, float seconds, float interval)
    {
        float elapsed = 0f;
        while (elapsed < seconds && health > 0)
        {
            health -= damage;
            ShowDamagePopup((int)damage);
            Debug.Log("Health: " + health);
            yield return new WaitForSeconds(interval);
            elapsed += interval;
        }
    }

    // Coroutine to handle enemy death
    private IEnumerator HandleDeath()
    {
        // Turn off enemy components so it stops interacting with the world
        if (spriteRenderer != null) spriteRenderer.enabled = false;
        if (seeker != null) seeker.enabled = false;
        if (boxCollider2D != null) boxCollider2D.enabled = false;

        DropXP(); // Drop XP before fully destroying the enemy

        // Wait until all damage popups are gone
        while (activePopups.Count > 0)
        {
            yield return null;
        }

        // Destroy the enemy after the last popup has been shown
        Destroy(gameObject);
    }

    // Method to drop XP orbs when the enemy dies
    private void DropXP()
    {
        if (xpOrbPrefab != null)
        {
            // Instantiate the XP orb prefab at the enemy's position
            Instantiate(xpOrbPrefab, transform.position, Quaternion.identity);
            Debug.Log("XP orb dropped");
        }
        else
        {
            Debug.LogWarning("XP orb prefab not assigned");
        }
    }
}
