using UnityEngine;
using UnityEngine.UI;

public class PlayerHealthUI : MonoBehaviour
{
    [Header("Health Segments")]
    public Image[] healthSegments;

    [Header("Settings")]
    public int healthPerSegment = 5;

   public void UpdateHealth(int currentHealth)
{
    Debug.Log("Updating health segments. Current health: " + currentHealth);

    if (healthSegments == null || healthSegments.Length == 0)
    {
        return;
    }

    int visibleSegments = Mathf.CeilToInt(
        currentHealth / (float)healthPerSegment
    );

    for (int i = 0; i < healthSegments.Length; i++)
    {
        healthSegments[i].enabled =
        i >= healthSegments.Length - visibleSegments;
    }
}
}