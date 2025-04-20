using UnityEngine;

public interface IInteractable
{
    void OnHoverEnter();
    void OnHoverExit();
    void Interact(GameObject interactor);
    string GetDisplayName();
    ReachType GetReachType();
    InteractionType GetInteractionType();
}
