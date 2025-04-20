using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public static class SpawnUtility
{
    public static void SpawnPrefabsOnTerrain(PrefabSpawnRule rule, NatureSpawner spawner, Transform parentContainer)
    {
        Terrain terrain = spawner.targetTerrain;
        TerrainData data = terrain.terrainData;

        Vector3 terrainSize = data.size;
        Vector3 terrainPos = terrain.transform.position;

        float area = terrainSize.x * terrainSize.z;
        float adjustedDensity = rule.density * spawner.globalDensityMultiplier;
        int estimatedInstances = Mathf.RoundToInt((area / 100f) * adjustedDensity);

        List<Vector3> candidates = GenerateSamplePoints(rule, terrain, estimatedInstances);

        int placed = 0;
        foreach (var pos in candidates)
        {
            if (IsValidSample(pos, rule, terrain))
            {
                Vector3 placement = pos;
                placement.y = terrain.SampleHeight(placement) + terrainPos.y - rule.sinkAmount;

                Quaternion rotation = Quaternion.identity;
                if (rule.randomYRotation)
                    rotation = Quaternion.Euler(0f, Random.Range(0f, 360f), 0f);

                if (rule.alignToTerrainNormal)
                {
                    Vector3 normal = data.GetInterpolatedNormal(
                        (placement.x - terrainPos.x) / terrainSize.x,
                        (placement.z - terrainPos.z) / terrainSize.z
                    );
                    rotation = Quaternion.LookRotation(Vector3.Cross(rotation * Vector3.right, normal), normal);
                }

                float scale = Random.Range(rule.scaleRange.x, rule.scaleRange.y);
                GameObject instance = (GameObject)PrefabUtility.InstantiatePrefab(rule.prefab);
                instance.transform.position = placement;
                instance.transform.rotation = rotation;
                instance.transform.localScale = Vector3.one * scale;

                // Parent to optional parent if defined, otherwise to the container
                if (rule.optionalParentObject != null)
                    instance.transform.SetParent(rule.optionalParentObject);
                else if (parentContainer != null)
                    instance.transform.SetParent(parentContainer);

                placed++;
            }
        }

        if (spawner.enableDebug)
            Debug.Log($"[SpawnUtility] Placed {placed} instances of {rule.prefab.name}");
    }

    private static List<Vector3> GenerateSamplePoints(PrefabSpawnRule rule, Terrain terrain, int count)
    {
        Vector3 size = terrain.terrainData.size;
        Vector3 origin = terrain.transform.position;
        List<Vector3> points = new List<Vector3>();

        float spacing = rule.minDistance;

        switch (rule.spawnAlgorithm)
        {
            case SpawnAlgorithm.RandomScatter:
                for (int i = 0; i < count; i++)
                {
                    float x = Random.Range(0f, size.x);
                    float z = Random.Range(0f, size.z);
                    points.Add(new Vector3(origin.x + x, 0f, origin.z + z));
                }
                break;

            case SpawnAlgorithm.JitteredGrid:
                for (float x = 0; x < size.x; x += spacing)
                {
                    for (float z = 0; z < size.z; z += spacing)
                    {
                        float offsetX = Random.Range(-spacing / 2f, spacing / 2f);
                        float offsetZ = Random.Range(-spacing / 2f, spacing / 2f);
                        points.Add(new Vector3(origin.x + x + offsetX, 0f, origin.z + z + offsetZ));
                    }
                }
                break;

            case SpawnAlgorithm.Clustered:
                int clusterCount = Mathf.Max(1, Mathf.RoundToInt(count * 0.25f));
                for (int i = 0; i < clusterCount; i++)
                {
                    Vector3 clusterCenter = new Vector3(
                        origin.x + Random.Range(0f, size.x),
                        0f,
                        origin.z + Random.Range(0f, size.z)
                    );

                    int clusterSize = count / clusterCount;
                    for (int j = 0; j < clusterSize; j++)
                    {
                        float x = clusterCenter.x + Random.Range(-spacing, spacing);
                        float z = clusterCenter.z + Random.Range(-spacing, spacing);
                        points.Add(new Vector3(
                            Mathf.Clamp(x, origin.x, origin.x + size.x),
                            0f,
                            Mathf.Clamp(z, origin.z, origin.z + size.z)
                        ));
                    }
                }
                break;

            case SpawnAlgorithm.PoissonDisk:
                points = PoissonDiskSampler.SampleTerrain(terrain, rule.minDistance);
                break;
        }

        return points;
    }

    private static bool IsValidSample(Vector3 worldPos, PrefabSpawnRule rule, Terrain terrain)
    {
        Vector3 terrainPos = terrain.transform.position;
        TerrainData data = terrain.terrainData;
        Vector3 size = data.size;

        float normX = (worldPos.x - terrainPos.x) / size.x;
        float normZ = (worldPos.z - terrainPos.z) / size.z;

        float height = terrain.SampleHeight(worldPos) + terrainPos.y;
        float slope = data.GetSteepness(normX, normZ);
        int xMap = Mathf.FloorToInt(normX * data.alphamapWidth);
        int zMap = Mathf.FloorToInt(normZ * data.alphamapHeight);

        if (height < rule.minHeight || height > rule.maxHeight)
            return false;

        if (slope < rule.minSlope || slope > rule.maxSlope)
            return false;

        float[,,] alpha = data.GetAlphamaps(xMap, zMap, 1, 1);
        float maxLayerStrength = 0f;
        foreach (int index in rule.validTextureIndices)
        {
            if (index >= 0 && index < data.alphamapLayers)
            {
                float strength = alpha[0, 0, index];
                if (rule.scaleWithTextureStrength)
                {
                    if (Random.value > strength)
                        return false;
                }
                else if (strength > maxLayerStrength)
                {
                    maxLayerStrength = strength;
                }
            }
        }

        if (!rule.scaleWithTextureStrength && maxLayerStrength <= 0f)
            return false;

        if (rule.enableCollisionCheck)
        {
            if (Physics.CheckBox(worldPos, rule.collisionBoxSize * 0.5f, Quaternion.identity, rule.collisionMask))
                return false;
        }

        return true;
    }
}
