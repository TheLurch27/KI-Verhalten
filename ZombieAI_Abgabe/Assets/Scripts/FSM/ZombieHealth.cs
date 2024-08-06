using UnityEngine;

public class ZombieHealth : MonoBehaviour
{
    public int maxHealth = 100;
    private int currentHealth;
    private ZombieAI zombieAI;

    private void Awake()
    {
        currentHealth = maxHealth;
        zombieAI = GetComponent<ZombieAI>();
    }

    public void TakeDamage(int damage)
    {
        currentHealth -= damage;
        if (currentHealth <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        zombieAI.Die();
    }
}
