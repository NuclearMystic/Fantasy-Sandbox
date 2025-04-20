using UnityEngine;

namespace DialogueSystem
{
    public class ConditionTrigger : MonoBehaviour
    {
        // Use this to set conditions in the dialogue managers game variables dictionary
        [Tooltip("Name of the condition you want this trigger to modify. This name must match the name of the condition set in the dialogue tree.")]
        public string conditionName;
        [Tooltip("The value you'd like to change the condition to upon interacting with this object.")]
        public string conditionValue;

        public void ModifyCondition()
        {
            DialogueManager dialogueManager = FindObjectOfType<DialogueManager>();
            if (dialogueManager.gameVariables.ContainsKey(conditionName))
            {
                dialogueManager.gameVariables[conditionName] = conditionValue;
                Debug.Log($"Variable '{conditionName}' changed to '{conditionValue}'.");
            }
            else
            {
                Debug.LogWarning($"Variable '{conditionName}' not found in DialogueManager.");
            }
        }
    }
}


