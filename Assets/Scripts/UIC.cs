using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
}
