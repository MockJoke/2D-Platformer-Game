using UnityEngine;
using UnityEngine.UI;

public class LobbyController : MonoBehaviour
{
    [SerializeField] private GameObject MainMenuButtonsObj; 
    [SerializeField] private GameObject LevelMenuButtonsObj; 

    [SerializeField] private Button StartBtn;
    [SerializeField] private Button ExitBtn;

    private void Awake()
    {
        StartBtn.onClick.AddListener(onStartBtnClick);
        ExitBtn.onClick.AddListener(onExitBtnClick); 
    }

    private void onStartBtnClick()
    {
        SoundManager.Instance.Play(SoundManager.Sounds.ButtonClick);
        LevelMenuButtonsObj.SetActive(true);
        MainMenuButtonsObj.SetActive(false);        
    }

    private void onExitBtnClick()
    {
        SoundManager.Instance.Play(SoundManager.Sounds.ButtonClick);
        Application.Quit();       
    }

    private void OnDestroy()
    {
        StartBtn.onClick.RemoveAllListeners();
        ExitBtn.onClick.RemoveAllListeners();
    }
}
