using UnityEngine;

public class WolfHealth : MonoBehaviour
{
    [Header("Health")]
    public int maxHealth = 50;

    private int currentHealth;
    private WolfAI wolfAI;

    private void Awake()
    {
        currentHealth = maxHealth;
        wolfAI = GetComponent<WolfAI>();
    }

    public void TakeDamage(int damage)
    {
        if (damage <= 0)
        {
            return;
        }

        currentHealth -= damage;

        Debug.Log(
            $"{gameObject.name} took {damage} damage. " +
            $"Health remaining: {currentHealth}"
        );

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        currentHealth = 0;

        Debug.Log($"{gameObject.name} died.");

        if (wolfAI != null)
        {
            wolfAI.Die();
        }
        else
        {
            Destroy(gameObject);
        }
    }
}