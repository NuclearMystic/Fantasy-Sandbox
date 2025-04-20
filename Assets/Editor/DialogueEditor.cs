using UnityEditor;
using UnityEngine;
using System.Collections.Generic;

namespace DialogueSystem
{
    public class DialogueEditor : EditorWindow
    {
        private DialogueTree currentDialogueTree;
        private Vector2 scrollPos;
        private string saveFolderPath = "Assets/DialogueSystem/DialogueTrees";
        [Tooltip("Name to save this tree as.")]
        public string newTreeName = "New Dialogue Tree";

        [MenuItem("Window/Dialogue Editor")]
        public static void ShowWindow()
        {
            GetWindow<DialogueEditor>("Dialogue Editor");
        }

        private void OnGUI()
        {
            GUILayout.Label("Dialogue Editor", EditorStyles.boldLabel);

            if (GUILayout.Button("Create New Dialogue Tree", GUILayout.Width(200)))
            {
                CreateDialogueTreePopup.ShowWindow(CreateNewDialogueTree);
            }

            if (GUILayout.Button("Load Dialogue Tree", GUILayout.Width(200)))
            {
                LoadDialogueTree();
            }

            if (currentDialogueTree != null)
            {
                EditorGUILayout.Space();
                EditorGUILayout.LabelField("Dialogue Tree ID:", currentDialogueTree.treeId);

                scrollPos = EditorGUILayout.BeginScrollView(scrollPos);
                foreach (var node in currentDialogueTree.nodes)
                {
                    DrawNodeEditor(node);
                    GUILayout.Space(10); // Add space between nodes
                    DrawHorizontalLine(); // Draw a horizontal line between nodes
                }
                EditorGUILayout.EndScrollView();

                if (GUILayout.Button("Add Node", GUILayout.Width(100)))
                {
                    currentDialogueTree.nodes.Add(new DialogueNode { id = System.Guid.NewGuid().ToString() });
                }

                EditorGUILayout.Space();
                if (GUILayout.Button("Save Dialogue Tree", GUILayout.Width(150)))
                {
                    SaveDialogueTree();
                }
            }

            EditorGUILayout.Space();
            saveFolderPath = EditorGUILayout.TextField("Save Folder Path", saveFolderPath);
        }

        private void CreateNewDialogueTree(string treeName)
        {
            currentDialogueTree = CreateInstance<DialogueTree>();
            currentDialogueTree.treeId = treeName;
            currentDialogueTree.nodes = new List<DialogueNode>();
        }

        private void LoadDialogueTree()
        {
            string path = EditorUtility.OpenFilePanel("Select Dialogue Tree", saveFolderPath, "asset");
            if (!string.IsNullOrEmpty(path))
            {
                path = FileUtil.GetProjectRelativePath(path);
                currentDialogueTree = AssetDatabase.LoadAssetAtPath<DialogueTree>(path);
            }
        }

        private void SaveDialogueTree()
        {
            if (currentDialogueTree == null) return;

            string path = $"{saveFolderPath}/{currentDialogueTree.treeId}.asset";

            if (!AssetDatabase.IsValidFolder(saveFolderPath))
            {
                AssetDatabase.CreateFolder("Assets", "DialogueTrees");
            }

            DialogueTree existingTree = AssetDatabase.LoadAssetAtPath<DialogueTree>(path);

            if (existingTree == null)
            {
                AssetDatabase.CreateAsset(currentDialogueTree, path);
            }
            else
            {
                EditorUtility.CopySerialized(currentDialogueTree, existingTree);
                AssetDatabase.SaveAssets();
            }

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }

        private void DrawNodeEditor(DialogueNode node)
        {
            EditorGUILayout.BeginVertical("box");
            node.id = EditorGUILayout.TextField("Node ID:", node.id);
            node.text = EditorGUILayout.TextField("Text:", node.text);
            node.characterProfile = (CharacterProfile)EditorGUILayout.ObjectField("Character Profile", node.characterProfile, typeof(CharacterProfile), false);

            EditorGUILayout.LabelField("Choices:");
            for (int i = 0; i < node.choices.Count; i++)
            {
                EditorGUILayout.BeginHorizontal();
                node.choices[i] = EditorGUILayout.TextField("Choice:", node.choices[i]);

                if (node.nextNodes[i] == null)
                {
                    EditorGUILayout.LabelField("Next Node: End Dialogue");
                    if (GUILayout.Button("Add Next Node", GUILayout.Width(120)))
                    {
                        node.nextNodes[i] = "";
                    }
                }
                else
                {
                    node.nextNodes[i] = EditorGUILayout.TextField("Next Node ID:", node.nextNodes[i]);
                    if (GUILayout.Button("Remove Next Node", GUILayout.Width(140)))
                    {
                        node.nextNodes[i] = null;
                    }
                }

                if (GUILayout.Button("Remove Choice", GUILayout.Width(120)))
                {
                    node.choices.RemoveAt(i);
                    node.nextNodes.RemoveAt(i);
                }
                EditorGUILayout.EndHorizontal();
            }

            if (GUILayout.Button("Add Choice", GUILayout.Width(100)))
            {
                node.choices.Add("");
                node.nextNodes.Add(null);
            }

            EditorGUILayout.LabelField("Conditions:");
            for (int i = 0; i < node.conditions.Count; i++)
            {
                var condition = node.conditions[i];
                EditorGUILayout.BeginHorizontal();
                condition.variableName = EditorGUILayout.TextField("Variable Name:", condition.variableName);
                condition.expectedValue = EditorGUILayout.TextField("Expected Value:", condition.expectedValue);
                if (GUILayout.Button("Remove Condition", GUILayout.Width(140)))
                {
                    node.conditions.RemoveAt(i);
                }
                EditorGUILayout.EndHorizontal();
            }

            if (GUILayout.Button("Add Condition", GUILayout.Width(110)))
            {
                node.conditions.Add(new Condition());
            }

            EditorGUILayout.LabelField("Actions:");
            for (int i = 0; i < node.actions.Count; i++)
            {
                var action = node.actions[i];
                EditorGUILayout.BeginHorizontal();
                action.variableName = EditorGUILayout.TextField("Variable Name:", action.variableName);
                action.value = EditorGUILayout.TextField("Value:", action.value);
                if (GUILayout.Button("Remove Action", GUILayout.Width(120)))
                {
                    node.actions.RemoveAt(i);
                }
                EditorGUILayout.EndHorizontal();
            }

            if (GUILayout.Button("Add Action", GUILayout.Width(100)))
            {
                node.actions.Add(new DialogueAction());
            }

            EditorGUILayout.LabelField("Events:");
            for (int i = 0; i < node.events.Count; i++)
            {
                var eventTrigger = node.events[i];
                EditorGUILayout.BeginHorizontal();
                eventTrigger.eventType = (EventTrigger.EventType)EditorGUILayout.EnumPopup("Event Type:", eventTrigger.eventType);
                eventTrigger.parameter = EditorGUILayout.TextField("Parameter:", eventTrigger.parameter);
                if (GUILayout.Button("Remove Event", GUILayout.Width(120)))
                {
                    node.events.RemoveAt(i);
                }
                EditorGUILayout.EndHorizontal();
            }

            if (GUILayout.Button("Add Event", GUILayout.Width(100)))
            {
                node.events.Add(new EventTrigger());
            }

            if (GUILayout.Button("Remove Node", GUILayout.Width(120)))
            {
                currentDialogueTree.nodes.Remove(node);
                return;
            }

            EditorGUILayout.EndVertical();
        }

        private void DrawHorizontalLine()
        {
            EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);
        }
    }
}
