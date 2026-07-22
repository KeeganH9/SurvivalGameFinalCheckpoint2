using UnityEngine;
using UnityEngine.AI;

public class ZombieHealth : MonoBehaviour
{
    [Header("Health")]
    public int maxHealth = 100;
    public int healthIncrease = 50;
    public int daysPerIncrease = 10;

    [Header("Day/Night Reference")]
    public DayNightManager dayNightManager;

    [Header("Death")]
    public string deathTrigger = "Die";
    public float destroyDelay = 5f;

    [Header("Attack Triggers To Cancel")]
    public string attackOneTrigger = "Attack1";
    public string attackTwoTrigger = "Attack2";

    private int currentHealth;
    private bool isDead;

    private Animator animator;
    private NavMeshAgent agent;
    private ZombieAI zombieAI;
    private Collider[] zombieColliders;

    private void Awake()
    {
        animator = GetComponentInChildren<Animator>();
        agent = GetComponent<NavMeshAgent>();
        zombieAI = GetComponent<ZombieAI>();
        zombieColliders = GetComponentsInChildren<Collider>();
    }

    private void Start()
    {
        if (dayNightManager == null)
        {
            dayNightManager = FindFirstObjectByType<DayNightManager>();
        }

        ApplyDayHealthIncrease();
        currentHealth = maxHealth;
    }

    private void ApplyDayHealthIncrease()
    {
        if (dayNightManager == null)
        {
            Debug.LogWarning(
                "ZombieHealth could not find a DayNightManager. " +
                "The zombie will use its base health."
            );

            return;
        }

        int currentDay = Mathf.Max(1, dayNightManager.currentDay);

        // Day 1-10 = no increase
        // Day 11-20 = one increase
        // Day 21-30 = two increases
        int healthIncreaseLevels = (currentDay - 1) / daysPerIncrease;

        maxHealth += healthIncreaseLevels * healthIncrease;

        Debug.Log(
            gameObject.name + " spawned on Day " + currentDay +
            " with " + maxHealth + " health."
        );
    }

    public void TakeDamage(int damage)
    {
        if (isDead)
        {
            return;
        }

        currentHealth -= damage;

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        if (isDead)
        {
            return;
        }

        isDead = true;

        if (zombieAI != null)
        {
            zombieAI.enabled = false;
        }

        if (agent != null)
        {
            agent.isStopped = true;
            agent.enabled = false;
        }

        foreach (Collider zombieCollider in zombieColliders)
        {
            zombieCollider.enabled = false;
        }

        if (animator != null)
        {
            animator.ResetTrigger(attackOneTrigger);
            animator.ResetTrigger(attackTwoTrigger);
            animator.ResetTrigger(deathTrigger);

            animator.SetFloat("Speed", 0f);
            animator.SetTrigger(deathTrigger);
        }

        Destroy(gameObject, destroyDelay);
    }

    public bool IsDead()
    {
        return isDead;
    }
}