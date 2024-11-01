using UnityEngine;
using System.Collections.Generic;

public interface IGrid<T>
{
    // Properties
    int Width { get; }
    int Height { get; }
    int CellSize { get; }

    bool IsInitialized {get;}

    // Methods
    T GetElement(Vector2Int localCoordinates);

    void SetElement(Vector2Int localCoordinates, T element);
    
    float CalculateDistance(Vector2Int orig, Vector2Int dest);

    HashSet<T> GetNeighbours(Vector2Int orig, int distance, bool diagonalAllowed);
    
    //Check that the coordinates are within Grid Boundary (0<= x < Width and 0 <= y < Height)
    bool AreValidCoordinates(Vector2Int localCoordinates);

    //Transform bi-dimensional local space coordinates into a world Vector3 position (botton right corner of Cell)
    //This is not universal for any grid geometry : not the same for square or hex => remove from common interface
    //Vector3 LocalToCellWorld(Vector2Int localCoordinates);

    Vector3 LocalToCellCenterWorld(Vector2Int localCoordinates);

    Vector2Int WorldToLocal(Vector3 worldPosition);

}
