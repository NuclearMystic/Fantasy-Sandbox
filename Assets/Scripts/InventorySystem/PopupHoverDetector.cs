using UnityEngine;
using UnityEngine.EventSystems;

public class PopupHoverDetector : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public InventoryUI inventoryUI;

    public void OnPointerEnter(PointerEventData eventData)
    {
        inventoryUI.SetPopupHovered(true);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        inventoryUI.SetPopupHovered(false);
    }
}
