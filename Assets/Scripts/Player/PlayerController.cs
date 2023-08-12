using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement; 

public class PlayerController : MonoBehaviour
{
    [SerializeField] private PlayerMovement playerMovement;
    private DeathController deathController;

    [SerializeField] private Animator playerAnimator;

    [Header("Movement Controls")]
    [SerializeField] private float runSpeed = 40f;   
    private float horizontalMove = 0f;
    
    [Header("Camera Movement")] 
    [SerializeField] private Transform camFollowTarget;
    [SerializeField] private float lookAheadAmount = 0.1f;
    [SerializeField] private float lookAheadSpeed = 0.1f;
    public float cameraHorizontalFacingOffset;
    public float cameraHorizontalSpeedOffset;
    public float cameraVerticalInputOffset;
    public float maxHorizontalDeltaDampTime;
    public float maxVerticalDeltaDampTime;
    public float verticalCameraOffsetDelay;
    protected float m_CamFollowHorizontalSpeed;
    protected float m_CamFollowVerticalSpeed;
    protected float m_VerticalCameraOffsetTimer;
    
    [Header("Input related fields for variable jump")]
    private float buttonPressedTime = 0f;                               // Amount of time for which the input button was being pressed
    [SerializeField] private float buttonPressWindow = 0.5f;            // Threshold amount of time for the button input
    private bool jumpCancelled = false;                                 // Bool to keep track of the jump input for cancellation
    
    // Animator hash strings
    private static readonly int isJumpingStringHash = Animator.StringToHash("isJumping");
    private static readonly int xSpeedStringHash = Animator.StringToHash("HorizontalSpeed");
    private static readonly int isDyingStringHash = Animator.StringToHash("isDying");
    private static readonly int isHurtStringHash = Animator.StringToHash("isHurt");
    private static readonly int isCrouchingStringHash = Animator.StringToHash("isCrouching");
    private static readonly int ySpeedStringHash = Animator.StringToHash("VerticalSpeed");

    #region MONOBEHAVIOUR METHODS
    private void Awake()
    {
        if (playerMovement == null)
            playerMovement = this.gameObject.GetComponent<PlayerMovement>();
        
        deathController = FindObjectOfType<DeathController>();
        GameManager.Instance.healthController.OnDamage += DamagePlayer;
    }

    private void Update()
    {
        runAnim();
        crouchAnimTrigger();

        if (Input.GetKeyDown(KeyCode.Space) && playerMovement.isAirborne is false)
        {
            jumpAnimTrigger(true);
            jumpCancelled = false;
            buttonPressedTime = 0;
            
            playerMovement.SetJumpForce();
            playerMovement.isAirborne = true;
        }

        if (playerMovement.isJumping)
        {
            playerMovement.SetVelocity();
            buttonPressedTime += Time.deltaTime;
            
            if (Input.GetKeyUp(KeyCode.Space) && (buttonPressedTime < buttonPressWindow))
            {
                jumpCancelled = true;
            }

            if (playerMovement.playerRB.velocity.y < 0)
            {
                jumpAnimTrigger(false);
            }
        }
        
        if (Input.GetKeyUp(KeyCode.Space) || (buttonPressedTime > buttonPressWindow))
        {
            jumpAnimTrigger(false);
        }
    }
    
    private void FixedUpdate()
    {
        if (jumpCancelled && playerMovement.isJumping && playerMovement.playerRB.velocity.y > 0)
        {
            playerMovement.AddCancelJumpForce();
        }
        
        playerMovement.Move(horizontalMove * Time.fixedDeltaTime,  playerMovement.isCrouching, playerMovement.isJumping);
        //UpdateCameraFollowTargetPosition();
        //playerMovement.isJumping = false;
    }

    private void OnDestroy()
    {
        if (GameManager.Instance.healthController)
            GameManager.Instance.healthController.OnDamage -= DamagePlayer;
    }
    #endregion
    
    public void MoveCameraTarget(float xVelocity)
    {
        var localPosition = camFollowTarget.localPosition;
        
        localPosition = new Vector3(Mathf.Lerp(localPosition.x, lookAheadAmount * xVelocity, lookAheadSpeed * Time.deltaTime), localPosition.y, localPosition.z);
        camFollowTarget.localPosition = localPosition;
    }
    
    protected void UpdateCameraFollowTargetPosition()
    {
        float newLocalPosX;
        float newLocalPosY = 0f;

        // float desiredLocalPosX = (playerMovement.IsFacingRight ? -1f : 1f) * cameraHorizontalFacingOffset;
        // desiredLocalPosX += m_MoveVector.x * cameraHorizontalSpeedOffset;
        // if (Mathf.Approximately(m_CamFollowHorizontalSpeed, 0f))
        //     newLocalPosX = desiredLocalPosX;
        // else
        //     newLocalPosX = Mathf.Lerp(camFollowTarget.localPosition.x, desiredLocalPosX, m_CamFollowHorizontalSpeed * Time.deltaTime);

        // bool moveVertically = false;
        // if (!Mathf.Approximately(PlayerInput.Instance.Vertical.Value, 0f))
        // {
        //     m_VerticalCameraOffsetTimer += Time.deltaTime;
        //
        //     if (m_VerticalCameraOffsetTimer >= verticalCameraOffsetDelay)
        //         moveVertically = true;
        // }
        // else
        // {
        //     moveVertically = true;
        //     m_VerticalCameraOffsetTimer = 0f;
        // }

        if (playerMovement.isJumping) //(moveVertically)
        {
            float desiredLocalPosY = cameraVerticalInputOffset; //PlayerInput.Instance.Vertical.Value * cameraVerticalInputOffset;
            if (Mathf.Approximately(m_CamFollowVerticalSpeed, 0f))
                newLocalPosY = desiredLocalPosY;
            else
                newLocalPosY = Mathf.MoveTowards(camFollowTarget.localPosition.y, desiredLocalPosY, m_CamFollowVerticalSpeed * Time.deltaTime);
        }

        camFollowTarget.localPosition = new Vector2(0, newLocalPosY);
    }
    
    #region ANIMATION TRIGGERS
    private void runAnim()
    {
        horizontalMove = Input.GetAxisRaw("Horizontal") * runSpeed;
        playerAnimator.SetFloat(xSpeedStringHash, Mathf.Abs(horizontalMove));
        playerAnimator.SetFloat(ySpeedStringHash, playerMovement.playerRB.velocity.y);
    }

    private void jumpAnimTrigger(bool status, bool setAnimationTrigger = true)
    {
        playerMovement.isJumping = status;
        
        if (setAnimationTrigger)
            playerAnimator.SetBool(isJumpingStringHash, status);
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
        playerAnimator.SetTrigger(isDyingStringHash);
    }

    private void hurtAnimTrigger()
    {
        playerAnimator.SetBool(isHurtStringHash, true);
        StartCoroutine(TimeDelayForHurting()); 
    }
    
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
    #endregion

    private void DamagePlayer(bool fromWater = false)
    {
        if (!fromWater) 
            playerMovement.hurtPlayer();
        
        if (fromWater)
            deathAnimTrigger();
        else 
            hurtAnimTrigger();
            
    }

    public void KillPlayer()
    {
        deathAnimTrigger();

        deathController.PlayerDied(false); 
        GameManager.Instance.gameOverController.GameOver();  
    }

    public bool HasKeys()
    {
        return GameManager.Instance.scoreController.HasAllKeys();
    }
    
    public void OnLevelComplete()
    {
        LevelManager.Instance.MarkLevelComplete();
        LevelManager.Instance.GoToNextLevel();
    }

    public void PickUpKey()
    {
        GameManager.Instance.scoreController.AddKey(); 
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
