using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StoredVars : MonoBehaviour
{
    public static string difficulty;

    public void Difficulty(string diff)
    {
        difficulty = diff;
    }
}
