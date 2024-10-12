//using UnityEngine;
using System.Collections.Generic;

public interface IGrid<T>
{
    // Properties
    int Height { get; }
    int Width { get; }

    // Methods
    T GetElement(Vector2Int localCoordinates);
    float CalculateDistance(Vector2Int orig, Vector2Int dest);
    public HashSet<T> GetNeighbours(Vector2Int orig, int distance, bool diagonalAllowed);
}
