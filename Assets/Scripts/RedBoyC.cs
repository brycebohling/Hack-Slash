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

    private Rigidbody2D rb;
    private float movementX;
    private bool isFacingRight;
    [SerializeField] private float speed;
    [SerializeField] private Transform target;
    [SerializeField] private float minDistance;
    Vector2 targetLocation;

    // Attack
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
            Destroy(gameObject);
            deathParticals.SetActive(true);
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

        movementX = rb.velocity.x;

        targetLocation = new Vector2(target.position.x, 0);

        Flip();

        if (Vector2.Distance(transform.position, targetLocation) > minDistance && !IsAnimationPlaying(anim, ENEMY_ATTACK))
        {
            ChangeAnimationState(ENEMY_NORMAL);
            transform.position =  Vector2.MoveTowards(transform.position, targetLocation, speed * Time.deltaTime);     
        } else
        {
            if (attackCountdown <= 0)
            {
                ChangeAnimationState(ENEMY_ATTACK);
                attackCountdown = attackTimer;
            } else if (!IsAnimationPlaying(anim, ENEMY_ATTACK))
            {
                ChangeAnimationState(ENEMY_NORMAL);
            }
        }
    }

    public void DmgPlayer()
    {
        canDmgPlayer = Physics2D.OverlapBox(attckPoint.position, new Vector2(2f, 2f), 0, attackLayer);
        if (canDmgPlayer)
        {
            PC.PlayerTakeDmg(10);
        }
    }

    private void Flip()
    {
        if (target.position.x > transform.position.x)
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
}
