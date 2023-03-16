using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlyerC : MonoBehaviour
{
    playerController PC;
    
    // Animation

    private Animator anim;
    string _currentState;
    const string ENEMY_NORMAL = "normal";
    const string ENEMY_DAMAGED = "damaged";

    // Physics
    [SerializeField] private float speed;
    
    // Health
    int currentHealth;
    [SerializeField] int health;

    // Attack

    [SerializeField] LayerMask playerLayer;
    bool isTouchingPlayer;
    [SerializeField] int dmg;
    [SerializeField] float attackSpeed;
    bool attacking;
    bool divedIn;
    bool divedOut;
    [SerializeField] float attackWaitTime;
    bool canDive;

    // Movement 
    
    Vector2 targetLocationX;
    Vector2 targetLocationY;
    Vector3 originalPos;
    Vector3 attackPlayerPos;
    float distanceX;
    float distanceY;
    float distance;
    [SerializeField] float minDistance;

    // Damaged

    float dmgTimerCountdown;
    [SerializeField] float dmgTime;
    bool isDead = false;
    bool takingDmg = false;
    [SerializeField] GameObject deathParticals;

    
    void Start()
    {
        PC = GameObject.FindGameObjectWithTag("Player").GetComponent<playerController>();
        anim = GetComponent<Animator>();
        currentHealth = health;
    }

    
    void Update()
    {
        if (isDead)
        {
            Instantiate(deathParticals, transform.position, Quaternion.identity);
            Destroy(gameObject);
        }

        if (takingDmg)
        {
            dmgTimerCountdown -= Time.deltaTime;

            if (dmgTimerCountdown <= 0) 
            {
                takingDmg = false;
            } else 
            {
                return;
            }            
        } else 
        {
            ChangeAnimationState(ENEMY_NORMAL);
        }

        isTouchingPlayer = Physics2D.OverlapBox(transform.position, new Vector2(1.6f, 1.1f), 0, playerLayer);

        if (isTouchingPlayer)
        {
            PC.PlayerTakeDmg(dmg);
        }

        targetLocationX = new Vector2(GameManager.gameManager.player.position.x, transform.position.y);
        targetLocationY = new Vector2(transform.position.x, GameManager.gameManager.player.position.y);
        distance = Vector2.Distance(transform.position, GameManager.gameManager.player.position);
        distanceX = Vector2.Distance(transform.position, targetLocationX);
        distanceY = Vector2.Distance(transform.position, targetLocationY);

        if (attacking)
        {   
            if (canDive)
            {
                if (!divedIn)
                {   
                    transform.position =  Vector2.Lerp(transform.position, attackPlayerPos, attackSpeed * Time.deltaTime);
                    if (Vector2.Distance(transform.position, attackPlayerPos) < 1)
                    {
                        divedIn = true;
                    }

                } else if (!divedOut)
                {
                    transform.position =  Vector2.Lerp(transform.position, originalPos, attackSpeed * Time.deltaTime);
                    if (Vector2.Distance(transform.position, originalPos) < 0.05f)
                    {
                        divedOut = true;
                    }
                }
            }
    
            return;
        }
                
        if (distance > minDistance)
        {
            transform.position = Vector2.MoveTowards(transform.position, GameManager.gameManager.player.position, speed * Time.deltaTime);

        } else
        {
            attacking = true;
            StartCoroutine(AttackPlayer());   
        }
    }

    private IEnumerator AttackPlayer()
    {
        yield return new WaitForSeconds(attackWaitTime);

        if (distance < minDistance)
        {
            originalPos = new Vector3(transform.position.x, transform.position.y, transform.position.z);
            attackPlayerPos = new Vector3(GameManager.gameManager.player.position.x, GameManager.gameManager.player.position.y, GameManager.gameManager.player.position.z);
            divedIn = false;
            divedOut = false;
            canDive = true;

            yield return new WaitUntil(() => divedIn);
            yield return new WaitUntil(() => divedOut);
        }

        attacking = false;
        canDive = false;
    }

    public void DmgFlyer(int dmg)
    {
        currentHealth -= dmg;   

        if (currentHealth <= 0)
        {
            isDead = true;
        } else
        {
            dmgTimerCountdown = dmgTime;
            takingDmg = true;
            ChangeAnimationState(ENEMY_DAMAGED);
        }
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

     private void OnDrawGizmos() 
    {   
        Gizmos.DrawWireCube(transform.position, new Vector2(1.6f, 1.1f));
    }
}
