using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager gameManager { get; private set; }

    public GameObject player;
    private playerController playerScript;
    private WaveSpawner waveScript;
    private daggerAmmoUI daggerUIScript;
    Renderer playerRenderer;
    public bool isPlayerRendered;
    public bool isPLayerInvicible;
    public float playerMaxHealth;
    public float playerCurrentHealth;
    public bool isPlayerDead;

    public int waveNum;


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
        waveScript = GameObject.Find("GameManager").GetComponent<WaveSpawner>();
        daggerUIScript = GameObject.Find("DaggerAmmo").GetComponent<daggerAmmoUI>();


        playerMaxHealth = playerScript.health;
    }

    void Update()
    {
        playerCurrentHealth = playerScript.currentHealth;
        waveNum = waveScript.waveNumber;
        
        if (playerRenderer.enabled == true)
        {
            isPlayerRendered = true;
            
        } else
        {
            isPlayerRendered = false;
        }

        if (playerScript.invicible)
        {
            isPLayerInvicible = true;
        } else
        {
            isPLayerInvicible = false;
        }
        if (playerScript.isDead)
        {
            isPlayerDead = true;
        }
    }

    public void DamageEnemy(Collider2D enemy, float dmg, Transform attacker)
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

    public void DamagePlayer(float dmg, Transform attacker)
    {
        playerScript.PlayerTakeDmg(dmg, attacker);
    }

    public void HealPlayer(float healAmount)
    {
        playerScript.PlayerHeal(healAmount);
    }

    public void SetDaggerAmmoUI(int ammo)
    {
        daggerUIScript.ChangeDaggerAmmoUI(ammo);
    }
}
