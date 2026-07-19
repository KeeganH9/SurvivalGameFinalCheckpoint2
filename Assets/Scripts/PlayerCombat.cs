using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerCombat : MonoBehaviour
{
    [Header("Default Unarmed Attack")]
    public Transform unarmedAttackPoint;
    public int unarmedDamage = 10;
    public float unarmedRange = 1.5f;

    [Header("Attack Timing")]
    public float attackCooldown = 0.8f;
    public float hitDelay = 0.3f;

    [Header("References")]
    public LayerMask zombieLayer;
    public Animator animator;

    [Header("Animation")]
    public string attackTrigger = "Attack";

    private Transform currentAttackPoint;
    private int currentDamage;
    private float currentRange;

    private float defaultAttackCooldown;
    private float nextAttackTime;
    private bool attackInProgress;

    private void Awake()
    {
        if (animator == null)
        {
            animator = GetComponentInChildren<Animator>();
        }

        defaultAttackCooldown = attackCooldown;

        EquipUnarmed();
    }

    private void Update()
    {
        // Prevent attacking while the inventory or another menu is open.
        if (InventoryUI.IsAnyMenuOpen)
        {
            return;
        }

        if (Mouse.current == null)
        {
            return;
        }

        if (!Mouse.current.leftButton.wasPressedThisFrame)
        {
            return;
        }

        if (Time.time < nextAttackTime || attackInProgress)
        {
            return;
        }

        StartCoroutine(AttackRoutine());
    }

    private IEnumerator AttackRoutine()
    {
        attackInProgress = true;
        nextAttackTime = Time.time + attackCooldown;

        if (animator != null && !string.IsNullOrEmpty(attackTrigger))
        {
            animator.SetTrigger(attackTrigger);
        }

        yield return new WaitForSeconds(hitDelay);

        DealAttackDamage();

        float remainingCooldown = attackCooldown - hitDelay;

        if (remainingCooldown > 0f)
        {
            yield return new WaitForSeconds(remainingCooldown);
        }

        attackInProgress = false;
    }

    private void DealAttackDamage()
    {
        if (currentAttackPoint == null)
        {
            Debug.LogWarning("PlayerCombat has no current attack point.");
            return;
        }

        Collider[] hits = Physics.OverlapSphere(
            currentAttackPoint.position,
            currentRange,
            zombieLayer
        );

        Debug.Log("Found " + hits.Length + " enemy colliders.");

        foreach (Collider hit in hits)
        {
            ZombieHealth zombieHealth =
                hit.GetComponentInParent<ZombieHealth>();

            if (zombieHealth != null)
            {
                zombieHealth.TakeDamage(currentDamage);
                continue;
            }

            WolfHealth wolfHealth =
                hit.GetComponentInParent<WolfHealth>();

            if (wolfHealth != null)
            {
                wolfHealth.TakeDamage(currentDamage);
            }
        }
    }

    public void EquipWeapon(
        Transform weaponAttackPoint,
        int weaponDamage,
        float weaponRange)
    {
        if (weaponAttackPoint == null)
        {
            EquipUnarmed();
            return;
        }

        currentAttackPoint = weaponAttackPoint;
        currentDamage = weaponDamage;
        currentRange = weaponRange;
    }

    public void EquipUnarmed()
    {
        currentAttackPoint = unarmedAttackPoint;
        currentDamage = unarmedDamage;
        currentRange = unarmedRange;
    }

    public void SetAttackCooldown(float newAttackCooldown)
    {
        attackCooldown = Mathf.Max(0.01f, newAttackCooldown);
    }

    public void ResetAttackCooldown()
    {
        attackCooldown = defaultAttackCooldown;
    }

    private void OnDisable()
    {
        StopAllCoroutines();
        attackInProgress = false;
    }

    private void OnDrawGizmosSelected()
    {
        Transform point = currentAttackPoint;

        if (point == null)
        {
            point = unarmedAttackPoint;
        }

        if (point == null)
        {
            return;
        }

        float range = currentRange > 0f
            ? currentRange
            : unarmedRange;

        Gizmos.DrawWireSphere(point.position, range);
    }
}