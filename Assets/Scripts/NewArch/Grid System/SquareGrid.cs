//using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class SquareGrid<T> : IGrid
{
    private T[,] grid;

    public int Height { get; private set; }
    public int Width { get; private set; }

    public SquareGrid(int width, int height)
    {
        Width = width;
        Height = height;
        grid = new T[width, height];
    }

    // Example implementation of GetElement method
    public object GetElement(Vector2Int localCoordinates)
    {
        if (IsWithinBounds(localCoordinates))
        {
            return grid[localCoordinates.x, localCoordinates.y];
        }
        return null;
    }

    // Example implementation of CalculateDistance method
    public float CalculateDistance(Vector2Int orig, Vector2Int dest)
    {
       return Vector2Int.Distance(orig, dest);
    }

    // Example implementation of GetNeighbours method
    public HashSet<object> GetNeighbours(Vector2Int orig, int distance, bool diagonalAllowed)
    {
        List<object> neighbours = new List<object>();
        int[,] directions = diagonalAllowed ? 
            new int[,] { {1, 0}, {-1, 0}, {0, 1}, {0, -1}, {1, 1}, {-1, -1}, {1, -1}, {-1, 1} } : 
            new int[,] { {1, 0}, {-1, 0}, {0, 1}, {0, -1} };

        for (int i = 0; i < directions.GetLength(0); i++)
        {
            Vector2Int neighborCoords = new Vector2Int(orig.x + directions[i, 0], orig.y + directions[i, 1]);
            if (IsWithinBounds(neighborCoords))
            {
                neighbours.Add(GetElement(neighborCoords));
            }
        }

        return neighbours.ToHashSet();
    }

    // Helper method to check if coordinates are within grid bounds
    private bool IsWithinBounds(Vector2Int coordinates)
    {
        return coordinates.x >= 0 && coordinates.x < Width &&
               coordinates.y >= 0 && coordinates.y < Height;
    }
}
