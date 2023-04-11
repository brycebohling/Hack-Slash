using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class daggerAmmoUI : MonoBehaviour
{
    Animator anim;
    string animDaggerAmmo3 = "3ammo";
    string animDaggerAmmo2 = "2ammo";
    string animDaggerAmmo1 = "1ammo";
    string animDaggerAmmo0 = "0ammo";

    private void Start() 
    {
        anim = GetComponent<Animator>();
    }

 public void ChangeDaggerAmmoUI(int ammo)
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
