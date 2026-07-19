using UnityEngine;

public enum ItemType
{
    Resource,
    Tool,
    Weapon,
    Shield,
    Consumable
}

[CreateAssetMenu(fileName = "New Item", menuName = "Inventory/Item")]
public class ItemData : ScriptableObject
{
    [Header("Basic Item Info")]
    public string itemName;
    public Sprite icon;
    public ItemType itemType;
    public int maxStackSize = 15;

    [Header("Equipping")]
    public bool canEquip;
    public GameObject equippedPrefab;

    [Header("Combat Stats")]
    public int damage = 10;
    public float attackCooldown = 0.8f;
    public float attackRange = 1.5f;

    [Header("Consumable Settings")]
    public int healAmount;
    public float useDelay = 1f;
    public string useAnimationTrigger = "Drink";

    [Header("Future Upgrade Settings")]
    public bool canUpgrade;
    public int maximumUpgradeLevel = 3;
}