using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIC : MonoBehaviour 
{
    public void Play()
    {
        SceneC.LoadScene(2);
    }

    public void Restart()
    {
        SceneC.Restart();
    }

    public void Menu()
    {
        SceneC.LoadScene(0);
    }

    public void Contols()
    {
        SceneC.LoadScene(1);
    }

    public void Hyperlinks(string link)
    {
        Application.OpenURL(link);
    }
}
