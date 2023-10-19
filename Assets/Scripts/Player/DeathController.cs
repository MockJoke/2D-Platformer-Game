using UnityEngine;
using UnityEngine.SceneManagement;

//when a player falls off the platform
public class DeathController : MonoBehaviour
{
    [SerializeField] private HealthController healthController;

    void Start()
    {
        if (healthController == null)
            healthController = FindObjectOfType<HealthController>();
    }

    public void OnDeathFromFall()
    {
        SoundManager.Instance.Play(SoundManager.Sounds.PlayerDeath);
        PlayerDied();
    }

    public void PlayerDied()
    {
        healthController.LoseLife(); 

        Invoke(nameof(ReloadScene), 1.5f);
    }

    private void ReloadScene()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}
