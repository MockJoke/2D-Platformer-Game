using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class ScoreController : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI levelIndicatorText;
    
    [SerializeField] private Image[] keys;
    
    private LevelLoader levelLoader;
    private LevelManager levelManager; 

    private int keysCount = 0;

    private void Awake()
    {
        levelIndicatorText = GetComponent<TextMeshProUGUI>();
    }

    private void Start()
    {
        ResetKeys();
    }

    private void ResetKeys()
    {
        keysCount = 0;
        
        for (int i = 0; i < keys.Length; i++)
        {
            keys[i].gameObject.SetActive(false);
        }
    }
    
    public void AddKey()
    {
        keysCount++;
        
        if(keysCount > 0)
        {
            keys[keysCount - 1].gameObject.SetActive(true);

            if (keysCount == 0)
            {
                keys[keysCount].gameObject.SetActive(false);
            }
        }
    }

    public bool HasAllKeys()
    {
        return keysCount >= keys.Length;
    }
}
