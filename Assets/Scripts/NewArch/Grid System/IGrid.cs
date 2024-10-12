//using UnityEngine;
using System.Collections.Generic;

public interface IGrid
{
    // Properties
    int Height { get; }
    int Width { get; }

    // Methods
    object GetElement(Vector2Int localCoordinates);
    float CalculateDistance(Vector2Int orig, Vector2Int dest);
    public HashSet<object> GetNeighbours(Vector2Int orig, int distance, bool diagonalAllowed);
}
