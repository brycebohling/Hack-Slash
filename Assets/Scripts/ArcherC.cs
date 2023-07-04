using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArcherC : MonoBehaviour
{
    // Animation
    private Animator anim;
    string _currentState;
    const string ENEMY_START_ATTACK = "startAttack";
    const string ENEMY_ATTACKING = "attacking";
    const string ENEMY_WALK = "walk";
    const string ENEMY_DAMAGED = "damaged";
    const string ENEMY_NORMAL = "normal";

    // Movement
    private Rigidbody2D rb;
    private bool isFacingRight = true;
    [SerializeField] private float speed;
    [SerializeField] private float shootingRange;
    [SerializeField] private float seeDistance;
    [SerializeField] float maxSeeDistanceY;
    Vector2 targetLocationX;
    float playerDistance;
    float playerDistanceY;
    [SerializeField] Transform groundCheck;
    [SerializeField] private LayerMask groundLayer;
    private bool isGrounded;
    [SerializeField] Transform wallCheck;
    [SerializeField] private LayerMask wallCheackLayer;
    private bool isWallClose;
    [SerializeField] float patrolTurnLow;
    [SerializeField] float patrolTurnHigh;
    float patrolTurnTimer;
    int randomDir;
    [SerializeField] float patrolSpeed;
    [SerializeField] float waitToPatrolTime;
    float waitToPatrolTimer;
    Vector2 playerDir;
    RaycastHit2D groundInWay;

    // Attack

    [SerializeField] GameObject arrow;
    [SerializeField] float attackTimer;
    float attackCountdown;
    [SerializeField] Transform attckPoint;

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
    [SerializeField] GameObject dmgParticles;

    // Death
    bool isDead;
    [SerializeField] GameObject deathParticals;
    int scoreValue = 100;


    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        currentHealth = health;
    }

    void Update()
    {
        if (isDead)
        {
            Instantiate(deathParticals, transform.position, Quaternion.identity);
            GameManager.gameManager.DropHealth(transform);
            GameManager.gameManager.EnemyDied(scoreValue, tag);
            GameManager.gameManager.GetComponent<WaveSpawner>().killedEnemies++;
            Destroy(gameObject);
        }

        if (attackCountdown > 0)
        {
            attackCountdown -= Time.deltaTime;
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

        if (beingKnockedback)
        {
            return;
        }

        isGrounded = Physics2D.OverlapBox(groundCheck.position, new Vector2(2.5f, 0.3f), 0, groundLayer);
        isWallClose = Physics2D.OverlapBox(wallCheck.position, new Vector2(.5f, 1.5f), 0, wallCheackLayer);
        targetLocationX = new Vector2(GameManager.gameManager.player.transform.position.x, transform.position.y);
        playerDistanceY = Mathf.Abs(transform.position.y - GameManager.gameManager.player.transform.position.y);
        
        playerDistance = Vector2.Distance(transform.position, GameManager.gameManager.player.transform.position);
        playerDir = GameManager.gameManager.player.transform.position - transform.position;
        groundInWay = Physics2D.Raycast(transform.position, playerDir, playerDistance, groundLayer);

        if (isGrounded)
        {
            if (playerDistance < seeDistance && !IsAnimationPlaying(anim, ENEMY_START_ATTACK) && !IsAnimationPlaying(anim, ENEMY_ATTACKING) && GameManager.gameManager.isPlayerRendered && !groundInWay)
            {
                FacePlayer();
                waitToPatrolTimer = waitToPatrolTime;
                patrolTurnTimer = Random.Range(patrolTurnLow, patrolTurnHigh);

                if (!isWallClose)
                {
                    if (playerDistance < shootingRange && playerDistanceY > maxSeeDistanceY || playerDistance > shootingRange)
                    {
                        ChangeAnimationState(ENEMY_WALK);
                        transform.position =  Vector2.MoveTowards(transform.position, targetLocationX, speed * Time.deltaTime);
                            
                    } else
                    { 
                        if (attackCountdown <= 0 && playerDistanceY <= maxSeeDistanceY)
                        {
                            ChangeAnimationState(ENEMY_START_ATTACK);
                            attackCountdown = attackTimer;
            
                        } else if (attackCountdown <= 0 && !IsAnimationPlaying(anim, ENEMY_START_ATTACK) && !IsAnimationPlaying(anim, ENEMY_ATTACKING))
                        {
                            ChangeAnimationState(ENEMY_NORMAL);
                        }

                    }
                }
                
            } else if (!IsAnimationPlaying(anim, ENEMY_START_ATTACK) && !IsAnimationPlaying(anim, ENEMY_ATTACKING))   
            {
                if (waitToPatrolTimer < 0f)
                {
                    ChangeAnimationState(ENEMY_WALK);

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

                    ChangeAnimationState(ENEMY_NORMAL);
                }
            }
        } else
        {
            ChangeAnimationState(ENEMY_NORMAL);
        }
        
    }
    public void FireArrow()
    {
        if (isFacingRight)
        {
            Instantiate(arrow, attckPoint.position, Quaternion.identity);
        } else
        {
            Instantiate(arrow, attckPoint.position, transform.rotation * Quaternion.Euler(0f, 180f, 0f));
        }
        
    }

    public void DmgArcher(float dmg, Transform attacker)
    {
        currentHealth -= dmg;   

        Instantiate(dmgParticles, transform.position, Quaternion.identity);

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
        Gizmos.DrawWireCube(transform.position, new Vector2(5, maxSeeDistanceY * 2));
        Gizmos.DrawWireSphere(transform.position, shootingRange);
        Gizmos.DrawWireSphere(transform.position, seeDistance);
    }
}
