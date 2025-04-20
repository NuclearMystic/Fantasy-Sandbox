using TMPro;
using UnityEngine;

namespace DialogueSystem
{
    public class CustomTextAnimation : MonoBehaviour, ICustomTextAnimation
    {
        public void AnimateText(TMP_Text textComponent, string text)
        {
            // Implement your custom text animation logic here
            // For example, you could fade in each character, apply a wave effect, or even create an Ienumerator
            // and make any or every letter or word do something different.
            textComponent.text = text;
        }
    }
}
