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
    private bool isFacingRight;
    [SerializeField] private float speed;
    [SerializeField] private float minDistanceX;
    [SerializeField] float seeDistance;
    Vector2 targetLocationX;
    Vector2 targetLocationY;
    float playerDistanceX;
    float playerDistanceY;
    float playerDistance;
    [SerializeField] Transform groundCheck;
    [SerializeField] private LayerMask groundLayer;
    private bool isGrounded;
    [SerializeField] Transform wallCheck;
    [SerializeField] private LayerMask wallCheackLayer;
    private bool isWallClose;

    // Attack
    [SerializeField] int dmg;
    [SerializeField] float attackRadius;
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
        playerDistanceX = Vector2.Distance(transform.position, targetLocationX);
        playerDistanceY = Vector2.Distance(transform.position, targetLocationY);
        playerDistance = Vector2.Distance(transform.position, GameManager.gameManager.player.transform.position);

        if (beingKnockedback)
        {
            return;
        }

        Flip();

        if (isGrounded && GameManager.gameManager.isPlayerRendered)
        {
            if (playerDistance < seeDistance && !isWallClose && !IsAnimationPlaying(anim, ENEMY_ATTACK))
            {
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
            }
        }
        // Need to add patrol code here!
        
    }

    public void DmgPlayer()
    {
        canDmgPlayer = Physics2D.OverlapCircle(attckPoint.position, attackRadius, attackLayer);
        if (canDmgPlayer)
        {
            GameManager.gameManager.DamagePlayer(dmg,transform);
        }
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

    public void DmgRedBoy(int dmg, Transform attacker)
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
