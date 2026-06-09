using System.Collections.Generic;
using UnityEngine;

public class AStarPathFinder
{
    public static List<GridTile> FindPath(GridTile startNode, GridTile targetNode)
    {
        List<GridTile> openSet = new List<GridTile>();
        HashSet<GridTile> closedSet = new HashSet<GridTile>();

        // Reset starting node
        startNode.gCost = 0;
        startNode.hCost = Heuristic(startNode, targetNode);
        startNode.parent = null;

        openSet.Add(startNode);

        while (openSet.Count > 0)
        {
            GridTile current = GetLowestFCost(openSet);

            // Reached target
            if (current == targetNode)
                return RetracePath(startNode, targetNode);

            openSet.Remove(current);
            closedSet.Add(current);

            foreach (GridTile neighbor in current.neighbors)
            {
                if (!neighbor.walkable || closedSet.Contains(neighbor))
                    continue;

                int tentativeG = current.gCost + Heuristic(current, neighbor);

                if (!openSet.Contains(neighbor) || tentativeG < neighbor.gCost)
                {
                    neighbor.gCost = tentativeG;
                    neighbor.hCost = Heuristic(neighbor, targetNode);
                    neighbor.parent = current;

                    if (!openSet.Contains(neighbor))
                        openSet.Add(neighbor);
                }
            }
        }

        return null; // No path found
    }

    private static int Heuristic(GridTile a, GridTile b)
    {
        int dx = Mathf.Abs(a.x - b.x);
        int dy = Mathf.Abs(a.y - b.y);
        return dx + dy; // Manhattan distance
    }

    private static GridTile GetLowestFCost(List<GridTile> list)
    {
        GridTile best = list[0];
        for (int i = 1; i < list.Count; i++)
        {
            if (list[i].fCost < best.fCost)
                best = list[i];
        }
        return best;
    }

    private static List<GridTile> RetracePath(GridTile startNode, GridTile endNode)
    {
        List<GridTile> path = new List<GridTile>();
        GridTile current = endNode;

        while (current != startNode)
        {
            path.Add(current);
            current = current.parent;
        }

        path.Add(startNode);
        path.Reverse();
        return path;
    }
}


/*
using System.Collections.Generic;
using UnityEngine;

public class AStarPathfinder : MonoBehaviour
{
    public static AStarPathfinder Instance;

    private void Awake()
    {
        Instance = this;
    }

    public List<Vector3> FindPath(
        int startLayer,
        Vector3 startWorldPos,
        int targetLayer,
        Vector3 targetWorldPos
    )
    {
        PathFindingGrid grid = PathFindingGrid.Instance;
        if (grid == null) return null;

        // Convert world positions to grid indices
        GridTile startNode = WorldToNode(grid, startLayer, startWorldPos);
        GridTile targetNode = WorldToNode(grid, targetLayer, targetWorldPos);

        if (startNode == null || targetNode == null)
            return null;

        if (!startNode.walkable || !targetNode.walkable)
            return null;

        List<GridTile> pathNodes = AStar(startNode, targetNode);
        if (pathNodes == null) return null;

        // Convert grid tiles back to world positions
        List<Vector3> path = new List<Vector3>();
        foreach (GridTile node in pathNodes)
        {
            Tilemap tilemap = grid.tilemaps[node.layer];
            Vector3Int cell = new Vector3Int(
                node.x + tilemap.cellBounds.min.x,
                node.y + tilemap.cellBounds.min.y,
                0
            );
            Vector3 world = tilemap.GetCellCenterWorld(cell);
            path.Add(world);
        }

        return path;
    }

    private GridTile WorldToNode(PathFindingGrid grid, int layer, Vector3 worldPos)
    {
        Tilemap tilemap = grid.tilemaps[layer];
        Vector3Int cell = tilemap.WorldToCell(worldPos);

        int gx = cell.x - tilemap.cellBounds.min.x;
        int gy = cell.y - tilemap.cellBounds.min.y;

        return grid.GetNode(layer, gx, gy);
    }

    private List<GridTile> AStar(GridTile startNode, GridTile targetNode)
    {
        HashSet<GridTile> closedSet = new HashSet<GridTile>();
        List<GridTile> openSet = new List<GridTile> { startNode };

        // reset costs
        ResetNode(startNode);
        ResetNode(targetNode);

        startNode.gCost = 0;
        startNode.hCost = Heuristic(startNode, targetNode);

        while (openSet.Count > 0)
        {
            GridTile current = GetLowestFCost(openSet);

            if (current == targetNode)
                return RetracePath(startNode, targetNode);

            openSet.Remove(current);
            closedSet.Add(current);

            foreach (GridTile neighbor in current.neighbors)
            {
                if (!neighbor.walkable || closedSet.Contains(neighbor))
                    continue;

                int tentativeG = current.gCost + Heuristic(current, neighbor);

                if (!openSet.Contains(neighbor) || tentativeG < neighbor.gCost)
                {
                    neighbor.gCost = tentativeG;
                    neighbor.hCost = Heuristic(neighbor, targetNode);
                    neighbor.parent = current;

                    if (!openSet.Contains(neighbor))
                        openSet.Add(neighbor);
                }
            }
        }

        return null;
    }

    private int Heuristic(GridTile a, GridTile b)
    {
        // Manhattan distance, ignoring layer (layer transitions are already in neighbors)
        int dx = Mathf.Abs(a.x - b.x);
        int dy = Mathf.Abs(a.y - b.y);
        return dx + dy;
    }

    private GridTile GetLowestFCost(List<GridTile> list)
    {
        GridTile best = list[0];
        for (int i = 1; i < list.Count; i++)
        {
            if (list[i].fCost < best.fCost)
                best = list[i];
        }
        return best;
    }

    private List<GridTile> RetracePath(GridTile startNode, GridTile endNode)
    {
        List<GridTile> path = new List<GridTile>();
        GridTile current = endNode;

        while (current != startNode)
        {
            path.Add(current);
            current = current.parent;
        }

        path.Add(startNode);
        path.Reverse();
        return path;
    }

    private void ResetNode(GridTile node)
    {
        node.gCost = 0;
        node.hCost = 0;
        node.parent = null;
    }
}
*/