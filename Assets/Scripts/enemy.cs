using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class enemy : MonoBehaviour
{
    private Rigidbody2D rb;
    [SerializeField] private float speed;
    [SerializeField] private Transform target;
    [SerializeField] private float minDistance;

    private void Start() 
    {
        rb = GetComponent<Rigidbody2D>();
    }

    private void FixedUpdate() 
    {
        if (Vector2.Distance(transform.position, target.position) > minDistance)
        {
            if (target.position.x > transform.position.x)
            {
                transform.position =  Vector2.MoveTowards(transform.position, target.position, speed);
            } else
            {
                rb.AddForce(Vector2.right * -speed);
            }        
        } 
    }
}
