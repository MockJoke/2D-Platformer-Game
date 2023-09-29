using UnityEngine;

public class KeyCollectible : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D obj)
    {
        PlayerController playerController = obj.gameObject.GetComponent<PlayerController>();
        
        if(playerController != null)
        {
            playerController.PickUpKey();
            Destroy(gameObject); 
        }
    }
}
