using UnityEngine;
using UnityEngine.InputSystem;

public class CraftingUI : MonoBehaviour
{
    public GameObject craftingPanel;

    private bool isOpen;

    private void Start()
    {
        if (craftingPanel != null)
        {
            craftingPanel.SetActive(false);
        }
    }

    public void OnCrafting(InputValue value)
    {
        if (!value.isPressed)
        {
            return;
        }

        ToggleCrafting();
    }

    private void ToggleCrafting()
    {
        isOpen = !isOpen;

        if (craftingPanel != null)
        {
            craftingPanel.SetActive(isOpen);
        }

        InventoryUI.IsAnyMenuOpen = isOpen;

        if (isOpen)
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
        else
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
    }
}