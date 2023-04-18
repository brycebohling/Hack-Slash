using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeartC : MonoBehaviour
{
    bool isTouchingPlayer;
    [SerializeField] LayerMask playerLayer;
    [SerializeField] Vector2 hitBoxSize;
    [SerializeField] float healingAmount;


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
