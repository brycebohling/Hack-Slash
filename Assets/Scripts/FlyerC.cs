using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlyerC : MonoBehaviour
{
    // Animation

    private Animator anim;
    string _currentState;
    const string ENEMY_NORMAL = "normal";
    const string ENEMY_ATTACK = "attack";
    const string ENEMY_DAMAGED = "damaged";

    // Physics
    [SerializeField] private float speed;
    Rigidbody2D rb;
    
    // Health
    float currentHealth;
    [SerializeField] float health;

    // Attack

    [SerializeField] LayerMask playerLayer;
    bool isTouchingPlayer;
    [SerializeField] float dmg;
    [SerializeField] float attackSpeed;
    bool attacking;
    bool divedIn;
    bool divedOut;
    [SerializeField] float attackWaitTime;
    bool canDive;
    [SerializeField] float seeDistance;
    RaycastHit2D groundInWay;
    bool hitPlayerOnce;

    // Movement 
    
    Vector2 targetLocationX;
    Vector2 targetLocationY;
    Vector3 originalPos;
    Vector3 attackPlayerPos;
    float distance;
    [SerializeField] float minDistance;
    [SerializeField] LayerMask groundLayer;
    [SerializeField] Vector2 boxCastSize;
    Vector2 playerDir;

    // Damaged

    float dmgTimerCountdown;
    [SerializeField] float dmgTime;
    
    bool takingDmg;
    [SerializeField] float knockbackPower;
    [SerializeField] float knockbackTime;
    bool beingKnockedback;

    // Spawning with tree
    bool spawningFromTree;
    Transform goToPoint;

    // Death
    int scoreValue = 70;
    [SerializeField] GameObject deathParticals;
    bool isDead;


    void Start()
    {
        anim = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
        currentHealth = health;
    }
    
    void Update()
    {
        if (isDead)
        {
            Died();
            return;
        }

        targetLocationX = new Vector2(GameManager.gameManager.player.transform.position.x, transform.position.y);
        targetLocationY = new Vector2(transform.position.x, GameManager.gameManager.player.transform.position.y);
        distance = Vector2.Distance(transform.position, GameManager.gameManager.player.transform.position); 
        playerDir = GameManager.gameManager.player.transform.position - transform.position;
        
        groundInWay = Physics2D.BoxCast(transform.position, boxCastSize, 0, playerDir, distance, groundLayer);

        Debug.DrawRay(transform.position, playerDir, Color.red);

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
        } else if (!IsAnimationPlaying(anim, ENEMY_ATTACK) && !attacking)
        {
            ChangeAnimationState(ENEMY_NORMAL);
        }

        if (beingKnockedback)
        {
            return;
        }

        if (spawningFromTree)
        {
            if (!transform)
            {
                transform.position =  Vector2.Lerp(transform.position, goToPoint.position, speed * Time.deltaTime);
                if (Vector2.Distance(transform.position, goToPoint.position) < 0.1f)
                {
                    spawningFromTree = false;
                }
            }
        }

        if (attacking)
        {   
        
            if (canDive)
            {
                isTouchingPlayer = Physics2D.OverlapBox(transform.position, new Vector2(1.6f, 1.1f), 0, playerLayer);
                
                if (isTouchingPlayer && !hitPlayerOnce)
                {
                    GameManager.gameManager.DamagePlayer(dmg,transform);
                    hitPlayerOnce = true;
                    ChangeAnimationState(ENEMY_NORMAL);
                }    

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

        if (groundInWay.collider == null && GameManager.gameManager.isPlayerRendered && Vector2.Distance(GameManager.gameManager.player.transform.position, transform.position) < seeDistance)
        {  
            if (distance > minDistance)
            {
                transform.position = Vector2.MoveTowards(transform.position, GameManager.gameManager.player.transform.position, speed * Time.deltaTime);

            } else
            {
                ChangeAnimationState(ENEMY_ATTACK);
            }
        }
    }

    public void SpawningInTree(Transform point)
    {
        spawningFromTree = true;
        
        goToPoint = point;
    }

    public IEnumerator AttackPlayer()
    {        
        if (distance < minDistance && !groundInWay)
        {
            attacking = true;
            originalPos = new Vector3(transform.position.x, transform.position.y, transform.position.z);
            attackPlayerPos = new Vector3(GameManager.gameManager.player.transform.position.x, GameManager.gameManager.player.transform.position.y, GameManager.gameManager.player.transform.position.z);
            divedIn = false;
            divedOut = false;
            canDive = true;

            yield return new WaitUntil(() => divedIn);
            yield return new WaitUntil(() => divedOut);
        }

        attacking = false;
        canDive = false;
        hitPlayerOnce = false;
    }

    public void DmgFlyer(float dmg, Transform attacker)
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
            Knockback(attacker);
        }
    }

    private void Knockback(Transform attacker)
    {
        StopAllCoroutines();
        Vector2 hitDir = (transform.position - attacker.position).normalized;
        rb.AddForce(hitDir * knockbackPower, ForceMode2D.Impulse);
        beingKnockedback = true;
        attacking = false;
        divedIn = false;
        divedOut = false;
        canDive = false;

        StartCoroutine(CancelKnockback());
    }

    private IEnumerator CancelKnockback()
    {
        yield return new WaitForSeconds(knockbackTime);
        beingKnockedback = false;
        rb.velocity = Vector3.zero;
    }

    private void Died()
    {
        Instantiate(deathParticals, transform.position, Quaternion.identity);

        GameManager.gameManager.EnemyDied(scoreValue, tag);
        GameManager.gameManager.GetComponent<WaveSpawner>().killedEnemies++; 

        Destroy(gameObject);
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
        Gizmos.DrawWireSphere(transform.position, seeDistance);
    }
}
