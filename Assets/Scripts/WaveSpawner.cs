using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class WaveSpawner : MonoBehaviour
{

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
    public float timeBetweenSpawns;
    [SerializeField] float timeBetweenWaves;
    [SerializeField] TMP_Text waveText;
    [SerializeField] TMP_Text nextWaveTimeText;
    float timeTillNextWave;
    float waveTimer;
    private float currentWaveValue;
    private float spawnTimer;
    public int waveNumber = 1;
    bool isNewWave;
    [SerializeField] float spawnPointRequiredDistance;
    

    void Start()
    {
        waveText.text = "Wave: " + waveNumber;
        currentWaveValue = waveValue;
    }

    void Update()
    {
        if (GameManager.gameManager.isPlayerDead)
        {
            return;
        }

        if (waveTimer > 0)
        {
            nextWaveTimeText.text = "Next wave in: " + Mathf.Round(waveTimer);
        } else
        {
            nextWaveTimeText.text = "Next wave in: 0";
        }

        if (spawnTimer <= 0f && currentWaveValue > 0f && waveTimer <= 0)
        {
            if (isNewWave)
            {
                ChangeWaveNumber(waveNumber.ToString());
                isNewWave = false;
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

            currentWaveValue -= enemyTypes[enemyIndex].value;

            spawnTimer = timeBetweenSpawns;

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

        if (currentWaveValue <= 0f)
        {
            waveValue += valueIncreasePerWave;

            currentWaveValue = waveValue;
            spawnTimer = 0f;

            waveTimer = timeBetweenWaves;

            waveNumber++;

            isNewWave = true;
        }
    }

    public void ChangeWaveNumber(string waveNumber)
    {
        waveText.text = "Wave: " + waveNumber;
    }
}

