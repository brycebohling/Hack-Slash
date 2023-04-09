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
    [SerializeField] LayerMask enemyLayer;
    [SerializeField] LayerMask TouchableObjects;
    bool isTouchingObject;
    [SerializeField] int enemyLayerNum;
    private Rigidbody2D rb;
    GameObject enemyObject;


    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.velocity = transform.right * speed;
    }

    void Update()
    {
        if (isRotating)
        {
            Quaternion rotation = Quaternion.AngleAxis(angle, Vector3.forward);
            transform.rotation = rotation;

            rb.velocity = transform.right * speed;
        }  
        
        isTouchingObject = Physics2D.OverlapBox(transform.position, new Vector2(1f, 0.5f), transform.rotation.z, TouchableObjects);

        if (isTouchingObject)
        {
            Collider2D[] enemy = Physics2D.OverlapBoxAll(transform.position, new Vector2(1f, 0.5f), enemyLayer);

            foreach (Collider2D enemyGameobject in enemy)
            {
                GameManager.gameManager.DamageEnemy(enemyGameobject, dmg, transform);
            }
            
            Destroy(gameObject);
        }
    } 

    private void OnTriggerEnter2D(Collider2D collision) 
    {
        if (collision.gameObject.layer == enemyLayerNum)
        {
            enemyObject = collision.gameObject;

            Vector2 daggerPos = new Vector2(transform.position.x, transform.position.y);
            Vector2 collisionPos = new Vector2(enemyObject.transform.position.x, enemyObject.transform.position.y);
            Vector2 direction = collisionPos - daggerPos;
            
            angle = (Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg);

            isRotating = true;
        }
    }

    private void OnDrawGizmos() 
    {
        Gizmos.DrawWireCube(transform.position, new Vector2(1.0f, 0.5f));    
    }
}
