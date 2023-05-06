using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TwigC : MonoBehaviour
{
    [SerializeField] private float speed;
    [SerializeField] float dmg;    
    [SerializeField] float groundLayerNum;
    private Rigidbody2D rb;
    


    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.velocity = transform.right * speed;
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
}
