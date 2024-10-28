using System.Collections.Generic;
using UnityEngine;

public class GridAdapter<T> : IGrid<IPathfindingNode>
{
    private readonly IGrid<T> _grid;

    public GridAdapter(IGrid<T> grid)
    {
        _grid = grid;
    }

    public int Width {get => _grid.Width;}
    public int Height {get => _grid.Height;}
    public int CellSize {get => _grid.CellSize;}
    public bool IsInitialized {get => _grid.IsInitialized;}

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

    public bool AreValidCoordinates(Vector2Int localCoordinates)
    {
        return _grid.AreValidCoordinates(localCoordinates);
    }

    public Vector3 LocalToCellWorld(Vector2Int localCoordinates)
    {
        return _grid.LocalToCellWorld(localCoordinates);
    }

    public Vector3 LocalToCellCenterWorld(Vector2Int localCoordinates)
    {
        return _grid.LocalToCellCenterWorld(localCoordinates);
    }

    public Vector2Int WorldToLocal(Vector3 worldPosition)
    {
        return _grid.WorldToLocal(worldPosition);
    }
}
