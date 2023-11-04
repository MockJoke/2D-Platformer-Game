using System;
using UnityEngine;
using UnityEngine.UI; 

public class HealthController : MonoBehaviour
{
    [SerializeField] private Image[] lives;
    private int livesCount = 5;
    
    public void LoseLife()
    {
        livesCount--;
        
        if(livesCount > 0)
        {
            //OnDamage?.Invoke(fromWater);

            lives[livesCount].gameObject.SetActive(false);

            if (livesCount == 0)
            {
                lives[livesCount].gameObject.SetActive(false);
            }
        }
        else
        {
            GameManager.Instance.gameOverController.GameOver();
        }
    }
}
