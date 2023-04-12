using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class daggerAmmoUI : MonoBehaviour
{
    [SerializeField] float yOffset;
    Animator anim;
    string animDaggerAmmo3 = "3ammo";
    string animDaggerAmmo2 = "2ammo";
    string animDaggerAmmo1 = "1ammo";
    string animDaggerAmmo0 = "0ammo";

    private void Start() 
    {
        anim = GetComponent<Animator>();
    }

    private void Update()
    {
        transform.position = new Vector2(GameManager.gameManager.player.transform.position.x, GameManager.gameManager.player.transform.position.y + yOffset);
    }

 public void ChangeDaggerAmmoUI(int ammo)
    {
        if (gameObject.activeSelf)
        {
            if (ammo == 3)
            {
                anim.Play(animDaggerAmmo3);
            } else if (ammo == 2)
            {
                anim.Play(animDaggerAmmo2);
            } else if (ammo == 1)
            {
                anim.Play(animDaggerAmmo1);
            } else if (ammo == 0)
            {
                anim.Play(animDaggerAmmo0);
            }       
        }
    }
}
