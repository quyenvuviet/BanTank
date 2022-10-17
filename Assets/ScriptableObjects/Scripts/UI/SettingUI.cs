using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SettingUI : MonoBehaviour
{
    [SerializeField] private Button btn_CloseSetting;
    [SerializeField] private GameObject online_Setting;
    [SerializeField] private Toggle toggleMusic;
    [SerializeField] private Toggle toggleSound;
    private bool SetActice = false;
    public static SettingUI Singleton { get; private set; }
    private void Awake()
    {
        if (Singleton != null)
            return;
        Singleton = this;
        
        gameObject.SetActive(false);
        

    }
    private void Start()
    {
        btn_CloseSetting.onClick.AddListener(HideSetting);
        toggleMusic.onValueChanged.AddListener(OnvaluechangerToggleMusic);
        toggleMusic.onValueChanged.AddListener(OnvaluechangerToggleSound);
    }
    /// <summary>
    /// ẩn Pale seting
    /// </summary>
    public void HideSetting()
    {
        gameObject.SetActive(false);
        online_Setting.SetActive(true);
    }
    /// <summary>
    /// Hiện panle Setting
    /// </summary>
    public void ShowSetting()
    {
        gameObject.SetActive(true);
        online_Setting.SetActive(false);
    }
    /// <summary>
    /// điều chỉnh âm thanh
    /// </summary>
    /// <param name="value"></param>
    private void OnvaluechangerToggleMusic(bool value)
    {
        AudioManager.Instance.MusicEnabled = value;
    }
    /// <summary>
    /// điều chỉnh hiệu ứng
    /// </summary>
    /// <param name="value"></param>
    private void OnvaluechangerToggleSound(bool value)
    {
        AudioManager.Instance.SoundEnabled = value;
    }
}
