using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UpgradeC : MonoBehaviour
{
    playerController PC;
    Animator anim1;
    Animator anim2;
    Animator anim3;
    Button btn1;
    Button btn2;
    Button btn3;
    string CardDrop = "Drop";
    string SelectedAnim = "Selected";
    [System.Serializable] public struct PowerUps
    {
        public GameObject powerUp;
    }

    public PowerUps[] powerUps;
    [SerializeField] Transform[] startPoints;
    [SerializeField] float cardDropOffest;
    [SerializeField] float cardUpOffset;
    int randomCard1;
    int randomCard2;
    int randomCard3;

    [SerializeField] private List <GameObject> cards = new List<GameObject>();

    // Player Stat Increase Amounts

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


    private void Start() 
    {
        PC = GameObject.FindGameObjectWithTag("Player").GetComponent<playerController>();
    }

    public IEnumerator LevelUp()
    {
        GameManager.gameManager.PauseResume();

        GameManager.gameManager.levelingUp = true;

        randomCard1 = Random.Range(0, powerUps.Length);
        randomCard2 = Random.Range(0, powerUps.Length);
        while (randomCard2 == randomCard1)
        {
            randomCard2 = Random.Range(0, powerUps.Length);
        }
        randomCard3 = Random.Range(0, powerUps.Length);
        while (randomCard3 == randomCard1 || randomCard3 == randomCard2)
        {
            randomCard3 = Random.Range(0, powerUps.Length);
        }

        anim1 = powerUps[randomCard1].powerUp.GetComponent<Animator>();
        anim2 = powerUps[randomCard2].powerUp.GetComponent<Animator>();
        anim3 = powerUps[randomCard3].powerUp.GetComponent<Animator>();

        btn1 = powerUps[randomCard1].powerUp.GetComponent<Button>();
        btn2 = powerUps[randomCard2].powerUp.GetComponent<Button>();
        btn3 = powerUps[randomCard3].powerUp.GetComponent<Button>();

        powerUps[randomCard1].powerUp.transform.position = new Vector2(startPoints[0].position.x, startPoints[0].position.y);
        powerUps[randomCard2].powerUp.transform.position = new Vector2(startPoints[1].position.x, startPoints[1].position.y);
        powerUps[randomCard3].powerUp.transform.position = new Vector2(startPoints[2].position.x, startPoints[2].position.y);

        powerUps[randomCard1].powerUp.SetActive(true);
        btn1.interactable = false;
        anim1.Play(CardDrop);

        yield return new WaitForSecondsRealtime(cardDropOffest);
        
        powerUps[randomCard2].powerUp.SetActive(true);
        btn2.interactable = false;
        anim2.Play(CardDrop);

        yield return new WaitForSecondsRealtime(cardDropOffest);

        powerUps[randomCard3].powerUp.SetActive(true);
        btn3.interactable = false;
        anim3.Play(CardDrop);

        yield return new WaitForSecondsRealtime(cardDropOffest);

        btn1.interactable = true;
        btn2.interactable = true;
        btn3.interactable = true;
    }

    private IEnumerator CardSelected()
    {
        btn1.interactable = false;
        btn2.interactable = false;
        btn3.interactable = false;

        anim1.Play(SelectedAnim);
        yield return new WaitForSecondsRealtime(cardUpOffset);
        anim2.Play(SelectedAnim);
        yield return new WaitForSecondsRealtime(cardUpOffset);
        anim3.Play(SelectedAnim);
        yield return new WaitForSecondsRealtime(cardUpOffset);

        yield return new WaitUntil(() => !IsAnimationPlaying(anim3, SelectedAnim));

        powerUps[randomCard1].powerUp.SetActive(false);
        powerUps[randomCard2].powerUp.SetActive(false);
        powerUps[randomCard3].powerUp.SetActive(false);
        
        GameManager.gameManager.levelingUp = false;

        GameManager.gameManager.PauseResume();
    }

    public void IncMovementSpeed()
    {
        StartCoroutine(CardSelected());

        PC.movementSpeed += movementSpeedIncAmount;

        if (PC.movementSpeed >= maxMovementSpeed)
        {
            // cards.Remove();
        }
    }

    public void IncJumpForce()
    {
        StartCoroutine(CardSelected());

        PC.jumpForce += jumpForceIncAmount;
    }

    public void IncNumberOfJumps()
    {
        StartCoroutine(CardSelected());

        PC.numberOfJumps += numberOfJumpsIncAmount;
    }
    
    public void IncMaxHealth()
    {
        StartCoroutine(CardSelected());

        PC.maxHealth += maxHealthIncAmount;
    }

    public void IncMaxStamina()
    {
        StartCoroutine(CardSelected());

        PC.maxStamina += maxStaminaIncAmount;
    }
    
    public void IncMeleeDmg()
    {
        StartCoroutine(CardSelected());

        PC.meleeDmg += meleeDmgIncAmount;
    }

    public void IncDaggerDmg()
    {
        StartCoroutine(CardSelected());

        PC.daggerDmg += daggerDmgIncAmount;
    }
    
    public void IncDaggerSpeed()
    {
        StartCoroutine(CardSelected());

        PC.daggerSpeed += daggerSpeedIncAmount;
    }

    public void IncHealthDropChance()
    {
        StartCoroutine(CardSelected());

        PC.healthDropChance += healthDropChanceIncAmount;
    }

    public void IncDmgReduction()
    {
        StartCoroutine(CardSelected());

        PC.dmgReduction += dmgReductionIncAmount;
    }

    public void IncCritDmg()
    {
        StartCoroutine(CardSelected());

        PC.critDmg += critDmgIncAmount;
    }
    
    public void IncDodgeChance()
    {
        StartCoroutine(CardSelected());

        PC.dodgeChance += dodgeChanceIncAmount;
    }

    public void IncRollSpeed()
    {
        StartCoroutine(CardSelected());

        PC.rollSpeed += rollSpeedIncAmount;
    }

    public void IncMeleeSpeed()
    {
        StartCoroutine(CardSelected());

        PC.meleeSpeed += meleeSpeedIncAmount;
    }

    public void IncHealthRegeneration()
    {
        StartCoroutine(CardSelected());

        PC.healthRegenerationPercent += h_RegenIncAmount;
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
