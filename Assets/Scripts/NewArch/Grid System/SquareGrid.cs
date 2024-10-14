using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class SquareGrid<T> : IGrid<T>
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

    public T GetElement(Vector2Int localCoordinates)
    {
        if (AreCoordWithinBounds(localCoordinates))
        {
            return grid[localCoordinates.x, localCoordinates.y];
        }
        return default; //default in classes is null
    }

    public void SetElement(Vector2Int localCoordinates, T element)
    {
        if (AreCoordWithinBounds(localCoordinates))
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
            if (AreCoordWithinBounds(neighborCoords))
            {
                neighbours.Add(GetElement(neighborCoords));
            }
        }

        return neighbours.ToHashSet();
    }

    // Helper method to check if coordinates are within grid bounds (ValidCoordinates)
    private bool AreCoordWithinBounds(Vector2Int coordinates)
    {
        return coordinates.x >= 0 && coordinates.x < Width &&
               coordinates.y >= 0 && coordinates.y < Height;
    }
}
