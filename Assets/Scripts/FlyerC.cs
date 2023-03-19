using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlyerC : MonoBehaviour
{
    playerController PC;
    
    // Animation

    private Animator anim;
    string _currentState;
    const string ENEMY_NORMAL = "normal";
    const string ENEMY_DAMAGED = "damaged";

    // Physics
    [SerializeField] private float speed;
    Rigidbody2D rb;
    
    // Health
    int currentHealth;
    [SerializeField] int health;

    // Attack

    [SerializeField] LayerMask playerLayer;
    bool isTouchingPlayer;
    [SerializeField] int dmg;
    [SerializeField] float attackSpeed;
    bool attacking;
    bool divedIn;
    bool divedOut;
    [SerializeField] float attackWaitTime;
    bool canDive;

    // Movement 
    
    Vector2 targetLocationX;
    Vector2 targetLocationY;
    Vector3 originalPos;
    Vector3 attackPlayerPos;
    float distance;
    [SerializeField] float minDistance;
    [SerializeField] LayerMask groundLayer;

    // Damaged

    float dmgTimerCountdown;
    [SerializeField] float dmgTime;
    bool isDead = false;
    bool takingDmg = false;
    [SerializeField] GameObject deathParticals;
    [SerializeField] float knockbackPower;
    [SerializeField] float knockbackTime;
    bool beingKnockedBack = false;


    void Start()
    {
        PC = GameObject.FindGameObjectWithTag("Player").GetComponent<playerController>();
        anim = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
        currentHealth = health;
    }
    
    void Update()
    {
        if (beingKnockedBack)
        {
            return;
        }

        if (isDead)
        {
            Instantiate(deathParticals, transform.position, Quaternion.identity);
            Destroy(gameObject);
        }

        targetLocationX = new Vector2(GameManager.gameManager.player.position.x, transform.position.y);
        targetLocationY = new Vector2(transform.position.x, GameManager.gameManager.player.position.y);
        distance = Vector2.Distance(transform.position, GameManager.gameManager.player.position);

        Vector2 playerDir = GameManager.gameManager.player.position - transform.position;
        RaycastHit2D groundInWay = Physics2D.Raycast(transform.position, playerDir, distance, groundLayer);
        Debug.DrawRay(transform.position, playerDir, Color.red);

        if (takingDmg)
        {
            dmgTimerCountdown -= Time.deltaTime;

            if (dmgTimerCountdown <= 0) 
            {
                takingDmg = false;
            } else 
            {
                return;
            }            
        } else 
        {
            ChangeAnimationState(ENEMY_NORMAL);
        }

        isTouchingPlayer = Physics2D.OverlapBox(transform.position, new Vector2(1.6f, 1.1f), 0, playerLayer);

        if (isTouchingPlayer)
        {
            PC.PlayerTakeDmg(dmg);
        }

        if (attacking)
        {   
            if (canDive)
            {
                if (!divedIn)
                {   
                    transform.position =  Vector2.Lerp(transform.position, attackPlayerPos, attackSpeed * Time.deltaTime);
                    if (Vector2.Distance(transform.position, attackPlayerPos) < 1)
                    {
                        divedIn = true;
                    }

                } else if (!divedOut)
                {
                    transform.position =  Vector2.Lerp(transform.position, originalPos, attackSpeed * Time.deltaTime);
                    if (Vector2.Distance(transform.position, originalPos) < 0.05f)
                    {
                        divedOut = true;
                    }
                }
            }
    
            return;
        }

        if (groundInWay.collider == null)
        {  
            if (distance > minDistance)
            {
                transform.position = Vector2.MoveTowards(transform.position, GameManager.gameManager.player.position, speed * Time.deltaTime);

            } else
            {
                attacking = true;
                StartCoroutine(AttackPlayer());   
            }
        }
    }

    private IEnumerator AttackPlayer()
    {
        yield return new WaitForSeconds(attackWaitTime);

        if (distance < minDistance)
        {
            originalPos = new Vector3(transform.position.x, transform.position.y, transform.position.z);
            attackPlayerPos = new Vector3(GameManager.gameManager.player.position.x, GameManager.gameManager.player.position.y, GameManager.gameManager.player.position.z);
            divedIn = false;
            divedOut = false;
            canDive = true;

            yield return new WaitUntil(() => divedIn);
            yield return new WaitUntil(() => divedOut);
        }

        attacking = false;
        canDive = false;
    }

    public void DmgFlyer(int dmg, GameObject attacker)
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

    private void Knockback(GameObject attacker)
    {
        StopAllCoroutines();
        Vector2 hitDir = (transform.position - attacker.transform.position).normalized;
        rb.AddForce(hitDir * knockbackPower, ForceMode2D.Impulse);
        beingKnockedBack = true;
        StartCoroutine(CancelKnockback());
        attacking = false;
        divedIn = false;
        divedOut = false;
    }

    private IEnumerator CancelKnockback()
    {
        yield return new WaitForSeconds(knockbackTime);
        beingKnockedBack = false;
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
        Gizmos.DrawWireCube(transform.position, new Vector2(1.6f, 1.1f));
    }
}
