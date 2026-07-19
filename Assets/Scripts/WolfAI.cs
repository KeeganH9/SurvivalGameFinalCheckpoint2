using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class WolfAI : MonoBehaviour
{
    [Header("Movement")]
    public float walkingSpeed = 2f;
    public float runningSpeed = 5f;
    public float wanderRadius = 12f;

    [Header("Idle")]
    public float minimumIdleTime = 2f;
    public float maximumIdleTime = 5f;

    [Header("Detection")]
    public float detectionRange = 15f;
    public float losePlayerRange = 22f;

    [Header("Attack")]
    public float attackRange = 2f;
    public int attackDamage = 10;
    public float attackCooldown = 1.5f;

    [Header("Turning")]
    public float turnSpeed = 8f;

    [Header("Animator Parameters")]
    public string speedParameter = "Speed";
    public string playerSeenParameter = "PlayerSeen";
    public string attackParameter = "Attack";

    private NavMeshAgent agent;
    private Animator animator;

    private Transform player;
    private PlayerHealth playerHealth;
    private WolfSpawner spawner;

    private Vector3 spawnPosition;

    private float idleTimer;
    private float nextAttackTime;

    private bool playerDetected;
    private bool isWandering;
    private bool isDead;

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponentInChildren<Animator>();

        spawnPosition = transform.position;
    }

    private void Start()
    {
        FindPlayer();
        BeginIdle();
    }

    private void Update()
    {
        if (isDead || !agent.isOnNavMesh)
        {
            return;
        }

        if (player == null)
        {
            FindPlayer();
            HandleWandering();
            UpdateAnimator();
            return;
        }

        float distanceToPlayer = Vector3.Distance(
            transform.position,
            player.position
        );

        UpdatePlayerDetection(distanceToPlayer);

        if (playerDetected)
        {
            HandlePlayerDetected(distanceToPlayer);
        }
        else
        {
            HandleWandering();
        }

        UpdateAnimator();
    }

    private void UpdatePlayerDetection(float distanceToPlayer)
    {
        if (!playerDetected &&
            distanceToPlayer <= detectionRange)
        {
            playerDetected = true;

            animator?.SetBool(
                playerSeenParameter,
                true
            );
        }
        else if (playerDetected &&
                 distanceToPlayer > losePlayerRange)
        {
            playerDetected = false;

            animator?.SetBool(
                playerSeenParameter,
                false
            );

            BeginIdle();
        }
    }

    private void HandlePlayerDetected(float distanceToPlayer)
    {
        if (distanceToPlayer <= attackRange)
        {
            HandleAttack();
        }
        else
        {
            ChasePlayer();
        }
    }

    private void ChasePlayer()
    {
        agent.isStopped = false;
        agent.speed = runningSpeed;
        agent.stoppingDistance = attackRange;

        agent.SetDestination(player.position);

        isWandering = false;
    }

    private void HandleAttack()
    {
        agent.isStopped = true;
        agent.ResetPath();

        FacePlayer();

        if (Time.time < nextAttackTime)
        {
            return;
        }

        nextAttackTime = Time.time + attackCooldown;

        if (animator != null)
        {
            animator.SetTrigger(attackParameter);
        }

        if (playerHealth != null)
        {
            playerHealth.TakeDamage(attackDamage);
        }
    }

    private void HandleWandering()
    {
        if (isWandering)
        {
            bool reachedDestination =
                !agent.pathPending &&
                agent.remainingDistance <= 0.25f;

            if (reachedDestination)
            {
                BeginIdle();
            }

            return;
        }

        agent.isStopped = true;

        idleTimer -= Time.deltaTime;

        if (idleTimer <= 0f)
        {
            PickWanderDestination();
        }
    }

    private void BeginIdle()
    {
        isWandering = false;

        if (agent.isOnNavMesh)
        {
            agent.isStopped = true;
            agent.ResetPath();
        }

        idleTimer = Random.Range(
            minimumIdleTime,
            maximumIdleTime
        );
    }

    private void PickWanderDestination()
    {
        Vector3 randomDirection =
            Random.insideUnitSphere * wanderRadius;

        randomDirection.y = 0f;

        Vector3 randomPosition =
            spawnPosition + randomDirection;

        if (NavMesh.SamplePosition(
                randomPosition,
                out NavMeshHit hit,
                wanderRadius,
                NavMesh.AllAreas))
        {
            agent.speed = walkingSpeed;
            agent.stoppingDistance = 0f;
            agent.isStopped = false;
            agent.SetDestination(hit.position);

            isWandering = true;
        }
        else
        {
            BeginIdle();
        }
    }

    private void FacePlayer()
    {
        if (player == null)
        {
            return;
        }

        Vector3 direction =
            player.position - transform.position;

        direction.y = 0f;

        if (direction.sqrMagnitude < 0.01f)
        {
            return;
        }

        Quaternion targetRotation =
            Quaternion.LookRotation(direction);

        transform.rotation = Quaternion.Slerp(
            transform.rotation,
            targetRotation,
            turnSpeed * Time.deltaTime
        );
    }

    private void UpdateAnimator()
    {
        if (animator == null)
        {
            return;
        }

        float currentSpeed = agent.isStopped
            ? 0f
            : agent.velocity.magnitude;

        animator.SetFloat(
            speedParameter,
            currentSpeed
        );

        animator.SetBool(
            playerSeenParameter,
            playerDetected
        );
    }

    private void FindPlayer()
    {
        GameObject playerObject =
            GameObject.FindGameObjectWithTag("Player");

        if (playerObject == null)
        {
            Debug.LogWarning(
                "WolfAI could not find an object tagged Player."
            );

            return;
        }

        player = playerObject.transform;
        playerHealth =
            playerObject.GetComponent<PlayerHealth>();
    }

    public void SetSpawner(WolfSpawner wolfSpawner)
    {
        spawner = wolfSpawner;
    }

    public void Die()
    {
        if (isDead)
        {
            return;
        }

        isDead = true;

        if (agent.isOnNavMesh)
        {
            agent.isStopped = true;
            agent.ResetPath();
        }

        if (spawner != null)
        {
            spawner.NotifyWolfDied();
        }

        Destroy(gameObject, 3f);
    }
}