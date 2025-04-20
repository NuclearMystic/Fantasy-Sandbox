using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(NatureSpawner))]
public class NatureSpawnerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        NatureSpawner spawner = (NatureSpawner)target;

        EditorGUILayout.LabelField("Nature Spawner Tool", EditorStyles.boldLabel);
        EditorGUILayout.Space();

        spawner.targetTerrain = (Terrain)EditorGUILayout.ObjectField("Target Terrain", spawner.targetTerrain, typeof(Terrain), true);
        spawner.randomSeed = EditorGUILayout.IntField("Random Seed", spawner.randomSeed);
        spawner.globalDensityMultiplier = EditorGUILayout.FloatField("Global Density Multiplier", spawner.globalDensityMultiplier);

        EditorGUILayout.Space();
        spawner.enableDebug = EditorGUILayout.Toggle("Enable Debug Logs", spawner.enableDebug);

        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Spawn Rules", EditorStyles.boldLabel);
        SerializedProperty ruleList = serializedObject.FindProperty("spawnRules");
        EditorGUILayout.PropertyField(ruleList, true);

        EditorGUILayout.Space();
        EditorGUILayout.HelpBox("Click the button below to scatter prefabs according to all defined rules. This will overwrite any previously spawned objects.", MessageType.Info);

        GUI.backgroundColor = Color.green;
        if (GUILayout.Button("Spawn All Prefabs"))
        {
            spawner.SpawnAllPrefabs();
        }
        GUI.backgroundColor = Color.white;

        serializedObject.ApplyModifiedProperties();
    }
}
