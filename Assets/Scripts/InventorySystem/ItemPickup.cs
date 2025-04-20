using UnityEngine;

[RequireComponent(typeof(Collider))]
public class ItemPickup : MonoBehaviour, IInteractable
{
    [Header("Item Info")]
    public Item itemData;

    [Header("Highlighting")]
    public Material highlightMaterial;
    private Material originalMaterial;
    private Renderer objectRenderer;

    private void Awake()
    {
        objectRenderer = GetComponent<Renderer>();
        if (objectRenderer != null)
            originalMaterial = objectRenderer.material;
    }

    public void OnHoverEnter()
    {
        if (objectRenderer != null && highlightMaterial != null)
            objectRenderer.material = highlightMaterial;
    }

    public void OnHoverExit()
    {
        if (objectRenderer != null && originalMaterial != null)
            objectRenderer.material = originalMaterial;
    }

    public void Interact(GameObject interactor)
    {
        if (Inventory.Instance != null && itemData != null)
        {
            Inventory.Instance.AddItem(itemData);
            Inventory.Instance.GetComponent<InventoryUI>()?.RefreshUI();
            Debug.Log("Picked up: " + itemData.itemName);
            Destroy(gameObject);
        }
        else
        {
            Debug.LogError("Inventory not found or item data is missing.");
        }
    }

    public string GetDisplayName()
    {
        return itemData != null ? itemData.itemName : "Item";
    }

    public ReachType GetReachType()
    {
        return ReachType.Immediate;
    }

    public InteractionType GetInteractionType()
    {
        return InteractionType.Pickup;
    }
}
