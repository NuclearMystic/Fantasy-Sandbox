using UnityEngine;

public class Interactor : MonoBehaviour
{
    [Header("Detection Settings")]
    public float interactDistance = 5f;
    public float maxReachDistance = 3f;
    public float sphereRadius = 0.15f;

    [Tooltip("Layers to interact with.")]
    public LayerMask interactableLayers = ~0;

    [Tooltip("Transform used to check if object is in front of the player.")]
    public Transform playerHead;

    [Header("UI")]
    public InteractionPromptUI promptUI;

    private IInteractable currentTarget;
    private GameObject currentTargetObject;
    private float timeSinceLastValidHit = 0f;
    private float maxMissTime = 0.2f;

    private void Update()
    {
        timeSinceLastValidHit += Time.deltaTime;

        Vector3 screenCenter = new Vector3(Screen.width / 2f, Screen.height / 2f, 0f);
        Ray ray = new Ray(playerHead.position, Camera.main.transform.forward);

        if (Physics.SphereCast(ray, sphereRadius, out RaycastHit hit, interactDistance, interactableLayers))
        {
            IInteractable interactable = hit.collider.GetComponentInParent<IInteractable>();
            if (interactable != null)
            {
                Transform targetTransform = (interactable as MonoBehaviour)?.transform;
                float distanceToPlayer = Vector3.Distance(transform.position, targetTransform.position);
                Vector3 toTarget = (targetTransform.position - playerHead.position).normalized;

                if (distanceToPlayer <= maxReachDistance && Vector3.Dot(playerHead.forward, toTarget) > 0.3f)
                {
                    currentTarget = interactable;
                    currentTargetObject = hit.collider.gameObject;
                    timeSinceLastValidHit = 0f;

                    HandleHighlight(currentTarget);
                    UIController.Instance?.ShowInteractionPrompt("Interact", currentTarget.GetDisplayName());

                    if (Input.GetButtonDown("Interact"))
                    {
                        currentTarget.Interact(gameObject);
                    }

                    return;
                }
            }
        }

        if (timeSinceLastValidHit > maxMissTime)
        {
            ClearTarget();
        }
    }

    private void HandleHighlight(IInteractable newTarget)
    {
        if (newTarget != currentTarget)
        {
            if (currentTarget != null)
                currentTarget.OnHoverExit();

            currentTarget = newTarget;
            currentTarget.OnHoverEnter();
        }
    }

    private void ClearTarget()
    {
        if (currentTarget != null)
        {
            currentTarget.OnHoverExit();
            currentTarget = null;
        }

        currentTargetObject = null;
        UIController.Instance?.HideInteractionPrompt();
    }

    private bool IsInPlayerView(Vector3 point)
    {
        Vector3 toPoint = (point - playerHead.position).normalized;
        return Vector3.Dot(playerHead.forward, toPoint) > 0.3f;
    }
}
