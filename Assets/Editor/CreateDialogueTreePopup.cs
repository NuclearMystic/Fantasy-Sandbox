using UnityEditor;
using UnityEngine;

namespace DialogueSystem
{
    public class CreateDialogueTreePopup : EditorWindow
    {
        public delegate void CreateDialogueTreeDelegate(string treeName);
        public CreateDialogueTreeDelegate OnCreateDialogueTree;

        private string treeName = "New Dialogue Tree";

        public static void ShowWindow(CreateDialogueTreeDelegate createDialogueTreeDelegate)
        {
            CreateDialogueTreePopup window = GetWindow<CreateDialogueTreePopup>("Create New Dialogue Tree");
            window.OnCreateDialogueTree = createDialogueTreeDelegate;
            window.Show();
        }

        private void OnGUI()
        {
            GUILayout.Label("Enter the name for the new dialogue tree:", EditorStyles.boldLabel);
            treeName = EditorGUILayout.TextField("Tree Name", treeName);

            EditorGUILayout.Space();

            if (GUILayout.Button("Create"))
            {
                OnCreateDialogueTree?.Invoke(treeName);
                Close();
            }

            if (GUILayout.Button("Cancel"))
            {
                Close();
            }
        }
    }
}
