using System;
using UnityEngine;

[Serializable]
public class InventorySlotData
{
    public ItemData item;
    public int amount;

    [Header("Upgrade Data")]
    public int upgradeLevel;

    public InventorySlotData(ItemData item, int amount)
    {
        this.item = item;
        this.amount = amount;
        upgradeLevel = 0;
    }
}