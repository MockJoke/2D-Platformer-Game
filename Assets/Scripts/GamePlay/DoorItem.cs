using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorItem : MonoBehaviour
{
    [SerializeField] private Animator animator;
    
    private static readonly int OpenDoor = Animator.StringToHash("OpenDoor");

    private void OnTriggerEnter2D(Collider2D obj)
    {
        PlayerController playerController = obj.gameObject.GetComponent<PlayerController>();
       
        if(playerController != null)
        {
            if (playerController.HasKeys())
            {
                playerController.OnLevelComplete();
                animator.SetTrigger(OpenDoor);
            }
        }
    }
}
