using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement; 

public class PlayerController : MonoBehaviour
{
    [SerializeField] private PlayerMovement playerMovement; 
    [SerializeField] private ScoreController scoreController;
    [SerializeField] private GameOverController gameOverController;
    private DeathController deathController;

    public Animator playerAnimator;

    [Header("Movement Controls")]
    public float runSpeed;   
    private float horizontalMove = 0f;
    public bool isJumping;
    public bool isCrouching; 
    
    #region MONOBEHAVIOUR METHODS
    private void Awake()
    {
        deathController = FindObjectOfType<DeathController>();
    }

    private void Update()
    {
        runAnim();
        crouchAnimTrigger();
    }
    
    private void FixedUpdate()
    {
        // Move our character 
        playerMovement.Move(horizontalMove * Time.fixedDeltaTime, isCrouching, isJumping);
        isJumping = false;
    }
    #endregion
    
    #region ANIMATION TRIGGERS
    private void runAnim()
    {
        horizontalMove = Input.GetAxisRaw("Horizontal") * runSpeed;
        playerAnimator.SetFloat("speed", Mathf.Abs(horizontalMove));
    }

    private void jumpAnimTrigger()
    {
        isJumping = true;
        playerAnimator.SetBool("isJumping", true);
    }

    private void crouchAnimTrigger()
    {
        //isCrouching = true;
        if (Input.GetButtonDown("Crouch"))
        {
            isCrouching = true;
        }
        else if (Input.GetButtonUp("Crouch"))
        {
            isCrouching = false;
        }
    }
    
    private void deathAnimTrigger()
    {
        playerAnimator.SetBool("isDying", true);
    }

    private void hurtAnimTrigger()
    {
        playerAnimator.SetBool("isHurt", true);
        StartCoroutine(TimeDelayForHurting()); 
    }
    #endregion

    public void OnLanding(bool isGrounded)
    {
        playerAnimator.SetBool("isJumping", !isGrounded);
    }

    public void OnCrouching(bool IsCrouching)
    {
        playerAnimator.SetBool("isCrouching", IsCrouching);
        crouchAnimTrigger();
    }
    
    public void DamagePlayer()
    {
        playerMovement.hurtPlayer();
        hurtAnimTrigger();
    }

    public void KillPlayer()
    {
        deathAnimTrigger();

        deathController.PlayerDied(); 
        gameOverController.GameOver();  
    }

    public void PickUpKey()
    {
        scoreController.AddKey(); 
    }

    IEnumerator TimeDelayForHurting()
    {
        yield return new WaitForSeconds(0.5f);
        playerAnimator.SetBool("isHurt", false);
    }
    
    #region INPUT SYSTEM ACTION METHODS
    // These are called from PlayerInput
    
    // When a joystick or arrow keys has been pushed. It stores the input Vector as a Vector3 to then be used by the smoothing function.
    public void OnMovement(InputAction.CallbackContext value)
    {
        
    }

    public void OnJump(InputAction.CallbackContext value)
    {
        if (value.started)
        {
            jumpAnimTrigger();
        }
    }

    public void OnCrouch(InputAction.CallbackContext value)
    {
        if (value.started)
        {
            crouchAnimTrigger();
        }
    }

    public void OnAttack(InputAction.CallbackContext value)
    {
        
    }
    #endregion
}
