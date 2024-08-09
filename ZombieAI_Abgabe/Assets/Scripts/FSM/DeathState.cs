using UnityEngine;

public class DeathState : IZombieState
{
    public void EnterState(ZombieAI zombie)
    {
        Debug.Log("Entering Death State.");
        zombie.Animator.SetBool("isDead", true);
        zombie.StopMoving();
        zombie.NavMeshAgent.enabled = false;
    }

    public void UpdateState(ZombieAI zombie)
    {
        
    }
}
