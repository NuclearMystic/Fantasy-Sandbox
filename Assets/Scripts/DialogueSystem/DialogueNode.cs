using System.Collections.Generic;
using UnityEngine;

namespace DialogueSystem
{
    [System.Serializable]
    public class DialogueNode
    {
        // Represents individual pieces of dialogue with text, and choices.

        [Tooltip("ID to find a specific dialogue node by.")]
        public string id;
        [Tooltip("Enter text for this line of dialogue.")]
        [TextArea(3, 10)]
        public string text;
        [Tooltip("Profile for the character whose line this is.")]
        public CharacterProfile characterProfile;
        [Tooltip("Choices the player will have for this line of dialogue.")]
        public List<string> choices = new List<string>();
        [Tooltip("Corresponding node for dialogue choice. This should be the ID of the next node based on the corresponding choice.")]
        public List<string> nextNodes = new List<string>();
        [Tooltip("Conditions to be met for this node to be displayed.")]
        public List<Condition> conditions = new List<Condition>();
        [Tooltip("Actions will set conditions that need to be met for certain lines of dialogue to be displayed.")]
        public List<DialogueAction> actions = new List<DialogueAction>();
        [Tooltip("Events that need to trigger on certain lines of dialogue. Give player item, display icon, move character, etc...")]
        public List<EventTrigger> events = new List<EventTrigger>(); // Add event triggers here
    }

    [System.Serializable]
    public class DialogueAction
    {
        [Tooltip("The name of the game variable to be modified.")]
        public string variableName;
        [Tooltip("The new value to be assigned to the game variable.")]
        public string value;
    }

    [System.Serializable]
    public class Condition
    {
        [Tooltip("The name of the game variable to check.")]
        public string variableName;
        [Tooltip("The expected value of the game variable.")]
        public string expectedValue;
    }
}
