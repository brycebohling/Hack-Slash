using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossTeleporter : MonoBehaviour
{
    playerController playerScript;
    [SerializeField] float maxDistanceFromTele;
    [SerializeField] Vector2 bossArenaTelePos;
    [SerializeField] Vector2 arenaTelePos;
    [SerializeField] float timeToActive;
    float timeToActiveTimer;
    bool active;
    public bool toBoss;
    

    private void Start() 
    {
        playerScript = GameObject.FindWithTag("Player").GetComponent<playerController>();
        timeToActiveTimer = timeToActive;
    }

    private void Update() 
    {
        if (timeToActiveTimer > 0)
        {
            timeToActiveTimer -= Time.deltaTime;
        } else
        {
            active = true;
        }

        float playerDistance = Vector2.Distance(GameManager.gameManager.playerPos, transform.position);
        
        if (playerDistance < maxDistanceFromTele && active)
        {
            if (toBoss)
            {
                playerScript.Teleport(bossArenaTelePos);
            } else
            {
                playerScript.Teleport(arenaTelePos);
            }

            Destroy(gameObject);
        }
    }
}
