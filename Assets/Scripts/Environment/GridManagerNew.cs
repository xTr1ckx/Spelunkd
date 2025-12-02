using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum CellType
{
    Empty,
    Destructible,
    Indestructible,
    Ore
}

public class GridCell
{
    public CellType type;
    public GameObject instance;
    public TileBlockNew tile;

    public GridCell(CellType type)
    {
        this.type = type;
        instance = null;
        tile = null;
    }
}

public class StructureCell
{
    public GameObject instance;

    public StructureCell()
    {
        instance = null;
    }
}

public class GridManagerNew : MonoBehaviour
{
    [Header("Grid Settings")]
    public int width = 50;
    public int height = 30;
    public float tileSize = 2.4f;
    public int currentLevel = 1;

    [Header("Tile Prefabs")]
    public GameObject destructibleTilePrefab;
    public GameObject indestructibleTilePrefab;

    [Header("Ore Data")]
    public OreData[] ores;

    [Header("Structures")]
    public GameObject ladderPrefab;
    public GameObject pillarPrefab;

    private GridCell[,] grid;
    private StructureCell[,] structureGrid;

    private void Start()
    {
        grid = new GridCell[width, height];
        structureGrid = new StructureCell[width, height];

        for(int x = 0; x < width; x++)
            for (int y = 0; y < height; y++)
                structureGrid[x, y] = new StructureCell();

        GenerateTerrain();
        GenerateOres();
        SpawnInitialTiles();
    }

    void GenerateTerrain()
    {
        float noiseScale = 10f;
        float heightMultiplier = 10f;
        float depthLimit = 5f;

        for (int x = 0; x < width; x++)
        {
            float perlinValue = Mathf.PerlinNoise(x / noiseScale, 0);
            int terrainHeight = Mathf.FloorToInt(perlinValue * heightMultiplier) + (height / 3);

            for (int y = 0; y <= terrainHeight; y++)
            {
                bool isBedrock = y < terrainHeight - depthLimit;

                grid[x, y] = new GridCell(isBedrock ? CellType.Indestructible : CellType.Destructible);
            }
        }
    }

    void GenerateOres()
    {
        foreach (OreData ore in ores)
        {
            if (currentLevel < ore.minLevel || currentLevel > ore.maxLevel)
                continue;

            for (int x = 0; x < width; x++)
                for (int y = ore.minDepth; y <= ore.maxDepth; y++)
                {
                    if (grid[x, y] == null) continue;

                    if (grid[x, y].type == CellType.Indestructible) continue;

                    if (Random.value < ore.spawnChance)
                        grid[x, y].type = CellType.Ore;
                }
        }
    }

    void SpawnInitialTiles()
    {
        for(int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                SpawnTileAt(x, y);
            }
        }
    }

    void SpawnTileAt(int x, int y)
    {
        GridCell cell = grid[x, y];
        if (cell == null || cell.type == CellType.Empty) return;

        GameObject prefab = null;

        switch (cell.type)
        {
            case CellType.Destructible: prefab = destructibleTilePrefab; break;

            case CellType.Indestructible: prefab = indestructibleTilePrefab; break;

            case CellType.Ore: prefab = ores[0].orePrefab; break;
        }

        if(prefab == null) return;

        Vector2 pos = new Vector2(x * tileSize, y * tileSize);
        GameObject obj = Instantiate(prefab, pos, Quaternion.identity, transform);

        cell.instance = obj;
        cell.tile = obj.GetComponent<TileBlockNew>();
        cell.tile.gridX = x;
        cell.tile.gridY = y;
        cell.tile.gridManager = this;
    }

    public void RemoveTileAt(int x, int y)
    {
        if (grid[x, y] == null) return;

        if (grid[x, y].instance != null)
            Destroy(grid[x, y].instance);

        grid[x, y] = new GridCell(CellType.Empty);
    }

    public void MakeTileFall(int x, int y)
    {
        GridCell cell = grid[x, y];
        if(cell == null || cell.instance == null) return;

        GameObject falling = cell.instance;
        cell.instance = null;
        cell.tile = null;
        cell.type = CellType.Empty;

        Rigidbody2D rb = falling.AddComponent<Rigidbody2D>();
        rb.gravityScale = 3f;
        rb.constraints = RigidbodyConstraints2D.FreezeRotation;
    }

    public bool IsSupported(int x, int y)
    {
        if (y == 0) return true;
        if (grid[x, y - 1] == null) return false;
        return grid[x, y - 1].type != CellType.Empty;
    }

    public void PlaceLadder(Vector2 worldPos)
    {
        Vector2Int gp = ToGrid(worldPos);

        if (!IsInside(gp)) return;
        if (grid[gp.x, gp.y].type != CellType.Empty) return;
        if (!IsSupported(gp.x, gp.y)) return;

        Vector2 spawnPos = ToWorld(gp);
        GameObject obj = Instantiate(ladderPrefab, spawnPos, Quaternion.identity);

        structureGrid[gp.x, gp.y].instance = obj;
    }

    public void PlacePillar(Vector2 worldPos)
    {
        Vector2Int gp = ToGrid(worldPos);

        if (!IsInside(gp)) return;
        if (grid[gp.x, gp.y].type != CellType.Empty) return;
        if (!IsSupported(gp.x, gp.y)) return;

        Vector2 spawnPos = ToWorld(gp);
        GameObject obj = Instantiate(pillarPrefab, spawnPos, Quaternion.identity);

        structureGrid[gp.x, gp.y].instance = obj;
    }

    public bool IsInside(Vector2Int gp) => gp.x >= 0 && gp.x < width && gp.y >= 0 && gp.y < height;

    public Vector2Int ToGrid(Vector2 worldPos)
    {
        int x = Mathf.RoundToInt(worldPos.x / tileSize);
        int y = Mathf.RoundToInt(worldPos.y / tileSize);
        return new Vector2Int(x, y);
    }

    public Vector2 ToWorld(Vector2Int gridPos) => new Vector2(gridPos.x * tileSize, gridPos.y * tileSize);
}
