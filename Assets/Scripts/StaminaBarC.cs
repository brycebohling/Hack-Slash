using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StaminaBarC : MonoBehaviour
{
    private Image barImage;
    private Image background;
    private float maxStaminaAmount;
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

    public void SetMaxStamina(float maxStamiina)
    {
        maxStaminaAmount = maxStamiina;
    }

    public void SetStamina(float currentStamina)
    {
        if (barImage.fillAmount != currentStamina / maxStaminaAmount)
        {
            barImage.fillAmount = currentStamina / maxStaminaAmount;
            fadeWaitTimer = fadeWaitTime;
        }
    }
}
