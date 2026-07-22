using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;

public class InventorySlotUI :
    MonoBehaviour,
    IPointerClickHandler
{
    [Header("UI References")]
    public Image itemIcon;
    public TextMeshProUGUI stackText;
    public Image equippedHighlight;

    private PlayerEquipment playerEquipment;
    private int slotIndex = -1;

    public void Setup(
        PlayerEquipment equipment,
        int index)
    {
        if (playerEquipment != null)
        {
            playerEquipment.EquippedSlotChanged -=
                UpdateEquippedHighlight;
        }

        playerEquipment = equipment;
        slotIndex = index;

        if (playerEquipment != null)
        {
            playerEquipment.EquippedSlotChanged +=
                UpdateEquippedHighlight;

            UpdateEquippedHighlight(
                playerEquipment.GetEquippedSlotIndex()
            );
        }
        else
        {
            UpdateEquippedHighlight(-1);
        }
    }

    public void SetSlot(ItemData item, int amount)
    {
        if (item == null)
        {
            Debug.LogError(
                "InventorySlotUI on '" +
                gameObject.name +
                "' received a null ItemData."
            );

            ClearSlot();
            return;
        }

        if (itemIcon == null)
        {
            Debug.LogError(
                "Item Icon is missing on inventory slot: " +
                gameObject.name
            );

            return;
        }

        if (stackText == null)
        {
            Debug.LogError(
                "Stack Text is missing on inventory slot: " +
                gameObject.name
            );

            return;
        }

        itemIcon.sprite = item.icon;
        itemIcon.gameObject.SetActive(true);

        stackText.text = amount.ToString();
        stackText.gameObject.SetActive(true);
    }

    public void ClearSlot()
    {
        if (itemIcon != null)
        {
            itemIcon.sprite = null;
            itemIcon.gameObject.SetActive(false);
        }
        else
        {
            Debug.LogError(
                "Item Icon is missing on inventory slot: " +
                gameObject.name
            );
        }

        if (stackText != null)
        {
            stackText.text = "";
            stackText.gameObject.SetActive(false);
        }
        else
        {
            Debug.LogError(
                "Stack Text is missing on inventory slot: " +
                gameObject.name
            );
        }
    }

    public void OnPointerClick(
        PointerEventData eventData)
    {
        if (playerEquipment == null)
        {
            Debug.LogWarning(
                "InventorySlotUI on '" +
                gameObject.name +
                "' does not have PlayerEquipment assigned."
            );

            return;
        }

        if (slotIndex < 0)
        {
            Debug.LogWarning(
                "InventorySlotUI on '" +
                gameObject.name +
                "' does not have a valid slot index."
            );

            return;
        }

        playerEquipment.EquipSlot(slotIndex);
    }

    private void UpdateEquippedHighlight(
        int equippedSlotIndex)
    {
        if (equippedHighlight == null)
        {
            return;
        }

        equippedHighlight.gameObject.SetActive(
            slotIndex == equippedSlotIndex
        );
    }

    private void OnDestroy()
    {
        if (playerEquipment != null)
        {
            playerEquipment.EquippedSlotChanged -=
                UpdateEquippedHighlight;
        }
    }
}