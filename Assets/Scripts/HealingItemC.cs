using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealingItemC : MonoBehaviour
{
    [SerializeField] LayerMask playerLayer;
    [SerializeField] Vector2 hitBoxSize;
    [SerializeField] int healingAmount;
    bool isTouchingPlayer;
    Animator anim;
    string _currentState;
    [SerializeField] float timeBetweenShines;
    float shineTimer;
    string NORMAL = "static";
    string SHINE = "shine";

    void Start()
    {
        anim = GetComponent<Animator>();
        shineTimer = timeBetweenShines;
    }

    void Update()
    {
        isTouchingPlayer = Physics2D.OverlapBox(transform.position, hitBoxSize, 0, playerLayer);

        if (isTouchingPlayer && GameManager.gameManager.playerCurrentHealth < GameManager.gameManager.playerMaxHealth)
        {
            GameManager.gameManager.HealPlayer(healingAmount);
            Destroy(gameObject);
        }

        if (shineTimer > 0f)
        {
            shineTimer -= Time.deltaTime;
            if (!IsAnimationPlaying(anim, SHINE))
            {
                ChangeAnimationState(NORMAL);
            }

        } else 
        {
            ChangeAnimationState(SHINE);
            shineTimer = timeBetweenShines;
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

    private void OnDrawGizmos() 
    {   
        Gizmos.DrawWireCube(transform.position, hitBoxSize);
    }
}
