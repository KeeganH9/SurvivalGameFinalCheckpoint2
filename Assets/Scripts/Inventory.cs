using UnityEngine;

public class Inventory : MonoBehaviour
{
    [Header("References")]
    public InventorySlotUI[] slotUIS;
    public PlayerEquipment playerEquipment;

    [Header("Inventory Data")]
    public InventorySlotData[] slots;

    private void Start()
    {
        slots = new InventorySlotData[slotUIS.Length];

        SetupSlotButtons();
        UpdateUI();
    }

    private void SetupSlotButtons()
    {
        if (playerEquipment == null)
        {
            playerEquipment = GetComponent<PlayerEquipment>();
        }

        for (int i = 0; i < slotUIS.Length; i++)
        {
            if (slotUIS[i] == null)
            {
                continue;
            }

            slotUIS[i].Setup(playerEquipment, i);
        }
    }

    public void AddItem(ItemData item, int amount)
    {
        if (item == null || amount <= 0)
        {
            return;
        }

        for (int i = 0; i < slots.Length; i++)
        {
            if (slots[i] == null)
            {
                continue;
            }

            if (slots[i].item == item && slots[i].amount < item.maxStackSize)
            {
                int spaceLeft = item.maxStackSize - slots[i].amount;
                int amountToAdd = Mathf.Min(spaceLeft, amount);

                slots[i].amount += amountToAdd;
                amount -= amountToAdd;

                if (amount <= 0)
                {
                    UpdateUI();
                    return;
                }
            }
        }

        for (int i = 0; i < slots.Length; i++)
        {
            if (slots[i] != null)
            {
                continue;
            }

            int amountToAdd = Mathf.Min(item.maxStackSize, amount);

            slots[i] = new InventorySlotData(item, amountToAdd);
            amount -= amountToAdd;

            if (amount <= 0)
            {
                UpdateUI();
                return;
            }
        }

        UpdateUI();
    }

    public int GetItemAmount(ItemData item)
    {
        if (item == null)
        {
            return 0;
        }

        int totalAmount = 0;

        for (int i = 0; i < slots.Length; i++)
        {
            if (slots[i] != null && slots[i].item == item)
            {
                totalAmount += slots[i].amount;
            }
        }

        return totalAmount;
    }

    public bool HasItem(ItemData item, int amount)
    {
        if (item == null || amount <= 0)
        {
            return false;
        }

        return GetItemAmount(item) >= amount;
    }

    public bool RemoveItem(ItemData item, int amount)
    {
        if (item == null || amount <= 0)
        {
            return false;
        }

        if (!HasItem(item, amount))
        {
            return false;
        }

        int amountRemaining = amount;

        for (int i = 0; i < slots.Length; i++)
        {
            if (slots[i] == null || slots[i].item != item)
            {
                continue;
            }

            int amountToRemove = Mathf.Min(
                slots[i].amount,
                amountRemaining
            );

            slots[i].amount -= amountToRemove;
            amountRemaining -= amountToRemove;

            if (slots[i].amount <= 0)
            {
                slots[i] = null;
            }

            if (amountRemaining <= 0)
            {
                UpdateUI();
                return true;
            }
        }

        UpdateUI();
        return true;
    }

    private void UpdateUI()
    {
        for (int i = 0; i < slotUIS.Length; i++)
        {
            if (slotUIS[i] == null)
            {
                continue;
            }

            if (slots[i] == null)
            {
                slotUIS[i].ClearSlot();
            }
            else
            {
                slotUIS[i].SetSlot(
                    slots[i].item,
                    slots[i].amount
                );
            }
        }
    }
}