using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Main Menu UI
/// </summary>
public class MainMenuUI : MonoBehaviour
{
    public static MainMenuUI Singleton { get; private set; }
    [SerializeField] private TMP_InputField nameInputField;
    [SerializeField] private TMP_InputField ipv4AddressInputField;

    // [SerializeField] private TMP_InputField nameRoomInputField;
    [SerializeField] private ushort port;

    [SerializeField] private SettingUI settingUI;
    [SerializeField] private PaneHost panelHost;
    [SerializeField] private Button ConnectLobby;

    /// <summary>
    /// Serever
    /// </summary>
    public Server server;

    /// <summary>
    /// clinet
    /// </summary>
    public Client client;

    private Animator mainMenuAnimator;

    public Action<string> OnHostOrJoinRoom;

    private void Awake()
    {
        if (Singleton != null)
            return;

        Singleton = this;
    }

    private void Start()
    {
        mainMenuAnimator = GetComponent<Animator>();
        ConnectLobby.onClick.AddListener(ShowPanle);

        registerToEvent(true);
    }

    private void registerToEvent(bool confirm)
    {
        if (confirm)
        {
            LobbyUI.Singleton.OnLobbyLeft += OnLobbyLeft;
            ClientInformation.Singleton.StartGame += StartGame;
        }
        else
        {
            LobbyUI.Singleton.OnLobbyLeft -= OnLobbyLeft;
            ClientInformation.Singleton.StartGame -= StartGame;
        }
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

    public void OnOnlineBtn()
    {
        mainMenuAnimator.SetTrigger("ToHostJoinMenu");
    }

    public void OnSettingBtn()
    {
        settingUI.ShowSetting();
    }

    /*
    Switch in to Lobby Menu
         Take the name from ipf + assign it to the player
    lấy ID mặc đình 127.0.0.1 gán cho người chơi
    */

    public void OnHostBtn()
    {
        panelHost.conectLobby = () =>
        {
            server.Init(port, 10); //This need to change (Stop hard-coded)
            client.Init("127.0.0.1", port, GetPlayerName);

            OnHostOrJoinRoom?.Invoke(GetPlayerName);
            mainMenuAnimator.SetTrigger("ToLobbyMenu");
        };
    }

    private void ShowPanle()
    {
        panelHost.ShowSetting();
    }

    public void OnJoinBtn()
    {
        mainMenuAnimator.SetTrigger("ToConnectMenu");
    }

    public void OnConnectBtn()
    {
        string ipInput = ipv4AddressInputField.text != "" ? ipv4AddressInputField.text : "127.0.0.1";
        client.Init(ipInput, port, GetPlayerNamejond);
        OnHostOrJoinRoom?.Invoke(GetPlayerNamejond);

        mainMenuAnimator.SetTrigger("ToLobbyMenu");
    }

    private string GetPlayerName => panelHost.InputName != "" ? panelHost.InputName : "I forgot to name myself";
    private string GetPlayerNamejond => panelHost.InputName != "" ? panelHost.InputName : nameInputField.text;
}