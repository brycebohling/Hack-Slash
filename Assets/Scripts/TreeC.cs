using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TreeC : MonoBehaviour
{
    Animator anim;
    string _currentState;
    string treeNormalAnim = "normal";
    string treeShakeAnim = "shake";
    [SerializeField] Transform shakeParticals;


    void Start()
    {
        anim = GetComponent<Animator>();
    }

    
    void Update()
    {
        if (!IsAnimationPlaying(anim, treeShakeAnim))
        {
            ChangeAnimationState(treeNormalAnim);
        }
    }

    public void TreeShake()
    {
        Instantiate(shakeParticals, transform.position, Quaternion.identity);
        ChangeAnimationState(treeShakeAnim);
        
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
