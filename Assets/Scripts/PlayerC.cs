using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerC : MonoBehaviour
{
    [Header("Refrences")]
    CameraC camC;
    bushC bushScript;
    [SerializeField] HealthBar HB;
    [SerializeField] StaminaBarC SB;
    daggerAmmoUI daggerUIScript;

    // States
    private enum State {
        idle,
        walking,
        running,
        rolling,
        jumping,
        attacking,
    }

    State currentState;

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
    bool wasNotGround;
    [SerializeField] Vector2 groundCheckSize;

    [Header("Ceiling Check")]
    [SerializeField] Vector2 ceillingCheckSize;
    [SerializeField] Transform ceillingCheck;
    [SerializeField] LayerMask headHitters;
    bool isCeiling;

    int currentNumOfJumps;
    [Header("Jumping")]
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
    [SerializeField] float daggerThrowTime;
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

    
    [Header("SFX")]
    AudioSource audioSource;
    [SerializeField] AudioClip runSFX;
    [SerializeField] AudioClip meleeSFX;
    [SerializeField] AudioClip daggerSFX;
    [SerializeField] AudioClip hitSFX;
    [SerializeField] AudioClip hit1SFX;
    [SerializeField] AudioClip jumpSFX;
    [SerializeField] AudioClip rollSFX;


    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        bcoll = GetComponent<BoxCollider2D>();
        camC = GameObject.FindGameObjectWithTag("vcam").GetComponent<CameraC>();
        spriteRend = GetComponent<SpriteRenderer>();
        audioSource = GetComponent<AudioSource>();

        daggerUIScript = GameObject.Find("DaggerAmmo").GetComponent<daggerAmmoUI>();

        currentHealth = maxHealth;
        currentDaggerAmmo = daggerAmmo;
        currentStamina = maxStamina;
        jumpOffJumpTimer = jumpOffJumpTime;
        waitToCheckForJumpTimer = waitToCheckForJump;   

        HB.SetHealth(currentHealth, maxHealth);
    }


    void Update()
    {
        if (isDead)
        {
            Died();
            return;
        }
    }
}
