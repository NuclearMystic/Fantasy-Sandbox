using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

namespace DialogueSystem
{
    public class DialogueDataManager : MonoBehaviour
    {
        // If you want your dialogue options and conditions to persist between game sessions, you will need to decided on a way
        // to handle saving and loading in your game. A few of the most common methods have been provided here. If your
        // game uses a different way of handling saving and loading, you will have to adapt the scripts to work for
        // your method. To use the provided methods, you can attach this script to a gameobject (I recommend having
        // it on the dialogue system prefab in your scene, or where ever you have the dialogue manager setup), then
        // choose which method you wish to use from the drop down in the inspector, and just call the SaveGameVariables 
        // or LoadGameVariables method below. This will save a copy of the dictionary containing all the current
        // conditions and actions the player has completed or interacted with.

        public enum SaveMethod { PlayerPrefs, JSON, Binary, ScriptableObject }
        
        [Tooltip("Choose which method your game uses for saving and loading conditions/actions for dialogue.")]
        public SaveMethod saveMethod;

        [Tooltip("This is only required if you are using the scriptable object method of saving/loading.")]
        public GameVariables gameVariablesScriptableObject; // For ScriptableObject method
        private DialogueManager dialogueManager;
        private string filePathJson;
        private string filePathBinary;

        void Start()
        {
            dialogueManager = FindObjectOfType<DialogueManager>();
            filePathJson = Path.Combine(Application.persistentDataPath, "gameData.json");
            filePathBinary = Path.Combine(Application.persistentDataPath, "gameData.dat");

            if (dialogueManager == null)
            {
                Debug.LogError("DialogueManager not found in the scene.");
            }
            else
            {
                LoadGameVariables();
            }
        }

        public void SaveGameVariables()
        {
            switch (saveMethod)
            {
                case SaveMethod.PlayerPrefs:
                    SaveToPlayerPrefs();
                    break;
                case SaveMethod.JSON:
                    SaveToJson();
                    break;
                case SaveMethod.Binary:
                    SaveToBinary();
                    break;
                case SaveMethod.ScriptableObject:
                    SaveToScriptableObject();
                    break;
            }
        }

        public void LoadGameVariables()
        {
            switch (saveMethod)
            {
                case SaveMethod.PlayerPrefs:
                    LoadFromPlayerPrefs();
                    break;
                case SaveMethod.JSON:
                    LoadFromJson();
                    break;
                case SaveMethod.Binary:
                    LoadFromBinary();
                    break;
                case SaveMethod.ScriptableObject:
                    LoadFromScriptableObject();
                    break;
            }
        }

        private void SaveToPlayerPrefs()
        {
            foreach (var kvp in dialogueManager.gameVariables)
            {
                PlayerPrefs.SetString(kvp.Key, kvp.Value);
            }
            PlayerPrefs.Save();
            Debug.Log("Game variables saved using PlayerPrefs.");
        }

        private void LoadFromPlayerPrefs()
        {
            dialogueManager.gameVariables.Clear();
            foreach (var kvp in PlayerPrefs.GetString("GameVariables", "").Split(';'))
            {
                if (!string.IsNullOrEmpty(kvp))
                {
                    var pair = kvp.Split('=');
                    dialogueManager.gameVariables.Add(pair[0], pair[1]);
                }
            }
            Debug.Log("Game variables loaded using PlayerPrefs.");
        }

        private void SaveToJson()
        {
            string json = JsonUtility.ToJson(new SerializableDictionary<string, string>(dialogueManager.gameVariables));
            File.WriteAllText(filePathJson, json);
            Debug.Log("Game variables saved to " + filePathJson);
        }

        private void LoadFromJson()
        {
            if (File.Exists(filePathJson))
            {
                string json = File.ReadAllText(filePathJson);
                SerializableDictionary<string, string> data = JsonUtility.FromJson<SerializableDictionary<string, string>>(json);
                dialogueManager.gameVariables = data.ToDictionary();
                Debug.Log("Game variables loaded from " + filePathJson);
            }
        }

        private void SaveToBinary()
        {
            BinaryFormatter formatter = new BinaryFormatter();
            using (FileStream stream = new FileStream(filePathBinary, FileMode.Create))
            {
                formatter.Serialize(stream, dialogueManager.gameVariables);
            }
            Debug.Log("Game variables saved to " + filePathBinary);
        }

        private void LoadFromBinary()
        {
            if (File.Exists(filePathBinary))
            {
                BinaryFormatter formatter = new BinaryFormatter();
                using (FileStream stream = new FileStream(filePathBinary, FileMode.Open))
                {
                    dialogueManager.gameVariables = (Dictionary<string, string>)formatter.Deserialize(stream);
                }
                Debug.Log("Game variables loaded from " + filePathBinary);
            }
        }

        private void SaveToScriptableObject()
        {
            gameVariablesScriptableObject.variables = new Dictionary<string, string>(dialogueManager.gameVariables);
            Debug.Log("Game variables saved to ScriptableObject");
        }

        private void LoadFromScriptableObject()
        {
            dialogueManager.gameVariables = new Dictionary<string, string>(gameVariablesScriptableObject.variables);
            Debug.Log("Game variables loaded from ScriptableObject");
        }
    }

    [Serializable]
    public class SerializableDictionary<TKey, TValue>
    {
        public List<TKey> keys = new List<TKey>();
        public List<TValue> values = new List<TValue>();

        public SerializableDictionary(Dictionary<TKey, TValue> dictionary)
        {
            foreach (var kvp in dictionary)
            {
                keys.Add(kvp.Key);
                values.Add(kvp.Value);
            }
        }

        public Dictionary<TKey, TValue> ToDictionary()
        {
            Dictionary<TKey, TValue> result = new Dictionary<TKey, TValue>();
            for (int i = 0; i < keys.Count; i++)
            {
                result[keys[i]] = values[i];
            }
            return result;
        }
    }

}
