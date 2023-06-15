using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class BossHealthBar : MonoBehaviour
{
    [SerializeField] IvanC ivanCScript;
    private Image barImage;
    private Image background;
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

    public void SetHealth(float health, float maxHealth)
    {
        if (barImage.fillAmount != health / maxHealth)
        {
            barImage.fillAmount = health / maxHealth;
            fadeWaitTimer = fadeWaitTime;
        }
    }
}
