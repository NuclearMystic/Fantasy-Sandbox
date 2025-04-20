using UnityEngine;

namespace DialogueSystem
{
    [CreateAssetMenu(fileName = "New Character Profile", menuName = "Dialogue/Character Profile")]
    public class CharacterProfile : ScriptableObject
    {
        // Stores information about characters involved in the dialogue, such as name, avatar.
        public string characterName;
        public Sprite characterPortrait;
    }
}
