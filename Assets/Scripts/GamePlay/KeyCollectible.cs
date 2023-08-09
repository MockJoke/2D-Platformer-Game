using UnityEngine;

public class KeyCollectible : MonoBehaviour
{
   private void OnCollisionEnter2D(Collision2D obj)
   {
       PlayerController playerController = obj.gameObject.GetComponent<PlayerController>();
       
        if(playerController != null)
        {
            playerController.PickUpKey();
            Destroy(gameObject); 
        }
   }
}
