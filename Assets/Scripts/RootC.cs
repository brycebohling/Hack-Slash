using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RootC : MonoBehaviour
{
    Animator anim;
    bool canAttack;
    [SerializeField] float lifeTime;
    [SerializeField] int dmg;
    [SerializeField] GameObject deathParticles;
    

    void Start()
    {
        anim = GetComponent<Animator>();

    }

    
    void Update()
    {
        lifeTime -= Time.deltaTime;
    
        if (lifeTime <= 0)
        {
            // Instantiate(deathParticles, transform.position, Quaternion.identity);
            Destroy(gameObject);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision) 
    {
        if (canAttack && collision.CompareTag("Player"))
        {
            GameManager.gameManager.DamagePlayer(dmg, transform);
        }
    }

    public void CanAttack()
    {
        canAttack = true;
    }
}
