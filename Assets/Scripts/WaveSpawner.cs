using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class WaveSpawner : MonoBehaviour
{
    public static WaveSpawner waveSpawner { get; private set; }
    [SerializeField] UpgradeC upgradeC;

    [System.Serializable]
    public struct EnemyType
    {
        public GameObject prefab;
        public float value; 
        public int spawnableWave;
    }

    [SerializeField] GameObject ivanPrefab;
    [SerializeField] Transform ivanSpawnPoint;
    public List<Transform> spawnPoints = new List<Transform>();
    public EnemyType[] enemyTypes; 
    [SerializeField] float waveValue;
    [SerializeField] float valueIncreasePerWave; 
    [SerializeField] float timeBetweenSpawns;
    [SerializeField] float spawnSpeedReductionTime;
    [SerializeField] float minSpawnSpeed;
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
    [SerializeField] int bossSpawnWave;
    [SerializeField] GameObject toBossTeleporter;

    // Tree stuff

    [SerializeField] private List<Transform> treeSpawns = new List<Transform>();
    private List<Transform> treePositions = new List<Transform>();
    [SerializeField] Transform tree;
    [SerializeField] float treeSpawnPercent;

    // Bush stuff
    
    [SerializeField] private List<Transform> bushSpawns = new List<Transform>();
    private List<Transform> bushPositions = new List<Transform>();
    [SerializeField] Transform bush;
    [SerializeField] float bushSpawnPercent;

    // Boss
    public bool inBossFight;


    private void Awake() 
    {
        if (waveSpawner != null && waveSpawner != this)
        {
            Debug.Log("More than one WaveSpawner in a scene!");
        } else
        {
            waveSpawner = this;
        }    
    }

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

        if (inBossFight)
        {
            return;
        }

        if (waveNumber - lastLeveledUpWave >= wavesToLevelUp)
        {   
            if (upgradeC.powerUps.Count > 2)
            {
                StartCoroutine(upgradeC.LevelUp());
            }

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


                if (waveNumber == bossSpawnWave)
                {
                    GameObject teleporter = Instantiate(toBossTeleporter, GameManager.gameManager.playerPos, Quaternion.identity);
                    teleporter.GetComponent<BossTeleporter>().toBoss = true;
                    inBossFight = true;

                    Instantiate(ivanPrefab, ivanSpawnPoint.position, Quaternion.identity);
                    return;
                }

                SpawnTrees();
                SpawnBushes();

                isNewWave = false;

                if (timeBetweenSpawns > minSpawnSpeed)
                {
                    timeBetweenSpawns -= spawnSpeedReductionTime;
                }

            }

            int enemyIndex = Random.Range(0, enemyTypes.Length);

            while (enemyTypes[enemyIndex].value > currentWaveValue || !CanSpawnEnemyType(enemyIndex)) 
            {
                enemyIndex = Random.Range(0, enemyTypes.Length);
            }

            Transform randomSpawnPoint = spawnPoints[Random.Range(0, spawnPoints.Count)];

            while (Vector2.Distance(randomSpawnPoint.position, GameManager.gameManager.player.transform.position) < spawnPointRequiredDistance)
            {
                randomSpawnPoint = spawnPoints[Random.Range(0, spawnPoints.Count)];
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

    bool CanSpawnEnemyType(int index)
    {
        if (waveNumber >= enemyTypes[index].spawnableWave)
        {
            return true;
        } else 
        {
            return false;
        }
    }

    void SpawnTrees()
    {
        foreach (Transform treeSpawnPoint in treeSpawns)
        {
            float randomPercent = Random.Range(0f, 1f);
            bool canSpawn = true;

            if (randomPercent <= treeSpawnPercent)
            {
                for (int i = 0; i < treePositions.Count; i++)
                {
                    if(treePositions[i].transform.position == treeSpawnPoint.position)
                    {
                        canSpawn = false;
                        break;    
                    }
                }

                if (canSpawn)
                {
                    Instantiate(tree, treeSpawnPoint.position, Quaternion.identity);
                    treePositions.Add(treeSpawnPoint);
                }
            }
        }
    }

    public void TreeDestroyed(Transform location)
    {
        for (int i = 0; i < treePositions.Count; i++)
        {
            if (treePositions[i].transform.position == location.position)
            {
                treePositions.RemoveAt(i);
            }
    
        }
    }

    void SpawnBushes()
    {
        foreach (Transform bushSpawnPoint in bushSpawns)
        {
            float randomPercent = Random.Range(0f, 1f);
            bool canSpawn = true;

            if (randomPercent <= bushSpawnPercent)
            {
                for (int i = 0; i < bushPositions.Count; i++)
                {
                    if(bushPositions[i].transform.position == bushSpawnPoint.position)
                    {
                        canSpawn = false;
                        break;    
                    }
                }

                if (canSpawn)
                {
                    Instantiate(bush, bushSpawnPoint.position, Quaternion.identity);
                    bushPositions.Add(bushSpawnPoint);
                }
            }
        }
    }

    public void BushDestroyed(Transform location)
    {
        for (int i = 0; i < bushPositions.Count; i++)
        {
            if (bushPositions[i].transform.position == location.position)
            {
                bushPositions.RemoveAt(i);
            }
    
        }
    }
}

