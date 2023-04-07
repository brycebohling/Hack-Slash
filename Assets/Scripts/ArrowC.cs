using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArrowC : MonoBehaviour
{
    [SerializeField] private float speed;
    [SerializeField] int dmg;    
    private Rigidbody2D rb;
    


    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.velocity = transform.right * speed;
    }

    void Update()
    {       

    } 

    private void OnTriggerEnter2D(Collider2D collision) 
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            GameManager.gameManager.DamagePlayer(dmg, transform);
        }
        Destroy(gameObject);
    }
}
