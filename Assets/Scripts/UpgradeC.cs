using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;
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
    [System.Serializable] public struct PowerUps
    {
        public GameObject powerUp;
    }

    public PowerUps[] powerUps;
    [SerializeField] Transform[] startPoints;
    [SerializeField] int cardDropOffest;
    int randomCard1;
    int randomCard2;
    int randomCard3;

    // Player Stat Increase Amounts

    [SerializeField] float movementSpeedIncAmount;
    [SerializeField] float jumpForce;
    // [SerializeField] float numberOfJumps;
    [SerializeField] float maxHealth;
    [SerializeField] float maxStamina;
    [SerializeField] float meleeDmg;
    [SerializeField] float daggerDmg;
    [SerializeField] int daggerAmmo = 3;
    [SerializeField] float daggerSpeed;
    // [SerializeField] float healthDropChance;
    // [SerializeField] float dmgReduction;
    [SerializeField] float critDmg;
    // [SerializeField] float dodgeChance;
    [SerializeField] float rollSpeed;
    // [SerializeField] float meleeSpeed;
    // [SerializeField] float healthRegeneration;


    private void Start() 
    {
        PC = GameObject.FindGameObjectWithTag("Player").GetComponent<playerController>();
    }

    public async void LevelUp()
    {
        GameManager.gameManager.PauseResume();
        
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

        await Task.Delay(cardDropOffest);

        powerUps[randomCard2].powerUp.SetActive(true);
        btn2.interactable = false;
        anim2.Play(CardDrop);

        await Task.Delay(cardDropOffest);

        powerUps[randomCard3].powerUp.SetActive(true);
        btn3.interactable = false;
        anim3.Play(CardDrop);

        await Task.Delay(cardDropOffest);

        btn1.interactable = true;
        btn2.interactable = true;
        btn3.interactable = true;
    }

    void CardSelected()
    {
        GameManager.gameManager.PauseResume();

        powerUps[randomCard1].powerUp.SetActive(false);
        powerUps[randomCard2].powerUp.SetActive(false);
        powerUps[randomCard3].powerUp.SetActive(false);
    }

    public void IncMovementSpeed()
    {
        CardSelected();

        PC.movementSpeed += movementSpeedIncAmount;
    }

    public void IncJumpForce()
    {
        CardSelected();
    }

    public void IncNumberOfJumps()
    {
        CardSelected();
    }
    
    public void IncMaxHealth()
    {
        CardSelected();
    }

    public void IncMaxStamina()
    {
        CardSelected();
    }
    
    public void IncMeleeDmg()
    {
        CardSelected();
    }

    public void IncDaggerDmg()
    {
        CardSelected();
    }
    
    public void IncDaggerAmmo()
    {
        CardSelected();
    }
    
    public void IncDaggerSpeed()
    {
        CardSelected();
    }

    public void IncHealthDropChance()
    {
        CardSelected();
    }

    public void IncDmgReduction()
    {
        CardSelected();
    }

    public void IncCritDmg()
    {
        CardSelected();
    }
    
    public void IncDodgeChance()
    {
        CardSelected();
    }

    public void IncRollSpeed()
    {
        CardSelected();
    }

    public void IncMeleeSpeed()
    {
        CardSelected();
    }

    public void IncHealthRegeneration()
    {
        CardSelected();
    }
}
