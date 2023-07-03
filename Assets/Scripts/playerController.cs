using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class playerController : MonoBehaviour
{
    [Header("Refrences")]
    CameraC camC;
    bushC bushScript;
    [SerializeField] HealthBar HB;
    [SerializeField] StaminaBarC SB;
    daggerAmmoUI daggerUIScript;
    [SerializeField] WaveSpawner waveSpawnerScript;
    

    [Header("Stats")]
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
    public float healthRegenerationPercent;

    [Header("Movement")]
    float movementX;
    public bool isFacingRight;

    [Header("Key Binds")]
    [SerializeField] List<KeyCode> meleeAttackBinds = new List<KeyCode>();
    [SerializeField] List<KeyCode> throwDaggerBinds = new List<KeyCode>();
    [SerializeField] List<KeyCode> jumpBinds = new List<KeyCode>();
    [SerializeField] List<KeyCode> rollBinds = new List<KeyCode>();
    [SerializeField] List<KeyCode> jumpInBushBinds = new List<KeyCode>();
    
    // Physics
    Rigidbody2D rb;
    BoxCollider2D bcoll;

    // Animatinon
    Animator anim;
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
    
    [Header("Ground Check")]
    [SerializeField] Transform groundCheck;
    [SerializeField] LayerMask groundLayer;
    bool isGrounded;
    bool wasGrounded;
    [SerializeField] Vector2 groundCheckSize;

    [Header("Ceiling Check")]
    [SerializeField] Vector2 ceillingCheckSize;
    [SerializeField] Transform ceillingCheck;
    [SerializeField] LayerMask headHitters;
    bool isCeiling;

    [Header("Jumping")]
    int currentNumOfJumps;
    [SerializeField] float jumpOffJumpTime;
    float jumpOffJumpTimer;
    [SerializeField] float waitToCheckForJump;
    float waitToCheckForJumpTimer;

    [Header("Rolling")]
    [SerializeField] float rollWidth;
    [SerializeField] float rollHight;
    [SerializeField] float rollOffsetX;
    [SerializeField] float rollOffsetY;
    [SerializeField] float rollTime;
    bool canRoll = true;
    bool isRolling; 
    bool wasRolling;
    float originalWidth;
    float originalHight;
    float originalOffsetX;
    float originalOffsetY;

    [Header("Stamina")]
    public float currentStamina;
    [SerializeField] float staminaRechargeTime;
    float staminaRechargeTimer;
    [SerializeField] float staminaRechargeSpeed;
    [SerializeField] float rollStaminaAmount;

    [Header("Melee Attack")]
    public int totalDamageDealt;
    public static int noOfClicks = 0;
    float lastClickedTime = 0;
    [SerializeField] float maxComboDelay = 1;
    bool attacking;
    [SerializeField] float attackMovementSpeed;
    [SerializeField] GameObject attackPoint;
    [SerializeField] float attackRadius;
    [SerializeField] LayerMask enemies;
    [SerializeField] Transform critParticle;
    bool willCrit;
    [SerializeField] LayerMask rootLayer;

    [Header("Dagger")]
    public GameObject dagger;
    int daggerAmmo = 3;
    [SerializeField] float daggerThrowCooldown;
    float daggerThrowCooldownTimer;
    int currentDaggerAmmo;
    float lastDaggerThrown;
    [SerializeField] float daggerWaitToRechargeTime;
    [SerializeField] float daggerRechargingTime;
    float currentDaggerRechargingTime;
    [SerializeField] Transform daggerSpawnPoint;
    [SerializeField] GameObject daggerUIObejct;

    [Header("Health")]
    public float currentHealth;
    int currentWave = 1;
    [SerializeField] Transform dodgeParticles;
    [SerializeField] GameObject healingText;

    [Header("Taking Damage")]
    [SerializeField] float iFrameTime;
    float iFrameCountdown;
    public bool invicible;
    bool isTakingDmg;
    [SerializeField] float dmgTime;
    float dmgTimerCountdown;
    [SerializeField] float knockbackPower;

    [Header("Death")]
    bool isDead;
    [SerializeField] GameObject deathParticals;
    
    [Header("Bush Mechanics")]
    Collider2D[] bushInRange;
    float bushJumpHeight = 5f;
    bool jumpingIntoBush;
    bool jumpingOutBush;
    float bushLerp = 0.0f;
    [SerializeField] float bushTime;
    [SerializeField] float bushLerpSpeed;
    float timeInBush;
    Vector3 bushStartPoint;
    Vector3 bushControlPoint;
    Vector3 bushEndPoint;
    Vector3 originalPos;
    [SerializeField] Vector2 bushCheckSize;
    [SerializeField] LayerMask bushLayer;
    Transform closestBushTransform;
    bool didBushShake = false;
    float bushHealthAmount = 5;

    [Header("Tree Mechanics")]
    [SerializeField] LayerMask treeLayer;

    [Header("UI")]
    [SerializeField] GameObject bushInRangeUI;

    [Header("Bush Teleporter")]
    [SerializeField] GameObject autoDestroyTeleporter;


    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        bcoll = GetComponent<BoxCollider2D>();
        camC = GameObject.FindGameObjectWithTag("vcam").GetComponent<CameraC>();
        spriteRend = GetComponent<SpriteRenderer>();

        daggerUIScript = GameObject.Find("DaggerAmmo").GetComponent<daggerAmmoUI>();
        waveSpawnerScript = GameManager.gameManager.GetComponent<WaveSpawner>();

        currentHealth = maxHealth;
        currentDaggerAmmo = daggerAmmo;
        currentStamina = maxStamina;
        jumpOffJumpTimer = jumpOffJumpTime;
        waitToCheckForJumpTimer = waitToCheckForJump;   

        HB.SetHealth(currentHealth, maxHealth);
    }
    

    private void Update()
    {   
        if (isDead)
        {
            Died();
            return;
        }

        movementX = Input.GetAxisRaw("Horizontal");

        DidWaveChange();    
        UpdateTimers();
        UpdateStaminaBar();
        UpdateDaggerAmmo();

        isGrounded = Physics2D.OverlapBox(groundCheck.position, groundCheckSize, 0, groundLayer);
        isCeiling = Physics2D.OverlapBox(ceillingCheck.position, ceillingCheckSize, 0, headHitters);

        UpdateJumpStatus();
        
        BushJumpCheck();
        if (jumpingIntoBush)
        {
            JumpIntoBush();
            return;
        }

        if (jumpingOutBush)
        {
            JumpOutBush();
            return;
        }

        if (isTakingDmg)
        {
            dmgTimerCountdown -= Time.deltaTime;

            if (dmgTimerCountdown <= 0) 
            {
                isTakingDmg = false;
            }

            return;
        } else
        {
            isTakingDmg = false;
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

        HandleJump();

        HandleAttack();
        if (attacking)
        {
            return;
        }
        
        if (isRolling)
        {
            return;
        }

        Flip();

        HandleMovementAnimations();

        HandleRoll();

        HandleDaggerThrowing();

        UpdateBushUI();
    }

    // -------------------------------- Fixed Update -----------------------------

    private void FixedUpdate() 
    {
        if (isRolling || attacking || isTakingDmg || isDead)
        {
            return;
        }

        if (!isGrounded && wasRolling || wasGrounded && wasRolling)
        {
            rb.velocity = new Vector2(movementX * rollSpeed, rb.velocity.y);   
        } else
        {
            rb.velocity = new Vector2(movementX * movementSpeed, rb.velocity.y);
        }
        
    }
    
    // ------------------------My homemade functions------------------------------


    private void Died()
    {
        Instantiate(deathParticals, transform.position, Quaternion.identity);
        GameManager.gameManager.PlayerDied();

        gameObject.SetActive(false);
    }

    private void UpdateStaminaBar()
    {
        if (staminaRechargeTimer > 0f)
        {
            staminaRechargeTimer -= Time.deltaTime;
        } else if (currentStamina < maxStamina)
        {
            currentStamina += Time.deltaTime * staminaRechargeSpeed;
            if (currentStamina > maxStamina)
            {
                currentStamina = maxStamina;
            }
            SB.SetStamina(currentStamina);
        }
    }

    private void UpdateDaggerAmmo()
    {
        if (Time.time - lastDaggerThrown > daggerWaitToRechargeTime && currentDaggerRechargingTime <= 0f && currentDaggerAmmo + 1 <= daggerAmmo)
        {
            currentDaggerRechargingTime = daggerRechargingTime;
            currentDaggerAmmo++;
            daggerUIScript.ChangeDaggerAmmoUI(currentDaggerAmmo);
        }
    }

    private void HandleAttack()
    {
        bool pressedAttack = false;

        foreach(KeyCode keyBind in meleeAttackBinds)
        {
            if (Input.GetKeyDown(keyBind))
            {
                pressedAttack = true;
                break;
            }
        }

        if (pressedAttack)
        {
            if (isRolling && !isCeiling || !isRolling)
            {
                AttackAnim();
                attacking = true;

                if (isRolling && !isCeiling)
                {
                    StopCoroutine(Roll());
                    StopRoll();
                }

                if (isGrounded)
                {
                    rb.velocity = new Vector2(transform.localScale.x * attackMovementSpeed, rb.velocity.y);
                } else
                {
                    rb.velocity = new Vector2(0, rb.velocity.y);
                }

                return;
            }
        }
    }

    void BushJumpCheck()
    {
        bool pressedBushJump = false;

        foreach(KeyCode keyBind in jumpInBushBinds)
        {
            if (Input.GetKeyDown(keyBind))
            {
                pressedBushJump = true;
                break;
            }
        }

        if (pressedBushJump && bushInRange.Length > 0)
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

            wasRolling = false;
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
        wasRolling = true;
    }

    private void AttackAnim()
    {
        lastClickedTime = Time.time;
        noOfClicks++;
        noOfClicks = Mathf.Clamp(noOfClicks, 0, 3);

        anim.speed = meleeSpeed;

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
                totalDamageDealt += Mathf.RoundToInt(meleeDmg * critDmg);
            }
            
        } else
        {
            foreach (Collider2D enemyGameobject in enemy)
            {
                GameManager.gameManager.DamageEnemy(enemyGameobject, meleeDmg, transform);
                totalDamageDealt += Mathf.RoundToInt(meleeDmg);
            }
        }
    
        // Hit Tree
        Collider2D[] trees = Physics2D.OverlapCircleAll(attackPoint.transform.position, attackRadius, treeLayer);

        foreach (Collider2D tree in trees)
        {
            var treeScript = tree.GetComponent<TreeC>();
            treeScript.TreeShake();
        }        

        // Hit Roots
        Collider2D[] roots = Physics2D.OverlapCircleAll(attackPoint.transform.position, attackRadius, rootLayer);

        foreach (Collider2D root in roots)
        {
            var rootScript = root.GetComponent<RootC>();
            if (rootScript.canAttack)
            {   
                rootScript.killed = true;
            }
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
                
                HB.SetHealth(currentHealth, maxHealth);
            
                GameManager.gameManager.killStreak = 0;

                if (currentHealth <= 0)
                {
                    isDead = true;
                } else
                {
                    camC.CameraStartShake(2, 2);
                    dmgTimerCountdown = dmgTime;
                    iFrameCountdown = iFrameTime;
                    isTakingDmg = true;
                    ChangeAnimationState(PLAYER_DAMAGED);
                    Knockback(attacker);
                }
            } else
            {
                iFrameCountdown = iFrameTime;
                Instantiate(dodgeParticles, transform.position, Quaternion.identity);
            }
        }
    }

    public void PlayerInHardcore()
    {
        currentHealth = 1f;
        HB.SetHealth(currentHealth, maxHealth);
    }

    public void PlayerHeal(float healAmount)
    {
        if (GameManager.gameManager.difficulty == "normal")
        {
            float healthToMax = maxHealth - currentHealth;
            
            if (healAmount != 0 && healthToMax > 0)
            {
                currentHealth += healAmount;

                float textOffsetX = 1.5f;
                float textOffsetY = 1f;

                GameObject prefab = Instantiate(healingText, new Vector2(transform.position.x + textOffsetX, transform.position.y + textOffsetY), Quaternion.identity);
                
                if (currentHealth > maxHealth)
                {
                    currentHealth = maxHealth;

                    prefab.GetComponentInChildren<TextMeshPro>().text = Mathf.RoundToInt(healthToMax).ToString();
                } else
                {
                    prefab.GetComponentInChildren<TextMeshPro>().text = Mathf.RoundToInt(healAmount).ToString();
                }            
            }
            
            HB.SetHealth(currentHealth, maxHealth);
        } else if (GameManager.gameManager.difficulty == "hardcore")
        {
            GameManager.gameManager.IncScore(Mathf.RoundToInt(healAmount * 10));
        }
    }

    private void Knockback(Transform attacker)
    {
        Vector2 hitDir = (transform.position - attacker.position).normalized;
        rb.velocity = Vector2.zero;
        rb.AddForce(hitDir * knockbackPower, ForceMode2D.Impulse);
        attacking = false;        
    }

    private void DidWaveChange()
    {
        if (currentWave != waveSpawnerScript.waveNumber)
        {
            currentWave = waveSpawnerScript.waveNumber;
            PlayerHeal(maxHealth * healthRegenerationPercent); 
        } 
    }

    public void Teleport(Vector2 pos)
    {
        transform.position = pos;

        Instantiate(autoDestroyTeleporter, transform.position, Quaternion.identity);
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

    private void UpdateTimers()
    {
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
    }

    private void UpdateJumpStatus()
    {
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
            wasRolling = false;
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
    }

    private void JumpIntoBush()
    {
        daggerUIObejct.SetActive(false);
        if (bushLerp < 1.0f) 
        {
            ChangeAnimationState(PLAYER_START_ROLL);
            
            bushLerp += bushLerpSpeed * Time.deltaTime;

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
                PlayerHeal(bushHealthAmount);
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
    }

    private void JumpOutBush()
    {
        daggerUIObejct.SetActive(true);
        daggerUIScript.ChangeDaggerAmmoUI(currentDaggerAmmo);

        if (bushLerp < 1.0f) 
        {
            if (!didBushShake)
            {
                bushScript.BushShake();
                didBushShake = true;
            }
    
            ChangeAnimationState(PLAYER_START_ROLL);

            bushLerp += bushLerpSpeed * Time.deltaTime;

            Vector3 m1 = Vector3.Lerp(bushStartPoint ,bushControlPoint, bushLerp);
            Vector3 m2 = Vector3.Lerp(bushControlPoint, bushEndPoint, bushLerp );
            transform.position = Vector3.Lerp(m1, m2, bushLerp);
        } else
        {
            bushLerp = 0;
            jumpingOutBush = false;
            didBushShake = false;
            invicible = false;
            bushScript.KillBush();
        }
    }

    private void HandleJump()
    {
        bool pressedJump = false;

        foreach(KeyCode keyBind in jumpBinds)
        {
            if (Input.GetKeyDown(keyBind))
            {
                pressedJump = true;
                break;
            }
        }

        if (pressedJump)
        {
            if (isGrounded || jumpOffJumpTimer > 0f) 
            {   
                if (isRolling && !isCeiling)
                {
                    rb.velocity = new Vector2(rb.velocity.x, jumpForce);
                    jumpOffJumpTimer = 0f;
                    waitToCheckForJumpTimer = waitToCheckForJump;
                    ChangeAnimationState(PLAYER_JUMP);
                    StopCoroutine(Roll());
                    StopRoll();
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
                    currentNumOfJumps--;
                    ChangeAnimationState(PLAYER_JUMP);
                    StopCoroutine(Roll());
                    StopRoll();
                } else if (!isRolling)
                {
                    rb.velocity = new Vector2(rb.velocity.x, jumpForce);
                    currentNumOfJumps--;
                }
            }
        }
    }

    private void HandleMovementAnimations()
    {
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
    }

    private void HandleDaggerThrowing()
    {
        bool pressedThrowDagger = false;

        foreach(KeyCode keyBind in throwDaggerBinds)
        {
            if (Input.GetKeyDown(keyBind))
            {
                pressedThrowDagger = true;
                break;
            }
        }

        if (pressedThrowDagger && daggerThrowCooldownTimer <= 0 && currentDaggerAmmo > 0)
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
            daggerUIScript.ChangeDaggerAmmoUI(currentDaggerAmmo);
        }
    }

    private void HandleRoll()
    {
        bool pressedRoll = false;

        foreach(KeyCode keyBind in rollBinds)
        {
            if (Input.GetKeyDown(keyBind))
            {
                pressedRoll = true;
                break;
            }
        }

        if (pressedRoll && canRoll)
        {
            if (currentStamina - rollStaminaAmount >= 0f)
            {
                StartCoroutine(Roll());
            }
        }
    }

    private void UpdateBushUI()
    {
        bushInRange =  Physics2D.OverlapBoxAll(transform.position, bushCheckSize, 0, bushLayer);

        if (bushInRange.Length > 0)
        {
            bushInRangeUI.SetActive(true);
        } else
        {
            bushInRangeUI.SetActive(false);
        }
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
    
        Gizmos.DrawWireCube(groundCheck.position, groundCheckSize); 
        Gizmos.DrawWireCube(ceillingCheck.position, ceillingCheckSize);

        Gizmos.DrawWireCube(transform.position, bushCheckSize);
    }
}
