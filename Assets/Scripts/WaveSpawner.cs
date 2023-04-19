using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class WaveSpawner : MonoBehaviour
{
    [SerializeField] UpgradeC upgradeC;
    [System.Serializable]
    public struct EnemyType
    {
        public GameObject prefab;
        public float value; 
    }

    [System.Serializable] public struct SpawnPoints
    {
        public Transform spawnPoint;
    }

    public SpawnPoints[] spawnPoints;
    public EnemyType[] enemyTypes; 
    [SerializeField] float waveValue;
    [SerializeField] float valueIncreasePerWave; 
    [SerializeField] float timeBetweenSpawns;
    [SerializeField] TMP_Text waveText;
    [SerializeField] TMP_Text enemiesLeftText;
    private float currentWaveValue;
    private float spawnTimer;
    public int waveNumber = 1;
    bool isNewWave;
    [SerializeField] float spawnPointRequiredDistance;
    [SerializeField] int wavesToLevelUp;
    int lastLeveledUpWave = 1;
    public int enemiesSpawned;
    public int killedEnemies;
    int enemiesLeft;
    bool spawnNextWave;
    

    void Start()
    {
        waveText.text = "Wave: " + waveNumber;
        currentWaveValue = waveValue;
    }

    void FixedUpdate()
    {
        if (GameManager.gameManager.isPlayerDead)
        {
            return;
        }

        if (waveNumber - lastLeveledUpWave >= wavesToLevelUp)
        {
            upgradeC.LevelUp();
            lastLeveledUpWave = waveNumber;
        }
        
        if (spawnNextWave)
        {
            enemiesLeftText.text = "Spawning...";
        } else
        {
            enemiesLeft = enemiesSpawned - killedEnemies;
            enemiesLeftText.text = "Enemies Left: " + enemiesLeft.ToString();

            if (enemiesLeft == 0)
            {
                spawnNextWave = true;
            } 
        }

        if (spawnTimer <= 0f && spawnNextWave)
        {
            if (isNewWave)
            {
                waveNumber++;
                ChangeWaveNumber(waveNumber.ToString());
                isNewWave = false;
            } else
            {

            }

            int enemyIndex = Random.Range(0, enemyTypes.Length);

            while(enemyTypes[enemyIndex].value > currentWaveValue) 
            {
                enemyIndex = Random.Range(0, enemyTypes.Length);
            }

            Transform randomSpawnPoint = spawnPoints[Random.Range(0, spawnPoints.Length)].spawnPoint;

            while (Vector2.Distance(randomSpawnPoint.position, GameManager.gameManager.player.transform.position) < spawnPointRequiredDistance)
            {
                randomSpawnPoint = spawnPoints[Random.Range(0, spawnPoints.Length)].spawnPoint;
            }

            float randomPosX = Random.Range(-2f, 2f);

            Vector2 spawnLocation = new Vector2(randomSpawnPoint.position.x + randomPosX, randomSpawnPoint.position.y);

            GameObject newEnemy = Instantiate(enemyTypes[enemyIndex].prefab, spawnLocation, randomSpawnPoint.rotation);
            enemiesSpawned++;

            currentWaveValue -= enemyTypes[enemyIndex].value;

            spawnTimer = timeBetweenSpawns;

            if (currentWaveValue <= 0f)
            {
                waveValue += valueIncreasePerWave;

                currentWaveValue = waveValue;
                spawnTimer = 0f;
                spawnNextWave = false;
                isNewWave = true;
            }

        } else
        {
            if (spawnTimer > 0)
            {
                spawnTimer -= Time.deltaTime;
            }
        }

        
    }

    public void ChangeWaveNumber(string waveNumber)
    {
        waveText.text = "Wave: " + waveNumber;
    }
}

