using UnityEngine;
using System.Collections.Generic;

public interface IGrid<T>
{
    // Properties
    int Width { get; }
    int Height { get; }
    int CellSize { get; }

    // Methods
    T GetElement(Vector2Int localCoordinates);

    void SetElement(Vector2Int localCoordinates, T element);
    
    float CalculateDistance(Vector2Int orig, Vector2Int dest);

    public HashSet<T> GetNeighbours(Vector2Int orig, int distance, bool diagonalAllowed);

    public bool AreValidCoordinates(Vector2Int localCoordinates);

    public Vector3 LocalToCellWorld(Vector2Int localCoordinates);

    public Vector3 LocalToCellCenterWorld(Vector2Int localCoordinates);

    public Vector2Int WorldToLocal(Vector3 worldPosition);

}
