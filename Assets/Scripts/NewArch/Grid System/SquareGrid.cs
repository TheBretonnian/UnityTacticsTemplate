using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class SquareGrid<T> : IGrid<T>
{
    private T[,] grid;
    private Vector3 _origin;
    private bool _isGridXZ;

    public int Width { get; private set; }
    public int Height { get; private set; }
    public int CellSize { get; private set; }
    public bool IsInitialized {get; private set;}

    public SquareGrid(int width, int height, int cellSize, Vector3 origin, bool isGridXZ = false)
    {
        Width = width;
        Height = height;
        CellSize = cellSize;
        IsInitialized = false;
        _origin = origin;
        _isGridXZ = isGridXZ;
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
        HashSet<T> neighbours = new HashSet<T>();
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

        return neighbours;
    }

    // Helper method to check if coordinates are within grid bounds (ValidCoordinates)
    public bool AreValidCoordinates(Vector2Int coordinates)
    {
        return coordinates.x >= 0 && coordinates.x < Width &&
               coordinates.y >= 0 && coordinates.y < Height;
    }

    //Grid Orientation agnostic:
    public Vector3 LocalToCellCenterWorld(Vector2Int localCoordinates)
    {
        //return new Vector3((float)localCoordinates.x + 0.5f, (float)localCoordinates.y + 0.5f, 0.0f) * CellSize + _origin;
        return LocalToWorld(new Vector2(localCoordinates.x + 0.5f, localCoordinates.y + 0.5f));
    }

    public Vector3 LocalToWorld(Vector2Int localCoordinates)
    {
        return LocalToWorld(new Vector2(localCoordinates.x,localCoordinates.y)); //localCoordinates.Vector2
    }

    //GridOrientation specific:
    public Vector3 LocalToWorld(Vector2 localCoordinates)
    {
        return _isGridXZ ? LocalToWorldXZ(localCoordinates) : LocalToWorldXY(localCoordinates); 
        //For sure this will only work as we only support these 2 variants
    }
    private Vector3 LocalToWorldXY(Vector2 localCoordinates)
    {
        return new Vector3(localCoordinates.x, localCoordinates.y, 0.0f) * CellSize + _origin;
    }
    private Vector3 LocalToWorldXZ(Vector2 localCoordinates)
    {
        return new Vector3(localCoordinates.x, 0.0f, localCoordinates.y) * CellSize + _origin;
    }

    public Vector2Int WorldToLocal(Vector3 worldPosition)
    {
        return _isGridXZ ? WorldXZToLocal(worldPosition) : WorldXYToLocal(worldPosition);
    }

    private Vector2Int WorldXYToLocal(Vector3 worldPosition)
    {
        return new Vector2Int(
            Mathf.FloorToInt(worldPosition.x - _origin.x),
            Mathf.FloorToInt(worldPosition.y - _origin.y));
    }

    private Vector2Int WorldXZToLocal(Vector3 worldPosition)
    {
        return new Vector2Int(
            Mathf.FloorToInt(worldPosition.x - _origin.x),
            Mathf.FloorToInt(worldPosition.z - _origin.z));
    }

    public void SetInitialized()
    {
        IsInitialized = true;
    }

    public List<T> GetLine(Vector2Int orig, Vector2Int dest)
    {
        //1. Sample the line from orig to dest (in local coords) in many points (the smaller the better)
        //2. For each point get the tile
        //3. Add the tile to a list if tile is not already included. Always check against last since the line is ordered.
        //4. Repeat until you reached dest point
        //5. Return the list

        //Error Handling: Return null if orig or dest are not valid
        const int SamplesPerTile = 10; //The smaller the efficient
        float sampleDistance = CellSize/SamplesPerTile;
        float sampleIncrementNormalized = sampleDistance/(CellSize*CalculateDistance(orig,dest));

        List<T> TilesInLine = new List<T>();

        //Inout check
        if(!AreValidCoordinates(orig) && !AreValidCoordinates(dest)) 
        {
            //Error: invalid input parameter so return null
            return null; 
        }

        //Add orig
        TilesInLine.Add(GetElement(orig));
        
        float t = 0.0f;
        while (t < 1.0f)
        {
            //For each point get the tile
            Vector3 worldCordSamplePoint = Vector3.Lerp(LocalToWorld(orig),LocalToWorld(dest),t);
            T newElement = GetElement(WorldToLocal(worldCordSamplePoint));
            //Add the tile to a list if tile is not already included. Always check against last since the line is ordered.
            if(!TilesInLine.Contains(newElement))
            {
                TilesInLine.Add(newElement);
            }
            t+=sampleIncrementNormalized;
        }

        return TilesInLine;
    }
}
