using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CraftingMenuUI : MonoBehaviour
{
    [Header("Recipes")]
    public CraftingRecipe[] recipes;

    [Header("Main References")]
    public CraftingManager craftingManager;
    public Transform recipeList;
    public RecipeButtonUI recipeButtonPrefab;

    [Header("Selected Recipe UI")]
    public Image recipeIcon;
    public TextMeshProUGUI recipeName;
    public TextMeshProUGUI description;

    [Header("Ingredient UI")]
    public Transform ingredientList;
    public IngredientRowUI ingredientRowPrefab;

    private CraftingRecipe selectedRecipe;

    private void Start()
    {
        GenerateRecipeButtons();
        ClearSelectedRecipeUI();
    }

    private void GenerateRecipeButtons()
    {
        if (recipeList == null)
        {
            Debug.LogWarning("CraftingMenuUI has no Recipe List assigned.");
            return;
        }

        if (recipeButtonPrefab == null)
        {
            Debug.LogWarning("CraftingMenuUI has no Recipe Button Prefab assigned.");
            return;
        }

        for (int i = recipeList.childCount - 1; i >= 0; i--)
        {
            Destroy(recipeList.GetChild(i).gameObject);
        }

        if (recipes == null)
        {
            return;
        }

        foreach (CraftingRecipe recipe in recipes)
        {
            if (recipe == null)
            {
                continue;
            }

            RecipeButtonUI newButton = Instantiate(
                recipeButtonPrefab,
                recipeList
            );

            newButton.Setup(recipe, this);
        }
    }

    public void SelectRecipe(CraftingRecipe recipe)
    {
        if (recipe == null)
        {
            return;
        }

        selectedRecipe = recipe;
        UpdateSelectedRecipeUI();

        Debug.Log("Selected recipe: " + selectedRecipe.recipeName);
    }

    private void UpdateSelectedRecipeUI()
    {
        if (selectedRecipe == null)
        {
            ClearSelectedRecipeUI();
            return;
        }

        if (recipeName != null)
        {
            recipeName.text = selectedRecipe.recipeName;
        }

        if (description != null)
        {
            description.text = selectedRecipe.description;
        }

        if (recipeIcon != null)
        {
            recipeIcon.sprite = selectedRecipe.recipeIcon;
            recipeIcon.gameObject.SetActive(
                selectedRecipe.recipeIcon != null
            );
        }

        GenerateIngredientRows();
    }

    private void GenerateIngredientRows()
    {
        if (ingredientList == null)
        {
            Debug.LogWarning("CraftingMenuUI has no Ingredient List assigned.");
            return;
        }

        if (ingredientRowPrefab == null)
        {
            Debug.LogWarning("CraftingMenuUI has no Ingredient Row Prefab assigned.");
            return;
        }

        for (int i = ingredientList.childCount - 1; i >= 0; i--)
        {
            Destroy(ingredientList.GetChild(i).gameObject);
        }

        if (selectedRecipe == null ||
            selectedRecipe.ingredients == null)
        {
            return;
        }

        Inventory inventory = null;

        if (craftingManager != null)
        {
            inventory = craftingManager.inventory;
        }

        foreach (RecipeIngredient ingredient in selectedRecipe.ingredients)
        {
            if (ingredient == null || ingredient.item == null)
            {
                continue;
            }

            IngredientRowUI newRow = Instantiate(
                ingredientRowPrefab,
                ingredientList
            );

            newRow.Setup(ingredient, inventory);
        }
    }

    private void ClearSelectedRecipeUI()
    {
        selectedRecipe = null;

        if (recipeName != null)
        {
            recipeName.text = "Select a Recipe";
        }

        if (description != null)
        {
            description.text = "";
        }

        if (recipeIcon != null)
        {
            recipeIcon.sprite = null;
            recipeIcon.gameObject.SetActive(false);
        }

        if (ingredientList != null)
        {
            for (int i = ingredientList.childCount - 1; i >= 0; i--)
            {
                Destroy(ingredientList.GetChild(i).gameObject);
            }
        }
    }

    public void CraftSelectedRecipe()
    {
        if (selectedRecipe == null)
        {
            Debug.LogWarning("No crafting recipe is selected.");
            return;
        }

        if (craftingManager == null)
        {
            Debug.LogWarning(
                "CraftingMenuUI has no CraftingManager assigned."
            );
            return;
        }

        bool craftedSuccessfully =
            craftingManager.Craft(selectedRecipe);

        if (craftedSuccessfully)
        {
            UpdateSelectedRecipeUI();
        }
    }

    public void CraftRecipe(int recipeIndex)
    {
        if (craftingManager == null)
        {
            Debug.LogWarning(
                "CraftingMenuUI has no CraftingManager assigned."
            );
            return;
        }

        if (recipes == null ||
            recipeIndex < 0 ||
            recipeIndex >= recipes.Length)
        {
            Debug.LogWarning("Invalid crafting recipe index.");
            return;
        }

        craftingManager.Craft(recipes[recipeIndex]);
    }
}