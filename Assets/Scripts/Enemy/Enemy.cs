using System.Collections;
using UnityEditor;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    [SerializeField] protected Animator animator;
    [SerializeField] protected Damager damager;
    [SerializeField] protected Damageable damageable;
    
    [SerializeField] private BoxCollider2D boxCollider;
    
    [SerializeField] private float speed = 5f;
    [SerializeField] private Transform[] patrolSpots;
    [SerializeField] protected float attackAgainDelay = 3.5f;
    private int currPatrolSpotIndex = 0;

    [SerializeField] protected bool shouldMove = true;
    protected bool canMove = true;
    protected bool isDying = false;
    protected bool isMovingRight;
    protected Vector2 spriteForward;
    
    [Header("Scanning settings")]
    [Tooltip("The angle of the forward of the view cone. 0 is forward of the sprite, 90 is up, 180 behind etc.")]
    [Range(0.0f,360.0f)]
    [SerializeField] protected float viewDirection = 0.0f;
    [Range(0.0f, 360.0f)]
    [SerializeField] protected float viewFov;
    [SerializeField] protected float viewDistance;
    [Tooltip("Time in seconds without the target in the view cone before the target is considered lost from sight")]
    [SerializeField] protected float timeBeforeTargetLost = 3.0f;
    protected float timeSinceLastTargetView;

    private readonly bool isHit = false;
    private static readonly int deathAnimString = Animator.StringToHash("Die");
    private static readonly int hitAnimString = Animator.StringToHash("Hit");
    protected static readonly int AttackAnimString = Animator.StringToHash("Attack");
    protected static readonly int SpottedAnimString = Animator.StringToHash("Spotted");
    
    protected virtual void Awake()
    {
        if (boxCollider == null)
            boxCollider = this.GetComponent<BoxCollider2D>();
    }

    protected virtual void Start()
    {
        isDying = false;
        damageable.SetHealth(damageable.startingHealth);

        if (shouldMove)
        {
            isMovingRight = transform.position.x < patrolSpots[currPatrolSpotIndex].position.x;
        }
        else
        {
            isMovingRight = transform.eulerAngles.y == 0;
        }

        spriteForward = isMovingRight ? Vector2.right : Vector2.left;
    }

    protected virtual void FixedUpdate()
    {
        if (shouldMove && canMove && !isHit)
            Move();
    }
    
    private void Move()
    {
        var currPos = transform.position;
        Vector2 movePos = new Vector2(patrolSpots[currPatrolSpotIndex].position.x, currPos.y);
        
        currPos = Vector2.MoveTowards(currPos, movePos, speed * Time.deltaTime);
        transform.position = currPos;

        if (Vector2.Distance(currPos, movePos) < 0.2f)
        {
            if (currPatrolSpotIndex < patrolSpots.Length - 1)
                currPatrolSpotIndex++;
            else
                currPatrolSpotIndex = 0;

            if (currPatrolSpotIndex == 0 || currPatrolSpotIndex == patrolSpots.Length - 1)
                Flip();
        }
    }

    protected void Flip()
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
        
        spriteForward = isMovingRight ? Vector2.right : Vector2.left;
    }

    public void OnHit()
    {
        canMove = false;
        animator.SetTrigger(hitAnimString);
    }

    public void OnDeath()
    {
        isDying = true;
        boxCollider.enabled = false;
        canMove = false;
        animator.ResetTrigger(hitAnimString);
        animator.ResetTrigger(AttackAnimString);
        animator.SetTrigger(deathAnimString);
    }

    public IEnumerator WaitToAttack(float time)
    {
        damager.DisableDamage();
        yield return new WaitForSeconds(time);
        damager.EnableDamage();
        canMove = true;
    }
    
    public void DestroyEnemy()
    {
        Destroy(gameObject);
    }
    
#if UNITY_EDITOR
    private void OnDrawGizmosSelected()
    {
        //draw the cone of view
        Vector3 forward = isMovingRight ? Vector2.right : Vector2.left;
        forward = Quaternion.Euler(0, 0, isMovingRight ? viewDirection : -viewDirection) * forward;

        if (GetComponent<SpriteRenderer>().flipX) forward.x = -forward.x;

        Vector3 endpoint = transform.position + (Quaternion.Euler(0, 0, viewFov * 0.5f) * forward);

        Handles.color = new Color(0, 1.0f, 0, 0.2f);
        Handles.DrawSolidArc(transform.position, -Vector3.forward, (endpoint - transform.position).normalized, viewFov, viewDistance);

        //Draw attack range
        // Handles.color = new Color(1.0f, 0,0, 0.1f);
        // Handles.DrawSolidDisc(transform.position, Vector3.back, meleeRange);
    }
#endif
}

