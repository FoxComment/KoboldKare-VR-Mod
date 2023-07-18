using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenuSongSpeedup : MonoBehaviour
{
    private void Awake()
    {
        GetComponent<AudioSource>().time = 34;
    }
}
