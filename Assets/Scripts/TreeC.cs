using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TreeC : MonoBehaviour
{
    [System.Serializable] public struct FlyerPoints
    {
        public Transform flyerPoint;
    }

    FlyerPoints[] flyerPoints;
    [SerializeField] GameObject flyer;
    Animator anim;
    string _currentState;
    string treeShakeAnim = "shake";
    [SerializeField] int hitPoints;
    int currentHitPoint;
    [SerializeField] Transform shakeParticals;

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
            // int randomNum = Random.Range(0, 1);
            int randomNum = 1;

            if (randomNum == 0)
            {
                // Instantiate(healingItem, transform.position, Quaternion.identity);
            } else
            {
                var instScript = Instantiate(flyer).GetComponent<FlyerC>();

                Instantiate(flyer, transform.position, Quaternion.identity);

                randomNum = Random.Range(0, 4);
                instScript.SpawningInTree(flyerPoints[randomNum].flyerPoint);

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
