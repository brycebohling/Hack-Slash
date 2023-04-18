using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class playerController : MonoBehaviour
{
    CameraC camC;
    bushC bushScript;
    [SerializeField] HealthBar HB;
    [SerializeField] StaminaBarC SB;

    // Stats
    public float movementSpeed;
    public float jumpForce;
    public int numberOfJumps;
    public float maxHealth;
    public float maxStamina;
    public float meleeDmg;
    public float daggerDmg;
    public float daggerSpeed;
    public float healthDropChance;
    public float dmgReduction;
    public float critDmg;
    public float dodgeChance;
    public float rollSpeed;
    public float meleeSpeed;
    public float healthRegenerationAmount;

    // Movement
    private float movementX;
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
    private bool wasGrounded;

    // Ceilling check
    [SerializeField] Vector2 ceillingCheckSize;
    [SerializeField] Transform ceillingCheck;
    [SerializeField] private LayerMask headHitters;
    private bool isCeiling;

    // Junping
    int currentNumOfJumps;
    [SerializeField] float jumpOffJumpTime;
    float jumpOffJumpTimer;
    [SerializeField] float waitToCheckForJump;
    float waitToCheckForJumpTimer;

    // Rolling 

    [SerializeField] private float rollWidth;
    [SerializeField] private float rollHight;
    [SerializeField] private float rollOffsetX;
    [SerializeField] private float rollOffsetY;
    [SerializeField] private float rollTime;
    private bool canRoll = true;
    private bool isRolling; 
    float originalWidth;
    float originalHight;
    float originalOffsetX;
    float originalOffsetY;

    // Stamina

    float currentStamina;
    [SerializeField] float staminaRechargeTime;
    float staminaRechargeTimer;
    [SerializeField] float staminaRechargeSpeed;
    [SerializeField] float rollStaminaAmount;

    // Attack
    public static int noOfClicks = 0;
    private float lastClickedTime = 0;
    [SerializeField] private float maxComboDelay = 1;
    private bool attacking;
    [SerializeField] private float attackMovementSpeed;
    [SerializeField] GameObject attackPoint;
    [SerializeField] float attackRadius;
    [SerializeField] LayerMask enemies;
    [SerializeField] Transform critParticle;
    bool willCrit;

    // Dagger
    public GameObject dagger;
    int daggerAmmo = 3;
    [SerializeField] float daggerThrowCooldown;
    float daggerThrowCooldownTimer;
    private int currentDaggerAmmo;
    float lastDaggerThrown;
    [SerializeField] float daggerWaitToRechargeTime;
    [SerializeField] float daggerRechargingTime;
    float currentDaggerRechargingTime;
    [SerializeField] Transform daggerSpawnPoint;
    [SerializeField] GameObject daggerUIObejct;

    // Health

    public float currentHealth;
    [SerializeField] float h_Regen_Wait;
    float h_Regen_Timer;
    [SerializeField] float h_Regen_BurstWait;
    float h_Regen_BurstWaitTimer;

    // Dmg
    [SerializeField] float iFrameTime;
    float iFrameCountdown;
    public bool invicible;
    bool takingDmg;
    [SerializeField] float dmgTime;
    float dmgTimerCountdown;
    [SerializeField] float knockbackPower;

    // Death
    public bool isDead;
    [SerializeField] GameObject deathParticals;

    // Bush mechanics
    Collider2D[] bushInRange;
    float bushJumpHeight = 5f;
    bool jumpingIntoBush;
    bool jumpingOutBush;
    float bushLerp = 0.0f;
    [SerializeField] float bushTime;
    float timeInBush;
    Vector3 bushStartPoint;
    Vector3 bushControlPoint;
    Vector3 bushEndPoint;
    Vector3 originalPos;
    [SerializeField] Vector2 bushCheckSize;
    [SerializeField] LayerMask bushLayer;
    Transform closestBushTransform;
    bool didBushShake = false;

    // Tree mechanices
    [SerializeField] LayerMask treeLayer;

    // UI
    [SerializeField] GameObject deathScreenUI;
    [SerializeField] GameObject bushInRangeUI;
    [SerializeField] TMP_Text totalWaveText;

    [System.Serializable] public struct TurnOffUI
    {
        public GameObject UIObject;
        
    }

    public TurnOffUI[] turnOffUI;


    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        bcoll = GetComponent<BoxCollider2D>();
        camC = GameObject.FindGameObjectWithTag("vcam").GetComponent<CameraC>();
        spriteRend = GetComponent<SpriteRenderer>();
        currentHealth = maxHealth;
        currentDaggerAmmo = daggerAmmo;
        currentStamina = maxStamina;
        jumpOffJumpTimer = jumpOffJumpTime;
        waitToCheckForJumpTimer = waitToCheckForJump;
        h_Regen_Timer = h_Regen_Wait;
        h_Regen_BurstWaitTimer = h_Regen_BurstWait;
        HB.SetMaxHealth(maxHealth);
        SB.SetMaxStamina(maxStamina);
    }
    

    private void Update()
    {   
        if (isDead)
        {
            Instantiate(deathParticals, transform.position, Quaternion.identity);
            deathScreenUI.SetActive(true);

            for(int i = 0; i < turnOffUI.Length; i++)
            {
                turnOffUI[i].UIObject.SetActive(false);
            }

            totalWaveText.text = "You made it to Wave: " + GameManager.gameManager.waveNum;

            gameObject.SetActive(false);
            return;
        }

        isGrounded = Physics2D.OverlapBox(groundCheck.position, new Vector2(1.5f, .2f), 0, groundLayer);
        isCeiling = Physics2D.OverlapBox(ceillingCheck.position, ceillingCheckSize, 0, headHitters);

        if (h_Regen_Timer <= 0f && h_Regen_BurstWaitTimer <= 0f && currentHealth < maxHealth)
        {
            currentHealth += healthRegenerationAmount;
            h_Regen_BurstWaitTimer = h_Regen_BurstWait;

            HB.SetHealth(currentHealth);
            
        } else
        {
            h_Regen_Timer -= Time.deltaTime;
            h_Regen_BurstWaitTimer -= Time.deltaTime;
        }

        if (iFrameCountdown > 0f)
        {
            iFrameCountdown -= Time.deltaTime;
        }

        if (daggerThrowCooldown > 0f)
        {
            daggerThrowCooldown -= Time.deltaTime;
        }

        if (currentDaggerRechargingTime > 0f)
        {
            currentDaggerRechargingTime -= Time.deltaTime;
        }

        if (staminaRechargeTimer > 0f)
        {
            staminaRechargeTimer -= Time.deltaTime;
        } else if (currentStamina < maxStamina)
        {
            currentStamina += Time.deltaTime * staminaRechargeSpeed;
            SB.SetStamina(currentStamina);
        }

        if (!isGrounded && jumpOffJumpTimer > 0f)
        {
            jumpOffJumpTimer -= Time.deltaTime;
        }

        if (!isGrounded && wasGrounded)
        {
            currentNumOfJumps = numberOfJumps;
        }

        if (isGrounded)
        {
            wasGrounded = true;
        } else 
        {
            wasGrounded = false;
        }

        if (isGrounded && waitToCheckForJumpTimer < 0f)
        {
            jumpOffJumpTimer = jumpOffJumpTime;
        } else
        {
            waitToCheckForJumpTimer -= Time.deltaTime;
        }

        if (Time.time - lastDaggerThrown > daggerWaitToRechargeTime && currentDaggerRechargingTime <= 0f && currentDaggerAmmo + 1 <= daggerAmmo)
        {
            currentDaggerRechargingTime = daggerRechargingTime;
            currentDaggerAmmo++;
            GameManager.gameManager.SetDaggerAmmoUI(currentDaggerAmmo);
        }

        if (jumpingIntoBush)
        {
            daggerUIObejct.SetActive(false);
            if (bushLerp < 1.0f) 
            {
                ChangeAnimationState(PLAYER_START_ROLL);
                
                bushLerp += 1.0f * Time.deltaTime;

                Vector3 m1 = Vector3.Lerp(bushStartPoint ,bushControlPoint, bushLerp);
                Vector3 m2 = Vector3.Lerp(bushControlPoint, bushEndPoint, bushLerp);
                transform.position = Vector3.Lerp(m1, m2, bushLerp);
 
                if (bushLerp > 0.8f)
                {
                    spriteRend.enabled = false;
                }
            
            } else
            {

                if (!didBushShake)
                {
                    bushScript.BushShake();
                    didBushShake = true;
                }

                if (Input.GetKeyDown(KeyCode.E) || timeInBush > bushTime)
                {
                    bushLerp = 0f;
                    timeInBush = 0f;
                    jumpingIntoBush = false;
                    jumpingOutBush = true;
                    didBushShake = false;
                    JumpBush(closestBushTransform);
                } else 
                {
                    timeInBush += Time.deltaTime;
                    transform.position = closestBushTransform.position;
                }
            }

            return;
        }

        if (jumpingOutBush)
        {
            daggerUIObejct.SetActive(true);
            GameManager.gameManager.SetDaggerAmmoUI(currentDaggerAmmo);
            if (bushLerp < 1.0f) 
            {
                if (!didBushShake)
                {
                    bushScript.BushShake();
                    didBushShake = true;
                }
        
                ChangeAnimationState(PLAYER_START_ROLL);

                bushLerp += 1.0f * Time.deltaTime;

                Vector3 m1 = Vector3.Lerp(bushStartPoint ,bushControlPoint, bushLerp);
                Vector3 m2 = Vector3.Lerp(bushControlPoint, bushEndPoint, bushLerp );
                transform.position = Vector3.Lerp(m1, m2, bushLerp);

                return;
            } else
            {
                bushLerp = 0;
                jumpingOutBush = false;
                didBushShake = false;
                invicible = false;
            }
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

        if (!IsAnimationPlaying(anim, PLAYER_ATTACK_1))
        {
            if (!IsAnimationPlaying(anim, PLAYER_ATTACK_2))
            {
                if (!IsAnimationPlaying(anim, PLAYER_ATTACK_3))
                {
                    attacking = false;
                    anim.speed = 1f;
                }
            }
        }

        if (attacking)
        {
            return;
        }

        if (Time.time - lastClickedTime > maxComboDelay)
        {
            noOfClicks = 0;
        }

        if (Input.GetButtonDown("Jump") || Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.UpArrow)) 
        {
            if (isGrounded || jumpOffJumpTimer > 0f) 
            {   
                if (isRolling && !isCeiling)
                {
                    rb.velocity = new Vector2(rb.velocity.x, jumpForce);
                    jumpOffJumpTimer = 0f;
                    waitToCheckForJumpTimer = waitToCheckForJump;
                    ChangeAnimationState(PLAYER_JUMP);
                    StopRoll();
                    StopCoroutine(Roll());
                } else if (!isRolling)
                {
                    rb.velocity = new Vector2(rb.velocity.x, jumpForce);
                    jumpOffJumpTimer = 0f;
                    waitToCheckForJumpTimer = waitToCheckForJump;
                }
                
            } else if (currentNumOfJumps >= 1)
            {
                if (isRolling && !isCeiling)
                {
                    rb.velocity = new Vector2(rb.velocity.x, jumpForce);
                    currentNumOfJumps -= 1;
                    ChangeAnimationState(PLAYER_JUMP);
                } else if (!isRolling)
                {
                    rb.velocity = new Vector2(rb.velocity.x, jumpForce);
                    currentNumOfJumps -= 1;
                }
            }
        }

        if (Input.GetMouseButtonDown(0) || Input.GetKeyDown(KeyCode.RightShift))
        {
            if (isRolling && !isCeiling)
            {
                AttackAnim();
                attacking = true;
                StopRoll();
                StopCoroutine(Roll());
                if (isGrounded)
                {
                    rb.velocity = new Vector2(transform.localScale.x * attackMovementSpeed, rb.velocity.y);
                }
                else
                {
                    rb.velocity = new Vector2(0, rb.velocity.y);
                }
                return;

            } else if (!isRolling)
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
           
        }

        if (isRolling)
        {
            return;
        }

        movementX = Input.GetAxisRaw("Horizontal");

        Flip();

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

        if (Input.GetKeyDown(KeyCode.LeftShift) && canRoll || Input.GetKeyDown(KeyCode.S) && canRoll || Input.GetKeyDown(KeyCode.DownArrow) && canRoll)
        {
            if (currentStamina - rollStaminaAmount >= 0f)
            {
                StartCoroutine(Roll());
            }
            
        }

        if (Input.GetKeyDown(KeyCode.C) && daggerThrowCooldownTimer <= 0 && currentDaggerAmmo > 0)
        {
            if (isFacingRight) 
            {
                Instantiate(dagger, daggerSpawnPoint.position, Quaternion.identity);
                
            } else
            {
                Instantiate(dagger, daggerSpawnPoint.position, transform.rotation * Quaternion.Euler(0f, 180f, 0f));
            }
            daggerThrowCooldownTimer = daggerThrowCooldown;
            currentDaggerAmmo -= 1;
            lastDaggerThrown = Time.time;
            GameManager.gameManager.SetDaggerAmmoUI(currentDaggerAmmo);
        }

        bushInRange =  Physics2D.OverlapBoxAll(transform.position, bushCheckSize, 0, bushLayer);

        if (bushInRange.Length > 0)
        {
            bushInRangeUI.SetActive(true);
        } else
        {
            bushInRangeUI.SetActive(false);
        }

        if (Input.GetKeyDown(KeyCode.E) && bushInRange.Length > 0)
        {
            BushJumpCheck();
        }
    }

    // -------------------------------- Fixed Update -----------------------------

    private void FixedUpdate() 
    {
        if (isRolling || attacking || takingDmg || isDead)
        {
            return;
        }
        rb.velocity = new Vector2(movementX * movementSpeed, rb.velocity.y);
    }
    
    // ------------------------My homemade functions------------------------------

    void BushJumpCheck()
    {
        float closestBush = 100000;

        foreach (Collider2D bush in bushInRange)
        {
            float bushDistance = Vector3.Distance(transform.position, bush.gameObject.transform.position);

            if (bushDistance < closestBush)
            {
                closestBush = bushDistance;
                
                closestBushTransform = bush.gameObject.transform;
                bushScript = closestBushTransform.GetComponent<bushC>();
            }
        }

        if (closestBush != 100000)
        {
            jumpingIntoBush = true;
            invicible = true;
            JumpBush(closestBushTransform);
        }
    }

    void JumpBush(Transform bush) 
    {
        StopAllCoroutines();
        canRoll = true;
        bushStartPoint = transform.position;
        
        if (jumpingIntoBush)
        {
            originalPos = transform.position;
            bushEndPoint = bush.position;
            bushControlPoint = bushStartPoint + (bushEndPoint - bushStartPoint)/2 + Vector3.up * bushJumpHeight;

        } else if (jumpingOutBush)
        {
            if (originalPos.x - bush.position.x > 0)
            {
                bushEndPoint = new Vector3(bush.position.x + 3f, bush.position.y + 1f, bush.position.z);
            } else
            {
                bushEndPoint = new Vector3(bush.position.x - 3f, bush.position.y + 1f, bush.position.z);
            }
            
            bushControlPoint = bushStartPoint +(bushEndPoint -bushStartPoint)/2 + Vector3.up * 5.0f;

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
        staminaRechargeTimer = staminaRechargeTime;
        currentStamina -= rollStaminaAmount;
        SB.SetStamina(currentStamina);
        invicible = true;
        canRoll = false;
        isRolling = true;
        originalWidth = bcoll.size.x;
        originalHight = bcoll.size.y;
        originalOffsetX = bcoll.offset.x;
        originalOffsetY = bcoll.offset.y;
        bcoll.size = new Vector2(rollWidth, rollHight);
        bcoll.offset = new Vector2(rollOffsetX, rollOffsetY);
        rb.velocity = new Vector2(transform.localScale.x * rollSpeed, rb.velocity.y);

        yield return new WaitForSeconds(rollTime);
        yield return new WaitUntil(() => !isCeiling);

        StopRoll();
    }

    private void StopRoll()
    {
        bcoll.size = new Vector2(originalWidth, originalHight);
        bcoll.offset = new Vector2(originalOffsetX, originalOffsetY);
        invicible = false;
        isRolling = false;
        canRoll = true;
    }

    private void AttackAnim()
    {
        lastClickedTime = Time.time;
        noOfClicks++;
        noOfClicks = Mathf.Clamp(noOfClicks, 0, 3);

        anim.speed = 1f + meleeSpeed;

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

    public void WillPlayerCrit()
    {
        if (rb.velocity.y < -4f)
        {
            willCrit = true;
        } else
        {
            willCrit = false;
        }
    }

    public void PlayerAttack() 
    {
        // Dmg Enemies 
        Collider2D[] enemy = Physics2D.OverlapCircleAll(attackPoint.transform.position, attackRadius, enemies);
        if (willCrit || rb.velocity.y < -4f)
        {
            willCrit = false;

            foreach (Collider2D enemyGameobject in enemy)
            {
                GameManager.gameManager.DamageEnemy(enemyGameobject, meleeDmg * critDmg, transform);
                Instantiate(critParticle, enemyGameobject.transform.position, Quaternion.identity);
            }
            
        } else
        {
            foreach (Collider2D enemyGameobject in enemy)
            {
                GameManager.gameManager.DamageEnemy(enemyGameobject, meleeDmg, transform);
            }
        }
    
        // Hit Tree
        Collider2D[] trees = Physics2D.OverlapCircleAll(attackPoint.transform.position, attackRadius, treeLayer);

        foreach (Collider2D tree in trees)
        {
            var treeScript = tree.GetComponent<TreeC>();
            treeScript.TreeShake();
        }        
    }

    public void PlayerTakeDmg(float dmg, Transform attacker)
    {
        
        if (iFrameCountdown <= 0 && !invicible)
        {
            float hitChance = Random.Range(0f, 1f);
            if (hitChance >= dodgeChance)
            {
                currentHealth -= dmg * (1 - dmgReduction);

                HB.SetHealth(currentHealth);

                if (currentHealth <= 0)
                {
                    isDead = true;
                } else
                {
                    camC.CameraStartShake(2, 2);
                    dmgTimerCountdown = dmgTime;
                    iFrameCountdown = iFrameTime;
                    h_Regen_Timer = h_Regen_Wait;
                    takingDmg = true;
                    ChangeAnimationState(PLAYER_DAMAGED);
                    Knockback(attacker);
                }
            } else
            {
                iFrameCountdown = iFrameTime;
                // player Dodge Animation
            }
            
        }
    }

    public void PlayerHeal(float healAmount)
    {
        currentHealth += healAmount;
        if (currentHealth > maxHealth)
        {
            currentHealth = maxHealth;
        }
        HB.SetHealth(currentHealth);

        // Heal UI above player
    }

    private void Knockback(Transform attacker)
    {
        Vector2 hitDir = (transform.position - attacker.position).normalized;
        rb.velocity = Vector2.zero;
        rb.AddForce(hitDir * knockbackPower, ForceMode2D.Impulse);
        attacking = false;        
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
        Gizmos.DrawWireCube(ceillingCheck.position, ceillingCheckSize);

        Gizmos.DrawWireCube(transform.position, bushCheckSize);
    }
}
