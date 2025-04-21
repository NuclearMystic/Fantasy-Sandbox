using UnityEngine;

[RequireComponent(typeof(Collider))]
public class Chest : MonoBehaviour, IInteractable
{
    [Header("Loot Contents")]
    public Item[] contents;

    [Header("Loot Once Only")]
    public bool canOnlyBeLootedOnce = true;
    private bool hasBeenLooted = false;

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
        if (hasBeenLooted)
            return;

        foreach (Item item in contents)
        {
            if (item != null)
            {
                Inventory.Instance.AddItem(item);
                Inventory.Instance.GetComponent<InventoryUI>()?.RefreshUI();

                Debug.Log($"Looted {item.itemName} from chest.");
            }
        }

        if (canOnlyBeLootedOnce)
            hasBeenLooted = true;
    }

    public string GetDisplayName()
    {
        return hasBeenLooted ? "(Empty Chest)" : "Loot Chest";
    }

    public ReachType GetReachType() => ReachType.Immediate;
    public InteractionType GetInteractionType() => InteractionType.Loot;
}
