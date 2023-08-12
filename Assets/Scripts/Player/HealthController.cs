using System;
using UnityEngine;
using UnityEngine.UI; 

public class HealthController : MonoBehaviour
{
    [SerializeField] private Image[] lives; 
    private int livesCount = 3;

    public Action<bool> OnDamage;

    public void LoseLife(bool fromWater)
    {
        livesCount--;
        
        if(livesCount > 0)
        {
            OnDamage?.Invoke(fromWater);

            lives[livesCount].gameObject.SetActive(false);

            if (livesCount == 0)
            {
                lives[livesCount].gameObject.SetActive(false);
            }
        }
    }
}
