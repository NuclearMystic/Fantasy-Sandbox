using System.Collections.Generic;
using UnityEngine;

namespace DialogueSystem
{
    public class DialogueManager : MonoBehaviour
    {
        // Manages the flow of dialogue, keeps track of the current node, and handles branching logic.

        // Contains various conditions for different dialogue options.
        public Dictionary<string, string> gameVariables = new Dictionary<string, string>();

        [Tooltip("This will be replaced at runtime with the current dialogue you want to show, you probably want to leave this empty. It's here in case you need to test a dialogue tree by triggering it in a start method.")]
        public DialogueTree currentDialogueTree;

        private int currentNodeIndex = 0;
        private DialogueUI dialogueUI;

        private void Awake()
        {
            dialogueUI = GetComponent<DialogueUI>();
        }

        void Start()
        {
            // Use for testing purposes.
            if (currentDialogueTree != null)
            {
                StartDialogue(currentDialogueTree);
            }
        }

        public void StartDialogue(DialogueTree dialogueTree)
        {
            currentDialogueTree = dialogueTree;
            currentNodeIndex = FindStartNodeIndex(dialogueTree);
            dialogueUI.ShowDialogue();
            DisplayCurrentNode();
        }

        private int FindStartNodeIndex(DialogueTree dialogueTree)
        {
            for (int i = 0; i < dialogueTree.nodes.Count; i++)
            {
                if (CheckConditions(dialogueTree.nodes[i]))
                {
                    return i;
                }
            }
            // Default to first node if no conditions are met
            return 0;
        }

        public void SelectChoice(int choiceIndex)
        {
            if (currentDialogueTree.nodes[currentNodeIndex].nextNodes.Count > choiceIndex)
            {
                string nextNodeId = currentDialogueTree.nodes[currentNodeIndex].nextNodes[choiceIndex];
                int nextNodeIndex = currentDialogueTree.nodes.FindIndex(node => node.id == nextNodeId);

                // Check conditions for the selected next node
                if (nextNodeIndex != -1 && CheckConditions(currentDialogueTree.nodes[nextNodeIndex]))
                {
                    currentNodeIndex = nextNodeIndex;
                    DisplayCurrentNode();
                }
                else
                {
                    // If conditions are not met, find the next valid node or default to the next node in order
                    while (++nextNodeIndex < currentDialogueTree.nodes.Count)
                    {
                        if (CheckConditions(currentDialogueTree.nodes[nextNodeIndex]))
                        {
                            currentNodeIndex = nextNodeIndex;
                            DisplayCurrentNode();
                            return;
                        }
                    }
                    // If no valid node is found, end the dialogue
                    dialogueUI.HideDialogue();
                }
            }
            else
            {
                dialogueUI.HideDialogue();
            }
        }

        private bool CheckConditions(DialogueNode node)
        {
            foreach (var condition in node.conditions)
            {
                if (!gameVariables.ContainsKey(condition.variableName) ||
                    gameVariables[condition.variableName] != condition.expectedValue)
                {
                    return false;
                }
            }
            return true;
        }

        private void DisplayCurrentNode()
        {
            // Perform actions associated with the current node
            DialogueNode currentNode = currentDialogueTree.nodes[currentNodeIndex];
            foreach (var action in currentNode.actions)
            {
                SetVariable(action.variableName, action.value);
            }

            HandleEvents(currentNode.events);
            dialogueUI.DisplayNode(currentNode);
        }

        private void HandleEvents(List<EventTrigger> events)
        {
            foreach (var eventTrigger in events)
            {
                switch (eventTrigger.eventType)
                {
                    case EventTrigger.EventType.MoveCharacter:
                        // Add logic to move a character
                        Debug.Log($"Moving character with parameter: {eventTrigger.parameter}");
                        break;
                    case EventTrigger.EventType.ShowIcon:
                        // Add logic to show an icon
                        Debug.Log($"Showing icon with parameter: {eventTrigger.parameter}");
                        break;
                    case EventTrigger.EventType.AddItem:
                        // Add logic to add an item to the inventory
                        Debug.Log($"Adding item with parameter: {eventTrigger.parameter}");
                        break;
                    case EventTrigger.EventType.Custom:
                        // Add logic for custom events
                        Debug.Log($"Triggering custom event with parameter: {eventTrigger.parameter}");
                        break;
                }
            }
        }

        public void SetVariable(string name, string value)
        {
            if (gameVariables.ContainsKey(name))
            {
                gameVariables[name] = value;
                Debug.Log($"Updated Variable: {name} = {value}");
            }
            else
            {
                gameVariables.Add(name, value);
                Debug.Log($"Added Variable: {name} = {value}");
            }
        }

    }
}
