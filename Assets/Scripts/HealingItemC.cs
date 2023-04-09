using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealingItemC : MonoBehaviour
{
    [SerializeField] LayerMask playerLayer;
    [SerializeField] Vector2 hitBoxSize;
    [SerializeField] int healingAmount;
    bool isTouchingPlayer;

    void Start()
    {
        
    }

    void Update()
    {
        isTouchingPlayer = Physics2D.OverlapBox(transform.position, hitBoxSize, 0, playerLayer);

        if (isTouchingPlayer && GameManager.gameManager.playerCurrentHealth < GameManager.gameManager.playerMaxHealth)
        {
            GameManager.gameManager.HealPlayer(healingAmount);
            Destroy(gameObject);
        }
    }

    private void OnDrawGizmos() 
    {   
        Gizmos.DrawWireCube(transform.position, hitBoxSize);
    }
}
