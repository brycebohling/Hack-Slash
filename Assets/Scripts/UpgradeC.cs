using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UpgradeC : MonoBehaviour
{
    playerController PC;
    [SerializeField] HealthBar HB;
    [SerializeField] StaminaBarC SB;
    Animator anim1;
    Animator anim2;
    Animator anim3;
    Button btn1;
    Button btn2;
    Button btn3;
    string CardDrop = "Drop";
    string SelectedAnim = "Selected";
    public List <GameObject> powerUps = new List<GameObject>();
    [SerializeField] List <GameObject> bannedHardcorePowerUps = new List<GameObject>();
    [SerializeField] Transform[] startPoints;
    [SerializeField] float cardDropOffest;
    [SerializeField] float cardUpOffset;
    int randomCard1;
    int randomCard2;
    int randomCard3;
    bool isCardSelected;

    [Header("Player Stat Increase Amounts")]
    [SerializeField] float movementSpeedIncAmount;
    [SerializeField] float jumpForceIncAmount;
    [SerializeField] int numberOfJumpsIncAmount;
    [SerializeField] float maxHealthIncAmount;
    [SerializeField] float maxStaminaIncAmount;
    [SerializeField] float meleeDmgIncAmount;
    [SerializeField] float daggerDmgIncAmount;  
    [SerializeField] float daggerSpeedIncAmount;
    [SerializeField] float healthDropChanceIncAmount;
    [SerializeField] float dmgReductionIncAmount;
    [SerializeField] float critDmgIncAmount;
    [SerializeField] float dodgeChanceIncAmount;
    [SerializeField] float rollSpeedIncAmount;
    [SerializeField] float meleeSpeedIncAmount;
    [SerializeField] float h_RegenIncAmount;

    [SerializeField] float maxMovementSpeed;
    [SerializeField] float maxJumpForce;
    [SerializeField] int maxNumberOfJumps;
    [SerializeField] float maxMaxHealth;
    [SerializeField] float maxMaxStamina;
    [SerializeField] float maxMeleeDmg;
    [SerializeField] float maxDaggerDmg;  
    [SerializeField] float maxDaggerSpeed;
    [SerializeField] float maxHealthDropChance;
    [SerializeField] float maxDmgReduction;
    [SerializeField] float maxCritDmg;
    [SerializeField] float maxDodgeChance;
    [SerializeField] float maxRollSpeed;
    [SerializeField] float maxMeleeSpeed;
    [SerializeField] float maxH_Regen;

    [Header("SFX")]
    AudioSource audioSource;
    [SerializeField] AudioClip cardsAppearSFX;
    [SerializeField] AudioClip cardHoverSFX;
    [SerializeField] AudioClip cardSelectedSFX;


    private void Start() 
    {
        PC = GameObject.FindGameObjectWithTag("Player").GetComponent<playerController>();
        audioSource = GetComponent<AudioSource>();
    }

    public IEnumerator LevelUp()
    {
        GameManager.gameManager.PauseResume();

        GameManager.gameManager.levelingUp = true;

        if (GameManager.gameManager.difficulty == "hardcore")
        {
            foreach (GameObject bannedPowerUp in bannedHardcorePowerUps.ToArray())
            {
                foreach (GameObject powerUp in powerUps.ToArray())
                {
                    if (bannedPowerUp == powerUp)
                    {
                        powerUps.Remove(powerUp);
                    }
                }
            }
        }

        randomCard1 = Random.Range(0, powerUps.Count);
        randomCard2 = Random.Range(0, powerUps.Count);
        while (randomCard2 == randomCard1)
        {
            randomCard2 = Random.Range(0, powerUps.Count);
        }

        randomCard3 = Random.Range(0, powerUps.Count);
        while (randomCard3 == randomCard1 || randomCard3 == randomCard2)
        {
            randomCard3 = Random.Range(0, powerUps.Count);
        }

        anim1 = powerUps[randomCard1].GetComponent<Animator>();
        anim2 = powerUps[randomCard2].GetComponent<Animator>();
        anim3 = powerUps[randomCard3].GetComponent<Animator>();

        btn1 = powerUps[randomCard1].GetComponent<Button>();
        btn2 = powerUps[randomCard2].GetComponent<Button>();
        btn3 = powerUps[randomCard3].GetComponent<Button>();

        powerUps[randomCard1].transform.position = new Vector2(startPoints[0].position.x, startPoints[0].position.y);
        powerUps[randomCard2].transform.position = new Vector2(startPoints[1].position.x, startPoints[1].position.y);
        powerUps[randomCard3].transform.position = new Vector2(startPoints[2].position.x, startPoints[2].position.y);

        powerUps[randomCard1].SetActive(true);
        btn1.interactable = false;
        anim1.Play(CardDrop);

        audioSource.PlayOneShot(cardsAppearSFX, 1f);

        yield return new WaitForSecondsRealtime(cardDropOffest);
        
        powerUps[randomCard2].SetActive(true);
        btn2.interactable = false;
        anim2.Play(CardDrop);

        audioSource.PlayOneShot(cardsAppearSFX, 1f);

        yield return new WaitForSecondsRealtime(cardDropOffest);

        powerUps[randomCard3].SetActive(true);
        btn3.interactable = false;
        anim3.Play(CardDrop);
        audioSource.PlayOneShot(cardsAppearSFX, 1f);

        yield return new WaitForSecondsRealtime(cardDropOffest);

        btn1.interactable = true;
        btn2.interactable = true;
        btn3.interactable = true;
    }

    private IEnumerator CardSelected(GameObject caller, bool shouldRemove)
    {
        isCardSelected = true;

        audioSource.PlayOneShot(cardSelectedSFX, 1f);

        btn1.interactable = false;
        btn2.interactable = false;
        btn3.interactable = false;
    
        if (caller == powerUps[randomCard1])
        {
            anim1.Play(SelectedAnim);
            yield return new WaitForSecondsRealtime(cardUpOffset);

            anim2.Play(SelectedAnim);
            yield return new WaitForSecondsRealtime(cardUpOffset);

            anim3.Play(SelectedAnim);
            yield return new WaitForSecondsRealtime(cardUpOffset);

            yield return new WaitUntil(() => !IsAnimationPlaying(anim3, SelectedAnim));

        } else if (caller == powerUps[randomCard2])
        {
            anim2.Play(SelectedAnim);
            yield return new WaitForSecondsRealtime(cardUpOffset);
            anim1.Play(SelectedAnim);
            yield return new WaitForSecondsRealtime(cardUpOffset);
            anim3.Play(SelectedAnim);
            yield return new WaitForSecondsRealtime(cardUpOffset);

            yield return new WaitUntil(() => !IsAnimationPlaying(anim3, SelectedAnim));

        } else
        {
            anim3.Play(SelectedAnim);
            yield return new WaitForSecondsRealtime(cardUpOffset);
            anim2.Play(SelectedAnim);
            yield return new WaitForSecondsRealtime(cardUpOffset);
            anim1.Play(SelectedAnim);
            yield return new WaitForSecondsRealtime(cardUpOffset);

            yield return new WaitUntil(() => !IsAnimationPlaying(anim1, SelectedAnim));
        }

        powerUps[randomCard1].SetActive(false);
        powerUps[randomCard2].SetActive(false);
        powerUps[randomCard3].SetActive(false);

        if (shouldRemove)
        {
            powerUps.Remove(caller);
        }
        
        isCardSelected = false;

        GameManager.gameManager.levelingUp = false;
        GameManager.gameManager.PauseResume();
    }

    public void canPlayHoverSFX()
    {
        if (!isCardSelected)
        {
            audioSource.PlayOneShot(cardHoverSFX, 0.15f);
        }
    }

    public void IncMovementSpeed(GameObject callerObject)
    {
        if (!isCardSelected)
        {
            PC.movementSpeed += movementSpeedIncAmount;

            if (PC.movementSpeed >= maxMovementSpeed)
            {
                StartCoroutine(CardSelected(callerObject, true));
            } else
            {
                StartCoroutine(CardSelected(callerObject, false));
            }

            for(int i = 0; i < GameManager.gameManager.statsText.Count; i++)
            {
                if (GameManager.gameManager.statsText[i].name == "MovementSpeed") 
                {
                    GameManager.gameManager.statsText[i].GetComponent<TMP_Text>().text = "Movement Speed: " + PC.movementSpeed;
                    break;
                }
            }
        }
    }

    public void IncJumpForce(GameObject callerObject)
    {
        if (!isCardSelected)
        {
            PC.jumpForce += jumpForceIncAmount;

            if (PC.jumpForce >= maxJumpForce)
            {
                StartCoroutine(CardSelected(callerObject, true));
            } else
            {
                StartCoroutine(CardSelected(callerObject, false));
            }

            for(int i = 0; i < GameManager.gameManager.statsText.Count; i++)
            {
                if (GameManager.gameManager.statsText[i].name == "JumpForce") 
                {
                    GameManager.gameManager.statsText[i].GetComponent<TMP_Text>().text = "Jump Force: " + PC.jumpForce;
                    break;
                }
            }
        }
        
    }

    public void IncNumberOfJumps(GameObject callerObject)
    {
        if (!isCardSelected)
        {
            PC.numberOfJumps += numberOfJumpsIncAmount;

            if (PC.numberOfJumps >= maxNumberOfJumps)
            {
                StartCoroutine(CardSelected(callerObject, true));
            } else
            {
                StartCoroutine(CardSelected(callerObject, false));
            }

            for(int i = 0; i < GameManager.gameManager.statsText.Count; i++)
            {
                if (GameManager.gameManager.statsText[i].name == "#OfJumps") 
                {
                    GameManager.gameManager.statsText[i].GetComponent<TMP_Text>().text = "Number of Jumps: " + PC.numberOfJumps;
                    break;
                }
            }
        }
        
    }
    
    public void IncMaxHealth(GameObject callerObject)
    {
        if (!isCardSelected)
        {
            PC.maxHealth += maxHealthIncAmount;

            HB.SetHealth(PC.currentHealth, PC.maxHealth);

            PC.PlayerHeal(maxHealthIncAmount);

            if (PC.maxHealth >= maxMaxHealth)
            {
                StartCoroutine(CardSelected(callerObject, true));
            } else
            {
                StartCoroutine(CardSelected(callerObject, false));
            }

            for(int i = 0; i < GameManager.gameManager.statsText.Count; i++)
            {
                if (GameManager.gameManager.statsText[i].name == "MaxHealth") 
                {
                    GameManager.gameManager.statsText[i].GetComponent<TMP_Text>().text = "Max Health: " + PC.maxHealth;
                    break;
                }
            }
        }
    }

    public void IncMaxStamina(GameObject callerObject)
    {
        if (!isCardSelected)
        {
            PC.maxStamina += maxStaminaIncAmount;

            SB.SetStamina(PC.currentStamina);

            if (PC.maxStamina >= maxMaxStamina)
            {
                StartCoroutine(CardSelected(callerObject, true));
            } else
            {
                StartCoroutine(CardSelected(callerObject, false));
            }

            for(int i = 0; i < GameManager.gameManager.statsText.Count; i++)
            {
                if (GameManager.gameManager.statsText[i].name == "MaxStamina") 
                {
                    GameManager.gameManager.statsText[i].GetComponent<TMP_Text>().text = "Max Stamina: " + PC.maxStamina;
                    break;
                }
            }
        }
    }
    
    public void IncMeleeDmg(GameObject callerObject)
    {
        if (!isCardSelected)
        {
            PC.meleeDmg += meleeDmgIncAmount;

            if (PC.meleeDmg >= maxMeleeDmg)
            {
                StartCoroutine(CardSelected(callerObject, true));
            } else
            {
                StartCoroutine(CardSelected(callerObject, false));
            }

            for(int i = 0; i < GameManager.gameManager.statsText.Count; i++)
            {
                if (GameManager.gameManager.statsText[i].name == "MeleeDamage") 
                {
                    GameManager.gameManager.statsText[i].GetComponent<TMP_Text>().text = "Melee Damage: " + PC.meleeDmg;
                    break;
                }
            }
        }
        
    }

    public void IncDaggerDmg(GameObject callerObject)
    {
        if (!isCardSelected)
        {
            PC.daggerDmg += daggerDmgIncAmount;

            if (PC.daggerDmg >= maxDaggerDmg)
            {
                StartCoroutine(CardSelected(callerObject, true));
            } else
            {
                StartCoroutine(CardSelected(callerObject, false));
            }

            for(int i = 0; i < GameManager.gameManager.statsText.Count; i++)
            {
                if (GameManager.gameManager.statsText[i].name == "DaggerDamage") 
                {
                    GameManager.gameManager.statsText[i].GetComponent<TMP_Text>().text = "Dagger Damage: " + PC.daggerDmg;
                    break;
                }
            }
        }
    }
    
    public void IncDaggerSpeed(GameObject callerObject)
    {
        if (!isCardSelected)
        {
            PC.daggerSpeed += daggerSpeedIncAmount;

            if (PC.daggerSpeed >= maxDaggerSpeed)
            {
                StartCoroutine(CardSelected(callerObject, true));
            } else
            {
                StartCoroutine(CardSelected(callerObject, false));
            }

            for(int i = 0; i < GameManager.gameManager.statsText.Count; i++)
            {
                if (GameManager.gameManager.statsText[i].name == "DaggerSpeed") 
                {
                    GameManager.gameManager.statsText[i].GetComponent<TMP_Text>().text = "Dagger Speed: " + PC.daggerSpeed;
                    break;
                }
            }
        }
        
    }

    public void IncHealthDropChance(GameObject callerObject)
    {
        if (!isCardSelected)
        {
            PC.healthDropChance += healthDropChanceIncAmount;

            if (PC.healthDropChance >= maxHealthDropChance)
            {
                StartCoroutine(CardSelected(callerObject, true));
            } else
            {
                StartCoroutine(CardSelected(callerObject, false));
            }

            for(int i = 0; i < GameManager.gameManager.statsText.Count; i++)
            {
                if (GameManager.gameManager.statsText[i].name == "HealthDropChance") 
                {
                    GameManager.gameManager.statsText[i].GetComponent<TMP_Text>().text = "Health Drop Chance: " + PC.healthDropChance * 100 + "%";
                    break;
                }
            }
        }
    }

    public void IncDmgReduction(GameObject callerObject)
    {
        if (!isCardSelected)
        {
            PC.dmgReduction += dmgReductionIncAmount;

            if (PC.dmgReduction >= maxDmgReduction)
            {
                StartCoroutine(CardSelected(callerObject, true));
            } else
            {
                StartCoroutine(CardSelected(callerObject, false));
            }

            for(int i = 0; i < GameManager.gameManager.statsText.Count; i++)
            {
                if (GameManager.gameManager.statsText[i].name == "DamageReduction") 
                {
                    GameManager.gameManager.statsText[i].GetComponent<TMP_Text>().text = "Damage Reduction: " + PC.dmgReduction * 100 + "%";
                    break;
                }
            }
        }
    }

    public void IncCritDmg(GameObject callerObject)
    {
        if (!isCardSelected)
        {
            PC.critDmg += critDmgIncAmount;

            if (PC.critDmg >= maxCritDmg)
            {
                StartCoroutine(CardSelected(callerObject, true));
            } else
            {
                StartCoroutine(CardSelected(callerObject, false));
            }

            for(int i = 0; i < GameManager.gameManager.statsText.Count; i++)
            {
                if (GameManager.gameManager.statsText[i].name == "CritDamage") 
                {
                    GameManager.gameManager.statsText[i].GetComponent<TMP_Text>().text = "Crit Damage: " + PC.critDmg * 100 + "%";
                    break;
                }
            }
        }
    }
    
    public void IncDodgeChance(GameObject callerObject)
    {
        if (!isCardSelected)
        {
            PC.dodgeChance += dodgeChanceIncAmount;

            if (PC.dodgeChance >= maxDodgeChance)
            {
                StartCoroutine(CardSelected(callerObject, true));
            } else
            {
                StartCoroutine(CardSelected(callerObject, false));
            }

            for(int i = 0; i < GameManager.gameManager.statsText.Count; i++)
            {
                if (GameManager.gameManager.statsText[i].name == "DodgeChance") 
                {
                    GameManager.gameManager.statsText[i].GetComponent<TMP_Text>().text = "Dodge Chance: " + PC.dodgeChance * 100 + "%";
                    break;
                }
            }
        }   
    }

    public void IncRollSpeed(GameObject callerObject)
    {
        if (!isCardSelected)
        {
            PC.rollSpeed += rollSpeedIncAmount;

            if (PC.rollSpeed >= maxRollSpeed)
            {
                StartCoroutine(CardSelected(callerObject, true));
            } else
            {
                StartCoroutine(CardSelected(callerObject, false));
            }

            for(int i = 0; i < GameManager.gameManager.statsText.Count; i++)
            {
                if (GameManager.gameManager.statsText[i].name == "RollSpeed") 
                {
                    GameManager.gameManager.statsText[i].GetComponent<TMP_Text>().text = "Roll Speed: " + PC.rollSpeed;
                    break;
                }
            }
        }
    }

    public void IncMeleeSpeed(GameObject callerObject)
    {
        if (!isCardSelected)
        {
            PC.meleeSpeed += meleeSpeedIncAmount;

            if (PC.meleeSpeed >= maxMeleeSpeed)
            {
                StartCoroutine(CardSelected(callerObject, true));
            } else
            {
                StartCoroutine(CardSelected(callerObject, false));
            }

            for(int i = 0; i < GameManager.gameManager.statsText.Count; i++)
            {
                if (GameManager.gameManager.statsText[i].name == "MeleeSpeed") 
                {
                    GameManager.gameManager.statsText[i].GetComponent<TMP_Text>().text = "MeleeSpeed: " + PC.meleeSpeed * 100 + "%";
                    break;
                }
            }
        }
    }

    public void IncHealthRegeneration(GameObject callerObject)
    {
        if (!isCardSelected)
        {
            PC.healthRegenerationPercent += h_RegenIncAmount;

            if (PC.healthRegenerationPercent >= maxH_Regen)
            {
                StartCoroutine(CardSelected(callerObject, true));
            } else
            {
                StartCoroutine(CardSelected(callerObject, false));
            }

            for(int i = 0; i < GameManager.gameManager.statsText.Count; i++)
            {
                if (GameManager.gameManager.statsText[i].name == "HealthRegeneration") 
                {
                    GameManager.gameManager.statsText[i].GetComponent<TMP_Text>().text = "Health Regeneration: " + PC.healthRegenerationPercent * 100 + "%";
                    break;
                }
            }
        }
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
