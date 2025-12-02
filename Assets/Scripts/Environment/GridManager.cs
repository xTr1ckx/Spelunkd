using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using Unity.VisualScripting.Antlr3.Runtime.Tree;
using UnityEngine;

public class GridManager : MonoBehaviour
{
    [Header("Grid Settings")]
    public int width = 50;
    public int height = 30;
    public float tileSize = 2.4f;
    public int currentLevel = 1;

    //[Header("Perlin Noise Settings")]
    //public float noiseScale = 10f;
    //public float heightMultiplier = 10f;
    //public float limitDepth = 5f;

    [Header("Tile Prefabs")]
    public GameObject[] destructibleTilePrefab;

    [Header("Ore Data")]
    public OreData[] ores;

    [Header("Structures")]
    public GameObject ladderPrefab;
    public GameObject pillarPrefab;

    private TileBlock[,] grid;
    private GameObject[,] structureGrid;
    private List<Vector2Int> destructiblePositions = new List<Vector2Int>();

    private Vector2 gridOffset;

    private void Start()
    {
        gridOffset = new Vector2(tileSize * 0.5f, tileSize * 0.5f);
        GenerateGrid();
        GenerateOres();
        structureGrid = new GameObject[width, height];
    }

    void GenerateGrid()
    {
        grid = new TileBlock[width, height];
        destructiblePositions.Clear();

        int terrainHeight = height - 7;

        for (int x = 0; x < width; x++)
        {
            for(int y = 0; y < height; y++)
            {
                if (y > terrainHeight)
                    continue;

                GameObject prefab = GetRandom(destructibleTilePrefab);

                Vector2 position = new Vector2(x * tileSize, y * tileSize) + gridOffset;
                GameObject tileObject = Instantiate(prefab, position, Quaternion.identity, transform);

                TileBlock tile = tileObject.GetComponent<TileBlock>();
                grid[x, y] = tile;

                destructiblePositions.Add(new Vector2Int(x, y));
            }
        }
    }

    void GenerateOres()
    {
        foreach (OreData ore in ores)
        {
            if (currentLevel < ore.minLevel || currentLevel > ore.maxLevel)
                continue;

            float levelMultiplier = 1f + ((currentLevel - ore.minLevel) * ore.spawnMultiplierPerLevel);
            float adjustedChance = ore.spawnChance * levelMultiplier;

            int spawnedCount = 0;

            foreach (Vector2Int pos in destructiblePositions)
            {
                int y = pos.y;

                if (y < ore.minDepth || y > ore.maxDepth) continue;

                if (Random.value < adjustedChance)
                {
                    SpawnOre(ore, pos);
                    spawnedCount++;
                }
            }

            while (spawnedCount < ore.minSpawnCount)
            {
                Vector2Int randomPos = destructiblePositions[Random.Range(0, destructiblePositions.Count)];
                SpawnOre(ore, randomPos);
                spawnedCount++;
            }

            Debug.Log($"Spawned {spawnedCount} of {ore.oreName}");
        }
    }

    void SpawnOre(OreData ore, Vector2Int pos)
    {
        TileBlock oldTile = grid[pos.x, pos.y];
        if (oldTile != null)
        {
            Destroy(oldTile.gameObject);
        }

        Vector2 spawnPos = new Vector2(pos.x * tileSize, pos.y * tileSize) + gridOffset;
        GameObject oreObject = Instantiate(ore.orePrefab, spawnPos, Quaternion.identity, transform);
        grid[pos.x, pos.y] = oreObject.GetComponent<TileBlock>();
    }

    GameObject GetRandom(GameObject[] array)
    {
        return array[Random.Range(0, array.Length)];
    }

    public Vector2Int GetClosestGridPosition(Vector2 worldPos)
    {
        int x = Mathf.RoundToInt(worldPos.x / tileSize);
        int y = Mathf.RoundToInt(worldPos.y / tileSize);
        x = Mathf.Clamp(x, 0, width - 1);
        y = Mathf.Clamp(y, 0, height - 1);
        return new Vector2Int(x, y);
    }

    public Vector2 GetWorldPosition(Vector2Int gridPos)
    {
        return new Vector2(gridPos.x * tileSize, gridPos.y * tileSize) + gridOffset;
    }

    public void PlaceLadder(Vector2 worldPos)
    {
        Vector2Int gridPos = GetClosestGridPosition(worldPos);

        if (structureGrid[gridPos.x, gridPos.y] != null)
        {
            Debug.Log("Ladder already exists here");
            return;
        }

        if (grid[gridPos.x, gridPos.y] != null)
        {
            Debug.Log("Cannot place ladder inside solid tile");
            return;
        }

        Vector2Int belowPos = new Vector2Int(gridPos.x, gridPos.y - 1);
        bool hasSupportBelow = false;

        if (belowPos.y >= 0)
        {
            if (grid[belowPos.x, belowPos.y] != null) hasSupportBelow = true;
            else if (structureGrid[belowPos.x, belowPos.y] != null) hasSupportBelow = true;
        }

        if (!hasSupportBelow)
        {
            Debug.Log("Cannot place ladder in air!");
            return;
        }

        Vector2 spawnPos = GetWorldPosition(gridPos);
        GameObject ladder = Instantiate(ladderPrefab, spawnPos, Quaternion.identity, transform);
        structureGrid[gridPos.x, gridPos.y] = ladder;

        Debug.Log($"Placed ladder at {gridPos}");
    }

    public bool IsLadderAt(Vector2Int gridPos)
    {
        if (gridPos.x < 0 || gridPos.x >= width || gridPos.y < 0 || gridPos.y >= height) return false;
        return structureGrid[gridPos.x, gridPos.y] != null;
    }

    public void PlacePillar(Vector2 worldPos)
    {
        Vector2Int gridPos = GetClosestGridPosition(worldPos);

        if (structureGrid[gridPos.x, gridPos.y] != null)
        {
            Debug.Log("Pillar already exists here");
            return;
        }

        if (grid[gridPos.x, gridPos.y] != null)
        {
            Debug.Log("Cannot place pillar inside solid tile");
            return;
        }

        Vector2Int belowPos = new Vector2Int(gridPos.x, gridPos.y - 1);
        bool supported = false;

        if (belowPos.y >= 0)
        {
            if (grid[belowPos.x, belowPos.y] != null) supported = true;
            else if (structureGrid[belowPos.x, belowPos.y] != null) supported = true;
        }

        if (!supported)
        {
            Debug.Log("Cannot place pillar in air!");
            return;
        }

        Vector2 spawnPos = GetWorldPosition(gridPos);
        GameObject pillar = Instantiate(pillarPrefab, spawnPos, Quaternion.identity, transform);
        structureGrid[gridPos.x, gridPos.y] = pillar;

        pillar.tag = "Pillar";

        Debug.Log($"Placed pillar at {gridPos}");
    }

    public bool IsTileAt(Vector2Int gridPos)
    {
        if (gridPos.x < 0 || gridPos.x >= width || gridPos.y < 0 || gridPos.y >= height)
            return false;
        return grid[gridPos.x, gridPos.y] != null;
    }

    public bool IsPillarAt(Vector2Int gridPos)
    {
        if (gridPos.x < 0 || gridPos.x >= width || gridPos.y < 0 || gridPos.y >= height)
            return false;
        return structureGrid[gridPos.x, gridPos.y] != null;
    }

    //public void RemoveStructureAt(Vector2Int gridPos)
    //{
    //    if (structureGrid[gridPos.x, gridPos.y] != null)
    //        structureGrid[gridPos.x, gridPos.y] = null;
    //}
}
