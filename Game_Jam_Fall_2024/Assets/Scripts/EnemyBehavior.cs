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
        if(health <= 0){
            Destroy(gameObject);
            Debug.Log("Enemy destroyed");
        }
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
            // Implement player damage logic here (if needed)
            collision.gameObject.GetComponent<Collider2D>().enabled = false;
            //Destroy(gameObject);
            if(collision.gameObject.CompareTag("Fire")){
                TakeDamage(10, "Fire");
            }
            else if(collision.gameObject.CompareTag("Ice")){
                TakeDamage(30, "Ice");
            }
            else{
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
        // Instantiate the damage popup at the enemy's position
        GameObject damagePopup = Instantiate(damagePopupPrefab, enemyPosition, Quaternion.identity);

        TMP_Text textMesh = damagePopup.GetComponentInChildren<TMP_Text>();
        if (textMesh != null)
        {
            textMesh.text = damage.ToString();
        }
        else
        {
            Debug.Log("No text mesh found");
        }

        StartCoroutine(AnimateAndDestroyPopup(damagePopup, 1.0f));
    }

    private IEnumerator AnimateAndDestroyPopup(GameObject obj, float seconds)
    {
        TMP_Text textMesh = obj.GetComponentInChildren<TMP_Text>();
        if (textMesh == null)
        {
            yield break;
        }

        Vector3 originalPosition = obj.transform.position;
        Color originalColor = textMesh.color;

        float elapsed = 0f;
        while (elapsed < seconds)
        {
            float t = elapsed / seconds;

            // Move the popup upwards
            obj.transform.position = originalPosition + Vector3.up * t;

            // Fade out the text
            textMesh.color = new Color(originalColor.r, originalColor.g, originalColor.b, Mathf.Lerp(1f, 0f, t));

            elapsed += Time.deltaTime;
            yield return null;
        }

        // Ensure the popup is fully faded out and then destroy it
        textMesh.color = new Color(originalColor.r, originalColor.g, originalColor.b, 0f);
        Destroy(obj);
    }

    private IEnumerator DamageOverTime(float damage, float seconds, float interval)
    {
        float elapsed = 0f;
        while (elapsed < seconds)
        {
            health -= damage;
            ShowDamagePopup((int)damage);
            Debug.Log("Health: " + health);
            yield return new WaitForSeconds(interval);
            elapsed += interval;
        }
    }

    private IEnumerator DestroyPopupAfterDelay(GameObject obj, float seconds)
    {
        yield return new WaitForSeconds(seconds);
        Destroy(obj);
    }
}
