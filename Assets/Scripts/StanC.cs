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
    private bool isFacingRight;
    [SerializeField] private float speed;
    [SerializeField] private float minAttackDisX;
    [SerializeField] float seeDistance;
    Vector2 targetLocationX;
    Vector2 targetLocationY;
    float playerDistanceY;
    float playerDistance;
    [SerializeField] Transform groundCheck;
    [SerializeField] private LayerMask groundLayer;
    private bool isGrounded;
    [SerializeField] Transform wallCheck;
    [SerializeField] private LayerMask wallCheckLayer;
    private bool isWallClose;
    [SerializeField] Vector2 groundCheckSize;
    [SerializeField] Vector2 wallCheckSize;

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

        if (beingKnockedback)
        {
            return;
        }

        Flip();

        if (isGrounded && GameManager.gameManager.isPlayerRendered)
        {
            if (playerDistance < seeDistance && !IsAnimationPlaying(anim, ATTACK))
            {
                if (playerDistance > minAttackDisX && !isWallClose)
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
            }

        } else
        {
            ChangeAnimationState(NORMAL);
        }
        // Need to add patrol code here!
        
    }

    private void Flip()
    {
        if (GameManager.gameManager.player.transform.position.x > transform.position.x)
        {
            if (isFacingRight)
            {
                isFacingRight = !isFacingRight;
                Vector3 localScale = transform.localScale;
                localScale.x *= -1f;    
                transform.localScale = localScale;
            }
        } else
        {
            if (!isFacingRight)
            {
                isFacingRight = !isFacingRight;
                Vector3 localScale = transform.localScale;
                localScale.x *= -1f;
                transform.localScale = localScale;
            }
        }
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
        RaycastHit2D groundPos = Physics2D.Raycast(GameManager.gameManager.playerPos, Vector2.down, 100f, groundLayer);
        
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
