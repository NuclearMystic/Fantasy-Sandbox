using UnityEngine;
using TMPro;

public class UIController : MonoBehaviour
{
    public static UIController Instance { get; private set; }

    [Header("Interaction Prompt")]
    [Tooltip("The entire prompt UI element.")]
    [SerializeField] private GameObject promptRoot;

    [Tooltip("The text element showing the prompt.")]
    [SerializeField] private TextMeshProUGUI promptText;

    [Tooltip("The reference to the UI animator.")]
    [SerializeField] private Animator uiAnimator;

    [Tooltip("The reference to the inventory animator.")]
    [SerializeField] public Animator inventoryAnimator;

    [Tooltip("The reference to the attribute and skills menu animator.")]
    [SerializeField] private Animator attributesAnimator;

    [Tooltip("The reference to the equipment menu animator.")]
    [SerializeField] private Animator equipmentAnimator;

    [Header("Level Up Popup")]
    [Tooltip("UI object for the level up popup.")]
    [SerializeField] private GameObject levelUpPopupRoot;

    [Tooltip("Text element inside the level up popup.")]
    [SerializeField] private TextMeshProUGUI levelUpPopupText;

    [Tooltip("The Reference to the SkillMenuUI.")]
    [SerializeField] public SkillMenuUI skillMenuUI;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;

        if (promptRoot != null)
            promptRoot.SetActive(false);

        if (levelUpPopupRoot != null)
            levelUpPopupRoot.SetActive(false);

        uiAnimator = GetComponent<Animator>();

    }

    private void Update()
    {
        if (Input.GetButtonDown("Inventory"))
        {
            bool isOpen = inventoryAnimator.GetBool("inventoryOpen");

            if (isOpen)
            {
                attributesAnimator.SetTrigger("Close");
                inventoryAnimator.SetTrigger("Close");
                equipmentAnimator.SetTrigger("Close");
                inventoryAnimator.SetBool("inventoryOpen", false);

                // Inventory closed: lock camera and cursor
                Camera.main.GetComponent<ThirdPersonCamera>().SetCameraControl(true);
            }
            else
            {
                attributesAnimator.SetTrigger("Open");
                inventoryAnimator.SetTrigger("Open");
                equipmentAnimator.SetTrigger("Open");
                inventoryAnimator.SetBool("inventoryOpen", true);

                skillMenuUI.RefreshAllSkills();

                // Inventory opened: unlock camera and cursor
                Camera.main.GetComponent<ThirdPersonCamera>().SetCameraControl(false);
            }
        }
    }



    public void ShowInteractionPrompt(string actionName, string targetName)
    {
        if (promptRoot != null && promptText != null)
        {
            promptText.text = $"[{actionName}] {targetName}";
            promptRoot.SetActive(true);
        }
    }

    public void HideInteractionPrompt()
    {
        if (promptRoot != null)
            promptRoot.SetActive(false);
    }

    public void SetCursorState(bool locked)
    {
        if (locked)
        {
            Cursor.lockState = CursorLockMode.Locked;
            //Cursor.visible = false;
        }
        else
        {
            Cursor.lockState = CursorLockMode.None;
            //Cursor.visible = true;
        }
    }

    public void ShowLevelUpPopup(string message)
    {
        Debug.Log($"[LEVEL UP POPUP] {message}");

        if (levelUpPopupText != null)
            levelUpPopupText.text = message;

        if (levelUpPopupRoot != null)
            levelUpPopupRoot.SetActive(true);

        // Shared animator should have a trigger for the popup animation
        if (uiAnimator != null)
            uiAnimator.SetTrigger("ShowLevelUp");
    }

    public void HideLevelUpPopup()
    {
        if (levelUpPopupRoot != null)
            levelUpPopupRoot.SetActive(false);
    }
}
