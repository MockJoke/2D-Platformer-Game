using System;
using UnityEngine;
using UnityEngine.Events;

public class PlayerMovement : MonoBehaviour
{
	[SerializeField] public float jumpForce = 400f;								// Amount of force added when the player jumps.
	[SerializeField] public float jumpHeight = 10f;							// Amount of force added when the player jumps.
	[SerializeField] private float recoilForce = 50f;                           // Amount of force added when the player gets hurt.
	[Range(0, 1)] 
	[SerializeField] private float crouchSpeed = .36f;							// Amount of maxSpeed applied to crouching movement. 1 = 100%
	[Range(0, .3f)] 
	[SerializeField] private float movementSmoothing = .05f;					// How much to smooth out the movement
	[SerializeField] private bool airControl = false;                           // Whether or not a player can steer while jumping;
	[SerializeField] private LayerMask whatIsGround;                            // A mask determining what is ground to the character
	[SerializeField] private Transform groundCheck;                             // A position marking where to check if the player is grounded.
	[SerializeField] private Transform ceilingCheck;                            // A position marking where to check for ceilings
	[SerializeField] private Collider2D crouchDisableCollider;                  // A collider that will be disabled when crouching
	[SerializeField] public float gravityScale = 1f;							//Strength of the player's gravity as a multiplier of gravity (set in ProjectSettings/Physics2D). Also the value the player's rigidbody2D.gravityScale is set to.
	[SerializeField] public float fallGravityScale = 2f;							
	
	[Range(0f, 1f)] 
	[SerializeField] private float groundedRadius = .5f;						// Radius of the overlap circle to determine if grounded
	[Range(0f, 1f)]
	[SerializeField] private float ceilingRadius = .5f;							// Radius of the overlap circle to determine if the player can stand up
	public bool grounded;														// Whether or not the player is grounded.
	public Rigidbody2D playerRB { get; private set; }	
	private bool facingRight = true;											// For determining which way the player is currently facing.
	private Vector3 velocity = Vector3.zero;
	private Vector3 scale;
	private Vector2 lookDir;

	[System.Serializable]
	public class BoolEvent : UnityEvent<bool> { }
	[Space]
	[Header("Events")]
	public BoolEvent OnLandEvent;

	public bool isJumping { get; set; } = false;
	public bool isAirborne { get; set; } = false;
	
	public BoolEvent onCrouchEvent;
	public bool isCrouching { get; set; } = false;
	private bool wasCrouching = false;
	
	#region MONOBEHAVIOUR METHODS
	private void Awake()
	{
		playerRB = GetComponent<Rigidbody2D>();

		if (OnLandEvent == null)
			OnLandEvent = new BoolEvent();

		if (onCrouchEvent == null)
			onCrouchEvent = new BoolEvent();
	}

	private void Start()
	{
		SetGravityScale(gravityScale);
	}

	private void Update()
	{
		SetGravityScale(playerRB.velocity.y >= 0 ? gravityScale : fallGravityScale);
	}

	private void FixedUpdate()
	{
		grounded = false;

		// The player is grounded if a circleCast to the groundCheck position hits anything designated as ground
		// This can be done using layers instead but Sample Assets will not overwrite your project settings.
		Collider2D[] colliders = Physics2D.OverlapCircleAll(groundCheck.position, groundedRadius, whatIsGround);
		for (int i = 0; i < colliders.Length; i++)
		{
			if (colliders[i].gameObject != gameObject)
			{
				if (colliders[i].isTrigger)
					continue;
				
				grounded = true;
				
				break;
			}
		}
		
        OnLandEvent.Invoke(grounded);
		
		if (Camera.main != null) 
			lookDir = Camera.main.ScreenToWorldPoint(transform.position);
	}
	
	private void OnDrawGizmos()
	{
		Gizmos.color = Color.yellow;
		
		if (groundCheck == null)
			return;
		
		Gizmos.DrawWireSphere(groundCheck.position, groundedRadius);
		
		if (ceilingCheck == null)
			return;
		
		Gizmos.DrawWireSphere(ceilingCheck.position, ceilingRadius);
	}
	#endregion
    
	public void SetGravityScale(float gScale)
	{
		playerRB.gravityScale = gScale;  
	}
	
	public void Move(float move, bool crouch, bool jump)
	{
		// If crouching, check to see if the character can stand up
		if (!crouch)
		{
			// If the character has a ceiling preventing them from standing up, keep them crouching
			if (Physics2D.OverlapCircle(ceilingCheck.position, ceilingRadius, whatIsGround))
				crouch = true;
		}

		//only control the player if grounded or airControl is turned on
		if (grounded || airControl)
		{
			// If crouching
			if (crouch)
			{
				if (!wasCrouching)
				{
					wasCrouching = true;
					onCrouchEvent.Invoke(true);
				}

				// Reduce the speed by the crouchSpeed multiplier
				move *= crouchSpeed;

				// Disable one of the colliders when crouching
				if (crouchDisableCollider != null)
					crouchDisableCollider.enabled = false;
			}
			else
			{
				// Enable the collider when not crouching
				if (crouchDisableCollider != null)
					crouchDisableCollider.enabled = true;

				if (wasCrouching)
				{
					wasCrouching = false;
					onCrouchEvent.Invoke(false);
				}
			}
            
			var rbVelocity = playerRB.velocity;
			// Move the character by finding the target velocity
			Vector3 targetVelocity = new Vector2(move * 10f, rbVelocity.y);
			// And then smoothing it out and applying it to the character
			playerRB.velocity = Vector3.SmoothDamp(rbVelocity, targetVelocity, ref velocity, movementSmoothing);

			// If the input is moving the player right and the player is facing left...
			if (move > 0 && !facingRight)
				flipPlayer();
			// Otherwise if the input is moving the player left and the player is facing right...
			else if (move < 0 && facingRight)
				flipPlayer();
		}
		
		// If the player should jump...
		if (grounded && jump)
		{
			// Add a vertical force to the player.
			grounded = false;
			//playerRB.AddForce(new Vector2(0f, jumpForce));
			//playerRB.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
		}
	}
	
	private void flipPlayer()
	{
		// Switch the way the player is labelled as facing.
		facingRight = !facingRight;

		// Multiply the player's x local scale by -1.
		Vector3 localScale = transform.localScale;
		localScale.x *= -1;
		transform.localScale = localScale;
	}

	public void hurtPlayer()
    {
		scale = transform.localScale;
		
		if (scale.x == 1)
			playerRB.AddForce(lookDir * recoilForce, ForceMode2D.Impulse);
		else
			playerRB.AddForce(-lookDir * recoilForce, ForceMode2D.Impulse);
	}
}