using System.Collections;
using UnityEngine;

public class MeleeEnemy : Enemy
{
    private static readonly int AttackAnimString = Animator.StringToHash("Attack");
    
    public void MeleeAttack()
    {
        canMove = false;
        
        animator.SetTrigger(AttackAnimString);
        
        StartCoroutine(WaitToAttack(attackAgainDelay));
    }
}
