using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class playerController : MonoBehaviour
{
    // Movement
    private float movementX;
    [SerializeField] private float speed;
    public bool isFacingRight;

    // Physics
    private Rigidbody2D rb;
    private BoxCollider2D bcoll;

    // Animatinon
    private Animator anim;
    string _currentState;
    const string PLAYER_IDLE = "idle";
    const string PLAYER_RUN = "running";
    const string PLAYER_JUMP = "jump";
    const string PLAYER_FALL = "fall";
    const string PLAYER_START_ROLL = "startRoll";
    const string PLAYER_ROLL = "rolling";
    const string PLAYER_ATTACK_1 = "attack-1";
    const string PLAYER_ATTACK_2 = "attack-2";
    const string PLAYER_ATTACK_3 = "attack-3";


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
    public static int noOfClicks = 0;
    private float lastClickedTime = 0;
    [SerializeField] private float maxComboDelay = 1;
    private bool attacking;
    [SerializeField] private float attackMovementSpeed;

    // Scythe
    public GameObject scythe;
    private Vector2 location;
    [SerializeField] private float throwCooldown;
    private float throwCountdown;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        bcoll = GetComponent<BoxCollider2D>();
    }


    private void Update()
    {
        if (throwCountdown > 0)
        {
            throwCountdown -= Time.deltaTime;
        }

        if (isRolling)
        {
            isCeiling = Physics2D.OverlapBox(ceillingCheck.position, new Vector2(1.3f, 1.7f), 0, headHitters);
            return;
        }

        isGrounded = Physics2D.OverlapBox(groundCheck.position, new Vector2(1.7f, .2f), 0, groundLayer);

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
            Attack();
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
        } else if (movementX != 0 && rb.velocity.y == 0)
        {
            ChangeAnimationState(PLAYER_RUN);
        } else if (rb.velocity.y != 0) 
        {
            if (rb.velocity.y > 0) 
            {
                ChangeAnimationState(PLAYER_JUMP);
            } else 
            {
                ChangeAnimationState(PLAYER_FALL);
            }
        }

        if (Input.GetKeyDown(KeyCode.LeftShift) && canRoll)
        {
            StartCoroutine(Roll());
        }

        if (Input.GetKeyDown(KeyCode.C) && throwCountdown <= 0)
        {
            if (isFacingRight) 
            {
                location = new Vector2(transform.position.x + 3f, transform.position.y + .55f);
                Instantiate(scythe, location, Quaternion.identity);
                
            } else
            {
                location = new Vector2(transform.position.x - 3, transform.position.y + .55f);
                Instantiate(scythe, location, transform.rotation * Quaternion.Euler (0f, 180f, 0f));
            }
            throwCountdown = throwCooldown;
            
        }
    }

    private void FixedUpdate() 
    {
        if (isRolling || attacking)
        {
            return;
        }
        rb.velocity = new Vector2(movementX * speed, rb.velocity.y);
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

    void Attack()
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
