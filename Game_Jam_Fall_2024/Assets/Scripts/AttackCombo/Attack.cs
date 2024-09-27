using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Attack : MonoBehaviour
{
    public GameObject fireboltPrefab;  // Firebolt
    public GameObject flamethrowerPrefab;  // Flamethrower
    public GameObject tsunamiPrefab;  // Tsunami wall
    public GameObject explosionPrefab;  // Explosion

    public GameObject iceProjectilePrefab;  // Small ice projectile
    public GameObject freezeConePrefab;  // Ice cone
    public GameObject randomIceProjPrefab;  // Random direction ice projectiles

    public GameObject lightningBoltPrefab;  // Lightning bolt
    public GameObject lightningIceComboPrefab;  // Ice Lightning combo

    public float speed = 10f;
    public float despawnTime = 3.0f;  // Time after which projectiles despawn
    private bool activeCombo = false;
    private float timeSinceLastAttack;
    public float attackCooldown = 1.0f;

    public List<string> spellBuffer = new List<string>(); // Spell input buffer

    void Start()
    {
        timeSinceLastAttack = attackCooldown;  // Allow immediate attack
    }

    void Update()
    {
        // Handle movement
        float horizontalInput = Input.GetAxis("Horizontal");
        float verticalInput = Input.GetAxis("Vertical");
        Vector3 direction = new Vector3(horizontalInput, verticalInput, 0);
        transform.Translate(direction * speed * Time.deltaTime);

        // Handle spell input (limit to 2 spells in the buffer)
        if (spellBuffer.Count < 2)
        {
            if (Input.GetKeyDown("1"))
            {
                spellBuffer.Add("Fire");
                Debug.Log("Added Fire to spell buffer");
            }
            else if (Input.GetKeyDown("2"))
            {
                spellBuffer.Add("Lightning");
                Debug.Log("Added Lightning to spell buffer");
            }
            else if (Input.GetKeyDown("3"))
            {
                spellBuffer.Add("Ice");
                Debug.Log("Added Ice to spell buffer");
            }
        }

        // Confirm and cast spell when pressing '4'
        if (Input.GetKeyDown("4") && spellBuffer.Count > 0)
        {
            CastSpell();
        }

        // Clear the buffer if '5' is pressed
        if (Input.GetKeyDown("5"))
        {
            spellBuffer.Clear();
            Debug.Log("Spell buffer cleared");
        }
    }

    private void CastSpell()
    {
        Debug.Log("Casting spell with buffer: " + string.Join(", ", spellBuffer));

        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector3 directionToMouse = (mousePos - transform.position).normalized;

        // Example combinations:
        if (spellBuffer.Count == 1 && spellBuffer[0] == "Fire")
        {
            // Cast single Fire spell (Firebolt)
            InstantiateDirectionalProjectile(fireboltPrefab, directionToMouse);
            Debug.Log("Cast Firebolt");
        }
        else if (spellBuffer.Count == 2)
        {
            // Sort the spellBuffer for 2-spell combinations
            List<string> sortedBuffer = new List<string>(spellBuffer);
            sortedBuffer.Sort();  // Sort the spells alphabetically

            // Handle sorted combinations:
            if (sortedBuffer[0] == "Fire" && sortedBuffer[1] == "Fire")
            {
                // Cast Fire Fire (Flamethrower)
                StartCoroutine(FireContinuous(flamethrowerPrefab, directionToMouse));
                Debug.Log("Cast Flamethrower");
            }
            else if (sortedBuffer[0] == "Fire" && sortedBuffer[1] == "Ice")
            {
                // Cast Fire Ice or Ice Fire (Tsunami)
                InstantiateDirectionalProjectile(tsunamiPrefab, directionToMouse, large: true);
                Debug.Log("Cast Tsunami");
            }
            else if (sortedBuffer[0] == "Fire" && sortedBuffer[1] == "Lightning")
            {
                // Cast Fire Lightning or Lightning Fire (Explosion at mouse position)
                InstantiateAtPosition(explosionPrefab, new Vector3(mousePos.x, mousePos.y, 0));
                Debug.Log("Cast Explosion at mouse position");
            }
            else if (sortedBuffer[0] == "Lightning" && sortedBuffer[1] == "Lightning")
            {
                // Cast Fire Lightning or Lightning Fire (Explosion at mouse position)
                InstantiateAtPosition(lightningBoltPrefab, new Vector3(mousePos.x, mousePos.y, 0));
                Debug.Log("Cast Chain Lightning at mouse position");
            }
            else if (sortedBuffer[0] == "Ice" && sortedBuffer[1] == "Ice")
            {
                // Cast Ice Ice (Freeze cone)
                InstantiateDirectionalProjectile(freezeConePrefab, directionToMouse);
                Debug.Log("Cast Freeze Cone");
            }
            else if (sortedBuffer[0] == "Ice" && sortedBuffer[1] == "Lightning")
            {
                CastAoePushBack(randomIceProjPrefab);
                Debug.Log("Cast Aoe Ice");
            }
        }
        else if (spellBuffer.Count == 1 && spellBuffer[0] == "Ice")
        {
            // Cast single Ice spell (Ice projectile)
            InstantiateDirectionalProjectile(iceProjectilePrefab, directionToMouse);
            Debug.Log("Cast Ice Projectile");
        }
        else if (spellBuffer.Count == 1 && spellBuffer[0] == "Lightning")
        {
            // Cast single Lightning spell (Lightning bolt)
            InstantiateDirectionalProjectile(lightningBoltPrefab, directionToMouse);
            Debug.Log("Cast Lightning Bolt");
        }

        // Reset the spell buffer and combo status after casting
        spellBuffer.Clear();
        activeCombo = true;
        timeSinceLastAttack = 0f;
        Debug.Log("Spell buffer cleared after casting");
    }


    // Helper function to instantiate directional projectiles
    private void InstantiateDirectionalProjectile(GameObject prefab, Vector3 direction, bool large = false)
    {
        Vector3 spawnPos = transform.position;
        GameObject projectile = Instantiate(prefab, spawnPos, Quaternion.identity);

        if (large)
        {
            projectile.transform.localScale *= 2.0f;
        }

        projectile.GetComponent<Rigidbody2D>().velocity = direction * speed;

        // Destroy the projectile after 'despawnTime' seconds
        Destroy(projectile, despawnTime);
    }

    private void InstantiateAtPosition(GameObject prefab, Vector3 position)
    {
        GameObject instance = Instantiate(prefab, position, Quaternion.identity);

        // Destroy the instance after 'despawnTime' seconds
        Destroy(instance, despawnTime);
    }

    private void CastAoePushBack(GameObject prefab, int numProjectiles = 8, float radius = 2.0f, float projectileSpeed = 5.0f)
    {
        Vector3 playerPos = transform.position;  // Player's position
        float angleStep = 360f / numProjectiles;  // Angle between each projectile

        // Loop to instantiate projectiles in a circle
        for (int i = 0; i < numProjectiles; i++)
        {
            // Calculate the angle in radians
            float angle = i * angleStep * Mathf.Deg2Rad;

            // Calculate the spawn position for the projectile
            Vector3 spawnPos = new Vector3(
                playerPos.x + Mathf.Cos(angle) * radius,
                playerPos.y + Mathf.Sin(angle) * radius,
                playerPos.z);

            // Instantiate the projectile at the calculated position
            GameObject projectile = Instantiate(prefab, spawnPos, Quaternion.identity);

            // Calculate the direction from the player to the spawn position
            Vector3 directionToProjectile = (spawnPos - playerPos).normalized;

            // Apply velocity to the projectile to send it outward
            projectile.GetComponent<Rigidbody2D>().velocity = directionToProjectile * projectileSpeed;

            // Optionally set the projectile to despawn after a certain time
            Destroy(projectile, 3.0f);  // Destroy after 3 seconds (adjust time as needed)
        }

        Debug.Log("Cast AOE Push-Back Attack");
    }


    private IEnumerator FireContinuous(GameObject prefab, Vector3 direction)
    {
        for (int i = 0; i < 10; i++)
        {
            InstantiateDirectionalProjectile(prefab, direction);
            yield return new WaitForSeconds(0.1f);
        }
    }
}
