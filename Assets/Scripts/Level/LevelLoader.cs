using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(Button))]
public class LevelLoader : MonoBehaviour
{
    [SerializeField] private Button levelBtn;
    [SerializeField] private string levelName;

    private void Awake()
    {
        if (levelBtn == null)
            levelBtn = GetComponent<Button>();
        
        levelBtn.onClick.AddListener(onLevelBtnClick); 
    }

    public void onLevelBtnClick()
    {
        // checking the levelStatus
        LevelStatus levelStatus = LevelManager.Instance.GetLevelStatus(levelName);

        switch (levelStatus)
        {
            case LevelStatus.Locked:
                Debug.Log("Cant play this level till you unlock it!");
                break;

            case LevelStatus.Unlocked:
                SoundManager.Instance.Play(SoundManager.Sounds.ButtonClick); 
                SceneManager.LoadScene(levelName);
                break;

            case LevelStatus.Completed:
                SoundManager.Instance.Play(SoundManager.Sounds.ButtonClick);
                SceneManager.LoadScene(levelName);
                break;
        }
    }
}
