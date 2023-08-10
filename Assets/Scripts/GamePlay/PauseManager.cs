using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PauseManager : MonoBehaviour
{
    [SerializeField] private Button RestartBtn;
    [SerializeField] private Button LobbyBtn;
    [SerializeField] private Button QuitBtn;
    [SerializeField] private Button CloseBtn;

    private void Awake()
    {
        RestartBtn.onClick.AddListener(onRestartBtnClick);
        LobbyBtn.onClick.AddListener(onLobbyBtnClick);
        QuitBtn.onClick.AddListener(onQuitBtnClick);
        CloseBtn.onClick.AddListener(onCloseBtnClick);
    }
    
    private void onRestartBtnClick()
    {
        SoundManager.Instance.Play(SoundManager.Sounds.ButtonClick);
        ToggleObj(false);
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
    
    private void onLobbyBtnClick()
    {
        SoundManager.Instance.Play(SoundManager.Sounds.ButtonClick);
        ToggleObj(false);
        SceneManager.LoadScene("Lobby");
    }
    
    private void onQuitBtnClick()
    {
        SoundManager.Instance.Play(SoundManager.Sounds.ButtonClick);
        ToggleObj(false);
        Application.Quit();
    }
    
    private void onCloseBtnClick()
    {
        SoundManager.Instance.Play(SoundManager.Sounds.ButtonClick);
        ToggleObj(false);
    }

    private void ToggleObj(bool status)
    {
        gameObject.SetActive(status);
        OptionsManager.Instance.ToggleBG(status);
    }
    
    private void OnDestroy()
    {
        RestartBtn.onClick.RemoveAllListeners();
        LobbyBtn.onClick.RemoveAllListeners();
        QuitBtn.onClick.RemoveAllListeners();
        CloseBtn.onClick.RemoveAllListeners(); 
    }
}
