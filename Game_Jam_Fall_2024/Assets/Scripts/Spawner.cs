using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    public GameObject zombiePrefab;

    public GameObject skeletonPrefab;

    public GameObject player;

    public AudioSource fireHitSound;

    public AudioSource iceHitSound;

    public AudioSource waterHitSound;

    public GameObject damagePopupPrefab;

    public GameObject audioManager;


    public float spawnInterval = 5f;
    private float timer = 0f;

    public float difficultyIncreaseRate = 0.1f;
    public float minSpawnInterval = 3f;

    public Vector2 spawnAreaMin;

    public Vector2 spawnAreaMax;
    // Start is called before the first frame update
    void Start()
    {
        timer = spawnInterval;
        spawnAreaMax = player.transform.position + new Vector3(10, 10, 0);
        spawnAreaMin = player.transform.position - new Vector3(10, 10, 0);
    }

    // Update is called once per frame
    void Update()
    {
        timer -= Time.deltaTime;
        if (timer <= 0f)
        {
            Vector2 randomPosition = new Vector2(Random.Range(spawnAreaMin.x, spawnAreaMax.x), Random.Range(spawnAreaMin.y, spawnAreaMax.y));
            GameObject enemyPrefab = Random.value < 0.5f ? zombiePrefab : skeletonPrefab; //i used a ternary operator for the first time are you proud
            enemyPrefab.GetComponent<EnemyBehavior>().audioManager = audioManager;
            enemyPrefab.GetComponent<EnemyBehavior>().fireHitSound = fireHitSound;
            enemyPrefab.GetComponent<EnemyBehavior>().iceHitSound = iceHitSound;
            enemyPrefab.GetComponent<EnemyBehavior>().waterHitSound = waterHitSound;
            enemyPrefab.GetComponent<EnemyBehavior>().damagePopupPrefab = damagePopupPrefab;
            Instantiate(enemyPrefab, randomPosition, Quaternion.identity);
            timer = spawnInterval;
            spawnInterval = Mathf.Max(minSpawnInterval, spawnInterval - difficultyIncreaseRate);
        }
    }
}
