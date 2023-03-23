using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class playerController : MonoBehaviour
{
    CameraC camC;

    // Movement
    private float movementX;
    [SerializeField] private float speed;
    public bool isFacingRight;

    // Physics
    private Rigidbody2D rb;
    private BoxCollider2D bcoll;

    // Animatinon
    private Animator anim;
    SpriteRenderer spriteRend;
    string _currentState;
    const string PLAYER_IDLE = "idle";
    const string PLAYER_RUN = "running";
    const string PLAYER_JUMP = "jump";
    const string PLAYER_FALL = "fall";
    const string PLAYER_START_ROLL = "startRoll";
    const string PLAYER_ATTACK_1 = "attack-1";
    const string PLAYER_ATTACK_2 = "attack-2";
    const string PLAYER_ATTACK_3 = "attack-3";
    const string PLAYER_DAMAGED = "damaged";

    // Ground check
    [SerializeField] Transform groundCheck;
    [SerializeField] private LayerMask groundLayer;
    private bool isGrounded;

    // Ceilling check
    [SerializeField] Transform ceillingCheck;
    [SerializeField] private LayerMask headHitters;
    private bool isCeiling;

    // Junping
    [SerializeField] private float jumpForce;
    private bool doubleJump;

    // Rolling 
    [SerializeField] private float rollWidth;
    [SerializeField] private float rollHight;
    [SerializeField] private float rollOffsetX;
    [SerializeField] private float rollOffsetY;
    [SerializeField] private float rollSpeed;
    [SerializeField] private float rollTime;
    [SerializeField] private float rollCoolDown;
    private bool canRoll = true;
    private bool isRolling; 

    // Attack
    [SerializeField] int dmg;
    public static int noOfClicks = 0;
    private float lastClickedTime = 0;
    [SerializeField] private float maxComboDelay = 1;
    private bool attacking;
    [SerializeField] private float attackMovementSpeed;
    [SerializeField] GameObject attackPoint;
    [SerializeField] float attackRadius;
    [SerializeField] LayerMask enemies;

    // Scythe
    public GameObject dagger;
    private Vector2 location;
    [SerializeField] private float throwCooldown;
    private float throwCountdown;
    [SerializeField] Transform daggerSpawnPoint;

    // Health

    int currentHealth;
    [SerializeField] int health;

    // Dmg
    [SerializeField] float iFrameTime;
    float iFrameCountdown;
    [SerializeField] HealthBar HB;
    bool takingDmg = false;
    [SerializeField] float dmgTime;
    float dmgTimerCountdown;
    [SerializeField] float knockbackPower;

    // Death
    bool isDead = false;
    [SerializeField] GameObject deathParticals;
    [SerializeField] GameObject deathScreen;

    // Bush mechanics
    [SerializeField] Transform bush;
    bool jumpingIntoBush = false;
    bool jumpingOutBush = false;
    float bushLerp = 0.0f;
    [SerializeField] float bushTime;
    float timeInBush;
    Vector3 startPoint;
    Vector3 controlPoint;
    Vector3 endPoint;
    Vector3 originalPos;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        bcoll = GetComponent<BoxCollider2D>();
        camC = GameObject.FindGameObjectWithTag("vcam").GetComponent<CameraC>();
        spriteRend = GetComponent<SpriteRenderer>();
        currentHealth = health;
    }


    private void Update()
    {   
        if (jumpingIntoBush)
        {
            if (bushLerp < 1.0f) 
            {
                bushLerp += 1.0f * Time.deltaTime;

                Vector3 m1 = Vector3.Lerp(startPoint ,controlPoint, bushLerp);
                Vector3 m2 = Vector3.Lerp(controlPoint, endPoint, bushLerp );
                transform.position = Vector3.Lerp(m1, m2, bushLerp);

            } else
            {
                spriteRend.enabled = false;

                if (Input.GetKeyDown(KeyCode.E) || timeInBush > bushTime)
                {
                    bushLerp = 0f;
                    timeInBush = 0f;
                    jumpingIntoBush = false;
                    jumpingOutBush = true;
                    JumpBush(bush);
                } else 
                {
                    timeInBush += Time.deltaTime;
                    transform.position = bush.position;
                }
            }

            return;
        }

        if (jumpingOutBush)
        {
            if (bushLerp < 1.0f) 
            {
                bushLerp += 1.0f * Time.deltaTime;

                Vector3 m1 = Vector3.Lerp(startPoint ,controlPoint, bushLerp);
                Vector3 m2 = Vector3.Lerp(controlPoint, endPoint, bushLerp );
                transform.position = Vector3.Lerp(m1, m2, bushLerp);

                return;
            } else
            {
                bushLerp = 0;
                jumpingOutBush = false;
            }
        }

        if (isDead)
        {
            Instantiate(deathParticals, transform.position, Quaternion.identity);
            deathScreen.SetActive(true);
            gameObject.SetActive(false);
            return;
        }

        if (takingDmg)
        {
            dmgTimerCountdown -= Time.deltaTime;

            if (dmgTimerCountdown <= 0) 
            {
                takingDmg = false;
            }

            return;
        } else
        {
            takingDmg = false;
        }

        if (iFrameCountdown > 0)
        {
            iFrameCountdown -= Time.deltaTime;
        }

        if (throwCountdown > 0)
        {
            throwCountdown -= Time.deltaTime;
        }

        if (isRolling)
        {
            isCeiling = Physics2D.OverlapBox(ceillingCheck.position, new Vector2(1.3f, 1.7f), 0, headHitters);
            return;
        }

        isGrounded = Physics2D.OverlapBox(groundCheck.position, new Vector2(1.5f, .2f), 0, groundLayer);

        if (!IsAnimationPlaying(anim, PLAYER_ATTACK_1))
        {
            if (!IsAnimationPlaying(anim, PLAYER_ATTACK_2))
            {
                if (!IsAnimationPlaying(anim, PLAYER_ATTACK_3))
                {
                    attacking = false;
                }
            }
        }

        if (Time.time - lastClickedTime > maxComboDelay)
        {
            noOfClicks = 0;
        }
 
        if (Input.GetMouseButtonDown(0))
        {
            AttackAnim();
            attacking = true;
            if (isGrounded)
            {
                rb.velocity = new Vector2(transform.localScale.x * attackMovementSpeed, rb.velocity.y);
            }
            else
            {
                rb.velocity = new Vector2(0, rb.velocity.y);
            }
            return;
        }

        if (attacking)
        {
            return;
        }

        Flip();

        movementX = Input.GetAxisRaw("Horizontal");

        if (isGrounded && !Input.GetButton("Jump")) 
        {
            doubleJump = false;   
        }

        if (Input.GetButtonDown("Jump")) 
        {
            if (isGrounded) 
            {
                rb.velocity = new Vector2(rb.velocity.x, jumpForce);
                doubleJump = !doubleJump;
                
            } else if (doubleJump)
            {
                rb.velocity = new Vector2(rb.velocity.x, jumpForce);
                doubleJump = !doubleJump;
            }
        }

        if (movementX == 0 && rb.velocity.y == 0)
        {
            ChangeAnimationState(PLAYER_IDLE);

        } else if (rb.velocity.y < -0.01f || rb.velocity.y > 0.01f) 
        {
            if (rb.velocity.y > 0) 
            {
                ChangeAnimationState(PLAYER_JUMP);
            } else 
            {
                ChangeAnimationState(PLAYER_FALL);
            }

        } else if (movementX != 0)
        {
            ChangeAnimationState(PLAYER_RUN);
        } 

        if (Input.GetKeyDown(KeyCode.LeftShift) && canRoll)
        {
            StartCoroutine(Roll());
        }

        if (Input.GetKeyDown(KeyCode.C) && throwCountdown <= 0)
        {
            if (isFacingRight) 
            {
                location = new Vector2(daggerSpawnPoint.position.x, daggerSpawnPoint.position.y);
                Instantiate(dagger, location, Quaternion.identity);
                
            } else
            {
                location = new Vector2(daggerSpawnPoint.position.x, daggerSpawnPoint.position.y);
                Instantiate(dagger, location, transform.rotation * Quaternion.Euler (0f, 180f, 0f));
            }
            throwCountdown = throwCooldown;
            
        }

        if (Input.GetKeyDown(KeyCode.E))
        {
            jumpingIntoBush = true;
            JumpBush(bush);
        }
    }

    private void FixedUpdate() 
    {
        if (isRolling || attacking || takingDmg || isDead)
        {
            return;
        }
        rb.velocity = new Vector2(movementX * speed, rb.velocity.y);
    }

    void JumpBush(Transform bush) 
    {
        StopAllCoroutines();
        startPoint = transform.position;
        
        if (jumpingIntoBush)
        {
            originalPos = transform.position;
            endPoint = bush.position;
            controlPoint = startPoint +(endPoint -startPoint)/2 + Vector3.up * 5.0f;

        } else if (jumpingOutBush)
        {
            if (originalPos.x - bush.position.x > 0)
            {
                endPoint = new Vector3(bush.position.x + 3f, bush.position.y + 1f, bush.position.z);
            } else
            {
                endPoint = new Vector3(bush.position.x - 3f, bush.position.y + 1f, bush.position.z);
            }
            
            controlPoint = startPoint +(endPoint -startPoint)/2 + Vector3.up * 5.0f;

            spriteRend.enabled = true;
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

    private IEnumerator Roll()
    {
        ChangeAnimationState(PLAYER_START_ROLL);
        canRoll = false;
        isRolling = true;
        float originalWidth = bcoll.size.x;
        float originalHight = bcoll.size.y;
        float originalOffsetX = bcoll.offset.x;
        float originalOffsetY = bcoll.offset.y;
        bcoll.size = new Vector2(rollWidth, rollHight);
        bcoll.offset = new Vector2(rollOffsetX, rollOffsetY);
        rb.velocity = new Vector2(transform.localScale.x * rollSpeed, rb.velocity.y);

        yield return new WaitForSeconds(rollTime);
        yield return new WaitUntil(() => !isCeiling);

        bcoll.size = new Vector2(originalWidth, originalHight);
        bcoll.offset = new Vector2(originalOffsetX, originalOffsetY);
        isRolling = false;

        yield return new WaitForSeconds(rollCoolDown);

        canRoll = true;
    }

    private void AttackAnim()
    {
        lastClickedTime = Time.time;
        noOfClicks++;
        noOfClicks = Mathf.Clamp(noOfClicks, 0, 3);

        if (noOfClicks == 1 && !IsAnimationPlaying(anim, PLAYER_ATTACK_1) && !IsAnimationPlaying(anim, PLAYER_ATTACK_2) && !IsAnimationPlaying(anim, PLAYER_ATTACK_3))
        {
            ChangeAnimationState(PLAYER_ATTACK_1);
        }
 
        if( noOfClicks == 2 && !IsAnimationPlaying(anim, PLAYER_ATTACK_1) && !IsAnimationPlaying(anim, PLAYER_ATTACK_2) && !IsAnimationPlaying(anim, PLAYER_ATTACK_3))
        {
            ChangeAnimationState(PLAYER_ATTACK_2);
        }

        if( noOfClicks == 3 && !IsAnimationPlaying(anim, PLAYER_ATTACK_1) && !IsAnimationPlaying(anim, PLAYER_ATTACK_2) && !IsAnimationPlaying(anim, PLAYER_ATTACK_3))
        {
            ChangeAnimationState(PLAYER_ATTACK_3);
            noOfClicks = 0;
        }
    }

    public void PlayerAttackEnemy() 
    {
        Collider2D[] enemy = Physics2D.OverlapCircleAll(attackPoint.transform.position, attackRadius, enemies);

        foreach (Collider2D enemyGameobject in enemy)
        {
            GameManager.gameManager.DamageEnemy(enemyGameobject, dmg, transform);
        }
    }

    public void PlayerTakeDmg(int dmg, Transform attacker)
    {
        
        if (iFrameCountdown <= 0)
        {
            currentHealth -= dmg;
            HB.SetHealth(currentHealth);

            if (currentHealth <= 0)
            {
                isDead = true;
            } else
            {
                camC.CameraStartShake(2, 2);
                dmgTimerCountdown = dmgTime;
                iFrameCountdown = iFrameTime;
                takingDmg = true;
                ChangeAnimationState(PLAYER_DAMAGED);
                Knockback(attacker);
            }
        }
    }

    private void Knockback(Transform attacker)
    {
        StopAllCoroutines();
        Vector2 hitDir = (transform.position - attacker.position).normalized;
        rb.AddForce(hitDir * knockbackPower, ForceMode2D.Impulse);
        attacking = false;
    
        StartCoroutine(CancelKnockback());
    }

    private IEnumerator CancelKnockback()
    {
        yield return new WaitUntil(() => !takingDmg);
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
        Gizmos.DrawWireSphere(attackPoint.transform.position, attackRadius);   
    
        Gizmos.DrawWireCube(groundCheck.position, new Vector2(1.5f, .2f)); 
        Gizmos.DrawWireCube(ceillingCheck.position, new Vector2(1.3f, 1.7f));
    }
}
