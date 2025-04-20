using UnityEngine;
using System.Collections.Generic;

[ExecuteInEditMode]
public class NatureSpawner : MonoBehaviour
{
    [Header("General Settings")]
    public Terrain targetTerrain;
    public int randomSeed = 1337;

    [Tooltip("Global multiplier applied to all spawn rule densities.")]
    public float globalDensityMultiplier = 1f;

    [Tooltip("Enable debug messages in console.")]
    public bool enableDebug = false;

    [Header("Spawn Rules")]
    public List<PrefabSpawnRule> spawnRules = new List<PrefabSpawnRule>();
    public static string ContainerName = "NatureSpawner_Spawned";

    public void SpawnAllPrefabs()
    {
        if (targetTerrain == null)
        {
            Debug.LogWarning("NatureSpawner: No terrain assigned.");
            return;
        }

        if (spawnRules == null || spawnRules.Count == 0)
        {
            Debug.LogWarning("NatureSpawner: No spawn rules defined.");
            return;
        }

        ClearPreviouslySpawned();
        Random.InitState(randomSeed);

        // Create a parent container for all spawned objects
        GameObject container = new GameObject(ContainerName);
        container.transform.SetParent(this.transform);

        foreach (var rule in spawnRules)
        {
            if (rule == null || rule.prefab == null) continue;

            if (enableDebug)
                Debug.Log($"[NatureSpawner] Spawning '{rule.prefab.name}' using {rule.spawnAlgorithm}");

            // Pass the container to SpawnUtility
            SpawnUtility.SpawnPrefabsOnTerrain(rule, this, container.transform);
        }

        Debug.Log("NatureSpawner: Spawning complete.");
    }

    public void ClearPreviouslySpawned()
    {
        Transform existing = transform.Find(ContainerName);
        if (existing != null)
            Object.DestroyImmediate(existing.gameObject);
    }

}
