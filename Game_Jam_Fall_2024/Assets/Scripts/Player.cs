using UnityEngine;
using UnityEngine.SceneManagement; 
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class Player : MonoBehaviour
{
    public float health = 100f; // Player's health
    public float maxHealth = 100f;
    public float speed = 10f;
    public int currentXP = 0;
    public int level = 1;
    public int xpToNextLevel = 100;

    public AudioSource spellSound;
    private Rigidbody2D rb;

    private Camera mainCamera;

    bool isZoomingIn = false;

    [SerializeField] GameObject particlePrefab;

    public Image GameOverFade;

    [SerializeField] HealthBarScript healthBarScript;
    
    private StatsManagerScript statsManager;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        mainCamera = Camera.main;
        healthBarScript = GameObject.FindGameObjectWithTag("HealthBar").GetComponent<HealthBarScript>();
        Debug.Log("health start at " + health);
        statsManager = GameObject.FindGameObjectWithTag("StatsManager").GetComponent<StatsManagerScript>();
        if (statsManager != null)
        {
            Debug.Log("holy shit its not null");
        }
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

        if (Input.GetKey(KeyCode.Mouse0))
        {
            if (!spellSound.isPlaying)
            {
                spellSound.Play();
                Debug.Log("spell sound played");
            }
        }
        else
        {
            spellSound.Stop();
        }
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
        healthBarScript.DecreaseBar(damage/maxHealth);
        if (health <= 0)
        {
            Die(); // Call a method to handle player death
        }
    }

    private void Die()
    {
        Debug.Log("Player has died!");
        // Implement death logic (e.g., restart game, show game over screen)
        statsManager.PrepareGameOverScreen();
        if(!isZoomingIn){
            isZoomingIn = true;
            StartCoroutine(ZoomInOnDeath());

        }
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

    public void AddHealth(int healthAmount)
    {
        if (health + healthAmount >= maxHealth)
        {
            health = maxHealth;
            healthBarScript.SetBar(1.0f);
        }
        else
        {
            health += healthAmount;
            healthBarScript.IncreaseBar(healthAmount/maxHealth);
        }
        Debug.Log("player health is now " + health);
        
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

    private IEnumerator ZoomInOnDeath()
    {
        float targetSize = 2f;
        float duration = 1f;
        float startSize = mainCamera.orthographicSize;

        float elapsed = 0f;

        while(elapsed < duration)
        {
            mainCamera.orthographicSize = Mathf.Lerp(startSize, targetSize, elapsed / duration);
            elapsed += Time.deltaTime;
            yield return null;
        }
        mainCamera.orthographicSize = targetSize;



        if (particlePrefab != null && transform != null)
        {
            GameObject particle = Instantiate(particlePrefab, transform.position, Quaternion.identity);
            var main = particle.GetComponent<ParticleSystem>().main;
            //main.startColor = GetRandomColor();
            SetRandomParticleColors(particle.GetComponent<ParticleSystem>(),50);
            
        }
        gameObject.GetComponent<SpriteRenderer>().enabled = false;

        yield return new WaitForSeconds(1f);

        StartCoroutine(FadeOutObject(GameOverFade, 2f));
        yield return new WaitForSeconds(2f);


        SceneManager.LoadScene("DeathScene");
    }

    private IEnumerator FadeOutObject(Image image, float duration)
    {
            Color color = image.color;
            float startAlpha = color.a;
            float elapsed = 0f;

            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                float alpha = Mathf.Lerp(startAlpha, 1f, elapsed / duration);
                color.a = alpha;
                image.color = color;
                yield return null;
            }

            // Ensure the final alpha is set to 0
            color.a = 1f;
            image.color = color;
    }

    private Color GetRandomColor()
    {
        List<Color> colors = new List<Color>
        {
            Color.red,
            Color.yellow,
            Color.cyan
        };
        int randomIndex = Random.Range(0, colors.Count);
        return colors[randomIndex];
    }

    private void SetRandomParticleColors(ParticleSystem particleSystem, int count)
    {
        for(int i = 0; i < count; i++)
        {
            ParticleSystem.EmitParams emitParams = new ParticleSystem.EmitParams();
            emitParams.startColor = GetRandomColor();
            particleSystem.Emit(emitParams, 1);
        }
    }
}
