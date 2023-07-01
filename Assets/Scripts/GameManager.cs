using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using TMPro;

public class GameManager : MonoBehaviour
{
    public static GameManager gameManager { get; private set; }
    public string difficulty;
    public GameObject bossHealthBar;
    public UnityEvent GamePaused;
    public UnityEvent GameResumed;
    [SerializeField] GameObject PauseScreen;
    bool isPaused;
    public GameObject player;
    private playerController playerScript;
    [SerializeField] UpgradeC upgradesScript;
    [SerializeField] LeaderboardDataManager leaderboardDataManagerScript;
    Renderer playerRenderer;
    public bool isPlayerRendered;
    public bool isPLayerInvicible;
    public float playerMaxHealth;
    public float playerCurrentHealth;
    public bool isPlayerDead;
    public Vector2 playerPos;
    public bool levelingUp;
    [SerializeField] Transform healthItem;
    float score;
    float incScoreValue;
    [SerializeField] TMP_Text scoreText;
    [SerializeField] GameObject scoreIncText;
    [SerializeField] Vector2 socreTextPos;
    public int killStreak;
    [SerializeField] int firstKillStreakThreshold;
    [SerializeField] int secondKillStreakThreshold;
    [SerializeField] float firstKillStreakMultiplier;
    [SerializeField] float secondKillStreakMultiplier;
    [SerializeField] Transform canvas;
    [SerializeField] float incScoreWaitTime;
    float incScoreWaitTimer;
    [SerializeField] TMP_Text killStreakText;
    [SerializeField] GameObject statBackground;
    public List<GameObject> statsText = new List<GameObject>();
    bool showingStats;
    [SerializeField] float statShowOffset;
    bool showingStatsAnim;
    bool showPauseMenu;
    [SerializeField] GameObject deathScreenUI;
    [SerializeField] TMP_Text totalWaveText;
    [SerializeField] List<GameObject> turnOffUI = new List<GameObject>();
    [SerializeField] TMP_Text rankingText;
    [SerializeField] int maxNameLength;
    [SerializeField] TMP_InputField nameInput;
    [SerializeField] TextMeshProUGUI warningText;
    [SerializeField] GameObject restartBtn;
    [SerializeField] GameObject menuBtn;
    [SerializeField] GameObject submitBtn;
    int ranking;



    void Awake()
    {
        if (gameManager != null && gameManager != this)
        {
            Debug.Log("More than one GameManager in a scene!");
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

        SetDifficulty();

        playerMaxHealth = playerScript.maxHealth;
        incScoreWaitTimer = incScoreWaitTime;

        leaderboardDataManagerScript.leaderboardDifficulty = difficulty;
    }

    void Update()
    {
        playerCurrentHealth = playerScript.currentHealth;
        playerMaxHealth = playerScript.maxHealth;
        playerPos = playerScript.transform.position;
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

        if (Input.GetKeyDown(KeyCode.Escape) && !levelingUp && !isPlayerDead && !showingStats && !showingStatsAnim)
        {
            PauseResume();

            if (isPaused)
            {
                PauseScreen.SetActive(true);
                showPauseMenu = true;
            } else
            {
                PauseScreen.SetActive(false);
            }
        }

        if (Input.GetKeyDown(KeyCode.Tab) && !levelingUp && !isPlayerDead && !showingStatsAnim && !showPauseMenu)
        {
            StartCoroutine(ShowStats());
        }

        if (incScoreWaitTimer < 0f)
        {
            if (incScoreValue > 0)
            {
                GameObject prefab = Instantiate(scoreIncText, socreTextPos, Quaternion.identity);

                prefab.transform.SetParent(canvas.transform, false);

                prefab.GetComponent<TMP_Text>().text = incScoreValue.ToString();

                if (killStreak >= secondKillStreakThreshold)
                {
                    prefab.GetComponent<Animator>().Play("dropOrange");

                } else if (killStreak >= firstKillStreakThreshold)
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

    public void SetDifficulty()
    {
        difficulty = StoredVars.difficulty;

        if (difficulty == "normal")
        {
        
        } else if (difficulty == "hardcore")
        {
            
            playerScript.PlayerInHardcore();
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
            showPauseMenu = false;
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

        } else if (enemy.CompareTag("stan"))
        {
            enemy.gameObject.GetComponent<StanC>().DmgStan(dmg, attacker);
        } else if (enemy.CompareTag("ivan"))
        {
            enemy.gameObject.GetComponent<IvanC>().DmgIvan(dmg, attacker);
        } else if (enemy.CompareTag("billy"))
        {
            enemy.gameObject.GetComponent<BillyC>().DmgBilly(dmg, attacker);
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

    public void EnemyDied(int scoreValue)
    {
        killStreak++;

        IncScore(scoreValue);
    }

    public void BossKilled(int scoreValue)
    {
        killStreak++;

        IncScore(scoreValue);
    }

    public void IncScore(int value)
    {
        if (killStreak >= secondKillStreakThreshold)
        {
            score += value * secondKillStreakMultiplier;
            incScoreValue += value * secondKillStreakMultiplier;

        } else if (killStreak >= firstKillStreakThreshold)
        {
            score += value * firstKillStreakMultiplier;
            incScoreValue += value * firstKillStreakMultiplier;

        } else
        {
            score += value;
            incScoreValue += value;
        }

        scoreText.text = "Score: " + score.ToString();
    }

    private IEnumerator ShowStats()
    {
        showingStats = !showingStats;

        if (showingStats)
        {
            PauseResume();
            showingStatsAnim = true;

            statsText[0].SetActive(true);
            statsText[1].SetActive(true);

            for(int i = 2; i < statsText.Count; i++)
            {
                statsText[i].SetActive(true);
                yield return new WaitForSecondsRealtime(statShowOffset);
            }

            showingStatsAnim = false;

        } else
        {
            showingStatsAnim = true;

            for(int i = 2; i < statsText.Count; i++)
            {
                statsText[i].GetComponent<Animator>().Play("hide");
                yield return new WaitForSecondsRealtime(statShowOffset);
            }

            yield return new WaitUntil(() => !IsAnimationPlaying(statsText[statsText.Count - 1].GetComponent<Animator>(), "hide"));

            for(int i = 0; i < statsText.Count; i++)
            {
                statsText[i].SetActive(false);
            }

            showingStatsAnim = false;
            PauseResume();
        }
    }

    public void PlayerDied()
    {
        isPlayerDead = true;

        deathScreenUI.SetActive(true);

        for(int i = 0; i < turnOffUI.Count; i++)
        {
            turnOffUI[i].SetActive(false);
        }

        totalWaveText.text = "You made it to Wave: " + WaveSpawner.waveSpawner.waveNumber;

        StartCoroutine(leaderboardDataManagerScript.FetchData());
        StartCoroutine(FindRanking());
    }
    private IEnumerator FindRanking()
    {
        yield return new WaitUntil(() => leaderboardDataManagerScript.gotData);
        leaderboardDataManagerScript.gotData = false;

        if (leaderboardDataManagerScript.IsTop100(score))
        {
            ranking = leaderboardDataManagerScript.Ranking(score);
            
            rankingText.gameObject.SetActive(true);
            nameInput.gameObject.SetActive(true);
            submitBtn.SetActive(true);
            
            rankingText.text = "You ranked: " + ranking + "!";
        } else
        {
            restartBtn.SetActive(true);
            menuBtn.SetActive(true);
        }
    }

    public void IsOverMaxLength(TMP_InputField nameInput)
    {
        if (nameInput.text.Length > maxNameLength)
        {
            nameInput.text = nameInput.text.Remove(nameInput.text.Length - 1);
            warningText.gameObject.SetActive(true);
            CancelInvoke("CancelNameLengthWarning");
            Invoke("CancelNameLengthWarning", 1.5f);
        }
    }

    private void CancelNameLengthWarning()
    {
        warningText.gameObject.SetActive(false);
    }

    public void SubittedName()
    {
        restartBtn.SetActive(true);
        menuBtn.SetActive(true);
        totalWaveText.gameObject.SetActive(false);
        rankingText.gameObject.SetActive(false);
        nameInput.gameObject.SetActive(false);
        submitBtn.SetActive(false);

        if (nameInput.text == "")
        {
            nameInput.text = "Untitled" + Random.Range(0, 1000);
        }

        StartCoroutine(leaderboardDataManagerScript.SendData(nameInput.text, Mathf.RoundToInt(score), WaveSpawner.waveSpawner.waveNumber, playerScript.totalDamageDealt, difficulty, WaveSpawner.waveSpawner.killedEnemies));

        leaderboardDataManagerScript.gameObject.SetActive(true);
    }

    private bool IsAnimationPlaying(Animator animator, string stateName)
    {
        if (animator.GetCurrentAnimatorStateInfo(0).IsName(stateName) && animator.GetCurrentAnimatorStateInfo(0).normalizedTime < 1.0f)
        {
            return true;
        } else
        {
            return false;
        }
    }
}
