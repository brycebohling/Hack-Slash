using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class HealthBar : MonoBehaviour
{
    Animator anim;
    [SerializeField] int shakeThreshold;
    [SerializeField] float timeBetweenShakes;
    float timeBetweenShakesTimer;
    bool shouldShake;
    private Image barImage;
    private Image background;
    [SerializeField] float fadeSpeed;
    [SerializeField] float fadeWaitTime;
    float fadeWaitTimer;

    [SerializeField] TMP_Text healthText;


    private void Awake() 
    {
        anim = GetComponent<Animator>();
        barImage = transform.Find("Bar").GetComponent<Image>(); 
        background = transform.Find("Background").GetComponent<Image>();

        timeBetweenShakesTimer = timeBetweenShakes;
    }

    private void Update() 
    {
        if (fadeWaitTimer <= 0f)
        {   
            if (background.fillAmount > barImage.fillAmount)
            {
                background.fillAmount = Mathf.Lerp(background.fillAmount, barImage.fillAmount, fadeSpeed);
            } else
            {
                background.fillAmount = barImage.fillAmount;
            }
            
        } else
        {
            fadeWaitTimer -= Time.deltaTime;
        }

        if (shouldShake && timeBetweenShakesTimer <= 0) 
        {
            anim.Play("Shake");
            timeBetweenShakesTimer = timeBetweenShakes;
        } else
        {
            timeBetweenShakesTimer -= Time.deltaTime;
        }
    }

    public void SetHealth(float health, float maxHealth)
    {
        if (barImage.fillAmount != health / maxHealth)
        {
            barImage.fillAmount = health / maxHealth;
            fadeWaitTimer = fadeWaitTime;
            healthText.text = Mathf.RoundToInt(health) + "/" + Mathf.RoundToInt(maxHealth);

            if (health <= shakeThreshold)
            {
                shouldShake = true;
            } else
            {
                shouldShake = false;
            }
        }
    }
}
