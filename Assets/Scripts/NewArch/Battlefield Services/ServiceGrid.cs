using System.Collections.Generic;
using UnityEngine;

public class ServiceGrid : IServiceGrid
{
    IGrid<ITile> grid;

    public ServiceGrid(IGrid<ITile> grid)
    {
        this.grid = grid;
    }

    public Range GetRange(ITile origin, int distance)
    {
        //diagonalAllowed hardcoded to true -> Consider parametrization
        return grid.GetNeighbours(origin.LocalCoordinates,distance,true) as Range;
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