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

    [SerializeField] private Animator playerAnimator;

    [Header("Movement Controls")]
    [SerializeField] private float runSpeed;   
    private float horizontalMove = 0f;
    
    [Header("Camera Movement")] 
    [SerializeField] private Transform camFollowTarget;
    [SerializeField] private float lookAheadAmount = 0.1f;
    [SerializeField] private float lookAheadSpeed = 0.1f;

    private float buttonPressedTime = 0f;
    public float buttonPressWindow = 0.5f;
    public float jumpCancelRate = 100f;
    private bool jumpCancelled = false;
    
    private static readonly int isJumpingStringHash = Animator.StringToHash("isJumping");
    private static readonly int xSpeedStringHash = Animator.StringToHash("HorizontalSpeed");
    private static readonly int isDyingStringHash = Animator.StringToHash("isDying");
    private static readonly int isHurtStringHash = Animator.StringToHash("isHurt");
    private static readonly int isCrouchingStringHash = Animator.StringToHash("isCrouching");
    private static readonly int ySpeedStringHash = Animator.StringToHash("VerticalSpeed");

    #region MONOBEHAVIOUR METHODS
    private void Awake()
    {
        deathController = FindObjectOfType<DeathController>();
    }

    private void Update()
    {
        runAnim();
        crouchAnimTrigger();

        if (Input.GetKeyDown(KeyCode.Space) && playerMovement.isAirborne is false)
        {
            jumpAnimTrigger();
            jumpCancelled = false;
            buttonPressedTime = 0;

            playerMovement.jumpForce = Mathf.Sqrt(playerMovement.jumpHeight * -2 * (Physics2D.gravity.y * playerMovement.playerRB.gravityScale));
            playerMovement.isAirborne = true;
        }

        if (playerMovement.isJumping)
        {
            playerMovement.playerRB.velocity = new Vector2(playerMovement.playerRB.velocity.x, playerMovement.jumpForce);
            buttonPressedTime += Time.deltaTime;
            
            if (Input.GetKeyUp(KeyCode.Space) && (buttonPressedTime < buttonPressWindow))
            {
                jumpCancelled = true;
            }

            if (playerMovement.playerRB.velocity.y < 0)
            {
                playerMovement.isJumping = false;
                playerAnimator.SetBool(isJumpingStringHash, false);
            }
        }
        
        if (Input.GetKeyUp(KeyCode.Space) || (buttonPressedTime > buttonPressWindow))
        {
            playerMovement.isJumping = false;
            playerAnimator.SetBool(isJumpingStringHash, false);
        }
    }
    
    private void FixedUpdate()
    {
        if (jumpCancelled && playerMovement.isJumping && playerMovement.playerRB.velocity.y > 0)
        {
            playerMovement.playerRB.AddForce(Vector2.down * jumpCancelRate);
        }
        
        // Move our character 
        playerMovement.Move(horizontalMove * Time.fixedDeltaTime,  playerMovement.isCrouching, playerMovement.isJumping);
        //playerMovement.isJumping = false;
    }
    #endregion
    
    public void MoveCameraTarget(float xVelocity)
    {
        var localPosition = camFollowTarget.localPosition;
        
        localPosition = new Vector3(Mathf.Lerp(localPosition.x, lookAheadAmount * xVelocity, lookAheadSpeed * Time.deltaTime), localPosition.y, localPosition.z);
        camFollowTarget.localPosition = localPosition;
    }
    
    #region ANIMATION TRIGGERS
    private void runAnim()
    {
        horizontalMove = Input.GetAxisRaw("Horizontal") * runSpeed;
        playerAnimator.SetFloat(xSpeedStringHash, Mathf.Abs(horizontalMove));
        playerAnimator.SetFloat(ySpeedStringHash, playerMovement.playerRB.velocity.y);
    }

    private void jumpAnimTrigger()
    {
        playerMovement.isJumping = true;
        playerAnimator.SetBool(isJumpingStringHash, true);
    }

    private void crouchAnimTrigger()
    {
        if (Input.GetButtonDown("Crouch"))
            playerMovement.isCrouching = true;
        else if (Input.GetButtonUp("Crouch"))
            playerMovement.isCrouching = false;
    }
    
    private void deathAnimTrigger()
    {
        playerAnimator.SetBool(isDyingStringHash, true);
    }

    private void hurtAnimTrigger()
    {
        playerAnimator.SetBool(isHurtStringHash, true);
        StartCoroutine(TimeDelayForHurting()); 
    }
    #endregion
    
    public void OnLanding(bool isGrounded)
    {
        playerMovement.isAirborne = !isGrounded;
        //playerAnimator.SetBool(isJumpingStringHash, !isGrounded);
    }

    public void OnCrouching(bool IsCrouching)
    {
        playerAnimator.SetBool(isCrouchingStringHash, IsCrouching);
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
        playerAnimator.SetBool(isHurtStringHash, false);
    }
    
    #region INPUT SYSTEM ACTION METHODS
    // These are called from PlayerInput
    
    // When a joystick or arrow keys has been pushed. It stores the input Vector as a Vector3 to then be used by the smoothing function.
    public void OnMovement(InputAction.CallbackContext value)
    {
        
    }

    public void OnJump(InputAction.CallbackContext value)
    {
        // if (value.started)
        // {
        //     jumpAnimTrigger();
        // }
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
