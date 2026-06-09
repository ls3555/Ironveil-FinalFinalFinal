using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class PathFindingGrid : MonoBehaviour
{
    public static PathFindingGrid Instance;

    [Header("All Tilemaps in the Scene (children of GridLayer parents)")]
    public List<Tilemap> tilemaps;

    // Grid stored using tilemap-local coordinates
    private Dictionary<(int layer, Vector3Int cell), GridTile> grid 
        = new Dictionary<(int, Vector3Int), GridTile>();

    // For layer detection
    public List<(Tilemap map, int layer)> layers = new();

    private void Awake()
    {
        Instance = this;

        layers.Clear();

        // Build layer mapping using GridLayer components
        foreach (Tilemap map in tilemaps)
        {
            GridLayer gl = map.GetComponentInParent<GridLayer>();
            if (gl == null)
            {
                Debug.LogError($"Tilemap {map.name} has no GridLayer parent!");
                continue;
            }

            int layerNumber = gl.layerNumber; // 1, 2, 3...
            layers.Add((map, layerNumber));
        }

        BuildGrid();
    }

    private void BuildGrid()
    {
        grid.Clear();

        foreach (var entry in layers)
        {
            Tilemap map = entry.map;
            int layer = entry.layer;

            BoundsInt bounds = map.cellBounds;

            foreach (var cell in bounds.allPositionsWithin)
            {
                TileBase tile = map.GetTile(cell);

                GridTile t = new GridTile();
                t.x = cell.x;
                t.y = cell.y;
                t.layer = layer;
                t.walkable = tile != null;
                t.neighbors = new List<GridTile>();

                grid[(layer, cell)] = t;
            }
        }

        GenerateNeighbors();
    }

    private void GenerateNeighbors()
    {
        foreach (var kvp in grid)
        {
            GridTile tile = kvp.Value;
            int layer = tile.layer;

            Vector3Int[] dirs = new Vector3Int[]
            {
                new Vector3Int(1, 0, 0),
                new Vector3Int(-1, 0, 0),
                new Vector3Int(0, 1, 0),
                new Vector3Int(0, -1, 0)
            };

            foreach (var d in dirs)
            {
                Vector3Int neighborCell = new Vector3Int(tile.x, tile.y, 0) + d;

                if (grid.TryGetValue((layer, neighborCell), out GridTile n))
                {
                    if (n.walkable)
                        tile.neighbors.Add(n);
                }
            }
        }
    }

    public GridTile GetNodeFromWorld(int layer, Vector3 worldPos)
    {
        foreach (var entry in layers)
        {
            if (entry.layer != layer) continue;

            Tilemap map = entry.map;
            Vector3Int cell = map.WorldToCell(worldPos);

            if (grid.TryGetValue((layer, cell), out GridTile node))
                return node;
        }

        return null;
    }
}

