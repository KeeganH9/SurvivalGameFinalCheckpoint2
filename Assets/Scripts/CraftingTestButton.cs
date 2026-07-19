using UnityEngine;

public class CraftingTestButton : MonoBehaviour
{
    public CraftingManager craftingManager;
    public CraftingRecipe recipe;

    public void TestCraft()
    {
        craftingManager.Craft(recipe);
    }
}