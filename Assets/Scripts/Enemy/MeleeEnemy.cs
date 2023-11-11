
public class MeleeEnemy : Enemy
{
    public void MeleeAttack()
    {
        if (isDying)
            return;
        
        canMove = false;
        
        animator.SetTrigger(AttackAnimString);
        
        StartCoroutine(WaitToAttack(attackAgainDelay));
    }
}
