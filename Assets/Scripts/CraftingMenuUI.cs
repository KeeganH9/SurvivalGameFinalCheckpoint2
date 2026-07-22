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

    [Header("Shared Craft Button")]
    public Button craftButton;
    public TextMeshProUGUI craftButtonText;

    private CraftingRecipe selectedRecipe;

    private void Start()
    {
        GenerateRecipeButtons();
        ClearSelectedRecipeUI();
    }

    private void OnEnable()
    {
        RefreshSelectedRecipeUI();
    }

    private void GenerateRecipeButtons()
    {
        if (recipeList == null)
        {
            Debug.LogWarning(
                "CraftingMenuUI has no Recipe List assigned."
            );
            return;
        }

        if (recipeButtonPrefab == null)
        {
            Debug.LogWarning(
                "CraftingMenuUI has no Recipe Button Prefab assigned."
            );
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

    private void RefreshSelectedRecipeUI()
    {
        if (selectedRecipe != null)
        {
            UpdateSelectedRecipeUI();
        }
        else
        {
            UpdateCraftButton();
        }
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
        UpdateCraftButton();
    }

    private bool ShouldUpgradeSelectedRecipe()
    {
        if (selectedRecipe == null ||
            selectedRecipe.result == null ||
            !selectedRecipe.result.canUpgrade ||
            craftingManager == null ||
            craftingManager.inventory == null)
        {
            return false;
        }

        InventorySlotData itemSlot =
            craftingManager.inventory.FindSlotWithItem(
                selectedRecipe.result
            );

        return itemSlot != null;
    }

    private void GenerateIngredientRows()
    {
        if (ingredientList == null)
        {
            Debug.LogWarning(
                "CraftingMenuUI has no Ingredient List assigned."
            );
            return;
        }

        if (ingredientRowPrefab == null)
        {
            Debug.LogWarning(
                "CraftingMenuUI has no Ingredient Row Prefab assigned."
            );
            return;
        }

        for (int i = ingredientList.childCount - 1; i >= 0; i--)
        {
            Destroy(ingredientList.GetChild(i).gameObject);
        }

        if (selectedRecipe == null)
        {
            return;
        }

        RecipeIngredient[] displayedIngredients;

        if (ShouldUpgradeSelectedRecipe())
        {
            displayedIngredients = selectedRecipe.upgradeIngredients;
        }
        else
        {
            displayedIngredients = selectedRecipe.ingredients;
        }

        if (displayedIngredients == null)
        {
            return;
        }

        Inventory inventory = null;

        if (craftingManager != null)
        {
            inventory = craftingManager.inventory;
        }

        foreach (RecipeIngredient ingredient in displayedIngredients)
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

    private void UpdateCraftButton()
    {
        if (craftButton == null || craftButtonText == null)
        {
            return;
        }

        if (selectedRecipe == null ||
            selectedRecipe.result == null)
        {
            craftButtonText.text = "Select a Recipe";
            craftButton.interactable = false;
            return;
        }

        if (!ShouldUpgradeSelectedRecipe())
        {
            craftButtonText.text = "Craft";
            craftButton.interactable = true;
            return;
        }

        InventorySlotData itemSlot =
            craftingManager.inventory.FindSlotWithItem(
                selectedRecipe.result
            );

        if (itemSlot.upgradeLevel >=
            selectedRecipe.result.maximumUpgradeLevel)
        {
            craftButtonText.text = "Max Level";
            craftButton.interactable = false;
            return;
        }

        craftButtonText.text =
            "Upgrade to Level " + (itemSlot.upgradeLevel + 2);

        craftButton.interactable = true;
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

        UpdateCraftButton();
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

        bool succeeded;

        if (ShouldUpgradeSelectedRecipe())
        {
            succeeded = craftingManager.Upgrade(selectedRecipe);
        }
        else
        {
            succeeded = craftingManager.Craft(selectedRecipe);
        }

        if (succeeded)
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