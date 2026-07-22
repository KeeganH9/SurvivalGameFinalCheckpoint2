using UnityEngine;

[System.Serializable]
public class RecipeIngredient
{
    public ItemData item;
    public int amount;
}

[CreateAssetMenu(
    fileName = "New Recipe",
    menuName = "Crafting/Recipe"
)]
public class CraftingRecipe : ScriptableObject
{
    [Header("Recipe Info")]
    public string recipeName;

    [TextArea]
    public string description;

    public Sprite recipeIcon;

    [Header("Crafting Ingredients")]
    public RecipeIngredient[] ingredients;

    [Header("Crafted Item")]
    public ItemData result;
    public int resultAmount = 1;

    [Header("Upgrade Ingredients")]
    public RecipeIngredient[] upgradeIngredients;

    [Header("Stats Gained Per Upgrade")]
    public int damageIncreasePerLevel = 5;
    public float rangeIncreasePerLevel = 0.25f;
}