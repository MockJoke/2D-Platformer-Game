using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    public Animator enemyAnimator;
    
    public float speed;

    public float RayDistance;

    public bool ismovingRight = true;

    public Transform groundDetection;

    public LivesController livesController;
     
    
    //when a player collide with an enemy
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.GetComponent<PlayerController>() != null)
        {
            PlayerController playerController = collision.gameObject.GetComponent<PlayerController>();
            livesController.LoseLife(); 
        }
    }

    private void Update()
    {
        transform.Translate(Vector2.right * speed * Time.deltaTime);

        RaycastHit2D GroundDetector = Physics2D.Raycast(groundDetection.position, Vector2.down, RayDistance);
        
        if (!GroundDetector.collider)
        {
            if (ismovingRight)
            {
                transform.eulerAngles = new Vector3(0, -180, 0);
                ismovingRight = false;
            }
            else
            {
                transform.eulerAngles = new Vector3(0, 0, 0);
                ismovingRight = true;
            }

        }

    }
}