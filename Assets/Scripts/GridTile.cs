using System.Collections.Generic;

public class GridTile
{
    public int x;
    public int y;
    public int layer;

    public bool walkable;
    public List<GridTile> neighbors;

    // A* fields
    public int gCost;
    public int hCost;
    public int fCost => gCost + hCost;
    public GridTile parent;
}
