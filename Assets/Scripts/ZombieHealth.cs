using UnityEngine;
using UnityEngine.AI;

public class ZombieHealth : MonoBehaviour
{
    [Header("Health")]
    public int maxHealth = 100;

    [Header("Death")]
    public string deathTrigger = "Die";
    public float destroyDelay = 5f;

    private int currentHealth;
    private bool isDead;

    private Animator animator;
    private NavMeshAgent agent;
    private ZombieAI zombieAI;
    private Collider[] zombieColliders;

    private void Awake()
    {
        currentHealth = maxHealth;

        animator = GetComponentInChildren<Animator>();
        agent = GetComponent<NavMeshAgent>();
        zombieAI = GetComponent<ZombieAI>();
        zombieColliders = GetComponentsInChildren<Collider>();
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
            animator.SetTrigger(deathTrigger);
        }

        Destroy(gameObject, destroyDelay);
    }

    public bool IsDead()
    {
        return isDead;
    }
}