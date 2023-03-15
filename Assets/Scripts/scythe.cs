using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class scythe : MonoBehaviour
{
    [SerializeField] private float speed;
    [SerializeField] int dmg;
    [SerializeField] LayerMask enemyLayer;
    private Rigidbody2D rb;
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.velocity = transform.right * speed;
    }

    private void OnTriggerEnter2D(Collider2D collision) 
    {
        if (collision.gameObject.layer == 9)
        {
            GameManager.gameManager.DamageEnemy(collision, dmg);
        }
        Destroy(gameObject);
    }
}
