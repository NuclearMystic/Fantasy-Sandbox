using System.Collections.Generic;
using UnityEngine;

namespace DialogueSystem
{
    [System.Serializable]
    [CreateAssetMenu(fileName = "New Dialogue Tree", menuName = "Dialogue/Dialogue Tree")]
    public class DialogueTree : ScriptableObject
    {
        // A collection of dialogue nodes forming a complete conversation.
        public string treeId;
        public List<DialogueNode> nodes;
    }
}
