using UnityEngine;
using System.Collections.Generic;

public enum SpawnAlgorithm
{
    JitteredGrid,
    PoissonDisk,
    Clustered,
    RandomScatter
}

[CreateAssetMenu(fileName = "New Prefab Spawn Rule", menuName = "Nature Spawner/Prefab Spawn Rule")]
public class PrefabSpawnRule : ScriptableObject
{
    [Header("Prefab Settings")]
    public GameObject prefab;
    public SpawnAlgorithm spawnAlgorithm = SpawnAlgorithm.PoissonDisk;

    [Tooltip("Number of objects per 100 square meters.")]
    public float density = 1f;

    public Vector2 scaleRange = new Vector2(1f, 1f);
    public bool randomYRotation = true;
    public bool alignToTerrainNormal = true;
    public float sinkAmount = 0f;

    [Tooltip("Minimum distance between instances when using Poisson Disk or Clustered sampling.")]
    public float minDistance = 5f;

    [Header("Terrain Filters")]
    public float minHeight = 0f;
    public float maxHeight = 1000f;

    public float minSlope = 0f;
    public float maxSlope = 90f;

    [Tooltip("Allowed texture indices (correspond to splatmap layers).")]
    public List<int> validTextureIndices = new List<int>();

    [Header("Texture Strength")]
    [Tooltip("If enabled, prefab spawn chance scales with the strength of the texture layer at each point.")]
    public bool scaleWithTextureStrength = true;

    [Header("Collision Settings")]
    public bool enableCollisionCheck = true;
    public Vector3 collisionBoxSize = new Vector3(1f, 1f, 1f);
    public LayerMask collisionMask;

    [Header("Editor Placement")]
    public Transform optionalParentObject;
}
