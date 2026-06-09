using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class PathFindingGrid : MonoBehaviour
{
    public static PathFindingGrid Instance;

    public List<Tilemap> tilemaps;

    public int gridWidth;
    public int gridHeight;

    private GridTile[,,] grid; // [layer, x, y]

    public List<(Tilemap map, int layer)> layers = new List<(Tilemap, int)>();

    private void Awake()
    {
        Instance = this;

        // Build layer mapping for Movement.cs
        layers.Clear();
        for (int i = 0; i < tilemaps.Count; i++)
        {
            layers.Add((tilemaps[i], i));
        }

        BuildGrid();
    }
    private void BuildGrid()
    {
        int layers = tilemaps.Count;
        grid = new GridTile[layers, gridWidth, gridHeight];

        for (int layer = 0; layer < layers; layer++)
        {
            Tilemap tilemap = tilemaps[layer];

            for (int x = 0; x < gridWidth; x++)
            {
                for (int y = 0; y < gridHeight; y++)
                {
                    Vector3Int cellPos = new Vector3Int(x, y, 0);
                    TileBase tile = tilemap.GetTile(cellPos);

                    GridTile t = new GridTile();
                    t.x = x;
                    t.y = y;
                    t.layer = layer;

                    t.walkable = tile != null;

                    t.neighbors = new List<GridTile>();

                    grid[layer, x, y] = t;
                }
            }
        }

        GenerateNeighbors();
    }

    private void GenerateNeighbors()
    {
        int layers = tilemaps.Count;

        for (int layer = 0; layer < layers; layer++)
        {
            for (int x = 0; x < gridWidth; x++)
            {
                for (int y = 0; y < gridHeight; y++)
                {
                    GridTile tile = grid[layer, x, y];
                    if (tile == null) continue;

                    // 4-direction neighbors
                    TryAddNeighbor(tile, x + 1, y, layer);
                    TryAddNeighbor(tile, x - 1, y, layer);
                    TryAddNeighbor(tile, x, y + 1, layer);
                    TryAddNeighbor(tile, x, y - 1, layer);
                }
            }
        }
    }

    private void TryAddNeighbor(GridTile tile, int x, int y, int layer)
    {
        GridTile n = GetNode(layer, x, y);
        if (n != null && n.walkable)
            tile.neighbors.Add(n);
    }

    public GridTile GetNode(int layer, int x, int y)
    {
        if (layer < 0 || layer >= tilemaps.Count)
            return null;

        if (x < 0 || x >= gridWidth || y < 0 || y >= gridHeight)
            return null;

        return grid[layer, x, y];
    }

    public GridTile GetNodeFromWorld(int layer, Vector3 worldPos)
    {
        Tilemap tilemap = tilemaps[layer];
        Vector3Int cell = tilemap.WorldToCell(worldPos);

        int gx = cell.x - tilemap.cellBounds.min.x;
        int gy = cell.y - tilemap.cellBounds.min.y;

        return GetNode(layer, gx, gy);
    }
}
