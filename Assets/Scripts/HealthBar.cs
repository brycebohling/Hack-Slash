using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class HealthBar : MonoBehaviour
{
    [SerializeField] playerController PC;
    private Image barImage;
    private Image background;
    private float maxHealthAmount;
    [SerializeField] float fadeSpeed;
    [SerializeField] float fadeWaitTime;
    float fadeWaitTimer;

    [SerializeField] TMP_Text healthText;


    private void Awake() 
    {
        barImage = transform.Find("Bar").GetComponent<Image>(); 
        background = transform.Find("Background").GetComponent<Image>();
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
    }

    public void SetHealth(float health)
    {
        if (barImage.fillAmount != health / PC.maxHealth)
        {
            barImage.fillAmount = health / PC.maxHealth;
            fadeWaitTimer = fadeWaitTime;
            healthText.text = Mathf.RoundToInt(health) + "/" + Mathf.RoundToInt(PC.maxHealth);
        }
    }
}
