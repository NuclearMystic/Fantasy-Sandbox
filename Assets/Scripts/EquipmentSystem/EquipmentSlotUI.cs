using UnityEngine;
using UnityEngine.EventSystems;

public class EquipmentSlotUI : MonoBehaviour, IPointerClickHandler
{
    public EquipmentSlot slot;

    private float lastClickTime = 0f;
    private const float doubleClickThreshold = 0.3f;

    public void OnPointerClick(PointerEventData eventData)
    {
        if (Time.time - lastClickTime < doubleClickThreshold)
        {
            EquipmentManager.Instance.UnequipSlot(slot);
        }

        lastClickTime = Time.time;
    }
}
