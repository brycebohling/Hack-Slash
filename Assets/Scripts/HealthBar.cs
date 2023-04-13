using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    private Image barImage;
    private Image background;
    private int maxHealthAmount;
    [SerializeField] float fadeSpeed;
    [SerializeField] float fadeWaitTime;
    float fadeWaitTimer;


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

    public void SetMaxHealth(int maxHealth)
    {
        maxHealthAmount = maxHealth;
    }

    public void SetHealth(float health)
    {
        if (barImage.fillAmount != health / maxHealthAmount)
        {
            barImage.fillAmount = health / maxHealthAmount;
            fadeWaitTimer = fadeWaitTime;
        }
    }
}
