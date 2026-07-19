using UnityEngine;

public class CraftingManager : MonoBehaviour
{
    public Inventory inventory;

    public bool Craft(CraftingRecipe recipe)
    {
        if (recipe == null)
            return false;

        // Check all ingredients
        foreach (RecipeIngredient ingredient in recipe.ingredients)
        {
            if (!inventory.HasItem(ingredient.item, ingredient.amount))
            {
                Debug.Log("Missing: " + ingredient.item.itemName);
                return false;
            }
        }

        // Remove ingredients
        foreach (RecipeIngredient ingredient in recipe.ingredients)
        {
            inventory.RemoveItem(ingredient.item, ingredient.amount);
        }

        // Give crafted item
        inventory.AddItem(recipe.result, recipe.resultAmount);

        Debug.Log("Crafted: " + recipe.result.itemName);

        return true;
    }
}