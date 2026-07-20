using UnityEngine;

public class InteractableResource : MonoBehaviour
{
    [Header("Resource")]
    public ItemData item;
    public int amountPerCollect = 1;
    public int usesRemaining = 1;
    public bool destroyWhenEmpty = true;

    [Header("Interaction")]
    public string promptText = "Press E to collect";
    public string animationTrigger = "PickFruit";

    [Header("Required Tool")]
    public bool requiresTool;
    public ToolType requiredTool = ToolType.None;

    public void Interact(Inventory inventory)
    {
        Interact(inventory, null);
    }

    public void Interact(
        Inventory inventory,
        PlayerEquipment playerEquipment
    )
    {
        if (usesRemaining <= 0 || inventory == null)
        {
            return;
        }

        if (requiresTool)
        {
            if (playerEquipment == null)
            {
                Debug.LogWarning(
                    "PlayerEquipment was not provided to the resource."
                );
                return;
            }

            ItemData equippedItem =
                playerEquipment.GetEquippedItem();

            if (equippedItem == null ||
                equippedItem.toolType != requiredTool)
            {
                Debug.Log(
                    "You need an equipped " +
                    requiredTool +
                    " to collect this resource."
                );
                return;
            }
        }

        if (item != null)
        {
            inventory.AddItem(item, amountPerCollect);
        }

        usesRemaining--;

        if (usesRemaining <= 0 && destroyWhenEmpty)
        {
            gameObject.SetActive(false);
        }
    }
}