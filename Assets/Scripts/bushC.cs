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

    void Start()
    {
        anim = GetComponent<Animator>();
    }

    void Update()
    {
        if (!IsAnimationPlaying(anim, bushShakeAnim))
        {
            ChangeAnimationState(bushNormalAnim);
        }
    }

    public void BushShake()
    {
        // StopAllCoroutines();
        Instantiate(shakeParticals, transform.position, Quaternion.identity);
        ChangeAnimationState(bushShakeAnim);
        // StartCoroutine(WaitToChange());
        
    }

    // private IEnumerator WaitToChange()
    // {
    //     yield return new WaitForSeconds(1f);
    //     ChangeAnimationState(bushNormalAnim);
    // }

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
