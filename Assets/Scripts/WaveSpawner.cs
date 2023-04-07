using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaveSpawner : MonoBehaviour
{
    [System.Serializable]
    public struct EnemyType
    {
        public GameObject prefab; // The enemy prefab
        public float value; 
    }

    [System.Serializable] public struct SpawnPoints
    {
        public Transform spawnPoint;
    }

    public SpawnPoints[] spawnPoints;
    public EnemyType[] enemyTypes; // Array of enemy types to choose from
    [SerializeField] float waveValue; // The total value of the wave
    [SerializeField] float valueIncreasePerWave; // The value increase for each wave
    public float timeBetweenSpawns; // The time between each enemy spawn
    // public Transform spawnPoint; // The point where enemies spawn
    [SerializeField] float timeBetweenWaves;

    float waveTimer;
    private float currentWaveValue; // The current value of the wave
    private float spawnTimer; // The timer for enemy spawns
    Transform randomSpawnPoint;

    void Start()
    {
        // Initialize the current wave value
        currentWaveValue = waveValue;
    }

    void Update()
    {
        

        // Check if it's time to spawn a new enemy
        if (spawnTimer <= 0f && currentWaveValue > 0f && waveTimer <= 0)
        {
            // Choose a random enemy type from the array based on its value
            float totalValue = 0f;
            foreach (EnemyType enemyType in enemyTypes)
            {
                totalValue += enemyType.value;
            }
            float randomValue = Random.Range(0f, totalValue);
            int enemyIndex = 0;
            foreach (EnemyType enemyType in enemyTypes)
            {
                if (randomValue <= enemyType.value)
                {
                    break;
                }
                randomValue -= enemyType.value;
                enemyIndex++;
            }

            randomSpawnPoint = spawnPoints[Random.Range(0, spawnPoints.Length)].spawnPoint;
            GameObject newEnemy = Instantiate(enemyTypes[enemyIndex].prefab, randomSpawnPoint.position, randomSpawnPoint.rotation);

            // Subtract the value cost of the enemy from the current wave value
            currentWaveValue -= enemyTypes[enemyIndex].value;

            // Reset the spawn timer
            spawnTimer = timeBetweenSpawns;

            Debug.Log(currentWaveValue);
        } else
        {
            if (spawnTimer > 0)
            {
                spawnTimer -= Time.deltaTime;
            }

            if (waveTimer > 0)
            {
                waveTimer -= Time.deltaTime;
            }
        }

        // Check if the wave is over
        if (currentWaveValue <= 0f)
        {
            // Increase the wave value for the next wave
            waveValue += valueIncreasePerWave;

            // Reset the current wave value and spawn timer
            currentWaveValue = waveValue;
            spawnTimer = 0f;

            waveTimer = timeBetweenWaves;
        }
    }
}

