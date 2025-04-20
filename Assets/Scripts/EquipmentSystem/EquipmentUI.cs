using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class EquipmentUI : MonoBehaviour
{
    [System.Serializable]
    public class SlotUI
    {
        public EquipmentSlot slot;
        public Image iconImage;
        public GameObject slotPanel; 
    }

    [Header("Slot UI Mapping")]
    public List<SlotUI> slotUIs;

    [Header("Placeholder Sprite")]
    public Sprite emptySlotSprite;

    private Dictionary<EquipmentSlot, SlotUI> slotMap;

    private void Awake()
    {
        slotMap = new Dictionary<EquipmentSlot, SlotUI>();
        foreach (var s in slotUIs)
        {
            slotMap[s.slot] = s;
        }
    }

    private void Start()
    {
        if (EquipmentManager.Instance != null)
        {
            EquipmentManager.Instance.OnEquipmentChanged += OnEquipmentChanged;
            RefreshAllSlots();
        }
        else
        {
            Debug.LogWarning("EquipmentManager instance not found in Start.");
        }
    }


    private void OnDestroy()
    {
        if (EquipmentManager.Instance != null)
            EquipmentManager.Instance.OnEquipmentChanged -= OnEquipmentChanged;
    }

    private void OnEquipmentChanged(EquipmentSlot slot, EquipmentItem item)
    {
        if (!slotMap.TryGetValue(slot, out var ui))
            return;

        if (item != null && item.icon != null)
        {
            ui.iconImage.sprite = item.icon;
            ui.iconImage.enabled = true;
        }
        else
        {
            // Clear the item icon and hide it
            ui.iconImage.sprite = null;
            ui.iconImage.enabled = false;
        }
    }



    public void RefreshAllSlots()
    {
        foreach (var slot in slotMap.Keys)
        {
            var item = EquipmentManager.Instance.GetEquipped(slot);
            OnEquipmentChanged(slot, item);
        }
    }
}

public enum EquipmentSlot
{
    Head,
    Hair,
    Bangs,
    FacialHair,
    Chest,
    Legs,
    Dress, // takes up both chest and legs sots
    Cloak,
    Gloves,
    Boots,
    MainHand,
    OffHand
}
