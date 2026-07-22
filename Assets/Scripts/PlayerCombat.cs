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
    public PlayerEquipment playerEquipment;

    [Header("Animation")]
    public string attackTrigger = "Attack";
    public string blockingParameter = "IsBlocking";

    private Transform currentAttackPoint;
    private int currentDamage;
    private float currentRange;

    private float defaultAttackCooldown;
    private float nextAttackTime;
    private bool attackInProgress;
    private bool isBlocking;

    public bool IsBlocking
    {
        get { return isBlocking; }
    }

    private void Awake()
    {
        if (animator == null)
        {
            animator = GetComponentInChildren<Animator>();
        }

        if (playerEquipment == null)
        {
            playerEquipment = GetComponent<PlayerEquipment>();
        }

        defaultAttackCooldown = attackCooldown;
        EquipUnarmed();
    }

    private void Update()
    {
        if (InventoryUI.IsAnyMenuOpen)
        {
            StopBlocking();
            return;
        }

        if (Mouse.current == null)
        {
            return;
        }

        HandleBlocking();

        if (isBlocking)
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

    private void HandleBlocking()
    {
        bool hasShield =
            playerEquipment != null &&
            playerEquipment.HasShieldEquipped();

        bool wantsToBlock =
            hasShield &&
            Mouse.current.rightButton.isPressed &&
            !attackInProgress;

        if (wantsToBlock == isBlocking)
        {
            return;
        }

        isBlocking = wantsToBlock;

        if (animator != null &&
            !string.IsNullOrEmpty(blockingParameter))
        {
            animator.SetBool(blockingParameter, isBlocking);
        }
    }

    private void StopBlocking()
    {
        if (!isBlocking)
        {
            return;
        }

        isBlocking = false;

        if (animator != null &&
            !string.IsNullOrEmpty(blockingParameter))
        {
            animator.SetBool(blockingParameter, false);
        }
    }

    private IEnumerator AttackRoutine()
    {
        attackInProgress = true;
        nextAttackTime = Time.time + attackCooldown;

        if (animator != null &&
            !string.IsNullOrEmpty(attackTrigger))
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
            Debug.LogWarning(
                "PlayerCombat has no current attack point."
            );
            return;
        }

        Collider[] hits = Physics.OverlapSphere(
            currentAttackPoint.position,
            currentRange,
            zombieLayer
        );

        Debug.Log(
            "Found " + hits.Length + " enemy colliders."
        );

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
        attackCooldown =
            Mathf.Max(0.01f, newAttackCooldown);
    }

    public void ResetAttackCooldown()
    {
        attackCooldown = defaultAttackCooldown;
    }

    private void OnDisable()
    {
        StopAllCoroutines();
        attackInProgress = false;
        StopBlocking();
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