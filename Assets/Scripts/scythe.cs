using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class scythe : MonoBehaviour
{
    [SerializeField] private float speed;
    [SerializeField] int dmg;
    private Rigidbody2D rb;
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.velocity = transform.right * speed;
    }

    private void OnTriggerEnter2D(Collider2D collision) 
    {
        if (collision.CompareTag("enemy"))
        {
            collision.gameObject.GetComponent<RedBoyC>().RedBoyTakeDmg(dmg);
        }
        Destroy(gameObject);
    }
}
