using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIC : MonoBehaviour 
{
    [SerializeField] GameObject tabsParent;
    [SerializeField] AudioClip tabSelectedSFX;
    [SerializeField] AudioClip tabHoverSFX;
    [SerializeField] GameObject[] tabs;
    [SerializeField] Animator[] tabBtnsAnims;
    List<bool> tabBtnsSelection = new List<bool>();

    private void Start() 
    {
        for (int i = 0; i < tabBtnsAnims.Length; i++)
        {
            tabBtnsSelection.Add(false);
        }    
    }

    public void Play()
    {
        SceneC.LoadScene(1);
    }

    public void Restart()
    {
        SceneC.Restart();
    }

    public void Menu()
    {
        SceneC.LoadScene(0);
    }

    public void Hyperlinks(string link)
    {
        Application.OpenURL(link);
    }

    public void TabSelected(GameObject selectedTab)
    {
        for (int i = 0; i < tabs.Length; i++)
        {
            if (selectedTab != tabs[i])
            {
                tabs[i].SetActive(false);
            } else
            {
                tabs[i].SetActive(true);
            }
        }
    }

    public void TabBtnEnter(Animator anim)
    {
        foreach(Animator tabAnim in tabBtnsAnims)
        {
            if (tabAnim == anim)
            {
                anim.Play("hover");
                break;
            }
        }

        tabsParent.GetComponent<AudioSource>().PlayOneShot(tabHoverSFX, 0.15f);
    }
    
    public void TabBtnExit(Animator anim)
    {
        int index = 0;

        foreach(Animator tabAnim in tabBtnsAnims)
        {
            if (tabAnim == anim)
            {
                if (tabBtnsSelection[index] == true)
                {
                    anim.Play("selected");
                } else
                {
                    anim.Play("normal");
                }
                break;
            }

            index++;
        }
    }  

    public void TabBtnSelected(Animator anim)
    {
        int index = 0;
        foreach(Animator tabAnim in tabBtnsAnims)
        {
            if (tabAnim == anim)
            {
                tabBtnsSelection[index] = true;
            } else
            {
                tabBtnsSelection[index] = false;
                tabAnim.Play("normal");
            }

            index++;
        }

        anim.Play("selected");
        
        tabsParent.GetComponent<AudioSource>().PlayOneShot(tabSelectedSFX);
    }
}
