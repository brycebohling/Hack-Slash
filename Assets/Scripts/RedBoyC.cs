using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RedBoyC : MonoBehaviour
{
    playerController PC;

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
    [SerializeField] private float minDistance;
    Vector2 targetLocationX;
    Vector2 targetLocationY;
    float distanceX;
    float distanceY;
    [SerializeField] Transform groundCheck;
    [SerializeField] private LayerMask groundLayer;
    private bool isGrounded;
    [SerializeField] Transform wallCheck;
    [SerializeField] private LayerMask wallCheackLayer;
    private bool isWallClose;

    // Attack
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

    // Death

    bool isDead;

    [SerializeField] GameObject deathParticals;



    private void Start() 
    {
        rb = GetComponent<Rigidbody2D>();
        PC = GameObject.FindGameObjectWithTag("Player").GetComponent<playerController>();
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
                takingDmg = false;
            } else 
            {
                rb.velocity = new Vector2(0,0);
                return;
            }            
        }

        if (attackCountdown > 0)
        {
            attackCountdown -= Time.deltaTime;
        }

        isGrounded = Physics2D.OverlapBox(groundCheck.position, new Vector2(2.5f, 0.3f), 0, groundLayer);
        isWallClose = Physics2D.OverlapBox(wallCheck.position, new Vector2(0.5f, 1.5f), 0, wallCheackLayer);
        targetLocationX = new Vector2(GameManager.gameManager.player.position.x, transform.position.y);
        targetLocationY = new Vector2(transform.position.x, GameManager.gameManager.player.position.y);
        distanceX = Vector2.Distance(transform.position, targetLocationX);
        distanceY = Vector2.Distance(transform.position, targetLocationY);

    
        Flip();

        if (isGrounded)
        {
            if (distanceX > minDistance && !isWallClose && !IsAnimationPlaying(anim, ENEMY_ATTACK))
            {
                ChangeAnimationState(ENEMY_NORMAL);
                transform.position =  Vector2.MoveTowards(transform.position, targetLocationX, speed * Time.deltaTime);     
            } else
            { 
                if (attackCountdown <= 0 && distanceY < 1)
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

    public void DmgPlayer()
    {
        canDmgPlayer = Physics2D.OverlapCircle(attckPoint.position, attackRadius, attackLayer);
        if (canDmgPlayer)
        {
            PC.PlayerTakeDmg(10);
        }
    }

    private void Flip()
    {
        if (GameManager.gameManager.player.position.x > transform.position.x)
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

    public void RedBoyTakeDmg(int dmg)
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
        Gizmos.DrawWireSphere(attckPoint.position, attackRadius);
        Gizmos.DrawWireCube(groundCheck.position, new Vector2(2.5f, .3f));
        Gizmos.DrawWireCube(wallCheck.position, new Vector2(0.5f, 1.5f));
    }
}
