using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;

public class InventoryUI : MonoBehaviour
{
    public GameObject inventorySlotPrefab;
    public Transform inventorySlotParent;

    public GameObject itemPopup;
    public TMP_Text popupItemName;
    public Button useButton;
    public Button removeButton;

    private InventorySlotUI currentSlotUI;
    private bool isMouseOverPopup = false;

    [Header("Popup Stat Display")]
    public GameObject armorStatPanel;
    public TMP_Text armorAmountText;
    public Image armorIcon;

    private void Start()
    {
        RefreshUI();
    }

    public void RefreshUI()
    {
        if (Inventory.Instance == null)
        {
            Debug.LogError("Inventory singleton not found.");
            return;
        }

        // Clear all existing slotty bois
        foreach (Transform child in inventorySlotParent)
        {
            Destroy(child.gameObject);
        }

        // Recreate sloots
        for (int i = 0; i < Inventory.Instance.maxSlots; i++)
        {
            GameObject newSlot = Instantiate(inventorySlotPrefab, inventorySlotParent);
            InventorySlotUI slotUI = newSlot.GetComponent<InventorySlotUI>();

            if (i < Inventory.Instance.inventorySlots.Count)
            {
                slotUI.SetSlot(Inventory.Instance.inventorySlots[i]);
            }
            else
            {
                slotUI.ClearSlot(); 
            }
        }
    }

    public void ShowPopup(InventorySlotUI slotUI, InventorySlot slot)
    {
        currentSlotUI = slotUI;
        popupItemName.text = slot.item.itemName;
        itemPopup.SetActive(true);
        itemPopup.transform.position = Input.mousePosition;

        if (slot.item is EquipmentItem equip)
        {
            armorStatPanel.SetActive(true);
            armorAmountText.text = ("+ " + equip.armorClass.ToString());
        }
        else
        {
            armorStatPanel.SetActive(false);
        }

        useButton.onClick.RemoveAllListeners();
        useButton.onClick.AddListener(() => UseItem(slot));

        removeButton.onClick.RemoveAllListeners();
        removeButton.onClick.AddListener(() => RemoveItem(slot));
    }

    public void HidePopup()
    {
        itemPopup.SetActive(false);
    }

    private void UseItem(InventorySlot slot)
    {
        if (slot.item is EquipmentItem equipItem)
        {
            EquipmentManager.Instance.EquipItem(equipItem);
            Inventory.Instance.RemoveItem(equipItem, 1);
            RefreshUI();
        }
        else
        {
            Debug.Log("Used item: " + slot.item.itemName);
        }

        HidePopup();
    }

    private void RemoveItem(InventorySlot slot)
    {
        Inventory.Instance.RemoveItem(slot.item, 1);
        RefreshUI();
        HidePopup();
    }

    public bool IsMouseOverPopup() => isMouseOverPopup;

    public void SetPopupHovered(bool hovered)
    {
        isMouseOverPopup = hovered;
    }
}
