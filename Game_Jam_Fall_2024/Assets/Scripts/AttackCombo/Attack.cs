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
    [SerializeField] GameObject particlePrefab;
    public float speed = 10f;
    public float despawnTime = 3.0f;  // Time after which projectiles despawn
    private bool activeCombo = false;
    private float timeSinceLastAttack;
    public float attackCooldown = 1.0f;

    public List<string> spellBuffer = new List<string>(); // Spell input buffer

    public Texture2D fireCursor;
    public Texture2D lightningCursor;
    public Texture2D defaultCursor;
    public Texture2D iceCursor;

    public Texture2D firelightningCursor;

    public Texture2D fireiceCursor;

    public Texture2D icelightningCursor;

    public Texture2D icefireCursor;

    public Texture2D lightningiceCursor;

    public Texture2D lightningfireCursor;

    public Texture2D firefireCursor;

    public Texture2D iceiceCursor;

    public Texture2D lightninglightningCursor;

    public AudioSource fireballSound;

    public AudioSource fireballhitSound;

    public AudioSource lightningSound;

    public AudioSource iceCastSound;

    public AudioSource waterCastSound;

    public AudioSource fireLightningBoom;

    public AudioSource iceLightningBoom;

    public AudioSource iceice;



    public GameObject audioManager;



    [SerializeField] private string prevSpell = "none";


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
                //spellBuffer.Add("Fire");
                Debug.Log("Added Fire to spell buffer");
                AddSpell("Fire");
            }
            else if (Input.GetKeyDown("2"))
            {
                //spellBuffer.Add("Lightning");
                Debug.Log("Added Lightning to spell buffer");
                AddSpell("Lightning");
            }
            else if (Input.GetKeyDown("3"))
            {
                //spellBuffer.Add("Ice");
                AddSpell("Ice");
                Debug.Log("Added Ice to spell buffer");
            }
        }
        if(spellBuffer.Count == 0){
            ChangeCursor(defaultCursor);
        }

        // Confirm and cast spell when pressing right click
        if (Input.GetMouseButtonDown(1) && spellBuffer.Count > 0 || Input.GetKeyDown("space") && spellBuffer.Count > 0)
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
                InstantiateDirectionalProjectile(fireboltPrefab, directionToMouse,type: "Fire");
                if (particlePrefab != null)
                {
                    GameObject particle = Instantiate(particlePrefab, transform.position, Quaternion.identity);
                    var main = particle.GetComponent<ParticleSystem>().main;
                    main.startColor = Color.red;
                }
                audioManager.GetComponent<AudioManager>().playSound(fireballSound);

                Debug.Log("Cast Firebolt");
            }
            else if (spellBuffer[0] == "Ice")
            {
                // Ice slow beam (starter effect)
                InstantiateDirectionalProjectile(coldBeamPrefab, directionToMouse, type: "Ice");
                if (particlePrefab != null)
                {
                    GameObject particle = Instantiate(particlePrefab, transform.position, Quaternion.identity);
                    var main = particle.GetComponent<ParticleSystem>().main;
                    main.startColor = Color.cyan;
                }
                audioManager.GetComponent<AudioManager>().playSound(iceCastSound);
                Debug.Log("Cast Cold Beam (slow/stun)");
            }
            else if (spellBuffer[0] == "Lightning")
            {
                // Lightning bolt
                InstantiateDirectionalProjectile(lightningBoltPrefab, directionToMouse, type: "Lightning");
                if (particlePrefab != null)
                {
                    GameObject particle = Instantiate(particlePrefab, transform.position, Quaternion.identity);
                    var main = particle.GetComponent<ParticleSystem>().main;
                    main.startColor = Color.blue;
                }

                audioManager.GetComponent<AudioManager>().playSound(fireLightningBoom);
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
                if (particlePrefab != null)
                {
                    GameObject particle = Instantiate(particlePrefab, transform.position, Quaternion.identity);
                    var main = particle.GetComponent<ParticleSystem>().main;
                    main.startColor = Color.red;
                    GameObject particle2 = Instantiate(particlePrefab, transform.position, Quaternion.identity);
                    var main2 = particle2.GetComponent<ParticleSystem>().main;
                    main2.startColor = Color.red;
                }
                Debug.Log("Cast Flamethrower");
            }
            else if (sortedBuffer[0] == "Fire" && sortedBuffer[1] == "Ice")
            {
                // Fire Ice - Water tsunami wall
                InstantiateDirectionalProjectile(tsunamiPrefab, directionToMouse, large: true, type: "Water");
                if (particlePrefab != null)
                {
                    GameObject particle = Instantiate(particlePrefab, transform.position, Quaternion.identity);
                    var main = particle.GetComponent<ParticleSystem>().main;
                    main.startColor = Color.red;
                    GameObject particle2 = Instantiate(particlePrefab, transform.position, Quaternion.identity);
                    var main2 = particle2.GetComponent<ParticleSystem>().main;
                    main2.startColor = Color.cyan;
                }
                audioManager.GetComponent<AudioManager>().playSound(waterCastSound);
                Debug.Log("Cast Tsunami Wall");
            }
            else if (sortedBuffer[0] == "Fire" && sortedBuffer[1] == "Lightning")
            {
                // Fire Lightning - Big explosion at mouse position
                InstantiateAtPosition(explosionPrefab, new Vector3(mousePos.x, mousePos.y, 0), type: "Fire");
                if (particlePrefab != null)
                {
                    GameObject particle = Instantiate(particlePrefab, transform.position, Quaternion.identity);
                    var main = particle.GetComponent<ParticleSystem>().main;
                    main.startColor = Color.red;
                    GameObject particle2 = Instantiate(particlePrefab, transform.position, Quaternion.identity);
                    var main2 = particle2.GetComponent<ParticleSystem>().main;
                    main2.startColor = Color.blue;
                }
                audioManager.GetComponent<AudioManager>().playSound(lightningSound);
                Debug.Log("Cast Big Explosion");
            }
            else if (sortedBuffer[0] == "Ice" && sortedBuffer[1] == "Ice")
            {
                // Ice Ice - Full freeze cone
                InstantiateDirectionalProjectile(freezeConePrefab, directionToMouse, type: "IceIce");
                if (particlePrefab != null)
                {
                    GameObject particle = Instantiate(particlePrefab, transform.position, Quaternion.identity);
                    var main = particle.GetComponent<ParticleSystem>().main;
                    main.startColor = Color.cyan;
                    GameObject particle2 = Instantiate(particlePrefab, transform.position, Quaternion.identity);
                    var main2 = particle2.GetComponent<ParticleSystem>().main;
                    main2.startColor = Color.cyan;
                }
                audioManager.GetComponent<AudioManager>().playSound(iceice);
                Debug.Log("Cast Freeze Cone");
            }
            else if (sortedBuffer[0] == "Ice" && sortedBuffer[1] == "Lightning")
            {
                // Ice Lightning - Random projectiles in 4 directions
                CastAoePushBack(randomIceProjPrefab, 4);
                if (particlePrefab != null)
                {
                    GameObject particle = Instantiate(particlePrefab, transform.position, Quaternion.identity);
                    var main = particle.GetComponent<ParticleSystem>().main;
                    main.startColor = Color.cyan;
                    GameObject particle2 = Instantiate(particlePrefab, transform.position, Quaternion.identity);
                    var main2 = particle2.GetComponent<ParticleSystem>().main;
                    main2.startColor = Color.blue;
                }
                audioManager.GetComponent<AudioManager>().playSound(iceLightningBoom);
                Debug.Log("Cast Ice Lightning (random projectiles)");
            }
            else if (sortedBuffer[0] == "Lightning" && sortedBuffer[1] == "Lightning")
            {
                // Lightning Lightning - Chain lightning
                Vector3 positionVector = new Vector3(mousePos.x, mousePos.y, 0);

                InstantiateAtPosition(chainLightningPrefab, positionVector, type: "Lightning");
                audioManager.GetComponent<AudioManager>().playSound(lightningSound);
                CastChainLightning(positionVector);
                if (particlePrefab != null)
                {
                    GameObject particle = Instantiate(particlePrefab, transform.position, Quaternion.identity);
                    var main = particle.GetComponent<ParticleSystem>().main;
                    main.startColor = Color.blue;
                    GameObject particle2 = Instantiate(particlePrefab, transform.position, Quaternion.identity);
                    var main2 = particle2.GetComponent<ParticleSystem>().main;
                    main2.startColor = Color.blue;
                }
                Debug.Log("Cast Chain Lightning");
            }
        }


        spellBuffer.Clear();
        activeCombo = true;
        timeSinceLastAttack = 0f;
        Debug.Log("Spell buffer cleared after casting");
    }

    // Helper function to instantiate directional projectiles
    private void InstantiateDirectionalProjectile(GameObject prefab, Vector3 direction, string type, bool large = false)
    {
        if(type == "IceIce"){
            float duration = 2f;
            float interval = 0.5f;

            StartCoroutine(CastIceCone(prefab, direction, duration, interval, large));
        }
        else{
            Vector3 spawnPos = transform.position;
            GameObject projectile = Instantiate(prefab, spawnPos, Quaternion.identity);

            if (large)
            {
                projectile.transform.localScale *= 2.0f;
            }

            projectile.GetComponent<Rigidbody2D>().velocity = direction * speed;

            // Ensure collision with enemies
            if(type == "Fire"){
                projectile.tag = "Fire";
            }
            else if(type == "Ice"){
                projectile.tag = "Ice";
            }
            else if(type == "Water"){
                projectile.tag = "Water";
            }
            else{
                projectile.tag = "PlayerProjectile";
            }


            // Destroy the projectile after 'despawnTime' seconds
            Destroy(projectile, despawnTime);
        }

    }

    private void InstantiateAtPosition(GameObject prefab, Vector3 position, string type)
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
            audioManager.GetComponent<AudioManager>().playSound(fireballSound);
            InstantiateDirectionalProjectile(prefab, direction, type: "Fire");
            yield return new WaitForSeconds(0.1f);
        }
    }

    private IEnumerator CastIceCone(GameObject prefab, Vector3 direction, float duration, float interval, bool large = false)
    {
        float elapsedTime = 0f;
        while (elapsedTime < duration)
        {
            int numProjectiles = 5; // Number of projectiles in the cone
            float coneAngle = 45f; // Total angle of the cone in degrees
            float angleStep = coneAngle / (numProjectiles - 1); // Angle between each projectile

            for (int i = 0; i < numProjectiles; i++)
            {
                // Calculate the angle for this projectile
                float angle = -coneAngle / 2 + angleStep * i;
                Vector3 projectileDirection = Quaternion.Euler(0, 0, angle) * direction;

                // Instantiate the projectile
                GameObject projectile = Instantiate(prefab, transform.position, Quaternion.identity);

                if (large)
                {
                    projectile.transform.localScale *= 2.0f;
                }

                projectile.GetComponent<Rigidbody2D>().velocity = projectileDirection * speed;
                projectile.tag = "Ice";

                // Destroy the projectile after 'despawnTime' seconds
                Destroy(projectile, despawnTime);
            }

            elapsedTime += interval;
            yield return new WaitForSeconds(interval);
        }
    }

    public void AddSpell(string spellName)
    {
        spellBuffer.Add(spellName);
        if(spellBuffer.Count > 1){
            if(spellBuffer[0] == "Fire" && spellBuffer[1] == "Lightning"){
                ChangeCursor(firelightningCursor);
            }
            else if(spellBuffer[0] == "Lightning" && spellBuffer[1] == "Fire"){
                ChangeCursor(lightningfireCursor);
            }
            else if(spellBuffer[0] == "Fire" && spellBuffer[1] == "Ice"){
                ChangeCursor(fireiceCursor);
            }
            else if(spellBuffer[0] == "Ice" && spellBuffer[1] == "Fire"){
                ChangeCursor(icefireCursor);
            }
            else if(spellBuffer[0] == "Lightning" && spellBuffer[1] == "Ice"){
                ChangeCursor(lightningiceCursor);
            }
            else if(spellBuffer[0] == "Ice" && spellBuffer[1] == "Lightning"){
                ChangeCursor(icelightningCursor);
            }
            else if(spellBuffer[0] == "Fire" && spellBuffer[1] == "Fire"){
                ChangeCursor(firefireCursor);
            }
            else if(spellBuffer[0] == "Ice" && spellBuffer[1] == "Ice"){
                ChangeCursor(iceiceCursor);
            }
            else{
                ChangeCursor(lightninglightningCursor);
            }
        }
        else{
            if(spellBuffer[0] == "Fire"){
                ChangeCursor(fireCursor);
            }
            else if(spellBuffer[0] == "Lightning"){
                ChangeCursor(lightningCursor);
            }
            else{
                ChangeCursor(iceCursor);
            }
        }
    }

    private void CastChainLightning(Vector3 positionVector)
    {
        Debug.Log("Casting chain lightning");
        List<GameObject> hitEnemies = new List<GameObject>();
        StartCoroutine(ChainLightningCoroutine(positionVector, hitEnemies));

    }

    private IEnumerator ChainLightningCoroutine(Vector3 positionVector, List<GameObject> hitEnemies){
        GameObject currentTarget = FindClosestEnemy(positionVector, hitEnemies);
        for(int i = 0; i < 3; i++)
        {
            if (currentTarget == null)
            {
                break;
            }
            hitEnemies.Add(currentTarget);
            currentTarget = FindClosestEnemy(currentTarget.transform.position, hitEnemies);
            Debug.Log("Hello " + currentTarget.name);

            if (currentTarget != null)
            {
                yield return new WaitForSeconds(0.8f);
                if(currentTarget != null){
                    audioManager.GetComponent<AudioManager>().playSound(lightningSound);
                    InstantiateAtPosition(chainLightningPrefab, currentTarget.transform.position, type: "Lightning");
                    Debug.Log("Chaining to: " + currentTarget.name);
                }

            }



        }
    }

    private GameObject FindClosestEnemy(Vector3 position, List<GameObject> hitEnemies)
    {
        Debug.Log("Finding closest enemy");
        Collider2D[] hitColliders = Physics2D.OverlapCircleAll(position, 1000.0f);
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

    private void ChangeCursor(Texture2D cursorTexture)
    {
        Cursor.SetCursor(cursorTexture, Vector2.zero, CursorMode.Auto);
    }



}
