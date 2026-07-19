using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class ZombieAI : MonoBehaviour
{
    [System.Serializable]
    public class ZombieAttack
    {
        [Tooltip("Animator trigger used to play this attack.")]
        public string triggerName = "Attack1";

        [Tooltip("Damage dealt when this attack hits.")]
        public int damage = 10;

        [Tooltip("Maximum distance from which this attack can hit.")]
        public float attackRange = 2f;

        [Tooltip("Time between starting the animation and checking for damage.")]
        public float damageDelay = 0.6f;

        [Tooltip("Time before the zombie can begin another attack.")]
        public float cooldown = 1.5f;
    }

    [Header("Detection")]
    public float detectionRange = 30f;

    [Header("Attacks")]
    public ZombieAttack[] attacks;

    [Header("Turning")]
    public float turnSpeed = 8f;

    [Header("Animation Parameters")]
    public string speedParameter = "Speed";

    private Transform player;
    private PlayerHealth playerHealth;

    private NavMeshAgent agent;
    private Animator animator;
    private ZombieHealth zombieHealth;
    private ZombieSpawner spawner;

    private float nextAttackTime;
    private Coroutine damageCoroutine;

    private readonly List<ZombieAttack> availableAttacks =
        new List<ZombieAttack>();

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponentInChildren<Animator>();
        zombieHealth = GetComponent<ZombieHealth>();
    }

    private void Start()
    {
        FindPlayer();
    }

    private void Update()
    {
        if (zombieHealth != null && zombieHealth.IsDead())
        {
            StopZombie();
            return;
        }

        if (player == null)
        {
            FindPlayer();
            return;
        }

        float distanceToPlayer = Vector3.Distance(
            transform.position,
            player.position
        );

        if (distanceToPlayer > detectionRange)
        {
            StopMoving();
            UpdateMovementAnimation();
            return;
        }

        if (CanAnyAttackReach(distanceToPlayer))
        {
            AttackPlayer(distanceToPlayer);
        }
        else
        {
            ChasePlayer();
        }

        UpdateMovementAnimation();
    }

    private void FindPlayer()
    {
        GameObject playerObject =
            GameObject.FindGameObjectWithTag("Player");

        if (playerObject == null)
        {
            return;
        }

        player = playerObject.transform;

        playerHealth =
            playerObject.GetComponent<PlayerHealth>();

        if (playerHealth == null)
        {
            playerHealth =
                playerObject.GetComponentInParent<PlayerHealth>();
        }

        if (playerHealth == null)
        {
            playerHealth =
                playerObject.GetComponentInChildren<PlayerHealth>();
        }

        if (playerHealth == null)
        {
            Debug.LogWarning(
                "ZombieAI found the Player, but could not find " +
                "a PlayerHealth component."
            );
        }
    }

    private void ChasePlayer()
    {
        if (agent == null ||
            !agent.enabled ||
            !agent.isOnNavMesh ||
            player == null)
        {
            return;
        }

        agent.isStopped = false;
        agent.SetDestination(player.position);
    }

    private bool CanAnyAttackReach(float distanceToPlayer)
    {
        if (attacks == null || attacks.Length == 0)
        {
            return false;
        }

        foreach (ZombieAttack attack in attacks)
        {
            if (attack == null)
            {
                continue;
            }

            if (distanceToPlayer <= attack.attackRange)
            {
                return true;
            }
        }

        return false;
    }

    private void AttackPlayer(float distanceToPlayer)
    {
        StopMoving();
        FacePlayer();

        if (Time.time < nextAttackTime)
        {
            return;
        }

        ZombieAttack selectedAttack =
            ChooseAvailableAttack(distanceToPlayer);

        if (selectedAttack == null)
        {
            return;
        }

        StartAttack(selectedAttack);
    }

    private ZombieAttack ChooseAvailableAttack(
        float distanceToPlayer
    )
    {
        availableAttacks.Clear();

        if (attacks == null || attacks.Length == 0)
        {
            Debug.LogWarning(
                gameObject.name +
                " does not have any attacks configured."
            );

            return null;
        }

        foreach (ZombieAttack attack in attacks)
        {
            if (attack == null)
            {
                continue;
            }

            if (distanceToPlayer <= attack.attackRange)
            {
                availableAttacks.Add(attack);
            }
        }

        if (availableAttacks.Count == 0)
        {
            return null;
        }

        int randomIndex = Random.Range(
            0,
            availableAttacks.Count
        );

        return availableAttacks[randomIndex];
    }

    private void StartAttack(ZombieAttack selectedAttack)
    {
        if (animator != null &&
            !string.IsNullOrWhiteSpace(
                selectedAttack.triggerName
            ))
        {
            ResetAllAttackTriggers();

            animator.SetTrigger(
                selectedAttack.triggerName
            );
        }

        if (damageCoroutine != null)
        {
            StopCoroutine(damageCoroutine);
        }

        damageCoroutine = StartCoroutine(
            CheckAttackHitAfterDelay(selectedAttack)
        );

        nextAttackTime =
            Time.time +
            Mathf.Max(0.1f, selectedAttack.cooldown);
    }

    private IEnumerator CheckAttackHitAfterDelay(
        ZombieAttack selectedAttack
    )
    {
        yield return new WaitForSeconds(
            Mathf.Max(0f, selectedAttack.damageDelay)
        );

        damageCoroutine = null;

        if (zombieHealth != null &&
            zombieHealth.IsDead())
        {
            yield break;
        }

        if (player == null || playerHealth == null)
        {
            yield break;
        }

        float distanceToPlayer = Vector3.Distance(
            transform.position,
            player.position
        );

        if (distanceToPlayer <= selectedAttack.attackRange)
        {
            playerHealth.TakeDamage(
                selectedAttack.damage
            );

            Debug.Log(
                gameObject.name +
                " hit the player with " +
                selectedAttack.triggerName +
                " for " +
                selectedAttack.damage +
                " damage."
            );
        }
        else
        {
            Debug.Log(
                gameObject.name +
                "'s " +
                selectedAttack.triggerName +
                " attack missed."
            );
        }
    }

    private void ResetAllAttackTriggers()
    {
        if (animator == null || attacks == null)
        {
            return;
        }

        foreach (ZombieAttack attack in attacks)
        {
            if (attack == null ||
                string.IsNullOrWhiteSpace(
                    attack.triggerName
                ))
            {
                continue;
            }

            animator.ResetTrigger(
                attack.triggerName
            );
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

        if (direction.sqrMagnitude <= 0.001f)
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

    private void StopMoving()
    {
        if (agent == null ||
            !agent.enabled ||
            !agent.isOnNavMesh)
        {
            return;
        }

        agent.isStopped = true;

        if (agent.hasPath)
        {
            agent.ResetPath();
        }
    }

    private void StopZombie()
    {
        StopMoving();

        if (damageCoroutine != null)
        {
            StopCoroutine(damageCoroutine);
            damageCoroutine = null;
        }

        if (animator != null)
        {
            animator.SetFloat(
                speedParameter,
                0f
            );

            ResetAllAttackTriggers();
        }
    }

    private void UpdateMovementAnimation()
    {
        if (animator == null)
        {
            return;
        }

        float movementSpeed = 0f;

        if (agent != null &&
            agent.enabled &&
            agent.isOnNavMesh)
        {
            movementSpeed =
                agent.velocity.magnitude;
        }

        animator.SetFloat(
            speedParameter,
            movementSpeed
        );
    }

    // Kept so old animation events do not create errors.
    // Damage is handled by the coroutine instead.
    public void ApplyAttackDamage()
    {
    }

    public void DealAttackDamage()
    {
    }

    public void FinishAttack()
    {
    }

    public void SetSpawner(
        ZombieSpawner newSpawner
    )
    {
        spawner = newSpawner;
    }

    public ZombieSpawner GetSpawner()
    {
        return spawner;
    }

    private void OnDisable()
    {
        if (damageCoroutine != null)
        {
            StopCoroutine(damageCoroutine);
            damageCoroutine = null;
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.DrawWireSphere(
            transform.position,
            detectionRange
        );

        if (attacks == null)
        {
            return;
        }

        foreach (ZombieAttack attack in attacks)
        {
            if (attack == null)
            {
                continue;
            }

            Gizmos.DrawWireSphere(
                transform.position,
                attack.attackRange
            );
        }
    }
}