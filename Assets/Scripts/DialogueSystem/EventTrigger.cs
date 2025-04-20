using UnityEngine;

namespace DialogueSystem
{
    [System.Serializable]
    public class EventTrigger
    {
        // Allows certain actions (like animations or sounds) to be triggered at specific points in the dialogue.
        public enum EventType
        {
            MoveCharacter,
            ShowIcon,
            AddItem,
            Custom
        }

        public EventType eventType;
        public string parameter; // Can be used for custom events or additional parameters for predefined events.
    }
}
