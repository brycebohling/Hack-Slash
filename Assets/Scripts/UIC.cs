using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIC : MonoBehaviour
{
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
        SceneC.Menu();
    }

    public void Quit()
    {
        Application.Quit();
    }
}
