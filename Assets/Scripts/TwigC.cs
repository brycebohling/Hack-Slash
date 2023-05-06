using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TwigC : MonoBehaviour
{
    [SerializeField] private float speed;
    [SerializeField] float dmg;    
    [SerializeField] float groundLayerNum;
    private Rigidbody2D rb;
    int randomRotationY;
    public bool shootRight;
    

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        SetDirection();
    }

    private void OnTriggerEnter2D(Collider2D collision) 
    {
        if (collision.gameObject.CompareTag("Player") && !GameManager.gameManager.isPLayerInvicible)
        {
            GameManager.gameManager.DamagePlayer(dmg, transform);
            Destroy(gameObject);
        } else if (collision.gameObject.layer == groundLayerNum)
        {
            Destroy(gameObject);
        }
        
    }

    void SetDirection()
    {
        if (shootRight)
        {
            randomRotationY = Random.Range(-2, 2);
            rb.velocity = new Vector2(speed, randomRotationY).normalized * speed;
        } else
        {
            randomRotationY = Random.Range(-2, 2);
            rb.velocity = new Vector2(-speed, randomRotationY).normalized * speed;
        }
    }
}
