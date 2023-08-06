using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

//when a player falls off the platform
public class DeathController : MonoBehaviour
{
    private HealthController healthController;

    private void Awake()
    {
        healthController = FindObjectOfType<HealthController>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.GetComponent<PlayerController>() != null)
        {
            SoundManager.Instance.Play(SoundManager.Sounds.PlayerDeath);
        }
    }

    public void PlayerDied()
    {
        healthController.LoseLife(); 

        Invoke(nameof(ReloadScene), 2f);
    }

    private void ReloadScene()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}
