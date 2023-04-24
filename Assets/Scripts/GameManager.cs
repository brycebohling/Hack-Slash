using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using TMPro;

public class GameManager : MonoBehaviour
{
    public static GameManager gameManager { get; private set; }
    public UnityEvent GamePaused;
    public UnityEvent GameResumed;
    [SerializeField] GameObject PauseScreen;
    bool isPaused;
    public GameObject player;
    private playerController playerScript;
    private WaveSpawner waveScript;
    private daggerAmmoUI daggerUIScript;
    [SerializeField] UpgradeC upgrades;
    Renderer playerRenderer;
    public bool isPlayerRendered;
    public bool isPLayerInvicible;
    public float playerMaxHealth;
    public float playerCurrentHealth;
    public bool isPlayerDead;
    public bool levelingUp;
    [SerializeField] Transform healthItem;
    public int waveNum;
    float score;
    float incScoreValue;
    [SerializeField] TMP_Text scoreText;
    [SerializeField] GameObject scoreIncText;
    [SerializeField] Vector2 socreTextPos;
    public int killStreak;
    [SerializeField] Transform canvas;
    [SerializeField] float incScoreWaitTime;
    float incScoreWaitTimer;
    [SerializeField] TMP_Text killStreakText;


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

        playerMaxHealth = playerScript.maxHealth;
        incScoreWaitTimer = incScoreWaitTime;
    }

    void Update()
    {
        playerCurrentHealth = playerScript.currentHealth;
        playerMaxHealth = playerScript.maxHealth;
        waveNum = waveScript.waveNumber;
        killStreakText.text = killStreak.ToString();
        
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

        if (Input.GetKeyDown(KeyCode.Escape) && !levelingUp && !isPlayerDead)
        {
            PauseResume();

            if (isPaused)
            {
                PauseScreen.SetActive(true);
            } else
            {
                PauseScreen.SetActive(false);
            }    
        }

        if (incScoreWaitTimer < 0f)
        {
            if (incScoreValue > 0)
            {
                GameObject prefab = Instantiate(scoreIncText, socreTextPos, Quaternion.identity);

                prefab.transform.SetParent(canvas.transform, false);

                prefab.GetComponent<TMP_Text>().text = incScoreValue.ToString();

                if (killStreak >= 50)
                {
                    prefab.GetComponent<Animator>().Play("dropOrange");

                } else if (killStreak >= 20)
                {
                    prefab.GetComponent<Animator>().Play("dropYellow");

                } else
                {
                    prefab.GetComponent<Animator>().Play("dropWhite");
                }

                incScoreValue = 0;
            }

            incScoreWaitTimer = incScoreWaitTime;

        } else
        {
            incScoreWaitTimer -= Time.deltaTime;
        }
    }

    public void PauseResume()
    {
        isPaused = !isPaused;
        
        if (isPaused)
        {
            Time.timeScale = 0f;
            GamePaused.Invoke();
            

        } else
        {
            Time.timeScale = 1f;
            GameResumed.Invoke();
            PauseScreen.SetActive(false);
        }
    }

    public void DropHealth(Transform attackPos)
    {
        float isDropingHealth = Random.Range(0f, 1f);
        if (isDropingHealth <= playerScript.healthDropChance)
        {
            Instantiate(healthItem, attackPos.position, Quaternion.identity);
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

    public void EnemyDied(int scoreValue)
    {
        waveScript.killedEnemies++;

        IncScore(scoreValue);
    }

    public void TreeDead(Transform location)
    {
        waveScript.TreeDestroyed(location);
    }

    public void BushDead(Transform location)
    {
        waveScript.BushDestroyed(location);
    }

    public void IncScore(int value)
    {
        killStreak++;

        if (killStreak >= 50)
        {
            score += value * 1.5f;
            incScoreValue += value * 1.5f;

        } else if (killStreak >= 3)
        {
            score += value * 1.2f;
            incScoreValue += value * 1.2f;

        } else
        {
            score += value;
            incScoreValue += value;
        }

        scoreText.text = "Score: " + score.ToString();
    }
}
