using UnityEngine;

public class SandDamage : MonoBehaviour
{
    [Header("Damage")]
    public int damagePerSecond = 5;

    [Header("Ground Detection")]
    public float groundCheckDistance = 5f;
    public string sandTerrainName = "SandTerrain";

    private PlayerHealth playerHealth;
    private float damageTimer;

    private void Awake()
    {
        playerHealth = GetComponent<PlayerHealth>();
    }

    private void Update()
    {
        if (playerHealth == null)
        {
            return;
        }

        if (IsStandingOnSand())
        {
            damageTimer += Time.deltaTime;

            if (damageTimer >= 1f)
            {
                playerHealth.TakeDamage(damagePerSecond);
                damageTimer -= 1f;
            }
        }
        else
        {
            damageTimer = 0f;
        }
    }

    private bool IsStandingOnSand()
    {
        Vector3 rayStart = transform.position + Vector3.up;

        RaycastHit[] hits = Physics.RaycastAll(
            rayStart,
            Vector3.down,
            groundCheckDistance
        );

        foreach (RaycastHit hit in hits)
        {
            Terrain terrain = hit.collider.GetComponent<Terrain>();

            if (
                terrain != null &&
                terrain.gameObject.name.StartsWith(sandTerrainName)
            )
            {
                return true;
            }
        }

        return false;
    }
}