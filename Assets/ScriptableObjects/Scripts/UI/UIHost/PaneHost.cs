using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PaneHost : MonoBehaviour
{
    [SerializeField] private TMP_InputField inputName;
    [SerializeField] private TMP_InputField inputRoom;
    [SerializeField] private Button btnConectLobby;
    [SerializeField] private Button btnCancel;
    [SerializeField] private GameObject PannleHost;
    public Action conectLobby;

    public string InputName
    {
        get
        {
            return inputName.text;
        }
        set
        {
             inputName.text= value ;
        }
    }
    public string InputRoom
    {
        get
        {
            return inputRoom.text;
        }
        set
        {
             inputRoom.text= value ;
        }
    }

    private void Awake()
    {
        gameObject.SetActive(false);
    }

    private void Start()
    {
        btnConectLobby.onClick.AddListener(OnClickButtonConectLobby);
        btnCancel.onClick.AddListener(HideSetting);
    }

    private void OnClickButtonConectLobby()
    {
        this.conectLobby?.Invoke();
        MainMenuUI.Singleton.OnHostBtn();
        HideSetting();
    }

    /// <summary>
    /// ẩn Pale Host
    /// </summary>
    public void HideSetting()
    {
        gameObject.SetActive(false);
        PannleHost.SetActive(true);
    }

    /// <summary>
    /// Hiện panle Host
    /// </summary>
    public void ShowSetting()
    {
        gameObject.SetActive(true);
        PannleHost.SetActive(false);
    }
}