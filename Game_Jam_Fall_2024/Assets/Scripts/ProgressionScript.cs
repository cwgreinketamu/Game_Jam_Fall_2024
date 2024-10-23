using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class ProgressionScript : MonoBehaviour
{
    [SerializeField] GameObject zombiePrefab;
    [SerializeField] GameObject skeletonPrefab;
    [SerializeField] GameObject player;
    [SerializeField] private bool waitForWave; //keeps buffer between waves spawning
    [SerializeField] private int enemyCount; //tracks number of active enemies
    [SerializeField] private int difficulty; //tracks current difficulty to determine types of waves to spawn
    // Start is called before the first frame update
    void Start()
    {
        waitForWave = false;
        enemyCount = 0;
        difficulty = 0;
        SpawnWave();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    //Spawns wave of enemies
    void SpawnWave()
    {
        if (difficulty == 0) //first enemy spawn, 1 zombie
        {
            //spawns zombie just offscreen, camera bounds are approximately 36 x 20
            Instantiate(zombiePrefab, new Vector3(player.transform.position.x - 15, player.transform.position.y - 12, 0), Quaternion.identity);
            waitForWave = true;
            ++enemyCount;
        }
        if (difficulty > 0) //further difficulties currently spawn 3 enemies in a group, will be expanded further
        {
            if (difficulty % 2 == 1) //alternates between zombies and skeletons, could be randomized
            {
                int randomX = Random.Range(0, 2); //outputs 0 or 1, will determine if spawn is left or right side of camera
                float spawnX = player.transform.position.x + (-1 * randomX * 20); //20 is arbitrary value to spawn enemy outside camera view
                int randomY = Random.Range(0, 2);
                float spawnY = player.transform.position.y + (-1 * randomY * 12); //12 is also arbitrary
                Instantiate(zombiePrefab, new Vector3(spawnX, spawnY, 0), Quaternion.identity);
                Instantiate(zombiePrefab, new Vector3(spawnX+1, spawnY+1, 0), Quaternion.identity);
                Instantiate(zombiePrefab, new Vector3(spawnX-1, spawnY-1, 0), Quaternion.identity);
                waitForWave = true;
                enemyCount += 3;
            }
            else
            {
                int randomX = Random.Range(0, 2); //outputs 0 or 1, will determine if spawn is left or right side of camera
                float spawnX = player.transform.position.x + (-1 * randomX * 20); //20 is arbitrary value to spawn enemy outside camera view
                int randomY = Random.Range(0, 2);
                float spawnY = player.transform.position.y + (-1 * randomY * 12); //12 is also arbitrary
                Instantiate(skeletonPrefab, new Vector3(spawnX, spawnY, 0), Quaternion.identity);
                Instantiate(skeletonPrefab, new Vector3(spawnX + 1, spawnY + 1, 0), Quaternion.identity);
                Instantiate(skeletonPrefab, new Vector3(spawnX - 1, spawnY - 1, 0), Quaternion.identity);
                waitForWave = true;
                enemyCount += 3;
            }
        }
    }

    //called by enemies when they die to update enemyCount
    public void EnemyDeath()
    {
        --enemyCount;
        if (enemyCount == 0)
        {
            ++difficulty;
            waitForWave = false;
            SpawnWave();
        }
    }
}
