using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIC : MonoBehaviour 
{
    [SerializeField] GameObject[] tabs;


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
                // selectedTab.GetComponent<Animator>().Play("");
            }
        }
    }
}
