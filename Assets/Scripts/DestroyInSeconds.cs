using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyInSeconds : MonoBehaviour
{
    [SerializeField] float destroyWaitTime;

    void Start()
    {
        Destroy(gameObject, destroyWaitTime);   
    }
}
