using UnityEngine;

public class AttackState : IZombieState
{
    public void EnterState(ZombieAI zombie)
    {
        Debug.Log("Entering Attack State.");
        zombie.StopMoving();
    }

    public void UpdateState(ZombieAI zombie)
    {
        if (!zombie.IsPlayerInAttackRange())
        {
            Debug.Log("Player out of attack range.");
            zombie.TransitionToState(new WalkState());
        }
        else
        {
            Debug.Log("Trying to attack player.");
            zombie.TryAttackPlayer();
        }
    }
}
