using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

public class UIConnectRoom : MonoBehaviour
{
    /// <summary>
    /// button phong
    /// </summary>
    [SerializeField] Button buttonRoom;
    /// <summary>
    /// button text phong
    /// </summary>
    [SerializeField] TextMeshProUGUI textRoom;

    [SerializeField] private TMP_InputField nameInputField;
    /// <summary>
    /// Serever
    /// </summary>
    public Server server;

    [SerializeField] private ushort port;
    /// <summary>
    /// clinet
    /// </summary>
    public Client client;
    public Action<string> OnHostOrJoinRoom;

    private Animator mainMenuAnimator;
    private void Start()
    {
        buttonRoom.onClick.AddListener(OnclickButtonRoom);

        mainMenuAnimator = GetComponent<Animator>();
    }
    private void OnclickButtonRoom()
    {
        string ipInput;
        //client.Init(ipInput, port, GetPlayerName);
        OnHostOrJoinRoom?.Invoke(GetPlayerName);

        mainMenuAnimator.SetTrigger("ToLobbyMenu");
    }
    private string GetPlayerName => nameInputField.text != "" ? nameInputField.text : "I forgot to name myself";

}
