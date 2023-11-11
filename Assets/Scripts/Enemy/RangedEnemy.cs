using System;
using UnityEngine;

public class RangedEnemy : Enemy
{
    [Header("Range Attack Data")]
    [SerializeField] private GameObject bulletPrefab;
    [SerializeField, Tooltip("From where the projectile are spawned")]
    private Transform shootingOriginPos;
    [SerializeField] private PlayerController targetPlayer;
    private Vector3 targetShootPos;

    protected override void Start()
    {
        base.Start();

        if (targetPlayer == null)
            targetPlayer = FindObjectOfType<PlayerController>();
        
        targetShootPos = targetPlayer.transform.position;
    }

    void Update()
    {
        ScanForPlayer();
    }

    private void ScanForPlayer()
    {
        //If the player don't have control, they can't react, so do not pursue them
        if (!targetPlayer.canMove)
            return;
        
        if (isDying || !canMove)
            return;
        
        targetShootPos = targetPlayer.transform.position;
        Vector3 dir = targetShootPos - transform.position;

        if (dir.sqrMagnitude > viewDistance * viewDistance)
            return;

        Vector3 testForward = Quaternion.Euler(0, 0, isMovingRight ? Mathf.Sign(spriteForward.x) * viewDirection : Mathf.Sign(spriteForward.x) * -viewDirection) * spriteForward;

        float angle = Vector3.Angle(testForward, dir);

        if (angle > viewFov * 0.5f)
            return;
        
        timeSinceLastTargetView = timeBeforeTargetLost;

        animator.SetTrigger(SpottedAnimString);
    }
    
    public void RangeAttack()
    {
        canMove = false;
        
        if ((targetShootPos.x - transform.position.x < 0 && isMovingRight) || (targetShootPos.x - transform.position.x > 0 && !isMovingRight))
            Flip();
        
        Vector2 shootPosition = shootingOriginPos.transform.localPosition;
        
        animator.SetTrigger(AttackAnimString);
        
        GameObject obj = Instantiate(bulletPrefab, shootingOriginPos);
        Bullet b = obj.GetComponent<Bullet>();
        obj.transform.position = shootingOriginPos.TransformPoint(shootPosition);

        b.rb.velocity = (GetProjectileVelocity(targetShootPos, shootingOriginPos.transform.position));
        
        StartCoroutine(WaitToAttack(attackAgainDelay));
    }
    
    //This will give the velocity vector needed to give to the bullet rigidbody so it reach the given target from the origin.
    private Vector3 GetProjectileVelocity(Vector3 targetObj, Vector3 origin)
    {
        const float projectileSpeed = 30.0f;

        Vector3 velocity;
        Vector3 toTarget = targetObj - origin;

        float gSquared = Physics.gravity.sqrMagnitude;
        float b = projectileSpeed * projectileSpeed + Vector3.Dot(toTarget, Physics.gravity);
        float discriminant = b * b - gSquared * toTarget.sqrMagnitude;

        // Check whether the target is reachable at max speed or less.
        if (discriminant < 0)
        {
            velocity = toTarget;
            velocity.y = 0;
            velocity.Normalize();
            velocity.y = 0.7f;

            velocity *= projectileSpeed;
            return velocity;
        }

        float discRoot = Mathf.Sqrt(discriminant);

        // Highest
        float T_max = Mathf.Sqrt((b + discRoot) * 2f / gSquared);

        // Lowest speed arc
        float T_lowEnergy = Mathf.Sqrt(Mathf.Sqrt(toTarget.sqrMagnitude * 4f / gSquared));

        // Most direct with max speed
        float T_min = Mathf.Sqrt((b - discRoot) * 2f / gSquared);

        float T = 0;

        // 0 = highest, 1 = lowest, 2 = most direct
        int shotType = 1;

        switch (shotType)
        {
            case 0:
                T = T_max;
                break;
            case 1:
                T = T_lowEnergy;
                break;
            case 2:
                T = T_min;
                break;
            default:
                break;
        }

        velocity = toTarget / T - Physics.gravity * T / 2f;

        return velocity;
    }
}
