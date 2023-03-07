using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class enemy : MonoBehaviour
{
    private Rigidbody2D rb;
    private float movementX;
    private bool isFacingRight;
    [SerializeField] private float speed;
    [SerializeField] private Transform target;
    [SerializeField] private float minDistance;

    private void Start() 
    {
        rb = GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
        movementX = rb.velocity.x;

        if (Vector2.Distance(transform.position, target.position) > minDistance)
        {
            transform.position =  Vector2.MoveTowards(transform.position, target.position, speed * Time.deltaTime);

            if (target.position.x > transform.position.x)
            {
                if (isFacingRight)
                {
                    isFacingRight = !isFacingRight;
                    Vector3 localScale = transform.localScale;
                    localScale.x *= -1f;    
                    transform.localScale = localScale;
                }
            } else
            {
                if (!isFacingRight)
                {
                    isFacingRight = !isFacingRight;
                    Vector3 localScale = transform.localScale;
                    localScale.x *= -1f;
                    transform.localScale = localScale;
                }
            }        
        } 
    }

    private void FixedUpdate() 
    {
        
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
}
