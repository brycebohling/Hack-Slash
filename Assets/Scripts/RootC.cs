using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RootC : MonoBehaviour
{
    Animator anim;
    bool canAttack;
    [SerializeField] float lifeTime;
    [SerializeField] int dmg;
    [SerializeField] Vector2 rootSize;
    [SerializeField] LayerMask playerLayer;
    [SerializeField] GameObject deathParticles;
    [SerializeField] Transform center;
    

    void Start()
    {
        anim = GetComponent<Animator>();

    }

    
    void Update()
    {
        lifeTime -= Time.deltaTime;
    
        if (lifeTime <= 0)
        {
            Instantiate(deathParticles, center.position, Quaternion.identity);
            Destroy(gameObject);
        }

        bool isTouchingPlayer = Physics2D.OverlapBox(center.position, rootSize, 0, playerLayer);

        if (isTouchingPlayer && canAttack)
        {
            GameManager.gameManager.DamagePlayer(dmg, transform);
        }
    }

    public void CanAttack()
    {
        canAttack = true;
    }

    private void OnDrawGizmos() 
    {   
        Gizmos.DrawWireCube(center.position, rootSize);
        
    }
}
