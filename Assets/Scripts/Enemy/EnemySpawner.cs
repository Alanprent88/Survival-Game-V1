using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [Header("Spawning")]
    public Transform[] spawnPoints;

    [Header("SpawnInfo")]
    public int currentWave = 1;
    int maxWeakEnemiesToSpawn;
    int maxStrongEnemiesToSpawn;
    public int startingWeakEnemies;
    public int startingStrongEnemies;
    int currentStrongEnemies = 0;
    int currentWeakEnemies = 0;
    int currentEnemies = 0;

    [Header("Enemy Types")]
    public GameObject WeakEnemy;
    public GameObject StrongEnemies;
    bool keepSpawningWeak;
    bool keepSpawningStrong;

    // Start is called before the first frame update
    void Awake()
    {
        maxWeakEnemiesToSpawn = startingWeakEnemies;
        maxStrongEnemiesToSpawn = startingStrongEnemies;
    }

    public void UpdateEnemyCounter()
    {
        currentEnemies = currentEnemies - 1;
    }

    void Spawn()
    {
        if(currentWeakEnemies <= maxWeakEnemiesToSpawn)
        {
            currentWeakEnemies = currentWeakEnemies + 1;
            currentEnemies = currentEnemies + 1;
            SpawnWeak();
        }
        if(currentStrongEnemies <= maxStrongEnemiesToSpawn && currentWave >=2)
        {
            currentStrongEnemies = currentStrongEnemies + 1;
            currentEnemies = currentEnemies + 1;
            SpawnStrong();
        }
    }

    void SpawnWeak()
    {
        int spawnPointIndex = Random.Range(0, spawnPoints.Length);

        Instantiate(WeakEnemy, spawnPoints[spawnPointIndex].position, spawnPoints[spawnPointIndex].rotation);

    }
    void SpawnStrong()
    {
        int spawnPointIndex = Random.Range(0, spawnPoints.Length);

        Instantiate(StrongEnemies, spawnPoints[spawnPointIndex].position, spawnPoints[spawnPointIndex].rotation);
    }
    void NewWave()
    {
        currentWave = currentWave + 1;

        currentWeakEnemies = 0;
        currentStrongEnemies = 0;

        currentEnemies = 0;

        maxWeakEnemiesToSpawn = maxWeakEnemiesToSpawn * 2;

        if(currentWave >= 2 )
        {
            maxStrongEnemiesToSpawn = maxStrongEnemiesToSpawn + 2;
        }
    }

    // Update is called once per frame
    void Update()
    {
        Spawn();

        if(currentEnemies < 1)
        {
            NewWave();
        }
    }
}
