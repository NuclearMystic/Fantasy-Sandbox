using UnityEngine;
using TMPro;

public class InteractionPromptUI : MonoBehaviour
{
    [Tooltip("The entire prompt UI element.")]
    public GameObject promptRoot;

    [Tooltip("The text element showing the prompt.")]
    public TextMeshProUGUI promptText;

    private void Awake()
    {
        if (promptRoot != null)
            promptRoot.SetActive(false);
    }

    public void Show(string actionName, string targetName)
    {
        if (promptRoot != null && promptText != null)
        {
            promptText.text = $"[{actionName}] {targetName}";
            promptRoot.SetActive(true);
        }
    }

    public void Hide()
    {
        if (promptRoot != null)
            promptRoot.SetActive(false);
    }
}
