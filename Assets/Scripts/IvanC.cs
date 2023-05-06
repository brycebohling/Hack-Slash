using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IvanC : MonoBehaviour
{
   // Animation
    private Animator anim;
    string _currentState;
    const string ENEMY_SLASH = "slash";
    const string ENEMY_STOMP = "stomp";
    const string ENEMY_KICK = "kick";
    const string ENEMY_THROW = "throw";
    const string ENEMY_WALK = "walk";
    const string ENEMY_NORMAL = "normal";

    // Movement
    private Rigidbody2D rb;
    [SerializeField] Vector2 speed;
    public bool isFacingRight = true;
    [SerializeField] private float minDistanceX;
    [SerializeField] float seeDistance;
    Vector2 targetLocationX;
    Vector2 targetLocationY;
    float playerDistanceY;
    float playerDistance;
    [SerializeField] Transform groundCheck;
    [SerializeField] private LayerMask groundLayer;
    private bool isGrounded;
    Vector2 playerDir;
    RaycastHit2D groundInWay;
    [SerializeField] float patrolTurnLow;
    [SerializeField] float patrolTurnHigh;
    float patrolTurnTimer;
    int randomDir;
    [SerializeField] float patrolSpeed;
    [SerializeField] float waitToPatrolTime;
    float waitToPatrolTimer;
    [SerializeField] Vector2 groundCheckSize;

    // Attack
    bool attacking;
    [SerializeField] float dmg;
    [SerializeField] float totalAttackRange;
    [SerializeField] float slashHitRadius;
    [SerializeField] float kickHitRadius;
    [SerializeField] float slashRadius;
    [SerializeField] float kickRadius;
    bool canSlash;
    bool canKick;
    bool canStomp;
    bool canThrow;
    [SerializeField] float attackTimer;
    float attackCountdown;
    [SerializeField] Transform slashPoint;
    [SerializeField] Transform kickPoint;
    [SerializeField] private LayerMask attackLayer;
    bool canDmgPlayer;
    List<string> attackPosiblities = new List<string>();
    [SerializeField] GameObject root;
    [SerializeField] float timeBetweenRootSpawns; 
    [SerializeField] float groundThickness;
    [SerializeField] int numberOfTwigSpawns;
    [SerializeField] GameObject twig;
    [SerializeField] List<Transform> twigSpawnPoints = new List<Transform>();

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
    int scoreValue = 1000;



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
            // Instantiate(deathParticals, transform.position, Quaternion.identity);
            GameManager.gameManager.EnemyDied(scoreValue);
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

        if (!IsAnimationPlaying(anim, ENEMY_NORMAL) && !IsAnimationPlaying(anim, ENEMY_WALK) && !IsAnimationPlaying(anim, ENEMY_SLASH)
        && !IsAnimationPlaying(anim, ENEMY_KICK) && !IsAnimationPlaying(anim, ENEMY_STOMP) && !IsAnimationPlaying(anim, ENEMY_THROW))
        {
            attacking = false;
        }

        if (attackCountdown > 0)
        {
            attackCountdown -= Time.deltaTime;
        }

        if (beingKnockedback)
        {
            return;
        }

        if (!IsAnimationPlaying(anim, ENEMY_WALK))
        {
            rb.velocity = new Vector2(0,0);
        }

        isGrounded = Physics2D.OverlapBox(groundCheck.position, groundCheckSize, 0, groundLayer);
        targetLocationX = new Vector2(GameManager.gameManager.player.transform.position.x, transform.position.y);
        targetLocationY = new Vector2(transform.position.x, GameManager.gameManager.player.transform.position.y);
        playerDistanceY = Vector2.Distance(transform.position, targetLocationY);

        playerDistance = Vector2.Distance(transform.position, GameManager.gameManager.player.transform.position);
        playerDir = GameManager.gameManager.player.transform.position - transform.position;
        groundInWay = Physics2D.Raycast(transform.position, playerDir, playerDistance, groundLayer);
        
        if (isGrounded && !attacking)
        {

            if (playerDistance < seeDistance && !groundInWay && GameManager.gameManager.isPlayerRendered)
            {
                FacePlayer();
                waitToPatrolTimer = waitToPatrolTime;
                patrolTurnTimer = Random.Range(patrolTurnLow, patrolTurnHigh);

                if (attackCountdown <= 0 && playerDistance < totalAttackRange)
                {
                    PickAttack();

                } else if (playerDistance < totalAttackRange)
                {   

                    ChangeAnimationState(ENEMY_NORMAL);
                } else
                {
                    if (GameManager.gameManager.playerPos.x > transform.position.x)
                    {
                        rb.velocity = speed;
                    } else
                    {
                        rb.velocity = -speed;
                    }   

                    ChangeAnimationState(ENEMY_WALK);
                }
            }
                
            if (!attacking)
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

    void PickAttack()
    {
        canSlash = Physics2D.OverlapCircle(slashPoint.position, slashRadius, attackLayer);
        canKick = Physics2D.OverlapCircle(kickPoint.position, kickRadius, attackLayer);

        if (!canSlash && !canKick)
        {
            canStomp = true;
            canThrow = true;
        }

        if (canKick)
        {
            attackPosiblities.Add(ENEMY_KICK);
        }

        if (canSlash)
        {
            attackPosiblities.Add(ENEMY_SLASH);
        }
        
        if (canStomp)
        {
            attackPosiblities.Add(ENEMY_STOMP);
        }
        
        if (canThrow)
        {
            attackPosiblities.Add(ENEMY_THROW);
        }

        int randomAttack = Random.Range(0, attackPosiblities.Count);

        ChangeAnimationState(attackPosiblities[randomAttack]);
        attackPosiblities.Clear();
        
        attacking = true;
        attackCountdown = attackTimer;
    }

    public void DmgPlayer()
    {
        if (IsAnimationPlaying(anim, ENEMY_SLASH))
        {
            canDmgPlayer = Physics2D.OverlapCircle(slashPoint.position, slashHitRadius, attackLayer);
        } else if (IsAnimationPlaying(anim, ENEMY_KICK))
        {
            canDmgPlayer = Physics2D.OverlapCircle(kickPoint.position, kickHitRadius, attackLayer);
        }

        if (canDmgPlayer)
        {
            GameManager.gameManager.DamagePlayer(dmg,transform);
            canDmgPlayer = false;
        }
    }

    public IEnumerator SpawnRoots()
    {
        float bottomOfTree = gameObject.GetComponent<SpriteRenderer>().bounds.size.y;

        Vector2 originalPos = transform.position;

        float rootLenght = 1.25f;

        if (!isFacingRight)
        {
            rootLenght = -rootLenght;
        }

        bool nextRootInGround = true;

        float rootOffset = rootLenght;

        Vector2 rootSpawnX;
        
        bool canSpawnNextRoot = true;

        while (canSpawnNextRoot && nextRootInGround)
        {
            rootOffset += rootLenght;

            rootSpawnX = new Vector2(originalPos.x + rootOffset, originalPos.y);

            if (Physics2D.OverlapBox(new Vector2(originalPos.x + rootOffset, originalPos.y - bottomOfTree - .3f), -new Vector2(.05f, .05f), 0, groundLayer))
            {
                nextRootInGround = true;
            }
            
            canSpawnNextRoot = !Physics2D.OverlapBox(rootSpawnX, new Vector2(Mathf.Abs(rootLenght), .1f), 0, groundLayer);

            if (canSpawnNextRoot && nextRootInGround)
            {
                RaycastHit2D groundPos = Physics2D.Raycast(rootSpawnX, Vector2.down, 1000f, groundLayer);
    
                Vector2 rootSpawnPoint = new Vector2(groundPos.point.x, groundPos.point.y + groundThickness);
                
                Instantiate(root, rootSpawnPoint, Quaternion.identity);

                yield return new WaitForSeconds(timeBetweenRootSpawns);
            }
        }
    }

    public void SpawnTwigs()
    {
        int randomSpawnPoint = Random.Range(0, twigSpawnPoints.Count);

        for(int i = 0; i < numberOfTwigSpawns; i++)
        {
            randomSpawnPoint = Random.Range(0, twigSpawnPoints.Count);

            if (isFacingRight)
            {
                GameObject prefab = Instantiate(twig, twigSpawnPoints[randomSpawnPoint].position, Quaternion.Euler(0f, 0f, 5f - i));
                prefab.GetComponent<TwigC>().shootRight = true;
            } else
            {
                GameObject prefab = Instantiate(twig, twigSpawnPoints[randomSpawnPoint].position, Quaternion.Euler(0f, 0f, 185f - i));
            }
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

    public void DmgIvan(float dmg, Transform attacker)
    {
        currentHealth -= dmg;   

        if (currentHealth <= 0)
        {
            isDead = true;
        } else
        {
            dmgTimerCountdown = dmgTime;
            takingDmg = true;
            
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
        Gizmos.DrawWireSphere(slashPoint.position, slashHitRadius);
        Gizmos.DrawWireSphere(kickPoint.position, kickHitRadius);
        Gizmos.DrawWireCube(groundCheck.position, groundCheckSize);
        Gizmos.DrawWireSphere(transform.position, seeDistance);
        Gizmos.DrawWireSphere(transform.position, totalAttackRange);
    }
}
