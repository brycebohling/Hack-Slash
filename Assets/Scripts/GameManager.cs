using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager gameManager { get; private set; }

    public GameObject player;
    private playerController playerScript;
    Renderer playerRenderer;
    public bool isPlayerRendered;

    void Awake()
    {
        if (gameManager != null && gameManager != this)
        {
            Destroy(this);
        } else
        {
            gameManager = this;
        }
    }

    void Start()
    {
        player = GameObject.Find("Player");
        playerScript = player.GetComponent<playerController>();
        playerRenderer = player.GetComponent<Renderer>();
    }

    void Update()
    {
        if (playerRenderer.enabled == true)
        {
            isPlayerRendered = true;
            
        } else
        {
            isPlayerRendered = false;
        }
    }

    public void DamageEnemy(Collider2D enemy, int dmg, Transform attacker)
    {
        if (enemy.CompareTag("redBoy"))
        {
            enemy.gameObject.GetComponent<RedBoyC>().DmgRedBoy(dmg, attacker);

        } else if (enemy.CompareTag("flyer"))
        {
            enemy.gameObject.GetComponent<FlyerC>().DmgFlyer(dmg, attacker);
        } else if (enemy.CompareTag("archer"))
        {
            enemy.gameObject.GetComponent<ArcherC>().DmgArcher(dmg, attacker);
        }
    }

    public void DamagePlayer(int dmg, Transform attacker)
    {
        playerScript.PlayerTakeDmg(dmg, attacker);
    }
}
