using System.Collections;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private Animator playerAnimator;
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private PlayerMovement playerMovement;
    [SerializeField] private Damager meleeDamager;
    [SerializeField] private Damageable damageable;

    [Header("Movement Controls")]
    [SerializeField] private float runSpeed = 40f;   
    private float horizontalMove = 0f;
    public bool canMove = true;
    
    [Space]
    [SerializeField] private float flickeringDuration = 0.1f;
    
    [Header("Input related fields for variable jump")]
    [SerializeField] private float buttonPressWindow = 0.5f;            // Threshold amount of time for the button input
    private float buttonPressedTime = 0f;                               // Amount of time for which the input button was being pressed
    private bool jumpCancelled = false;                                 // Bool to keep track of the jump input for cancellation
    
    [Header("Controllers")]
    [SerializeField] private DeathController deathController;
    [SerializeField] private ScoreController scoreController;
    [SerializeField] private HealthController healthController;
    
    private Vector3 initialPos;

    private Coroutine hurtFlickerCoroutine = null;
    
    // Animator hash strings
    private static readonly int isJumpingAnimString = Animator.StringToHash("isJumping");
    private static readonly int isDyingAnimString = Animator.StringToHash("isDying");
    private static readonly int isHurtAnimString = Animator.StringToHash("isHurt");
    private static readonly int isCrouchingAnimString = Animator.StringToHash("isCrouching");
    private static readonly int attackAnimString = Animator.StringToHash("MeleeAttack");
    private static readonly int xSpeedAnimString = Animator.StringToHash("HorizontalSpeed");
    private static readonly int ySpeedAnimString = Animator.StringToHash("VerticalSpeed");

    #region Monobehaviour Methods
    private void Awake()
    {
        if (playerMovement == null)
            playerMovement = this.gameObject.GetComponent<PlayerMovement>();
        
        if (deathController == null)
            deathController = FindObjectOfType<DeathController>();

        if (scoreController == null)
            scoreController = FindObjectOfType<ScoreController>();

        if (healthController == null)
            healthController = FindObjectOfType<HealthController>();
        
        initialPos = transform.position;
    }

    private void Start()
    {
        meleeDamager.DisableDamage();
    }

    private void Update()
    {
        if (Input.GetKeyUp(KeyCode.Space) || (buttonPressedTime > buttonPressWindow))
        {
            jumpAnimTrigger(false);
        }
        
        if (canMove is false) 
            return;
        
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
        
        if (Input.GetKeyDown(KeyCode.K) && !playerMovement.isAirborne && !playerMovement.isCrouching)
        {
            EnableMeleeAttack();
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
    }
    
    private void FixedUpdate()
    {
        if (jumpCancelled && playerMovement.isJumping && playerMovement.playerRB.velocity.y > 0)
        {
            playerMovement.AddCancelJumpForce();
        }
        
        playerMovement.Move(horizontalMove * Time.fixedDeltaTime,  playerMovement.isCrouching, playerMovement.isJumping);
        //playerMovement.isJumping = false;
    }
    #endregion
    
    #region Animation Triggers
    private void runAnim()
    {
        horizontalMove = Input.GetAxisRaw("Horizontal") * runSpeed;
        playerAnimator.SetFloat(xSpeedAnimString, Mathf.Abs(horizontalMove));
        playerAnimator.SetFloat(ySpeedAnimString, playerMovement.playerRB.velocity.y);
    }

    private void jumpAnimTrigger(bool status, bool setAnimationTrigger = true)
    {
        playerMovement.isJumping = status;
        
        if (setAnimationTrigger)
            playerAnimator.SetBool(isJumpingAnimString, status);
    }

    private void crouchAnimTrigger()
    {
        if (Input.GetButtonDown("Crouch"))
            playerMovement.isCrouching = true;
        else if (Input.GetButtonUp("Crouch"))
            playerMovement.isCrouching = false;
    }

    private void attackAnimTrigger()
    {
        playerAnimator.SetTrigger(attackAnimString);
    }
    
    private void deathAnimTrigger()
    {
        playerAnimator.SetTrigger(isDyingAnimString);
    }

    private void hurtAnimTrigger()
    {
        playerAnimator.SetTrigger(isHurtAnimString);
    }
    
    public void OnLanding(bool isGrounded)
    {
        playerMovement.isAirborne = !isGrounded;
    }

    public void OnCrouching(bool IsCrouching)
    {
        playerAnimator.SetBool(isCrouchingAnimString, IsCrouching);
        crouchAnimTrigger();
    }
    #endregion

    public void Respawn(bool resetHealth)
    {
        transform.position = initialPos;
        
        if (resetHealth)
            damageable.SetHealth(damageable.startingHealth);
    }

    private void EnableMeleeAttack()
    {
        meleeDamager.EnableDamage();
        damageable.EnableInvulnerability();
        meleeDamager.disableDamageAfterHit = true;
        attackAnimTrigger();

        StartCoroutine(disableMeleeAttack());
    }

    private IEnumerator disableMeleeAttack()
    {
        yield return new WaitForSeconds(1.5f);
        meleeDamager.DisableDamage();
        damageable.DisableInvulnerability();
    }
    
    public void DamagePlayer()
    {
        hurtAnimTrigger();
        playerMovement.PushPlayerOnHurting();
        // StartFlickering();
        damageable.EnableInvulnerability();
        healthController.LoseLife();
        StartCoroutine(TimeDelayForHurtingAgain());
    }
    
    private IEnumerator TimeDelayForHurtingAgain()
    {
        yield return new WaitForSeconds(2.5f);
        damageable.DisableInvulnerability();
        StopFlickering();
    }
    
    public void StartFlickering()
    {
        hurtFlickerCoroutine = StartCoroutine(Flicker());
    }
    
    private IEnumerator Flicker()
    {
        float timer = 0f;

        canMove = false;

        while (timer < damageable.invulnerabilityDuration)
        {
            spriteRenderer.enabled = !spriteRenderer.enabled;
            yield return new WaitForSeconds(flickeringDuration);
            timer += flickeringDuration;
        }

        spriteRenderer.enabled = true;
    }
    
    private void StopFlickering()
    {
        StopCoroutine(hurtFlickerCoroutine);
        canMove = true;
        spriteRenderer.enabled = true;
    }
    
    public void KillPlayer()
    {
        deathAnimTrigger();

        deathController.PlayerDied();
        GameManager.Instance.gameOverController.GameOver();
    }

    public bool HasKeys()
    {
        return scoreController.HasAllKeys();
    }
    
    public void OnLevelComplete()
    {
        LevelManager.Instance.MarkLevelComplete();
        LevelManager.Instance.GoToNextLevel();
    }

    public void PickUpKey()
    {
        scoreController.AddKey(); 
    }
    
    #region INPUT SYSTEM ACTION METHODS
    // // These are called from PlayerInput
    //
    // // When a joystick or arrow keys has been pushed. It stores the input Vector as a Vector3 to then be used by the smoothing function.
    // public void OnMovement(InputAction.CallbackContext value)
    // {
    //     
    // }
    //
    // public void OnJump(InputAction.CallbackContext value)
    // {
    //     // if (value.started)
    //     // {
    //     //     jumpAnimTrigger();
    //     // }
    // }
    //
    // public void OnCrouch(InputAction.CallbackContext value)
    // {
    //     if (value.started)
    //     {
    //         crouchAnimTrigger();
    //     }
    // }
    //
    // public void OnAttack(InputAction.CallbackContext value)
    // {
    //     
    // }
    #endregion
}
