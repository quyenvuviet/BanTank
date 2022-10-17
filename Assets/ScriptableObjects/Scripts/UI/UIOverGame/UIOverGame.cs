﻿using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIOverGame : MonoBehaviour
{
    public static UIOverGame Singleton { get; private set; }
    [SerializeField]
    private Button btn_Home;
    [SerializeField]
    private Button btn_Again;
    [SerializeField]
    private TextMeshProUGUI text_Tile;
   
    private Animator mainMenuAnimator;

    public Action<Team, int> GameOver;

    private void Awake()
    {
        if (Singleton == null)
            Singleton = this;
    }
    void Start()
    {
        btn_Again.onClick.AddListener(OnLobbyLeft);
        btn_Home.onClick.AddListener(StartGame);
        mainMenuAnimator = GetComponent<Animator>();
        registerToEvent(true);
    }

    private void registerToEvent(bool confirm)
    {
        if (confirm)
        {
            LobbyUI.Singleton.OnLobbyLeft += OnLobbyLeft;
            ClientInformation.Singleton.StartGame += StartGame;
            NetworkUIManager.Singleton.OnPlayerGameover += GameOvers;
        }
        else
        {
            LobbyUI.Singleton.OnLobbyLeft -= OnLobbyLeft;
            ClientInformation.Singleton.StartGame -= StartGame;
           NetworkUIManager.Singleton.OnPlayerGameover -= GameOvers;
        }
    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="player"></param>
    /// <param name="count"></param>
    private void GameOvers(Team player, int count)
    {
        // 1 trong 2 team win sẽ hiện bảng Over game , 
        //  Player killed = PlayerManager.Singleton.GetPlayer(player.ID);
        //  Debug.Log("game over ===>count" + count + "id" + killed);
        Debug.Log(Team.Blue+"so lan chet" + count);
        if (count >= 3 )
        {
            gameObject.SetActive(true);

            text_Tile.text = Team.Blue == player ? StringUti.Format(StringUti.Singleton.YouLose) : StringUti.Format(StringUti.Singleton.YouWin);
        }
        GameOver?.Invoke(player, count);


    }

    /// <summary>
    ///
    /// </summary>
    public void StartGame()
    {
        mainMenuAnimator.SetTrigger("ToStartGame");
    }

    private void OnLobbyLeft()
    {
        mainMenuAnimator.SetTrigger("ToOnlineSettingMenu");
    }

    /// <summary>
    /// ẩn Pale Host
    /// </summary>
    public void HideSetting()
    {
        gameObject.SetActive(false);
        
    }

    /// <summary>
    /// Hiện panle Host
    /// </summary>
    public void ShowSetting()
    {
        gameObject.SetActive(true);
       
    }

}
