using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlyerC : MonoBehaviour
{
    playerController PC;

    // Physics
    private Rigidbody2D rb;
    [SerializeField] private float speed;
    
    // Health
    int currentHealth;
    [SerializeField] int health;
    
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        PC = GameObject.FindGameObjectWithTag("Player").GetComponent<playerController>();
        currentHealth = health;
    }

    
    void Update()
    {
        transform.position =  Vector2.Lerp(transform.position, GameManager.gameManager.player.position, speed * Time.deltaTime);
    }

    
}
