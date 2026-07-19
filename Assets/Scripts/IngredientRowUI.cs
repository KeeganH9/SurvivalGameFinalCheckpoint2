using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class IngredientRowUI : MonoBehaviour
{
    [Header("UI")]
    public Image ingredientIcon;
    public TextMeshProUGUI ingredientName;
    public TextMeshProUGUI ingredientAmount;

    public void Setup(
        RecipeIngredient ingredient,
        Inventory inventory)
    {
        if (ingredient == null || ingredient.item == null)
        {
            gameObject.SetActive(false);
            return;
        }

        int ownedAmount = 0;

        if (inventory != null)
        {
            ownedAmount = inventory.GetItemAmount(ingredient.item);
        }

        if (ingredientIcon != null)
        {
            ingredientIcon.sprite = ingredient.item.icon;
            ingredientIcon.gameObject.SetActive(
                ingredient.item.icon != null
            );
        }

        if (ingredientName != null)
        {
            ingredientName.text = ingredient.item.itemName;
        }

        if (ingredientAmount != null)
        {
            ingredientAmount.text =
                ownedAmount + " / " + ingredient.amount;

            ingredientAmount.color =
                ownedAmount >= ingredient.amount
                    ? Color.green
                    : Color.red;
        }
    }
}