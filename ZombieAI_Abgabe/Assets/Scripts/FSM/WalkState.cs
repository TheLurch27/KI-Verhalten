using UnityEngine;

public class WalkState : IZombieState
{
    public void EnterState(ZombieAI zombie)
    {
        Debug.Log("Entering Walk State.");
        zombie.Animator.SetTrigger("WalkTrigger");
    }

    public void UpdateState(ZombieAI zombie)
    {
        if (!zombie.IsPlayerInDetectionRange())
        {
            Debug.Log("Player out of detection range.");
            zombie.TransitionToState(new IdleState());
        }
        else if (zombie.IsPlayerInAttackRange())
        {
            Debug.Log("Player in attack range.");
            zombie.TransitionToState(new AttackState());
        }
        else
        {
            zombie.MoveToPlayer();
        }
    }
}
