using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerHealth : MonoBehaviour
{
    [Header("Health")]
    public int maximumHealth = 100;

    [Header("UI")]
    public PlayerHealthUI healthUI;

    [Header("Death")]
    public string deathSceneName = "DeathScreen";

    private int currentHealth;
    private bool isDead;

    private void Start()
    {
        currentHealth = maximumHealth;
        isDead = false;
        UpdateHealthUI();
    }

    public void TakeDamage(int damage)
    {
        if (damage <= 0 || isDead)
        {
            return;
        }

        currentHealth -= damage;
        currentHealth = Mathf.Clamp(
            currentHealth,
            0,
            maximumHealth
        );

        UpdateHealthUI();

        Debug.Log("Player Health: " + currentHealth);

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    public void Heal(int amount)
    {
        if (amount <= 0 || isDead)
        {
            return;
        }

        if (currentHealth >= maximumHealth)
        {
            Debug.Log("Player is already at maximum health.");
            return;
        }

        int previousHealth = currentHealth;

        currentHealth += amount;
        currentHealth = Mathf.Clamp(
            currentHealth,
            0,
            maximumHealth
        );

        int actualHealing = currentHealth - previousHealth;

        UpdateHealthUI();

        Debug.Log(
            "Player healed for " +
            actualHealing +
            ". Current Health: " +
            currentHealth
        );
    }

    public int GetCurrentHealth()
    {
        return currentHealth;
    }

    public bool IsAtMaximumHealth()
    {
        return currentHealth >= maximumHealth;
    }

    private void UpdateHealthUI()
    {
        if (healthUI != null)
        {
            healthUI.UpdateHealth(currentHealth);
        }
    }

    private void Die()
    {
        if (isDead)
        {
            return;
        }

        isDead = true;
        currentHealth = 0;
        UpdateHealthUI();

        Debug.Log("Player died. Loading death screen.");

        Time.timeScale = 1f;
        SceneManager.LoadScene(deathSceneName);
    }
}