using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

//when a player falls off the platform
public class DeathController : MonoBehaviour
{
    public void OnDeathFromFall()
    {
        SoundManager.Instance.Play(SoundManager.Sounds.PlayerDeath);
        PlayerDied(true);
    }

    public void PlayerDied(bool fromWater)
    {
        GameManager.Instance.healthController.LoseLife(fromWater); 

        Invoke(nameof(ReloadScene), 1.5f);
    }

    private void ReloadScene()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}
