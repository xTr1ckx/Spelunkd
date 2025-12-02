using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class OreData
{
    public string oreName;
    public GameObject orePrefab;

    [Header("Spawn Settings")]
    [Range(0f, 1f)] public float spawnChance = 0.02f;
    public int minSpawnCount = 10;
    public int minDepth = 5;
    public int maxDepth = 25;

    [Header("Level Settings")]
    public int minLevel = 1;
    public int maxLevel = 5;

    [Header("Scaling")]
    [Range(0f, 2f)] public float spawnMultiplierPerLevel = 0.2f;
}
