using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class StaminaBarC : MonoBehaviour
{
    [SerializeField] playerController PC;
    private Image barImage;
    private Image background;
    private float maxStaminaAmount;
    [SerializeField] float fadeSpeed;
    [SerializeField] float fadeWaitTime;
    float fadeWaitTimer;
    [SerializeField] TMP_Text staminaText;


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

    public void SetStamina(float stamina)
    {
        if (barImage.fillAmount != stamina / PC.maxStamina)
        {
            barImage.fillAmount = stamina / PC.maxStamina;
            fadeWaitTimer = fadeWaitTime;

            staminaText.text = Mathf.RoundToInt(stamina) + "/" + Mathf.RoundToInt(PC.maxStamina);
        }
    }
}
