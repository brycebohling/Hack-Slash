using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BGAnimation : MonoBehaviour
{
    Animator anim;
    [SerializeField] float timeBetweenAnimMin;
    [SerializeField] float timeBetweenAnimMax;
    float timeToNextAnim;
    string currentAnim;
    
    [SerializeField] List<string> animNames = new List<string>();


    void Start()
    {
        timeToNextAnim = Random.Range(timeBetweenAnimMin, timeBetweenAnimMax);
        anim = GetComponent<Animator>();
    }

    void Update()
    {
        if (!IsAnimationPlaying(anim, currentAnim))
        {
            anim.Play("Empty");
        }

        if (timeToNextAnim > 0)
        {
            timeToNextAnim -= Time.deltaTime;
        } else
        {
            currentAnim = animNames[Random.Range(0, animNames.Count)];
            anim.Play(currentAnim);
            timeToNextAnim = Random.Range(timeBetweenAnimMin, timeBetweenAnimMax);
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
}
