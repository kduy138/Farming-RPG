using UnityEngine;
using UnityEngine.AI;

public class EnemyAIManager : MonoBehaviour
{
    [Header("References")]
    public Player player;
    public Animator animator;
    public EnemyScriptableObject enemySO;

    [Header("Settings")]
    public float detectionRadius = 15f;
    public float attackRange = 2f;
    public float patrolRadius = 20f;
    public float attackCooldown = 2f;
    public float patrolIdleTime = 3f;
    public float rotationSpeed = 7f;
    public float attackDuration = 1.0f;

    private NavMeshAgent agent;
    private float cooldownTimer;
    private float idleTimer;
    private float attackTimer;

    private Vector3 patrolPoint;
    private bool isPatrolling;
    private bool isIdle;
    private bool isAttacking;

    [Header("Stats")]
    private float currentMaxHealth;
    private float currentHealth;

    private enum State { Patrol, Chase, Attack }
    private State currentState;

    private void Awake()
    {
        player = FindAnyObjectByType<Player>();
    }

    private void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        if (animator == null) animator = GetComponent<Animator>();

        SetNewPatrolPoint();
        currentState = State.Patrol;
        InitializeEnemyStats();
    }

    private void Update()
    {
        cooldownTimer -= Time.deltaTime;

        float distanceToPlayer = Vector3.Distance(transform.position, player.GetPlayerTransform().position);

        // Cancel attack if player leaves attack range
        if (isAttacking && distanceToPlayer > attackRange)
        {
            CancelAttack();
            currentState = State.Chase;
        }

        // Handle attack duration manually (no animation event needed)
        if (isAttacking)
        {
            attackTimer -= Time.deltaTime;
            if (attackTimer <= 0f)
            {
                EndAttack();
            }
        }

        // State switching
        if (!isAttacking)
        {
            if (distanceToPlayer <= attackRange && cooldownTimer <= 0f)
                currentState = State.Attack;
            else if (distanceToPlayer <= detectionRadius)
                currentState = State.Chase;
            else
                currentState = State.Patrol;
        }

        // Execute state
        switch (currentState)
        {
            case State.Patrol: Patrol(); break;
            case State.Chase: ChasePlayer(); break;
            case State.Attack: Attack(); break;
        }

        animator.SetBool("isWalking", agent.velocity.magnitude > 0.1f && !isAttacking);

        if (!isAttacking)
            RotateTowardsMovementDirection();
    }

    private void InitializeEnemyStats()
    {
        if (enemySO == null) return;

        currentHealth = enemySO.Health;
        currentMaxHealth = enemySO.Health;
    }

    private void Patrol()
    {
        if (isIdle)
        {
            idleTimer += Time.deltaTime;
            if (idleTimer >= patrolIdleTime)
            {
                SetNewPatrolPoint();
                idleTimer = 0f;
            }
            return;
        }

        if (!isPatrolling || Vector3.Distance(transform.position, patrolPoint) < 1.5f)
        {
            isIdle = true;
            isPatrolling = false;
            agent.ResetPath();
        }
    }

    private void SetNewPatrolPoint()
    {
        Vector3 randomDirection = Random.insideUnitSphere * patrolRadius + transform.position;

        if (NavMesh.SamplePosition(randomDirection, out NavMeshHit hit, patrolRadius, NavMesh.AllAreas))
        {
            patrolPoint = hit.position;
            agent.SetDestination(patrolPoint);
            isPatrolling = true;
            isIdle = false;
        }
    }

    private void ChasePlayer()
    {
        isIdle = false;
        isPatrolling = false;

        if (agent.isOnNavMesh && player != null)
            agent.SetDestination(player.GetPlayerTransform().position);
    }

    private void Attack()
    {
        if (isAttacking) return;

        float distance = Vector3.Distance(transform.position, player.GetPlayerTransform().position);
        if (distance > attackRange)
        {
            currentState = State.Chase;
            return;
        }

        isAttacking = true;
        cooldownTimer = attackCooldown;
        attackTimer = attackDuration;
        agent.ResetPath();

        // Rotate to face player instantly
        Vector3 lookPos = new Vector3(player.GetPlayerTransform().position.x, transform.position.y, player.GetPlayerTransform().position.z);
        transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(lookPos - transform.position), Time.deltaTime * rotationSpeed);

        animator.ResetTrigger("Attack");
        animator.SetTrigger("Attack");
    }

    public void DealDamage()
    {
        if (Vector3.Distance(transform.position, player.GetPlayerTransform().position) <= attackRange)
        {
            player.TakeDamage(enemySO.ATKDamage);
        }
    }

    public void EndAttack()
    {
        isAttacking = false;
        attackTimer = 0f;
        DealDamage();
    }

    public void CancelAttack()
    {
        if (!isAttacking) return;

        isAttacking = false;
        attackTimer = 0f;
        cooldownTimer = attackCooldown;

        animator.ResetTrigger("Attack");

        // Instantly cut the attack animation
        if (animator.HasState(0, Animator.StringToHash("Walk")))
            animator.CrossFade("Walk", 0.1f);
        else if (animator.HasState(0, Animator.StringToHash("Walk")))
            animator.CrossFade("Walk", 0.1f);

        if (agent.isOnNavMesh && player != null)
            agent.SetDestination(player.GetPlayerTransform().position);
    }

    private void RotateTowardsMovementDirection()
    {
        if (agent.velocity.sqrMagnitude > 0.1f)
        {
            Quaternion targetRotation = Quaternion.LookRotation(agent.velocity.normalized);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * rotationSpeed);
        }
    }
}
