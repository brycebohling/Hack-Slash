using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager gameManager { get; private set; }

    public unitHealth _playerHealth = new unitHealth(100, 100);
    public unitHealth _RedBoyHealth = new unitHealth(100, 100);

    void Awake()
    {
        if (gameManager != null && gameManager != this)
        {
            Destroy(this);
        } else
        {
            gameManager = this;
        }
    }
}
