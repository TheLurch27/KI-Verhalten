using UnityEngine;
using UnityEngine.AI;

public class ZombieAI : MonoBehaviour
{
    public float detectionRadius = 15f;
    public float stopDistance = 1f; // Abstand vor dem Spieler
    public float attackRange = 2f;
    public int attackDamage = 10; // Schaden pro Angriff
    public float attackCooldown = 1.0f; // Angriff alle 1 Sekunde

    private Animator _animator;
    private NavMeshAgent _navMeshAgent;
    private Transform _player;
    private PlayerHealth playerHealth;
    private bool isPlayerInRange;
    private float lastAttackTime;

    public Animator Animator => _animator;  // Öffentliche Eigenschaft
    public NavMeshAgent NavMeshAgent => _navMeshAgent;  // Öffentliche Eigenschaft
    public Transform Player => _player;  // Öffentliche Eigenschaft

    private IZombieState currentState;

    private void Awake()
    {
        _animator = GetComponent<Animator>();
        _navMeshAgent = GetComponent<NavMeshAgent>();
        _player = GameObject.FindGameObjectWithTag("Player").transform;
        playerHealth = _player.GetComponent<PlayerHealth>();
    }

    private void Start()
    {
        TransitionToState(new IdleState());
    }

    private void Update()
    {
        currentState?.UpdateState(this);

        if (Vector3.Distance(transform.position, _player.position) <= detectionRadius)
        {
            TransitionToState(new WalkState());
        }

        // Überprüfen, ob der Spieler im Angriffsradius ist
        isPlayerInRange = Vector3.Distance(transform.position, _player.position) <= attackRange;
    }

    public void TransitionToState(IZombieState newState)
    {
        currentState = newState;
        currentState.EnterState(this);
    }

    public void Die()
    {
        TransitionToState(new DeathState());
    }

    public bool IsPlayerInAttackRange()
    {
        return isPlayerInRange;
    }

    public bool IsPlayerInDetectionRange()
    {
        return Vector3.Distance(transform.position, _player.position) <= detectionRadius;
    }

    public void MoveToPlayer()
    {
        Vector3 directionToPlayer = (_player.position - transform.position).normalized;
        Vector3 targetPosition = _player.position - directionToPlayer * stopDistance;

        if (Vector3.Distance(transform.position, _player.position) > stopDistance)
        {
            _navMeshAgent.SetDestination(targetPosition);
        }
        else
        {
            _navMeshAgent.ResetPath();
        }
    }

    public void StopMoving()
    {
        _navMeshAgent.ResetPath();
    }

    public void TryAttackPlayer()
    {
        if (isPlayerInRange && Time.time > lastAttackTime + attackCooldown)
        {
            AnimatorStateInfo stateInfo = _animator.GetCurrentAnimatorStateInfo(0);
            if (stateInfo.IsName("Attack"))  // Sicherstellen, dass der Zustand korrekt überprüft wird
            {
                Debug.Log("Attacking player.");
                playerHealth.TakeDamage(attackDamage);
                lastAttackTime = Time.time;
            }
        }
    }

    // Diese Methode wird von der Animation aufgerufen
    public void ApplyDamage()
    {
        if (isPlayerInRange)
        {
            Debug.Log("Applying damage to player.");
            playerHealth.TakeDamage(attackDamage);
        }
    }

    // Kollisionen verwalten
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            Debug.Log("Player collided with Zombie.");
            // Hier können Sie zusätzliche Logik hinzufügen, wenn der Spieler mit dem Zombie kollidiert
        }
    }
}
