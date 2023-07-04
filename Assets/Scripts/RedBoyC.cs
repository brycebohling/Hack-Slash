using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RedBoyC : MonoBehaviour
{
    // Animation
    private Animator anim;
    string _currentState;
    const string ENEMY_ATTACK = "attack";
    const string ENEMY_DAMAGED = "damaged";
    const string ENEMY_NORMAL = "normal";

    // Movement
    private Rigidbody2D rb;
    private bool isFacingRight = true;
    [SerializeField] private float speed;
    [SerializeField] private float minDistanceX;
    [SerializeField] float seeDistance;
    Vector2 targetLocationX;
    Vector2 targetLocationY;
    float playerDistanceY;
    float playerDistance;
    [SerializeField] Transform groundCheck;
    [SerializeField] private LayerMask groundLayer;
    private bool isGrounded;
    [SerializeField] Transform wallCheck;
    [SerializeField] private LayerMask wallCheackLayer;
    private bool isWallClose;
    Vector2 playerDir;
    RaycastHit2D groundInWay;
    [SerializeField] float patrolTurnLow;
    [SerializeField] float patrolTurnHigh;
    float patrolTurnTimer;
    int randomDir;
    [SerializeField] float patrolSpeed;
    [SerializeField] float waitToPatrolTime;
    float waitToPatrolTimer;

    // Attack
    [SerializeField] float dmg;
    [SerializeField] float attackRadius;
    [SerializeField] float attackTimer;
    float attackCountdown;
    [SerializeField] Transform attckPoint;
    [SerializeField] private LayerMask attackLayer;
    bool canDmgPlayer;

    // Health

    float currentHealth;
    [SerializeField] float health;

    // Damaged

    bool takingDmg;
    float dmgTimerCountdown;
    [SerializeField] float dmgTime;
    [SerializeField] float knockbackPower;
    [SerializeField] float knockbackTime;
    bool beingKnockedback;


    // Death
    bool isDead;
    [SerializeField] GameObject deathParticals;
    int scoreValue = 50;



    private void Start() 
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        currentHealth = health;
        waitToPatrolTimer = waitToPatrolTime;
    }

    private void Update()
    {
        if (isDead)
        {
            Instantiate(deathParticals, transform.position, Quaternion.identity);
            GameManager.gameManager.EnemyDied(scoreValue, tag);
            GameManager.gameManager.GetComponent<WaveSpawner>().killedEnemies++;
            Destroy(gameObject);
        }

        if (takingDmg)
        {
            dmgTimerCountdown -= Time.deltaTime;

            if (dmgTimerCountdown <= 0) 
            {
                ChangeAnimationState(ENEMY_NORMAL);
                takingDmg = false;
            } else 
            {
                return;
            }            
        }

        if (attackCountdown > 0)
        {
            attackCountdown -= Time.deltaTime;
        }

        isGrounded = Physics2D.OverlapBox(groundCheck.position, new Vector2(2.5f, 0.3f), 0, groundLayer);
        isWallClose = Physics2D.OverlapBox(wallCheck.position, new Vector2(2f, 1.5f), 0, wallCheackLayer);
        targetLocationX = new Vector2(GameManager.gameManager.player.transform.position.x, transform.position.y);
        targetLocationY = new Vector2(transform.position.x, GameManager.gameManager.player.transform.position.y);
        playerDistanceY = Vector2.Distance(transform.position, targetLocationY);

        playerDistance = Vector2.Distance(transform.position, GameManager.gameManager.player.transform.position);
        playerDir = GameManager.gameManager.player.transform.position - transform.position;
        groundInWay = Physics2D.Raycast(transform.position, playerDir, playerDistance, groundLayer);

        if (beingKnockedback)
        {
            return;
        }

        if (isGrounded)
        {
            if (playerDistance < seeDistance && !isWallClose && !IsAnimationPlaying(anim, ENEMY_ATTACK) && !groundInWay && GameManager.gameManager.isPlayerRendered)
            {
                FacePlayer();
                waitToPatrolTimer = waitToPatrolTime;
                patrolTurnTimer = Random.Range(patrolTurnLow, patrolTurnHigh);

                if (playerDistance > minDistanceX)
                {
                    ChangeAnimationState(ENEMY_NORMAL);
                    transform.position =  Vector2.MoveTowards(transform.position, targetLocationX, speed * Time.deltaTime);     
                } else
                { 
                    if (attackCountdown <= 0 && playerDistanceY < 1)
                    {
                        ChangeAnimationState(ENEMY_ATTACK);
                        attackCountdown = attackTimer;
                    } else if (!IsAnimationPlaying(anim, ENEMY_ATTACK))
                    {
                        ChangeAnimationState(ENEMY_NORMAL);
                    }
                }
            } else if (!IsAnimationPlaying(anim, ENEMY_ATTACK))
            {
                if (waitToPatrolTimer < 0f)
                {
                    if (patrolTurnTimer < 0f)
                    {
                        patrolTurnTimer = Random.Range(patrolTurnLow, patrolTurnHigh);
                        randomDir = Random.Range(0, 2);
                        
                    } else
                    {
                        patrolTurnTimer -= Time.deltaTime;
                    }

                    if (randomDir == 0)
                    {
                        rb.velocity = new Vector2(-patrolSpeed, 0);
                
                        if (isFacingRight)
                        {
                            Flip();
                        }

                    } else
                    {
                        rb.velocity = new Vector2(patrolSpeed, 0);
                    
                        if (!isFacingRight)
                        {
                            Flip();
                        }
                    }
                } else
                {
                    waitToPatrolTimer -= Time.deltaTime;
                }
            }
        } 
    }

    public void DmgPlayer()
    {
        canDmgPlayer = Physics2D.OverlapCircle(attckPoint.position, attackRadius, attackLayer);
        if (canDmgPlayer)
        {
            GameManager.gameManager.DamagePlayer(dmg,transform);
        }
    }

    private void FacePlayer()
    {
        if (GameManager.gameManager.player.transform.position.x > transform.position.x)
        {
            if (!isFacingRight)
            {
                Flip();
            }
        } else
        {
            if (isFacingRight)
            {
                Flip();
            }
        }
    }

    private void Flip()
    {
        isFacingRight = !isFacingRight;
        Vector3 localScale = transform.localScale;
        localScale.x *= -1f;    
        transform.localScale = localScale;
    }

    public void DmgRedBoy(float dmg, Transform attacker)
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
        StartCoroutine(CancelKnockback());
        beingKnockedback = true;
    }

    private IEnumerator CancelKnockback()
    {
        yield return new WaitForSeconds(knockbackTime);
        beingKnockedback = false;
        rb.velocity = Vector3.zero;
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
        Gizmos.DrawWireSphere(attckPoint.position, attackRadius);
        Gizmos.DrawWireCube(groundCheck.position, new Vector2(2.5f, .3f));
        Gizmos.DrawWireCube(wallCheck.position, new Vector2(2f, 1.5f));
        Gizmos.DrawWireSphere(transform.position, seeDistance);
        
    }
}
