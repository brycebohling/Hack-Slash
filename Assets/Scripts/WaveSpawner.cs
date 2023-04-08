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
    float waveTimer;
    private float currentWaveValue;
    private float spawnTimer;
    int waveNumber = 1;
    bool isNewWave;

    void Start()
    {
        waveText.text = "Wave: " + waveNumber;
        currentWaveValue = waveValue;
    }

    void Update()
    {
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

            GameObject newEnemy = Instantiate(enemyTypes[enemyIndex].prefab, randomSpawnPoint.position, randomSpawnPoint.rotation);

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

