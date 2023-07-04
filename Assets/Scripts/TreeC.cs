using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TreeC : MonoBehaviour
{
    [System.Serializable] public struct FlyerPoints
    {
        public Transform flyerPoint;
    }

    [System.Serializable] public struct SpawnPoints
    {
        public Transform spawnPoint;
    }

    [System.Serializable] public struct TreeParticles
    {
        public Transform treeParticle;
    }

    public FlyerPoints[] flyerPoints;
    public SpawnPoints[] spawnPoints;
    public TreeParticles[] treeParticles;
    [SerializeField] GameObject flyerPrefab;
    [SerializeField] float enemySpawnDelay;
    Animator anim;
    string _currentState;
    string treeShakeAnim = "shake";
    [SerializeField] int hitPoints;
    int currentHitPoint;
    [SerializeField] GameObject healingItem;
    bool isDead;
    [SerializeField] float fadeSpeed;
    SpriteRenderer rend;
    Color color;
    [SerializeField] float waitForDeathTime;
    float waitForDeathCountdown;


    void Start()
    {
        anim = GetComponent<Animator>();
        rend = GetComponent<SpriteRenderer>();
        color = rend.color;
        currentHitPoint = hitPoints;
        waitForDeathCountdown = waitForDeathTime;
    }

    void Update()
    {
        if (isDead)
        {
            waitForDeathCountdown -= Time.deltaTime;

            if (waitForDeathCountdown <= 0)
            {  
                color.a -= fadeSpeed * Time.deltaTime;

                rend.color = color;
            }   

            if (color.a <= 0)
            {
                GameManager.gameManager.GetComponent<WaveSpawner>().TreeDestroyed(transform);

                Destroy(gameObject);
            }
        }
    }

    public void TreeShake()
    {
        if (isDead)
        {
            return;
        }

        currentHitPoint--;

        anim.Play(treeShakeAnim);
        int randomParticle = Random.Range(0, treeParticles.Length);
        Instantiate(treeParticles[randomParticle].treeParticle, spawnPoints[1].spawnPoint.position, Quaternion.identity);
        
        if (currentHitPoint <= 0)
        {
            isDead = true;
            
            int randomReward = Random.Range(0, 2);
            int randomSpawnPoint = Random.Range(0, spawnPoints.Length);
        
            if (randomReward == 0)
            {
                Instantiate(healingItem, spawnPoints[randomSpawnPoint].spawnPoint.position, Quaternion.identity);
            } else
            {
                StartCoroutine(SpawnEnemy(randomSpawnPoint));
            }
        }
      
    }

    private IEnumerator SpawnEnemy(int randomSpawnPoint)
    {
        yield return new WaitForSeconds(enemySpawnDelay);
        Instantiate(flyerPrefab, spawnPoints[randomSpawnPoint].spawnPoint.position, Quaternion.identity).GetComponent<FlyerC>();
        int randomNum = Random.Range(0, flyerPoints.Length);

        WaveSpawner.waveSpawner.TreeSpawnedEnemy();
    }

    private void ChangeAnimationState(string newState)
    {
        if (newState == _currentState)
        {
            return;
        }

        anim.Play(newState);
        _currentState = newState;
    }

    private bool IsAnimationPlaying(Animator animator, string stateName)
    {
        if (animator.GetCurrentAnimatorStateInfo(0).IsName(stateName) && animator.GetCurrentAnimatorStateInfo(0).normalizedTime < 1.0f)
        {
            return true;
        } else
        {
            return false;
        }
    }
}
