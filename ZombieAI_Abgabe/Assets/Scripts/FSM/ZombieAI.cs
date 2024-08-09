using UnityEngine;
using UnityEngine.AI;

public class ZombieAI : MonoBehaviour
{
    public float detectionRadius = 15f;
    public float stopDistance = 1f;
    public float attackRange = 2f;
    public int attackDamage = 10;
    public float attackCooldown = 1.0f;

    private Animator _animator;
    private NavMeshAgent _navMeshAgent;
    private Transform _player;
    private PlayerHealth playerHealth;
    private bool isPlayerInRange;
    private float lastAttackTime;
    private bool isDead = false;

    public Animator Animator => _animator;
    public NavMeshAgent NavMeshAgent => _navMeshAgent;
    public Transform Player => _player;

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
        if (isDead) return;

        currentState?.UpdateState(this);

        if (Vector3.Distance(transform.position, _player.position) <= detectionRadius)
        {
            TransitionToState(new WalkState());
        }

        isPlayerInRange = Vector3.Distance(transform.position, _player.position) <= attackRange;
    }

    public void TransitionToState(IZombieState newState)
    {
        currentState = newState;
        currentState.EnterState(this);
    }

    public void Die()
    {
        if (!isDead)
        {
            isDead = true;
            TransitionToState(new DeathState());
        }
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
            if (stateInfo.IsName("Attack"))
            {
                playerHealth.TakeDamage(attackDamage);
                lastAttackTime = Time.time;
            }
        }
    }

    public void ApplyDamage()
    {
        if (isPlayerInRange)
        {
            playerHealth.TakeDamage(attackDamage);
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            
        }
    }
}
