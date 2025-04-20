using UnityEngine;

[RequireComponent(typeof(Collider))]
public class InspectObject : MonoBehaviour, IInteractable
{
    [Header("Inspect Info")]
    public string objectName = "Ancient Statue";
    [TextArea(2, 5)]
    public string description = "A mysterious statue with strange markings.";

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
        Debug.Log("Inspected: " + objectName);
        Debug.Log("Description: " + description);

        // TODO: Hook into inspect UI system
        // Example: InspectUI.Instance.Show(objectName, description);
    }

    public string GetDisplayName()
    {
        return objectName;
    }

    public ReachType GetReachType()
    {
        return ReachType.Near;
    }

    public InteractionType GetInteractionType()
    {
        return InteractionType.Inspect;
    }
}
