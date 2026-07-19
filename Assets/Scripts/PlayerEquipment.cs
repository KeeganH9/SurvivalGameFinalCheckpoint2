using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerEquipment : MonoBehaviour
{
    [Header("References")]
    public Inventory inventory;
    public Transform handPoint;
    public PlayerCombat playerCombat;
    public PlayerHealth playerHealth;

    [Header("Equipped Item")]
    [SerializeField] private ItemData equippedItem;

    private GameObject equippedObject;
    private int equippedSlotIndex = -1;
    private Animator animator;

    public event Action<int> EquippedSlotChanged;

    private void Awake()
    {
        if (playerCombat == null)
        {
            playerCombat = GetComponent<PlayerCombat>();
        }

        if (playerHealth == null)
        {
            playerHealth = GetComponent<PlayerHealth>();
        }

        animator = GetComponentInChildren<Animator>();

        if (animator == null)
        {
            Debug.LogWarning(
                "PlayerEquipment could not find an Animator on the player or its children."
            );
        }
    }

    public void EquipSlot(int slotIndex)
    {
        if (inventory == null)
        {
            Debug.LogWarning(
                "PlayerEquipment does not have an Inventory assigned."
            );
            return;
        }

        if (handPoint == null)
        {
            Debug.LogWarning(
                "PlayerEquipment does not have a Hand Point assigned."
            );
            return;
        }

        if (inventory.slots == null ||
            slotIndex < 0 ||
            slotIndex >= inventory.slots.Length)
        {
            return;
        }

        if (slotIndex == equippedSlotIndex)
        {
            Unequip();
            Debug.Log("Unequipped current item.");
            return;
        }

        InventorySlotData slot = inventory.slots[slotIndex];

        if (slot == null || slot.item == null)
        {
            Unequip();
            return;
        }

        ItemData item = slot.item;

        if (!item.canEquip)
        {
            Debug.Log(item.itemName + " cannot be equipped.");
            return;
        }

        if (item.equippedPrefab == null)
        {
            Debug.LogWarning(
                item.itemName +
                " does not have an equipped prefab assigned."
            );
            return;
        }

        Unequip();

        equippedObject = Instantiate(
            item.equippedPrefab,
            handPoint
        );

        equippedObject.transform.localPosition = Vector3.zero;
        equippedObject.transform.localRotation = Quaternion.identity;

        equippedItem = item;
        equippedSlotIndex = slotIndex;

        if (item.itemType == ItemType.Consumable)
        {
            PrepareConsumable();
        }
        else
        {
            ConnectItemToCombat(item);
        }

        EquippedSlotChanged?.Invoke(equippedSlotIndex);

        Debug.Log("Equipped: " + item.itemName);
    }

    private void PrepareConsumable()
    {
        if (playerCombat != null)
        {
            playerCombat.enabled = false;
        }
    }

    private void ConnectItemToCombat(ItemData item)
    {
        if (playerCombat == null)
        {
            Debug.LogWarning(
                "PlayerEquipment does not have PlayerCombat assigned."
            );
            return;
        }

        playerCombat.enabled = true;

        Transform weaponAttackPoint =
            equippedObject.transform.Find("AttackPoint");

        if (weaponAttackPoint == null)
        {
            weaponAttackPoint = equippedObject.transform;

            Debug.LogWarning(
                item.itemName +
                " has no child named AttackPoint. " +
                "Using the equipped prefab root temporarily."
            );
        }

        playerCombat.EquipWeapon(
            weaponAttackPoint,
            item.damage,
            item.attackRange
        );

        playerCombat.SetAttackCooldown(item.attackCooldown);
    }

    public void Unequip()
    {
        if (equippedObject != null)
        {
            Destroy(equippedObject);
        }

        equippedObject = null;
        equippedItem = null;
        equippedSlotIndex = -1;

        if (playerCombat != null)
        {
            playerCombat.enabled = true;
            playerCombat.EquipUnarmed();
            playerCombat.ResetAttackCooldown();
        }

        EquippedSlotChanged?.Invoke(-1);
    }

    public ItemData GetEquippedItem()
    {
        return equippedItem;
    }

    public GameObject GetEquippedObject()
    {
        return equippedObject;
    }

    public int GetEquippedSlotIndex()
    {
        return equippedSlotIndex;
    }

    public Animator GetAnimator()
    {
        return animator;
    }

    public void OnSlot1(InputValue value)
    {
        if (value.isPressed)
        {
            EquipSlot(0);
        }
    }

    public void OnSlot2(InputValue value)
    {
        if (value.isPressed)
        {
            EquipSlot(1);
        }
    }

    public void OnSlot3(InputValue value)
    {
        if (value.isPressed)
        {
            EquipSlot(2);
        }
    }

    public void OnSlot4(InputValue value)
    {
        if (value.isPressed)
        {
            EquipSlot(3);
        }
    }

    public void OnSlot5(InputValue value)
    {
        if (value.isPressed)
        {
            EquipSlot(4);
        }
    }

    public void OnSlot6(InputValue value)
    {
        if (value.isPressed)
        {
            EquipSlot(5);
        }
    }

    public void OnSlot7(InputValue value)
    {
        if (value.isPressed)
        {
            EquipSlot(6);
        }
    }

    public void OnSlot8(InputValue value)
    {
        if (value.isPressed)
        {
            EquipSlot(7);
        }
    }

    public void OnSlot9(InputValue value)
    {
        if (value.isPressed)
        {
            EquipSlot(8);
        }
    }

    public void OnSlot10(InputValue value)
    {
        if (value.isPressed)
        {
            EquipSlot(9);
        }
    }
}