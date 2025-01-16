using System.Collections.Generic;
using UnityEngine;

public class ServiceGrid : IServiceGrid
{
    IGrid<ITile> grid;
    bool diagonalAllowed;

    public ServiceGrid(IGrid<ITile> grid, bool diagonalAllowed=true)
    {
        this.grid = grid;
        this.diagonalAllowed = diagonalAllowed;
    }

    public Range GetRange(ITile origin, int distance)
    {
        return grid.GetNeighbours(origin.LocalCoordinates,distance,diagonalAllowed) as Range;
    }
    
    public ITile GetTileFromWorldPosition(Vector3 worldPosition)
    {
        return grid.GetElement(grid.WorldToLocal(worldPosition));
    }
    
    //Line Methods
    public float GetDistance(ITile orig, ITile dest)
    {
        return grid.CalculateDistance(orig.LocalCoordinates, dest.LocalCoordinates);
    }
    public List<ITile> GetLineOfTiles(ITile orig, ITile dest)
    {
        return grid.GetLine(orig.LocalCoordinates, dest.LocalCoordinates);
    }
}
