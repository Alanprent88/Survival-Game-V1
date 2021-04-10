using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerUpSpawner : MonoBehaviour
{
    public Transform[] spawnPoints;
    float spawnTimer;
    public GameObject cannonSpawner;
    public GameObject bulletSpawner;
    public GameObject bombSpawner;
    public GameObject coinSpawner;
    public int maxPowerUpObjects = 12;
    int currentObj = 0;
    public float maxSpawnTimer = 20.0f;



    // Update is called once per frame
    void Update()
    {
        spawnTimer += Time.deltaTime;
    }

    void SpawnPowerUp()
    {
        currentObj = currentObj + 1;

        int spawnPointIndex = Random.Range(0, spawnPoints.Length);

        int objectToSpawnNo = Random.Range(0, 8);

        GameObject SpawnOB = null;

        switch (objectToSpawnNo)
        {
            case 2:
                SpawnOB = cannonSpawner;
                break;
            case 4:
                SpawnOB = bulletSpawner;
                break;
            case 7:
                SpawnOB = bombSpawner;
                break;
            default:
                SpawnOB = coinSpawner;
                break;
        }

        Instantiate(SpawnOB, spawnPoints[spawnPointIndex].position, spawnPoints[spawnPointIndex].rotation);

        if(currentObj > maxPowerUpObjects)
        {
            currentObj = 0;

            spawnTimer = 0.0f;
        }
    }

}
