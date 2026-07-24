using UnityEngine;
using UnityEngine.InputSystem;

public class GameMenuController : MonoBehaviour
{
    [Header("Menu Panels")]
    public GameObject inventoryPanel;
    public GameObject craftingPanel;

    private void Start()
    {
        CloseAllMenus();
    }

    private void Update()
    {
        if (PauseMenuUI.IsPaused || Keyboard.current == null)
        {
            return;
        }

        if (Keyboard.current.iKey.wasPressedThisFrame)
        {
            ToggleInventory();
        }

        if (Keyboard.current.cKey.wasPressedThisFrame)
        {
            ToggleCrafting();
        }
    }

    private void ToggleInventory()
    {
        bool shouldOpen = !inventoryPanel.activeSelf;

        craftingPanel.SetActive(false);
        inventoryPanel.SetActive(shouldOpen);

        UpdateMenuState();
    }

    private void ToggleCrafting()
    {
        bool shouldOpen = !craftingPanel.activeSelf;

        inventoryPanel.SetActive(false);
        craftingPanel.SetActive(shouldOpen);

        UpdateMenuState();
    }

    public void CloseAllMenus()
    {
        inventoryPanel.SetActive(false);
        craftingPanel.SetActive(false);

        UpdateMenuState();
    }

    private void UpdateMenuState()
    {
        bool menuOpen =
            inventoryPanel.activeSelf ||
            craftingPanel.activeSelf;

        InventoryUI.IsAnyMenuOpen = menuOpen;

        Cursor.visible = menuOpen;
        Cursor.lockState = menuOpen
            ? CursorLockMode.None
            : CursorLockMode.Locked;
    }
}