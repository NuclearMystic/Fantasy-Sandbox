using System.Collections.Generic;
using UnityEngine;

public class Inventory : MonoBehaviour
{
    public static Inventory Instance { get; private set; }

    public List<InventorySlot> inventorySlots = new List<InventorySlot>();
    [Tooltip("Define the maximum number of slots")]
    public int maxSlots = 20;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;

        // Initialize with empty slots
        while (inventorySlots.Count < maxSlots)
            inventorySlots.Add(new InventorySlot(null, 0));
    }

    public void AddItem(Item item, int quantity = 1)
    {
        // Try stacking
        if (item.isStackable)
        {
            foreach (var slot in inventorySlots)
            {
                if (slot.IsOccupied && slot.item == item && slot.quantity < item.maxStackSize)
                {
                    int spaceLeft = item.maxStackSize - slot.quantity;
                    int addAmount = Mathf.Min(spaceLeft, quantity);
                    slot.quantity += addAmount;
                    quantity -= addAmount;
                    if (quantity <= 0) return;
                }
            }
        }

        // Fill empty slot(s)
        foreach (var slot in inventorySlots)
        {
            if (!slot.IsOccupied)
            {
                slot.item = item;
                slot.quantity = quantity;
                return;
            }
        }

        Debug.LogWarning("Inventory is full!");
    }

    public void RemoveItem(Item item, int quantity = 1)
    {
        InventorySlot slot = inventorySlots.Find(s => s.IsOccupied && s.item == item);
        if (slot != null)
        {
            slot.quantity -= quantity;
            if (slot.quantity <= 0)
                slot.Clear();
        }
    }
}


[System.Serializable]
public class InventorySlot
{
    public Item item;
    public int quantity;

    public bool IsOccupied => item != null;

    public InventorySlot(Item item, int quantity)
    {
        this.item = item;
        this.quantity = quantity;
    }

    public void Clear()
    {
        item = null;
        quantity = 0;
    }
}
