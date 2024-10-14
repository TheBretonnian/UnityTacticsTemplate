using System.Collections.Generic;
using UnityEngine;

public class GridAdapter<T> : IGrid<IPathfindingNode>
{
    private readonly IGrid<T> _grid;

    public GridAdapter(IGrid<T> grid)
    {
        _grid = grid;
    }

    public int Height {get => _grid.Height;}

    public int Width {get => _grid.Width;}

    public float CalculateDistance(Vector2Int orig, Vector2Int dest)
    {
        return _grid.CalculateDistance(orig,dest);
    }

    public IPathfindingNode GetElement(Vector2Int localCoordinates)
    {
        return (IPathfindingNode)_grid.GetElement(localCoordinates);
    }

    public void SetElement(Vector2Int localCoordinates, IPathfindingNode element)
    {
        if (element is T castedElement)
        {
            _grid.SetElement(localCoordinates, castedElement);
        }
    }
    
    public HashSet<IPathfindingNode> GetNeighbours(Vector2Int orig, int distance, bool diagonalAllowed)
    {
        HashSet<IPathfindingNode> neighbours = new HashSet<IPathfindingNode>(); 
        foreach (T element in _grid.GetNeighbours(orig, distance, diagonalAllowed))
        {
            neighbours.Add((IPathfindingNode)element);
        }
        return neighbours;
    }
}
