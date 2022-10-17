using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HomeScentController : MonoBehaviour
{
    [SerializeField] Audio MusicMenu;
    private void Start()
    {
        OnSceneStart();
    }
    public void OnSceneStart()
    {
        MusicMenu.Play();
    }
}
