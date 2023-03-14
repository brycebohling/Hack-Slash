using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlyerC : MonoBehaviour
{
    playerController PC;

    // Physics
    private Rigidbody2D rb;
    [SerializeField] private float speed;
    
    // Health
    int currentHealth;
    [SerializeField] int health;

    // Attack

    float attackCountdown;
    [SerializeField] float attackTimer;
    [SerializeField] float attackSpeed;
    bool attacking;
    bool divedIn;
    bool divedOut;
    Vector3 originalPos;
    Vector3 attackPlayerPos;

    // Movement 
    
    Vector2 targetLocationX;
    Vector2 targetLocationY;
    float distanceX;
    float distanceY;
    float distance;
    [SerializeField] float minDistanceX;

    
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        PC = GameObject.FindGameObjectWithTag("Player").GetComponent<playerController>();
        currentHealth = health;
    }

    
    void Update()
    {

        targetLocationX = new Vector2(GameManager.gameManager.player.position.x, transform.position.y);
        targetLocationY = new Vector2(transform.position.x, GameManager.gameManager.player.position.y);
        distance = Vector2.Distance(transform.position, GameManager.gameManager.player.position);
        distanceX = Vector2.Distance(transform.position, targetLocationX);
        distanceY = Vector2.Distance(transform.position, targetLocationY);

        if (attackCountdown > 0)
        {
            attackCountdown -= Time.deltaTime;
        }

        if (attacking)
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
                if (Vector2.Distance(transform.position, originalPos) < 1f)
                {
                    divedOut = true;
                }
            }
            return;
        }
                
        if (distanceX > minDistanceX)
        {
            transform.position = Vector2.MoveTowards(transform.position, GameManager.gameManager.player.position, speed * Time.deltaTime);

        } else if (attackCountdown <= 0 && distanceY < 5)
        {
            attacking = true;
            attackCountdown = attackTimer;
            StartCoroutine(AttackPlayer());   
        }
    }

    private IEnumerator AttackPlayer()
    {
        originalPos = new Vector3(transform.position.x, transform.position.y, transform.position.z);
        attackPlayerPos = new Vector3(GameManager.gameManager.player.position.x, GameManager.gameManager.player.position.y, GameManager.gameManager.player.position.z);
        divedIn = false;
        divedOut = false;

        yield return new WaitUntil(() => divedIn);
        yield return new WaitUntil(() => divedOut);

        attacking = false;
    }
}
