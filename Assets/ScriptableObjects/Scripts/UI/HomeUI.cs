using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class HomeUI : MonoBehaviour
{
    [SerializeField] private Button btn_PlayGame;
    [SerializeField] private Button btn_SettingGame;
    [SerializeField] private Button btn_Quit;
    private MainMenuUI mainMenu;
    private void Awake()
    {
       // mainMenu.GetComponent<MainMenuUI>();
    }
    private void Start()
    {
        btn_PlayGame.onClick.AddListener(PlayGame);
        btn_Quit.onClick.AddListener(QuitGame);
        btn_SettingGame.onClick.AddListener(QuitGame);
        
    }
    public void PlayGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex+1);
       // mainMenu.StartGame();
    }
    public void QuitGame()
    {
        Application.Quit();
    }
    public void GotoMainMenu()
    {
        SceneManager.LoadScene("Home");
    }
    public void SettingGame()
    {
        Application.Quit();
    }
    public void StastGame()
    {
        mainMenu.StartGame();
    }
}
