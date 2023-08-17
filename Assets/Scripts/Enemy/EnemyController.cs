using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    public Animator enemyAnimator;
    
    public float speed;
    public float RayDistance;

    public bool isMovingRight;

    public Transform groundDetection;
    
    //when a player collide with an enemy
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.GetComponent<PlayerController>() != null)
        {
            GameManager.Instance.healthController.LoseLife(false); 

            SoundManager.Instance.Play(SoundManager.Sounds.ChomperEnemyCollision);
        }
    }

    private void Update()
    {
        transform.Translate(Vector2.right * speed * Time.deltaTime);

        changeEnemyDir(); 
    }

    // flip the enemy when it reaches the end of the ground
    private void changeEnemyDir()
    {
        RaycastHit2D GroundDetector = Physics2D.Raycast(groundDetection.position, Vector2.down, RayDistance);

        if (!GroundDetector.collider)
        {
            if (isMovingRight)
            {
                transform.eulerAngles = new Vector3(0, -180, 0);
                isMovingRight = false;
            }
            else
            {
                transform.eulerAngles = new Vector3(0, 0, 0);
                isMovingRight = true;
            }
        }
    }
}
