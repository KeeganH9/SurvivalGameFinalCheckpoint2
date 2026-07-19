using TMPro;
using UnityEngine;

public class RecipeButtonUI : MonoBehaviour
{
    [Header("UI")]
    public TextMeshProUGUI recipeNameText;

    private CraftingRecipe recipe;
    private CraftingMenuUI craftingMenu;

    public void Setup(
        CraftingRecipe newRecipe,
        CraftingMenuUI newCraftingMenu)
    {
        recipe = newRecipe;
        craftingMenu = newCraftingMenu;

        if (recipeNameText != null && recipe != null)
        {
            recipeNameText.text = recipe.recipeName;
        }
    }

    public void SelectRecipe()
    {
        if (craftingMenu == null || recipe == null)
        {
            Debug.LogWarning("Recipe button is missing its recipe or crafting menu.");
            return;
        }

        craftingMenu.SelectRecipe(recipe);
    }
}