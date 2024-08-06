using UnityEngine;

public class IdleState : IZombieState
{
    public void EnterState(ZombieAI zombie)
    {
        zombie.Animator.SetBool("isWalking", false);
        zombie.StopMoving();
    }

    public void UpdateState(ZombieAI zombie)
    {
        if (zombie.IsPlayerInDetectionRange())
        {
            zombie.TransitionToState(new WalkState());
        }
    }
}
