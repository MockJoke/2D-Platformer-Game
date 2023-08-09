using UnityEngine;

public class OptionsManager : Singleton<OptionsManager>
{
    [SerializeField] private GameObject PauseCanvas;
    [SerializeField] private GameObject SettingsCanvas;
    [SerializeField] private GameObject AudioCanvas;
    [SerializeField] private GameObject ControlsCanvas;
    [SerializeField] private GameObject BG;

    private void Update()
    {
        if (Input.GetKeyUp(KeyCode.Escape))
        {
            PauseCanvas.SetActive(true);
            ToggleBG(true);
        }
    }

    public void ToggleBG(bool status)
    {
        BG.SetActive(status);
    }
}
