using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class enemy : MonoBehaviour
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


    private void Start() 
    {
        rb = GetComponent<Rigidbody2D>();
        PC = GameObject.FindGameObjectWithTag("Player").GetComponent<playerController>();
        anim = GetComponent<Animator>();
    }

    private void Update()
    {
        if (attackCountdown > 0)
        {
            attackCountdown -= Time.deltaTime;
        }

        movementX = rb.velocity.x;

        targetLocation = new Vector2(target.position.x, 0);

        if (Vector2.Distance(transform.position, targetLocation) > minDistance)
        {
            transform.position =  Vector2.MoveTowards(transform.position, targetLocation, speed * Time.deltaTime);

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
        canDmgPlayer = Physics2D.OverlapBox(attckPoint.position, new Vector2(1f, 1f), 0, attackLayer);
        if (canDmgPlayer)
        {
            PC.PlayerTakeDmg(10);
        }
    }

    private void Flip()
    {
        if (isFacingRight && movementX < 0f || !isFacingRight && movementX > 0f)
        {
            isFacingRight = !isFacingRight;
            Vector3 localScale = transform.localScale;
            localScale.x *= -1f;
            transform.localScale = localScale;
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
