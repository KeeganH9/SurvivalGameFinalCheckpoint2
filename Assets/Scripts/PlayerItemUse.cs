using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerItemUse : MonoBehaviour
{
    [Header("References")]
    public PlayerEquipment playerEquipment;
    public PlayerHealth playerHealth;
    public Inventory inventory;

    private Animator animator;
    private bool isUsingItem;

    private void Awake()
    {
        if (playerEquipment == null)
        {
            playerEquipment = GetComponent<PlayerEquipment>();
        }

        if (playerHealth == null)
        {
            playerHealth = GetComponent<PlayerHealth>();
        }

        // Your Inventory is referenced through PlayerEquipment/GameManager.
        if (inventory == null &&
            playerEquipment != null)
        {
            inventory = playerEquipment.inventory;
        }

        animator = GetComponentInChildren<Animator>();

        if (playerEquipment == null)
        {
            Debug.LogWarning(
                "PlayerItemUse could not find PlayerEquipment."
            );
        }

        if (playerHealth == null)
        {
            Debug.LogWarning(
                "PlayerItemUse could not find PlayerHealth."
            );
        }

        if (inventory == null)
        {
            Debug.LogWarning(
                "PlayerItemUse does not have an Inventory reference."
            );
        }

        if (animator == null)
        {
            Debug.LogWarning(
                "PlayerItemUse could not find the player Animator."
            );
        }
    }

    public void OnUseItem(InputValue value)
    {
        if (!value.isPressed)
        {
            return;
        }

        if (InventoryUI.IsAnyMenuOpen || isUsingItem)
        {
            return;
        }

        TryUseEquippedItem();
    }

    private void TryUseEquippedItem()
    {
        if (playerEquipment == null)
        {
            return;
        }

        ItemData item = playerEquipment.GetEquippedItem();

        if (item == null)
        {
            Debug.Log("No item is equipped.");
            return;
        }

        if (item.itemType != ItemType.Consumable)
        {
            Debug.Log(item.itemName + " is not a consumable.");
            return;
        }

        StartCoroutine(UseConsumable(item));
    }

    private IEnumerator UseConsumable(ItemData item)
    {
        if (playerHealth == null || inventory == null)
        {
            Debug.LogWarning(
                "Cannot use consumable because a required reference is missing."
            );
            yield break;
        }

        if (playerHealth.IsAtMaximumHealth())
        {
            Debug.Log("Player is already at maximum health.");
            yield break;
        }

        isUsingItem = true;

        int equippedSlot =
            playerEquipment.GetEquippedSlotIndex();

        if (animator != null &&
            !string.IsNullOrEmpty(item.useAnimationTrigger))
        {
            animator.SetTrigger(item.useAnimationTrigger);
        }

        yield return new WaitForSeconds(item.useDelay);

        playerHealth.Heal(item.healAmount);

        bool removedSuccessfully =
            inventory.RemoveItem(item, 1);

        if (!removedSuccessfully)
        {
            Debug.LogWarning(
                "Failed to remove the used consumable."
            );

            isUsingItem = false;
            yield break;
        }

        Debug.Log(
            "Used " +
            item.itemName +
            " and healed " +
            item.healAmount +
            " health."
        );

        isUsingItem = false;

        if (inventory.slots == null ||
            equippedSlot < 0 ||
            equippedSlot >= inventory.slots.Length ||
            inventory.slots[equippedSlot] == null ||
            inventory.slots[equippedSlot].item != item)
        {
            playerEquipment.Unequip();
        }
    }
}