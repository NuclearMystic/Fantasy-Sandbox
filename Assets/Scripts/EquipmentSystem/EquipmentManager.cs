using System;
using System.Collections.Generic;
using UnityEngine;

public class EquipmentManager : MonoBehaviour
{
    public static EquipmentManager Instance { get; private set; }

    [Header("References")]
    public Inventory playerInventory;

    [Tooltip("All SkinnedMeshRenderers from the base body.")]
    public SkinnedMeshRenderer[] baseBodyRenderers;

    [Header("Weapon/Shield Attachment Points")]
    public Transform mainHandAttachPoint;
    public Transform offHandAttachPoint;

    [Header("Player Root Transform")]
    public Transform playerModelRoot;

    private Transform sharedRootBone;

    private Dictionary<EquipmentSlot, EquipmentItem> equippedItems = new();
    private Dictionary<EquipmentSlot, GameObject> spawnedPrefabs = new();

    public event Action<EquipmentSlot, EquipmentItem> OnEquipmentChanged;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    private void Start()
    {
        if (baseBodyRenderers != null && baseBodyRenderers.Length > 0)
        {
            sharedRootBone = baseBodyRenderers[0].rootBone;
        }

        if (sharedRootBone == null)
        {
            Debug.LogWarning("Root bone could not be auto-assigned. Check base body setup.");
        }
    }

    public void EquipItem(EquipmentItem item)
    {
        if (item == null || item.prefab == null)
            return;

        // Handle slot conflicts
        if (item.slot == EquipmentSlot.Dress)
        {
            UnequipSlot(EquipmentSlot.Chest);
            UnequipSlot(EquipmentSlot.Legs);
        }
        else if (item.slot == EquipmentSlot.Chest || item.slot == EquipmentSlot.Legs)
        {
            UnequipSlot(EquipmentSlot.Dress);
        }

        UnequipSlot(item.slot);

        GameObject spawned = Instantiate(item.prefab, playerModelRoot);

        SkinnedMeshRenderer smr = spawned.GetComponentInChildren<SkinnedMeshRenderer>();
        if (smr != null)
        {
            AssignBones(smr);
        }

        spawned.transform.localPosition = Vector3.zero;
        spawned.transform.localRotation = Quaternion.identity;

        spawnedPrefabs[item.slot] = spawned;
        equippedItems[item.slot] = item;

        OnEquipmentChanged?.Invoke(item.slot, item);
    }

    private void AssignBones(SkinnedMeshRenderer smr)
    {
        if (baseBodyRenderers == null || baseBodyRenderers.Length == 0)
            return;

        List<Transform> allBones = new();
        foreach (var r in baseBodyRenderers)
        {
            if (r != null && r.bones != null)
                allBones.AddRange(r.bones);
        }

        smr.bones = allBones.ToArray();
        smr.rootBone = sharedRootBone;
    }

    public void UnequipSlot(EquipmentSlot slot)
    {
        if (equippedItems.TryGetValue(slot, out var oldItem))
        {
            if (oldItem != null)
                Inventory.Instance.AddItem(oldItem, 1);

            equippedItems.Remove(slot);
        }

        if (spawnedPrefabs.TryGetValue(slot, out var obj))
        {
            Destroy(obj);
            spawnedPrefabs.Remove(slot);
        }

        OnEquipmentChanged?.Invoke(slot, null);

        InventoryUI ui = Inventory.Instance.GetComponent<InventoryUI>();
        if (ui != null)
            ui.RefreshUI();
    }

    public EquipmentItem GetEquipped(EquipmentSlot slot)
    {
        return equippedItems.TryGetValue(slot, out var item) ? item : null;
    }

    public int GetTotalArmorClass()
    {
        int total = 0;
        foreach (var item in equippedItems.Values)
            total += item.armorClass;
        return total;
    }
}
