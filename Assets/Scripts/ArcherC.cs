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
    private bool isFacingRight;
    [SerializeField] private float speed;
    [SerializeField] private float minDistanceX;
    [SerializeField] private float playerMaxDistance;
    Vector2 targetLocationX;
    Vector2 targetLocationY;
    float playerDistanceX;
    float playerDistanceY;
    [SerializeField] Transform groundCheck;
    [SerializeField] private LayerMask groundLayer;
    private bool isGrounded;
    [SerializeField] Transform wallCheck;
    [SerializeField] private LayerMask wallCheackLayer;
    private bool isWallClose;

    // Attack

    [SerializeField] GameObject arrow;
    [SerializeField] float attackTimer;
    float attackCountdown;
    [SerializeField] Transform attckPoint;
    [SerializeField] private LayerMask attackLayer;
    bool canDmgPlayer;

    // Health

    int currentHealth;
    [SerializeField] int health;

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
        isWallClose = Physics2D.OverlapBox(wallCheck.position, new Vector2(2f, 1.5f), 0, wallCheackLayer);
        targetLocationX = new Vector2(GameManager.gameManager.player.transform.position.x, transform.position.y);
        targetLocationY = new Vector2(transform.position.x, GameManager.gameManager.player.transform.position.y);
        playerDistanceX = Vector2.Distance(transform.position, GameManager.gameManager.player.transform.position);
        playerDistanceY = Vector2.Distance(transform.position, targetLocationY);

        Flip();

        if (isGrounded && GameManager.gameManager.isPlayerRendered)
        {
            if (playerDistanceX < playerMaxDistance && !isWallClose && !IsAnimationPlaying(anim, ENEMY_START_ATTACK) && !IsAnimationPlaying(anim, ENEMY_ATTACKING))
            {
                if (playerDistanceX > minDistanceX)
                {
                    ChangeAnimationState(ENEMY_WALK);
                    transform.position =  Vector2.MoveTowards(transform.position, targetLocationX, speed * Time.deltaTime);     
                } else
                { 
                if (attackCountdown <= 0 && playerDistanceY < 1)
                {
                    ChangeAnimationState(ENEMY_START_ATTACK);
                    attackCountdown = attackTimer;
    
                } else if (!IsAnimationPlaying(anim, ENEMY_START_ATTACK) && !IsAnimationPlaying(anim, ENEMY_ATTACKING))
                {
                    ChangeAnimationState(ENEMY_NORMAL);
                }

                }
            } else if (!IsAnimationPlaying(anim, ENEMY_START_ATTACK) && !IsAnimationPlaying(anim, ENEMY_ATTACKING))   
            {
                ChangeAnimationState(ENEMY_NORMAL);
            }
        }
        
    }
    public void FireArrow()
    {
        if (!isFacingRight)
        {
            Instantiate(arrow, attckPoint.position, Quaternion.identity);
        } else
        {
            Instantiate(arrow, attckPoint.position, transform.rotation * Quaternion.Euler(0f, 180f, 0f));
        }
        
    }

    public void DmgArcher(int dmg, Transform attacker)
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
        Gizmos.DrawWireSphere(transform.position, playerMaxDistance);
    }
}
