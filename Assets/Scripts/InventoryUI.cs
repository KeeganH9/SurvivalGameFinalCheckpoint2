using UnityEngine;

public class InventoryUI : MonoBehaviour
{
    [Header("Menu Panels")]
    public GameObject inventoryPanel;

    [Header("Other Menus")]
    public CraftingMenuUI craftingMenu;

    public static bool IsAnyMenuOpen { get; set; }

    public bool IsOpen { get; private set; }

    private void Start()
    {
        if (inventoryPanel != null)
        {
            inventoryPanel.SetActive(false);
        }

        IsOpen = false;
        UpdateMenuState();
        LockCursor();
    }


    public void ToggleInventory()
    {
        if (IsOpen)
        {
            CloseInventory();
        }
        else
        {
            OpenInventory();
        }
    }

   public void OpenInventory()
{
    if (craftingMenu == null)
    {
        craftingMenu = GetComponent<CraftingMenuUI>();
    }

    if (craftingMenu != null)
    {
        craftingMenu.CloseCraftingMenu();

        // Forces the crafting panel closed if its state became unsynchronized.
        if (craftingMenu.craftingPanel != null)
        {
            craftingMenu.craftingPanel.SetActive(false);
        }
    }

    IsOpen = true;

    if (inventoryPanel != null)
    {
        inventoryPanel.SetActive(true);
    }

    UpdateMenuState();
    UnlockCursor();
}

    public void CloseInventory()
    {
        IsOpen = false;

        if (inventoryPanel != null)
        {
            inventoryPanel.SetActive(false);
        }

        UpdateMenuState();

        if (!IsAnyMenuOpen && !PauseMenuUI.IsPaused)
        {
            LockCursor();
        }
    }

    public void CloseForPauseMenu()
    {
        CloseInventory();
    }

    private void UpdateMenuState()
    {
        bool craftingOpen =
            craftingMenu != null && craftingMenu.IsOpen;

        IsAnyMenuOpen = IsOpen || craftingOpen;
    }

    private void UnlockCursor()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    private void LockCursor()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }
}