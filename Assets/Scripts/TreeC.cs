using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TreeC : MonoBehaviour
{
    Animator anim;
    string _currentState;
    string treeNormalAnim = "normal";
    string treeShakeAnim = "shake";
    [SerializeField] int hitPoints;
    int currentHitPoint;
    [SerializeField] Transform shakeParticals;
    bool isDead = false;


    void Start()
    {
        anim = GetComponent<Animator>();
        currentHitPoint = hitPoints;
    }

    public void TreeShake()
    {
        currentHitPoint--;

        if (currentHitPoint <= 0)
        {
            isDead = true;
            int randomNum = Random.Range(0, 1);
            if (randomNum == 0)
            {
                // Instantiate(healingItem, transform.position, Quaternion.identity);
            } else
            {
                // var instScript = Instantiate(flyer).GetComponent<FlyerC>();
                // Instantiate(flyer, transform.position, Quaternion.identity);
                // instScript.TheFlyerFunction
            }
            
        } else
        {   
            anim.Play(treeShakeAnim);
            // Instantiate(shakeParticals, transform.position, Quaternion.identity);
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
