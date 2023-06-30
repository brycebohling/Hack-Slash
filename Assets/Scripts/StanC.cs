using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StanC : MonoBehaviour
{
    // Animation
    private Animator anim;
    string _currentState;
    const string ATTACK = "attack";
    const string DAMAGED = "damaged";
    const string WALK = "walk";
    const string NORMAL = "normal";

    // Movement
    private Rigidbody2D rb;
    private bool isFacingRight = true;
    [SerializeField] private float speed;
    [SerializeField] float seeDistance;
    Vector2 targetLocationX;
    float playerDistance;
    [SerializeField] Transform groundCheck;
    [SerializeField] private LayerMask groundLayer;
    private bool isGrounded;
    [SerializeField] Transform wallCheck;
    [SerializeField] private LayerMask wallCheckLayer;
    private bool isWallClose;
    [SerializeField] Vector2 groundCheckSize;
    [SerializeField] Vector2 wallCheckSize;
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
    [SerializeField] float attackRadius;
    [SerializeField] float attackTimer;
    float attackCountdown;
    [SerializeField] GameObject root;
    [SerializeField] float groundThickness;

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
    int scoreValue = 150;


    private void Start() 
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        currentHealth = health;
    }

    private void Update()
    {
        if (isDead)
        {
            Instantiate(deathParticals, transform.position, Quaternion.identity);
            GameManager.gameManager.EnemyDied(scoreValue);
            GameManager.gameManager.GetComponent<WaveSpawner>().killedEnemies++;
            Destroy(gameObject);
        }

        if (takingDmg)
        {
            dmgTimerCountdown -= Time.deltaTime;

            if (dmgTimerCountdown <= 0) 
            {
                ChangeAnimationState(DAMAGED);
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

        isGrounded = Physics2D.OverlapBox(groundCheck.position, groundCheckSize, 0, groundLayer);
        isWallClose = Physics2D.OverlapBox(wallCheck.position, wallCheckSize, 0, wallCheckLayer);
        targetLocationX = new Vector2(GameManager.gameManager.player.transform.position.x, transform.position.y);

        playerDistance = Vector2.Distance(transform.position, GameManager.gameManager.player.transform.position);
        playerDir = GameManager.gameManager.player.transform.position - transform.position;
        groundInWay = Physics2D.Raycast(transform.position, playerDir, playerDistance, groundLayer);

        if (beingKnockedback)
        {
            return;
        }

        if (isGrounded)
        {
            if (playerDistance < seeDistance && !IsAnimationPlaying(anim, ATTACK) && !groundInWay && GameManager.gameManager.isPlayerRendered)
            {
                FacePlayer();
                waitToPatrolTimer = waitToPatrolTime;
                patrolTurnTimer = Random.Range(patrolTurnLow, patrolTurnHigh);

                if (playerDistance > attackRadius && !isWallClose)
                {
                    ChangeAnimationState(WALK);
                    transform.position =  Vector2.MoveTowards(transform.position, targetLocationX, speed * Time.deltaTime);     

                } else
                { 
                    if (attackCountdown <= 0)
                    {
                        ChangeAnimationState(ATTACK);
                        attackCountdown = attackTimer;
                    } else if (!IsAnimationPlaying(anim, ATTACK))
                    {
                        ChangeAnimationState(NORMAL);
                    }
                }
            } else if (!IsAnimationPlaying(anim, ATTACK))
            {
                ChangeAnimationState(NORMAL);

                if (waitToPatrolTimer < 0f)
                {
                    ChangeAnimationState(WALK);

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

                    ChangeAnimationState(NORMAL);
                }
            }

        } else
        {
            ChangeAnimationState(NORMAL);
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

    public void DmgStan(float dmg, Transform attacker)
    {
        currentHealth -= dmg;   

        if (currentHealth <= 0)
        {
            isDead = true;
        } else
        {
            dmgTimerCountdown = dmgTime;
            takingDmg = true;
            ChangeAnimationState(DAMAGED);
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

    public void SpawnRoot()
    {
        RaycastHit2D groundPos = Physics2D.Raycast(GameManager.gameManager.playerPos, Vector2.down, 1000f, groundLayer);
    
        Vector2 rootSpawnPoint = new Vector2(groundPos.point.x, groundPos.point.y + groundThickness);

        Instantiate(root, rootSpawnPoint, Quaternion.identity);
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
        Gizmos.DrawWireSphere(transform.position, attackRadius);
        Gizmos.DrawWireCube(groundCheck.position, groundCheckSize);
        Gizmos.DrawWireCube(wallCheck.position, wallCheckSize);
        Gizmos.DrawWireSphere(transform.position, seeDistance);
        
    }
}
