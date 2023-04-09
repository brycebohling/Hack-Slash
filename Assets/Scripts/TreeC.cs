using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TreeC : MonoBehaviour
{
    [System.Serializable] public struct FlyerPoints
    {
        public Transform flyerPoint;
    }

    public FlyerPoints[] flyerPoints;
    [SerializeField] Transform flyerSpawnPoint;
    [SerializeField] GameObject flyerPrefab;
    [SerializeField] float enemySpawnDelay;
    Animator anim;
    string _currentState;
    string treeShakeAnim = "shake";
    [SerializeField] int hitPoints;
    int currentHitPoint;
    [SerializeField] Transform shakeParticals;
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
        // Instantiate(shakeParticals, transform.position, Quaternion.identity);
        
        if (currentHitPoint <= 0)
        {
            isDead = true;
            
            
            int randomNum = Random.Range(0, 2);

            if (randomNum == 0)
            {
                Debug.Log("healing item dropped");
                // Instantiate(healingItem, transform.position, Quaternion.identity);
            } else
            {
                StartCoroutine(SpawnEnemy());
            }
        }
      
    }

    private IEnumerator SpawnEnemy()
    {
        yield return new WaitForSeconds(enemySpawnDelay);
        var instScript = Instantiate(flyerPrefab, flyerSpawnPoint.position, Quaternion.identity).GetComponent<FlyerC>();
        int randomNum = Random.Range(0, flyerPoints.Length);

        instScript.SpawningInTree(flyerPoints[randomNum].flyerPoint);
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
