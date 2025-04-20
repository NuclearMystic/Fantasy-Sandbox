using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;
using System.Collections;

public class InventorySlotUI : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler
{
    public Image icon;
    public TMP_Text quantityText;

    private InventorySlot slot;
    private InventoryUI inventoryUI;

    private bool isMouseOver = false;

    public bool IsOccupied => slot != null && slot.IsOccupied;

    void Start()
    {
        inventoryUI = Inventory.Instance.GetComponent<InventoryUI>();
    }

    public void SetSlot(InventorySlot newSlot)
    {
        slot = newSlot;

        RefreshSlot();
    }

    public void ClearSlot()
    {
        slot = null;
        icon.sprite = null;
        icon.enabled = false;
        quantityText.text = "";
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (IsOccupied)
        {
            inventoryUI.ShowPopup(this, slot);
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        isMouseOver = true;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        isMouseOver = false;
        StartCoroutine(CheckMouseExit());
    }

    private IEnumerator CheckMouseExit()
    {
        yield return new WaitForSeconds(0.1f);
        if (!isMouseOver && !inventoryUI.IsMouseOverPopup())
        {
            inventoryUI.HidePopup();
        }
    }

    public void RefreshSlot()
    {
        if (IsOccupied)
        {
            icon.sprite = slot.item.icon;
            icon.enabled = true;
            quantityText.text = slot.item.isStackable ? slot.quantity.ToString() : "";
        }
        else
        {
            ClearSlot();
        }
    }

}
