using System.Collections;
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
    
    protected bool canMove = true;
    protected bool isDying = false;
    private bool isMovingRight;

    private readonly bool isHit = false;
    private static readonly int deathAnimString = Animator.StringToHash("Die");
    private static readonly int hitAnimString = Animator.StringToHash("Hit");
    protected static readonly int AttackAnimString = Animator.StringToHash("Attack");
    
    protected void Awake()
    {
        if (boxCollider == null)
            boxCollider = this.GetComponent<BoxCollider2D>();
    }

    protected void Start()
    {
        isDying = false;
        damageable.SetHealth(damageable.startingHealth);
        
        isMovingRight = transform.position.x < patrolSpots[currPatrolSpotIndex].position.x;
    }

    protected void FixedUpdate()
    {
        if (canMove && !isHit)
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

    private void Flip()
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
}

