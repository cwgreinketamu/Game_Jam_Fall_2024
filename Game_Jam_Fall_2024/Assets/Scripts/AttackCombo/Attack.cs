using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Attack : MonoBehaviour
{
    public GameObject fireboltPrefab;  // Firebolt
    public GameObject flamethrowerPrefab;  // Flamethrower
    public GameObject tsunamiPrefab;  // Tsunami wall
    public GameObject explosionPrefab;  // Explosion
    public GameObject coldBeamPrefab;  // Cold beam for slow/stun
    public GameObject freezeConePrefab;  // Freeze cone
    public GameObject randomIceProjPrefab;  // Random direction ice projectiles
    public GameObject lightningBoltPrefab;  // Lightning bolt
    public GameObject chainLightningPrefab;  // Chain Lightning
    public float speed = 10f;
    public float despawnTime = 3.0f;  // Time after which projectiles despawn
    private bool activeCombo = false;
    private float timeSinceLastAttack;
    public float attackCooldown = 1.0f;

    public List<string> spellBuffer = new List<string>(); // Spell input buffer

    [SerializeField] private Image screenBorder;
    [SerializeField] private float borderEffectDuration;

    void Start()
    {
        timeSinceLastAttack = attackCooldown;  // Allow immediate attack
    }

    void Update()
    {

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

        // Confirm and cast spell when pressing right click
        if (Input.GetMouseButtonDown(1) && spellBuffer.Count > 0)
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

        if (spellBuffer.Count == 1)
        {
            if (spellBuffer[0] == "Fire")
            {
                // Fire single firebolt (DoT)
                InstantiateDirectionalProjectile(fireboltPrefab, directionToMouse);
                Debug.Log("Cast Firebolt");
            }
            else if (spellBuffer[0] == "Ice")
            {
                // Ice slow beam (starter effect)
                InstantiateDirectionalProjectile(coldBeamPrefab, directionToMouse);
                Debug.Log("Cast Cold Beam (slow/stun)");
            }
            else if (spellBuffer[0] == "Lightning")
            {
                // Lightning bolt
                InstantiateDirectionalProjectile(lightningBoltPrefab, directionToMouse);
                Debug.Log("Cast Lightning Bolt");
            }
        }
        else if (spellBuffer.Count == 2)
        {
            List<string> sortedBuffer = new List<string>(spellBuffer);
            sortedBuffer.Sort();  // Sort the spells alphabetically

            if (sortedBuffer[0] == "Fire" && sortedBuffer[1] == "Fire")
            {
                // Fire Fire - Flamethrower
                StartCoroutine(FireContinuous(flamethrowerPrefab, directionToMouse));
                Debug.Log("Cast Flamethrower");
            }
            else if (sortedBuffer[0] == "Fire" && sortedBuffer[1] == "Ice")
            {
                // Fire Ice - Water tsunami wall
                InstantiateDirectionalProjectile(tsunamiPrefab, directionToMouse, large: true);
                Debug.Log("Cast Tsunami Wall");
            }
            else if (sortedBuffer[0] == "Fire" && sortedBuffer[1] == "Lightning")
            {
                // Fire Lightning - Big explosion at mouse position
                InstantiateAtPosition(explosionPrefab, new Vector3(mousePos.x, mousePos.y, 0));
                Debug.Log("Cast Big Explosion");
            }
            else if (sortedBuffer[0] == "Ice" && sortedBuffer[1] == "Ice")
            {
                // Ice Ice - Full freeze cone
                InstantiateDirectionalProjectile(freezeConePrefab, directionToMouse);
                Debug.Log("Cast Freeze Cone");
            }
            else if (sortedBuffer[0] == "Ice" && sortedBuffer[1] == "Lightning")
            {
                // Ice Lightning - Random projectiles in 4 directions
                CastAoePushBack(randomIceProjPrefab, 4);
                Debug.Log("Cast Ice Lightning (random projectiles)");
            }
            else if (sortedBuffer[0] == "Lightning" && sortedBuffer[1] == "Lightning")
            {
                // Lightning Lightning - Chain lightning
                Vector3 positionVector = new Vector3(mousePos.x, mousePos.y, 0);
                InstantiateAtPosition(chainLightningPrefab, positionVector);
                CastChainLightning(positionVector);
                Debug.Log("Cast Chain Lightning");
            }
        }


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

        // Ensure collision with enemies
        projectile.tag = "PlayerProjectile";

        // Destroy the projectile after 'despawnTime' seconds
        Destroy(projectile, despawnTime);
    }

    private void InstantiateAtPosition(GameObject prefab, Vector3 position)
    {
        GameObject instance = Instantiate(prefab, position, Quaternion.identity);

        // Ensure collision with enemies
        instance.tag = "PlayerProjectile";

        // Destroy the instance after 'despawnTime' seconds
        Destroy(instance, despawnTime);
    }

    private void CastAoePushBack(GameObject prefab, int numProjectiles = 8, float radius = 2.0f, float projectileSpeed = 5.0f)
    {
        Vector3 playerPos = transform.position;
        float angleStep = 360f / numProjectiles;

        for (int i = 0; i < numProjectiles; i++)
        {
            float angle = i * angleStep * Mathf.Deg2Rad;
            Vector3 spawnPos = new Vector3(
                playerPos.x + Mathf.Cos(angle) * radius,
                playerPos.y + Mathf.Sin(angle) * radius,
                playerPos.z
            );
            GameObject projectile = Instantiate(prefab, spawnPos, Quaternion.identity);
            Vector3 directionToProjectile = (spawnPos - playerPos).normalized;
            projectile.GetComponent<Rigidbody2D>().velocity = directionToProjectile * projectileSpeed;

            // Ensure collision with enemies
            projectile.tag = "PlayerProjectile";

            Destroy(projectile, 3.0f);
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

    public void AddSpell(string spellName)
    {
        spellBuffer.Add(spellName);
    }

    private void CastChainLightning(Vector3 positionVector)
    {
        Debug.Log("Casting chain lightning");
        List<GameObject> hitEnemies = new List<GameObject>();
        GameObject currentTarget = FindClosestEnemy(positionVector, hitEnemies);
        for(int i = 0; i < 3; i++)
        {
            if (currentTarget == null)
            {
                break;
            }
            hitEnemies.Add(currentTarget);
            currentTarget = FindClosestEnemy(currentTarget.transform.position, hitEnemies);

            if (currentTarget != null)
            {
                InstantiateAtPosition(chainLightningPrefab, currentTarget.transform.position);
                StartCoroutine(waiter());
                Debug.Log("Chaining to: " + currentTarget.name);
            }



        }
    }

    IEnumerator waiter(){
        yield return new WaitForSeconds(0.5f);
    }

    private GameObject FindClosestEnemy(Vector3 position, List<GameObject> hitEnemies)
    {
        Debug.Log("Finding closest enemy");
        Collider2D[] hitColliders = Physics2D.OverlapCircleAll(position, 100.0f);
        Debug.Log("Found " + hitColliders.Length + " colliders");
        GameObject closestEnemy = null;
        float closestDistance = float.MaxValue;

        foreach(Collider2D collider in hitColliders)
        {
            Debug.Log("Checking collider: " + collider.gameObject.name);
            GameObject enemy = collider.gameObject;
            
            if (enemy.CompareTag("Enemy") && !hitEnemies.Contains(enemy))
            {
                float distance = Vector3.Distance(position, enemy.transform.position);
                if (distance < closestDistance)
                {
                    closestDistance = distance;
                    closestEnemy = enemy;
                    Debug.Log("Found closest enemy: " + closestEnemy.name);
                }
            }

        }

        return closestEnemy;
    }




}
