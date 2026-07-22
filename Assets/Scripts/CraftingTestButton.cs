using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CraftingTestButton : MonoBehaviour
{
    [Header("Crafting")]
    public CraftingManager craftingManager;
    public CraftingRecipe recipe;

    [Header("Button UI")]
    public Button button;
    public TextMeshProUGUI buttonText;

    private void Awake()
    {
        if (button == null)
        {
            button = GetComponent<Button>();
        }
    }

    private void OnEnable()
    {
        RefreshButton();
    }

    public void TestCraft()
    {
        if (craftingManager == null || recipe == null)
        {
            Debug.LogWarning(
                "CraftingTestButton is missing its CraftingManager or Recipe."
            );

            return;
        }

        Inventory inventory = craftingManager.inventory;

        if (inventory == null || recipe.result == null)
        {
            Debug.LogWarning(
                "The CraftingManager needs an Inventory and the recipe needs a result."
            );

            return;
        }

        InventorySlotData itemSlot =
            inventory.FindSlotWithItem(recipe.result);

        bool succeeded;

        if (itemSlot == null)
        {
            succeeded = craftingManager.Craft(recipe);
        }
        else if (recipe.result.canUpgrade)
        {
            succeeded = craftingManager.Upgrade(recipe);
        }
        else
        {
            succeeded = craftingManager.Craft(recipe);
        }

        if (succeeded)
        {
            RefreshButton();
        }
    }

    public void RefreshButton()
    {
        if (buttonText == null ||
            craftingManager == null ||
            craftingManager.inventory == null ||
            recipe == null ||
            recipe.result == null)
        {
            return;
        }

        InventorySlotData itemSlot =
            craftingManager.inventory.FindSlotWithItem(recipe.result);

        if (itemSlot == null)
        {
            buttonText.text = "Craft";

            if (button != null)
            {
                button.interactable = true;
            }

            return;
        }

        if (!recipe.result.canUpgrade)
        {
            buttonText.text = "Craft";

            if (button != null)
            {
                button.interactable = true;
            }

            return;
        }

        if (itemSlot.upgradeLevel >=
            recipe.result.maximumUpgradeLevel)
        {
            buttonText.text = "Max Level";

            if (button != null)
            {
                button.interactable = false;
            }

            return;
        }

        buttonText.text =
            "Upgrade (Level " + itemSlot.upgradeLevel + ")";

        if (button != null)
        {
            button.interactable = true;
        }
    }
}