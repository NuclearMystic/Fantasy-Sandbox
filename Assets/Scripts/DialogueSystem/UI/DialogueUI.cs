using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace DialogueSystem
{
    public class DialogueUI : MonoBehaviour
    {
        private DialogueManager dialogueManager;

        public enum TextAnimationMode
        {
            Default,
            Typewriter,
            Custom
        }

        [Header("UI References")]
        [Tooltip("Place the text object to display the text here. Thats whichever object has the actual text component on it.")]
        public TMP_Text dialogueText;
        [Tooltip("Place the text object to display the character name here.")]
        public TMP_Text characterNameText;
        [Tooltip("Place the image object to display the character portrait here.")]
        public Image characterPortraitImage;
        [Tooltip("This is the wrapper around the choice buttons. Set the transparency to 0 if you don't need this to be visible. You can also alter the layout and size fitter for the buttons in the prefab.")]
        public GameObject choicesContainer;
        [Tooltip("Place a button prefab here. There does not need to be a button in the scene, this will be the one you wish to instantiate for each choice.")]
        public Button choiceButtonPrefab;
        [Tooltip("The graphic for the panel holding the dialogue UI elements. If you don't need the panel to be visible, turn the transparency of the sprite image to 0.")]
        public GameObject dialoguePanel;

        [Header("Animation Settings")]
        [Tooltip("Place the animator for your dialogue box here. If you don't have animations to play, this can be left empty.")]
        public Animator dialogueAnimator;
        [Tooltip("Animation to trigger for the dialogue box when dialogue starts.")]
        public string showTrigger;
        [Tooltip("Animation to trigger for the dialogue box when dialogue ends.")]
        public string hideTrigger;
        [Tooltip("Select the text animation mode.")]
        public TextAnimationMode textAnimationMode = TextAnimationMode.Default;
        [Tooltip("Time delay between each character for typewriter effect.")]
        public float typewriterDelay = 0.05f;
        [Tooltip("Custom animation script for text display.")]
        public MonoBehaviour customTextAnimation;

        [Header("Audio Settings")]
        [Tooltip("Place audio to play when dialogue starts here.")]
        public AudioSource startSFX;
        [Tooltip("Place audio to play when dialogue ends here.")]
        public AudioSource endSFX;
        [Tooltip("Place audio to play when dialogue text appears, or for each letter in type writer mode.")]
        public AudioSource typingSFX;

        private List<Button> choiceButtons = new List<Button>();
        private Coroutine typewriterCoroutine;
        private bool isDialogueActive = false;

        [Header("Other Settings")]
        [Tooltip("Allow the use of the keyboard for selecting choices.")]
        public bool pauseOnDialogue = false;

        private void Awake()
        {
            dialogueManager = GetComponent<DialogueManager>();
        }

        public void DisplayNode(DialogueNode node)
        {
            isDialogueActive = true;

            switch (textAnimationMode)
            {
                case TextAnimationMode.Default:
                    DisplayTextAllAtOnce(node.text);
                    break;
                case TextAnimationMode.Typewriter:
                    if (typewriterCoroutine != null)
                    {
                        StopCoroutine(typewriterCoroutine);
                    }
                    typewriterCoroutine = StartCoroutine(DisplayTextTypewriter(node.text)); break;
                case TextAnimationMode.Custom:
                    if (customTextAnimation != null)
                    {
                        ICustomTextAnimation customAnimation = customTextAnimation as ICustomTextAnimation;
                        if (customAnimation != null)
                        {
                            customAnimation.AnimateText(dialogueText, node.text);
                        }
                    }
                    break;
            }

            if (node.characterProfile != null)
            {
                characterNameText.text = node.characterProfile.characterName;
                characterPortraitImage.sprite = node.characterProfile.characterPortrait;
                characterPortraitImage.gameObject.SetActive(true);
            }
            else
            {
                characterNameText.text = "";
                characterPortraitImage.gameObject.SetActive(false);
            }

            // Clear previous choices
            foreach (Transform child in choicesContainer.transform)
            {
                Destroy(child.gameObject);
            }

            // Create new choice buttons
            for (int i = 0; i < node.choices.Count; i++)
            {
                Button choiceButton = Instantiate(choiceButtonPrefab, choicesContainer.transform);
                choiceButton.GetComponentInChildren<TMP_Text>().text = node.choices[i];
                int choiceIndex = i;
                choiceButton.onClick.AddListener(() => { dialogueManager.SelectChoice(choiceIndex); });
                choiceButtons.Add(choiceButton);
            }
        }

        private void DisplayTextAllAtOnce(string text)
        {
            if (typingSFX != null)
            {
                typingSFX.Play();
            }
            dialogueText.text = text;
        }

        private IEnumerator DisplayTextTypewriter(string text)
        {
            yield return new WaitForSeconds(0.5f);
            dialogueText.text = "";
            foreach (char c in text)
            {
                if (typingSFX != null)
                {
                    typingSFX.Play();
                }
                dialogueText.text += c;
                yield return new WaitForSeconds(typewriterDelay);
            }
        }

        private void StopTypewriterCoroutine()
        {
            if (typewriterCoroutine != null)
            {
                StopCoroutine(typewriterCoroutine);
                typewriterCoroutine = null;
            }
            dialogueText.text = "";
        }

        public void HideDialogue()
        {
            StopTypewriterCoroutine();
            isDialogueActive = false;

            if (dialogueAnimator != null)
            {
                dialogueAnimator.SetTrigger(hideTrigger);
            }
            else
            {
                dialoguePanel.SetActive(false);
            }

            if (endSFX != null)
            {
                endSFX.Play();
            }

            var cam = Camera.main?.GetComponent<ThirdPersonCamera>();
            if (cam != null)
                cam.SetCameraControl(true);
        }


        public void ShowDialogue()
        {
            isDialogueActive = true;

            if (dialogueAnimator != null)
            {
                dialogueAnimator.SetTrigger(showTrigger);
            }
            else
            {
                dialoguePanel.SetActive(true);
            }

            if (startSFX != null)
            {
                startSFX.Play();
            }

            var cam = Camera.main?.GetComponent<ThirdPersonCamera>();
            if (cam != null)
                cam.SetCameraControl(false);
        }

        public bool IsDialogueActive()
        {
            return isDialogueActive;
        }

    }

    public interface ICustomTextAnimation
    {
        void AnimateText(TMP_Text textComponent, string text);
    }
}
