using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dagger : MonoBehaviour
{
    playerController PC;
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
        PC = GameObject.FindWithTag("Player").GetComponent<playerController>();
        rb = GetComponent<Rigidbody2D>();
        rb.velocity = transform.right * PC.daggerSpeed;
    }

    void Update()
    {
        if (isRotating)
        {
            Quaternion rotation = Quaternion.AngleAxis(angle, Vector3.forward);
            transform.rotation = rotation;

            rb.velocity = transform.right * PC.daggerSpeed;
        }  
        
        isTouchingObject = Physics2D.OverlapBox(transform.position, new Vector2(1f, 0.5f), transform.rotation.z, TouchableObjects);

        if (isTouchingObject)
        {
            Collider2D[] enemy = Physics2D.OverlapBoxAll(transform.position, new Vector2(1f, 0.5f), enemyLayer);

            foreach (Collider2D enemyGameobject in enemy)
            {
                GameManager.gameManager.DamageEnemy(enemyGameobject, PC.daggerDmg, transform);
                PC.totalDamageDealt += Mathf.RoundToInt(PC.daggerDmg);
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
