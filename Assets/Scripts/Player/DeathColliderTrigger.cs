using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeathColliderTrigger : MonoBehaviour
{
    private DeathController deathController;

    private void Awake()
    {
        deathController = gameObject.GetComponentInParent<DeathController>();
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.GetComponent<PlayerController>() != null)
        {
            deathController.OnDeathFromFall();
        }
    }
}
