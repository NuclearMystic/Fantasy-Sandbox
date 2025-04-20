using DialogueSystem;
using UnityEngine;

[RequireComponent(typeof(Collider))]
public class DialogueTrigger : MonoBehaviour, IInteractable
{
    [Header("Dialogue")]
    public DialogueTree dialogueTree;
    public string speakerName = "NPC";

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
        if (dialogueTree != null)
        {
            DialogueManager manager = FindObjectOfType<DialogueManager>();
            if (manager != null)
            {
                manager.StartDialogue(dialogueTree);
            }
            else
            {
                Debug.LogWarning("DialogueManager not found in scene.");
            }
        }
    }

    public string GetDisplayName()
    {
        return speakerName;
    }

    public ReachType GetReachType()
    {
        return ReachType.Near;
    }

    public InteractionType GetInteractionType()
    {
        return InteractionType.Dialogue;
    }
}
