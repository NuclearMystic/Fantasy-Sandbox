using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(Collider))]
public class ActivateObject : MonoBehaviour, IInteractable
{
    [Header("Settings")]
    public string objectName = "Lever";
    public ReachType reachType = ReachType.Near;

    [Header("Activation Event")]
    public UnityEvent onActivate;

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
        Debug.Log("Activated: " + objectName);
        onActivate?.Invoke();
    }

    public string GetDisplayName()
    {
        return objectName;
    }

    public ReachType GetReachType()
    {
        return reachType;
    }

    public InteractionType GetInteractionType()
    {
        return InteractionType.Activate;
    }
}
