using System.Collections.Generic;
using UnityEngine;

public static class PoissonDiskSampler
{
    public static List<Vector3> SampleTerrain(Terrain terrain, float radius, int maxAttempts = 30)
    {
        TerrainData data = terrain.terrainData;
        Vector3 size = data.size;
        Vector3 origin = terrain.transform.position;

        return GeneratePoints(
            radius,
            new Vector2(origin.x, origin.z),
            new Vector2(origin.x + size.x, origin.z + size.z),
            maxAttempts
        );
    }

    private static List<Vector3> GeneratePoints(float radius, Vector2 bottomLeft, Vector2 topRight, int maxAttempts)
    {
        float cellSize = radius / Mathf.Sqrt(2);
        int gridWidth = Mathf.CeilToInt((topRight.x - bottomLeft.x) / cellSize);
        int gridHeight = Mathf.CeilToInt((topRight.y - bottomLeft.y) / cellSize);

        Vector2[,] grid = new Vector2[gridWidth, gridHeight];
        List<Vector3> points = new List<Vector3>();
        List<Vector2> spawnPoints = new List<Vector2>();

        Vector2 startPoint = new Vector2(
            Random.Range(bottomLeft.x, topRight.x),
            Random.Range(bottomLeft.y, topRight.y)
        );

        spawnPoints.Add(startPoint);
        points.Add(new Vector3(startPoint.x, 0f, startPoint.y));

        while (spawnPoints.Count > 0)
        {
            int spawnIndex = Random.Range(0, spawnPoints.Count);
            Vector2 spawnCenter = spawnPoints[spawnIndex];
            bool accepted = false;

            for (int i = 0; i < maxAttempts; i++)
            {
                float angle = Random.value * Mathf.PI * 2;
                float dist = Random.Range(radius, 2 * radius);
                Vector2 dir = new Vector2(Mathf.Cos(angle), Mathf.Sin(angle));
                Vector2 candidate = spawnCenter + dir * dist;

                if (candidate.x >= bottomLeft.x && candidate.x < topRight.x &&
                    candidate.y >= bottomLeft.y && candidate.y < topRight.y)
                {

                    int cellX = (int)((candidate.x - bottomLeft.x) / cellSize);
                    int cellY = (int)((candidate.y - bottomLeft.y) / cellSize);

                    bool tooClose = false;
                    for (int x = Mathf.Max(0, cellX - 2); x <= Mathf.Min(cellX + 2, gridWidth - 1); x++)
                    {
                        for (int y = Mathf.Max(0, cellY - 2); y <= Mathf.Min(cellY + 2, gridHeight - 1); y++)
                        {
                            if (grid[x, y] != Vector2.zero && (grid[x, y] - candidate).sqrMagnitude < radius * radius)
                            {
                                tooClose = true;
                                break;
                            }
                        }
                        if (tooClose) break;
                    }

                    if (!tooClose)
                    {
                        grid[cellX, cellY] = candidate;
                        spawnPoints.Add(candidate);
                        points.Add(new Vector3(candidate.x, 0f, candidate.y));
                        accepted = true;
                        break;
                    }
                }
            }

            if (!accepted)
                spawnPoints.RemoveAt(spawnIndex);
        }

        return points;
    }
}
