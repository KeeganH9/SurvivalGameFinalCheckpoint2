using UnityEngine;

public class CraftingManager : MonoBehaviour
{
    [Header("References")]
    public Inventory inventory;
    public PlayerEquipment playerEquipment;

    public bool Craft(CraftingRecipe recipe)
    {
        if (recipe == null ||
            recipe.result == null ||
            inventory == null)
        {
            return false;
        }

        // Check that the player has every crafting ingredient.
        if (recipe.ingredients != null)
        {
            foreach (RecipeIngredient ingredient in recipe.ingredients)
            {
                if (ingredient == null ||
                    ingredient.item == null ||
                    ingredient.amount <= 0)
                {
                    continue;
                }

                if (!inventory.HasItem(
                    ingredient.item,
                    ingredient.amount))
                {
                    Debug.Log(
                        "Missing: " + ingredient.item.itemName
                    );

                    return false;
                }
            }

            // Remove the crafting ingredients.
            foreach (RecipeIngredient ingredient in recipe.ingredients)
            {
                if (ingredient == null ||
                    ingredient.item == null ||
                    ingredient.amount <= 0)
                {
                    continue;
                }

                inventory.RemoveItem(
                    ingredient.item,
                    ingredient.amount
                );
            }
        }

        // Give the crafted item to the player.
        inventory.AddItem(
            recipe.result,
            recipe.resultAmount
        );

        Debug.Log("Crafted: " + recipe.result.itemName);

        return true;
    }

    public bool Upgrade(CraftingRecipe recipe)
    {
        if (recipe == null ||
            recipe.result == null ||
            inventory == null)
        {
            return false;
        }

        ItemData item = recipe.result;

        if (!item.canUpgrade)
        {
            Debug.Log(item.itemName + " cannot be upgraded.");
            return false;
        }

        InventorySlotData itemSlot =
            inventory.FindSlotWithItem(item);

        if (itemSlot == null)
        {
            Debug.Log(
                "You must craft " + item.itemName +
                " before upgrading it."
            );

            return false;
        }

        if (itemSlot.upgradeLevel >= item.maximumUpgradeLevel)
        {
            Debug.Log(
                item.itemName +
                " is already at its maximum upgrade level."
            );

            return false;
        }

        // Check that the player has every upgrade ingredient.
        if (recipe.upgradeIngredients != null)
        {
            foreach (RecipeIngredient ingredient
                     in recipe.upgradeIngredients)
            {
                if (ingredient == null ||
                    ingredient.item == null ||
                    ingredient.amount <= 0)
                {
                    continue;
                }

                if (!inventory.HasItem(
                    ingredient.item,
                    ingredient.amount))
                {
                    Debug.Log(
                        "Missing upgrade material: " +
                        ingredient.item.itemName
                    );

                    return false;
                }
            }

            // Remove the upgrade ingredients.
            foreach (RecipeIngredient ingredient
                     in recipe.upgradeIngredients)
            {
                if (ingredient == null ||
                    ingredient.item == null ||
                    ingredient.amount <= 0)
                {
                    continue;
                }

                inventory.RemoveItem(
                    ingredient.item,
                    ingredient.amount
                );
            }
        }

        itemSlot.upgradeLevel++;

        inventory.RefreshUI();

        // Immediately update its combat stats if it is equipped.
        if (playerEquipment != null)
        {
            playerEquipment.RefreshEquippedWeaponStats();
        }

        int upgradedDamage =
            item.damage +
            (recipe.damageIncreasePerLevel *
             itemSlot.upgradeLevel);

        float upgradedRange =
            item.attackRange +
            (recipe.rangeIncreasePerLevel *
             itemSlot.upgradeLevel);

        Debug.Log(
            item.itemName +
            " upgraded to level " +
            itemSlot.upgradeLevel +
            ". Damage: " + upgradedDamage +
            ", Range: " + upgradedRange
        );

        return true;
    }
}