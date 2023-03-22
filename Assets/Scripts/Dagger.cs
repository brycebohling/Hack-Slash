using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dagger : MonoBehaviour
{
    [SerializeField] private float speed;
    [SerializeField] int dmg;
    [SerializeField] float rotationSpeed;
    bool isRotating = false;
    float angle;
    [SerializeField] LayerMask playerEnemy;
    bool isTouchingEnemy;
    [SerializeField] int enemyLayerNum;
    private Rigidbody2D rb;
    Vector2 direction;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.velocity = transform.right * speed;
    }

    void Update()
    {
        if (isRotating)
        {
            // transform.rotation = Quaternion.Euler(Vector3.forward * angle);
            
            // transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.Euler(transform.rotation.x, transform.rotation.y, angle), Time.deltaTime * rotationSpeed);
            Quaternion rotation = Quaternion.AngleAxis(angle, Vector3.forward);
            transform.rotation = Quaternion.Lerp(transform.rotation, rotation, rotationSpeed * Time.deltaTime);

            // rb.velocity = Vector3.RotateTowards(rb.velocity, direction, speed * Time.deltaTime * Mathf.Deg2Rad, 0);            
            rb.velocity = direction.normalized * rb.velocity;
        }  
        
        isTouchingEnemy = Physics2D.OverlapBox(transform.position, new Vector2(1f, 0.5f), transform.rotation.z, playerEnemy);

        if (isTouchingEnemy)
        {
            Collider2D[] enemy = Physics2D.OverlapBoxAll(transform.position, new Vector2(1f, 0.5f), playerEnemy);

            foreach (Collider2D enemyGameobject in enemy)
            {
                GameManager.gameManager.DamageEnemy(enemyGameobject, dmg, transform);
            }
        }
        
    } 

    private void OnTriggerEnter2D(Collider2D collision) 
    {
        if (collision.gameObject.layer == enemyLayerNum)
        {
            Vector2 daggerPos = new Vector2(transform.position.x, transform.position.y);
            
            Vector2 collisionPos = new Vector2(collision.gameObject.transform.position.x, collision.gameObject.transform.position.y);
            
            direction = collisionPos - daggerPos;
            
            angle = (Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg);

            isRotating = true;
        }
    }

    private void OnDrawGizmos() 
    {
        Gizmos.DrawWireCube(transform.position, new Vector2(1.0f, 0.5f));    
    }
}
