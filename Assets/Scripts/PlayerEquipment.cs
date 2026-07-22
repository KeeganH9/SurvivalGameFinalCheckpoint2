using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerEquipment : MonoBehaviour
{
    [Header("References")]
    public Inventory inventory;
    public Transform handPoint;
    public Transform handPointL;
    public PlayerCombat playerCombat;
    public PlayerHealth playerHealth;
    public CraftingRecipe[] craftingRecipes;

    [Header("Equipped Items")]
    [SerializeField] private ItemData equippedItem;
    [SerializeField] private ItemData equippedShield;

    private GameObject equippedObject;
    private GameObject equippedShieldObject;

    private int equippedSlotIndex = -1;
    private int equippedShieldSlotIndex = -1;

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

        if (inventory.slots == null ||
            slotIndex < 0 ||
            slotIndex >= inventory.slots.Length)
        {
            return;
        }

        InventorySlotData slot = inventory.slots[slotIndex];

        if (slot == null || slot.item == null)
        {
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

        if (item.itemType == ItemType.Shield)
        {
            EquipShield(item, slotIndex);
        }
        else
        {
            EquipRightHandItem(item, slotIndex);
        }
    }

    private void EquipRightHandItem(ItemData item, int slotIndex)
    {
        if (slotIndex == equippedSlotIndex)
        {
            UnequipRightHand();
            Debug.Log("Unequipped right-hand item.");
            return;
        }

        if (handPoint == null)
        {
            Debug.LogWarning(
                "PlayerEquipment does not have the right hand point assigned."
            );
            return;
        }

        UnequipRightHand();

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

        EquippedSlotChanged?.Invoke(slotIndex);

        Debug.Log("Equipped right-hand item: " + item.itemName);
    }

    private void EquipShield(ItemData item, int slotIndex)
    {
        if (slotIndex == equippedShieldSlotIndex)
        {
            UnequipShield();
            Debug.Log("Unequipped shield.");
            return;
        }

        if (handPointL == null)
        {
            Debug.LogWarning(
                "PlayerEquipment does not have the left hand point assigned."
            );
            return;
        }

        UnequipShield();

        equippedShieldObject = Instantiate(
            item.equippedPrefab,
            handPointL
        );

        equippedShieldObject.transform.localPosition = Vector3.zero;
        equippedShieldObject.transform.localRotation = Quaternion.identity;

        equippedShield = item;
        equippedShieldSlotIndex = slotIndex;

        EquippedSlotChanged?.Invoke(slotIndex);

        Debug.Log("Equipped shield: " + item.itemName);
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

        if (equippedObject == null)
        {
            Debug.LogWarning(
                "PlayerEquipment has no equipped object to connect to combat."
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

        int upgradedDamage = item.damage;
        float upgradedRange = item.attackRange;
        int upgradeLevel = 0;

        if (inventory != null &&
            inventory.slots != null &&
            equippedSlotIndex >= 0 &&
            equippedSlotIndex < inventory.slots.Length)
        {
            InventorySlotData equippedSlot =
                inventory.slots[equippedSlotIndex];

            if (equippedSlot != null &&
                equippedSlot.item == item)
            {
                upgradeLevel = equippedSlot.upgradeLevel;
            }
        }

        CraftingRecipe upgradeRecipe = FindRecipeForItem(item);

        if (upgradeRecipe != null && upgradeLevel > 0)
        {
            upgradedDamage +=
                upgradeRecipe.damageIncreasePerLevel *
                upgradeLevel;

            upgradedRange +=
                upgradeRecipe.rangeIncreasePerLevel *
                upgradeLevel;
        }
        else if (upgradeLevel > 0)
        {
            Debug.LogWarning(
                "No crafting recipe was found for " +
                item.itemName +
                ". Its upgraded combat stats could not be calculated."
            );
        }

        playerCombat.EquipWeapon(
            weaponAttackPoint,
            upgradedDamage,
            upgradedRange
        );

        playerCombat.SetAttackCooldown(item.attackCooldown);

        Debug.Log(
            "Equipped " + item.itemName +
            " at upgrade level " + upgradeLevel +
            ". Damage: " + upgradedDamage +
            ", Range: " + upgradedRange
        );
    }

    private CraftingRecipe FindRecipeForItem(ItemData item)
    {
        if (item == null || craftingRecipes == null)
        {
            return null;
        }

        foreach (CraftingRecipe recipe in craftingRecipes)
        {
            if (recipe != null && recipe.result == item)
            {
                return recipe;
            }
        }

        return null;
    }

    public void RefreshEquippedWeaponStats()
    {
        if (equippedItem == null ||
            equippedObject == null ||
            equippedSlotIndex < 0)
        {
            return;
        }

        ConnectItemToCombat(equippedItem);
    }

    public void UnequipRightHand()
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

    public void UnequipShield()
    {
        if (equippedShieldObject != null)
        {
            Destroy(equippedShieldObject);
        }

        equippedShieldObject = null;
        equippedShield = null;
        equippedShieldSlotIndex = -1;

        EquippedSlotChanged?.Invoke(-1);
    }

    public void Unequip()
    {
        UnequipRightHand();
        UnequipShield();
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

    public ItemData GetEquippedShield()
    {
        return equippedShield;
    }

    public GameObject GetEquippedShieldObject()
    {
        return equippedShieldObject;
    }

    public int GetEquippedShieldSlotIndex()
    {
        return equippedShieldSlotIndex;
    }

    public bool HasShieldEquipped()
    {
        return equippedShield != null &&
               equippedShieldObject != null;
    }

    public Animator GetAnimator()
    {
        return animator;
    }

    public void OnSlot1(InputValue value)
    {
        if (value.isPressed) EquipSlot(0);
    }

    public void OnSlot2(InputValue value)
    {
        if (value.isPressed) EquipSlot(1);
    }

    public void OnSlot3(InputValue value)
    {
        if (value.isPressed) EquipSlot(2);
    }

    public void OnSlot4(InputValue value)
    {
        if (value.isPressed) EquipSlot(3);
    }

    public void OnSlot5(InputValue value)
    {
        if (value.isPressed) EquipSlot(4);
    }

    public void OnSlot6(InputValue value)
    {
        if (value.isPressed) EquipSlot(5);
    }

    public void OnSlot7(InputValue value)
    {
        if (value.isPressed) EquipSlot(6);
    }

    public void OnSlot8(InputValue value)
    {
        if (value.isPressed) EquipSlot(7);
    }

    public void OnSlot9(InputValue value)
    {
        if (value.isPressed) EquipSlot(8);
    }

    public void OnSlot10(InputValue value)
    {
        if (value.isPressed) EquipSlot(9);
    }
}