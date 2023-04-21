using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class bushC : MonoBehaviour
{
    // Animations
    Animator anim;
    string _currentState;
    string bushShakeAnim = "shake";
    string bushNormalAnim = "normal";
    bool isPlayingShake;
    [SerializeField] Transform shakeParticals;
    bool isDead;
    [SerializeField] float fadeSpeed;
    SpriteRenderer rend;
    Color color;
    [SerializeField] float waitForDeathTime;
    float waitForDeathCountdown;
    [SerializeField] int dyingBushLayer;


    void Start()
    {
        anim = GetComponent<Animator>();
        rend = GetComponent<SpriteRenderer>();
        color = rend.color;
    }

    void Update()
    {
        if (isDead)
        {
            gameObject.layer = dyingBushLayer;
            waitForDeathCountdown -= Time.deltaTime;

            if (waitForDeathCountdown <= 0)
            {  
                color.a -= fadeSpeed * Time.deltaTime;

                rend.color = color;
            }   

            if (color.a <= 0)
            {
                GameManager.gameManager.BushDead(transform);
                Destroy(gameObject);
            }
        }

        if (!IsAnimationPlaying(anim, bushShakeAnim))
        {
            ChangeAnimationState(bushNormalAnim);
        }
    }

    public void KillBush()
    {
        isDead = true;
    }

    public void BushShake()
    {
        Instantiate(shakeParticals, transform.position, Quaternion.identity);
        ChangeAnimationState(bushShakeAnim);
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
