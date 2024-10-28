using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class SquareGrid<T> : IGrid<T>
{
    private T[,] grid;
    private Vector3 _origin;

    public int Width { get; private set; }
    public int Height { get; private set; }
    public int CellSize { get; private set; }
    public bool IsInitialized {get; private set;}

    public SquareGrid(int width, int height, int cellSize, Vector3 origin)
    {
        Width = width;
        Height = height;
        CellSize = cellSize;
        IsInitialized = false;
        _origin = origin;
        grid = new T[width, height];
    }

    public T GetElement(Vector2Int localCoordinates)
    {
        if (AreValidCoordinates(localCoordinates))
        {
            return grid[localCoordinates.x, localCoordinates.y];
        }
        return default; //default in classes is null
    }

    public void SetElement(Vector2Int localCoordinates, T element)
    {
        if (AreValidCoordinates(localCoordinates))
        {
            grid[localCoordinates.x, localCoordinates.y] = element;
        }   
    }
    
    public float CalculateDistance(Vector2Int orig, Vector2Int dest)
    {
       return Vector2Int.Distance(orig, dest);
    }

    public HashSet<T> GetNeighbours(Vector2Int orig, int distance, bool diagonalAllowed)
    {
        List<T> neighbours = new List<T>();
        int[,] directions = diagonalAllowed ? 
            new int[,] { {1, 0}, {-1, 0}, {0, 1}, {0, -1}, {1, 1}, {-1, -1}, {1, -1}, {-1, 1} } : 
            new int[,] { {1, 0}, {-1, 0}, {0, 1}, {0, -1} };

        for (int i = 0; i < directions.GetLength(0); i++)
        {
            Vector2Int neighborCoords = new Vector2Int(orig.x + directions[i, 0], orig.y + directions[i, 1]);
            if (AreValidCoordinates(neighborCoords))
            {
                neighbours.Add(GetElement(neighborCoords));
            }
        }

        return neighbours.ToHashSet();
    }

    // Helper method to check if coordinates are within grid bounds (ValidCoordinates)
    public bool AreValidCoordinates(Vector2Int coordinates)
    {
        return coordinates.x >= 0 && coordinates.x < Width &&
               coordinates.y >= 0 && coordinates.y < Height;
    }

    public Vector3 LocalToCellWorld(Vector2Int localCoordinates)
    {
        //return new Vector3((float)localCoordinates.x, (float)localCoordinates.y, 0.0f) * CellSize + _origin;
        return new Vector3((float)localCoordinates.x, (float)localCoordinates.y, 0.0f); //Mockup since operator * does not work with mock up class
    }

    public Vector3 LocalToCellCenterWorld(Vector2Int localCoordinates)
    {
        //return new Vector3((float)localCoordinates.x + 0.5f, (float)localCoordinates.y + 0.5f, 0.0f) * CellSize + _origin;
        return new Vector3((float)localCoordinates.x + 0.5f, (float)localCoordinates.y + 0.5f, 0.0f);
    }

    public Vector2Int WorldToLocal(Vector3 worldPosition)
    {
        return new Vector2Int(
            Mathf.FloorToInt(worldPosition.x - _origin.x),
            Mathf.FloorToInt(worldPosition.y - _origin.y));
    }

    public void SetInitialized()
    {
        this.IsInitialized = true;
    }
}
