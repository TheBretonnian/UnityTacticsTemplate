using System;
using System.Collections.Generic;
using UnityEngine;

/*
Explanation:
1. Finding Border Tiles: The IsBorderTile method identifies any tile on the edge of the shape by 
checking if any of its neighboring cells is empty.
2. Creating an Ordered Outline: Starting from the first border tile, we follow a clockwise path around the edge, 
identifying which directions form valid border edges. This ensures we create a continuous path.
3. Adding Border Points: Each border edge point is added in sequence to form the outline. 
The GetEdgePoint method adds offset points based on the direction, creating a clear, continuous border.
4. Single LineRenderer Usage: Finally, the list of ordered points is passed to the LineRenderer, 
drawing the entire border in one component.

Notes:
1. Continuous Outline: This approach ensures a continuous outline by following the border in order.
2. Efficiency: Using a single LineRenderer reduces overhead and is better suited for dynamic or larger grids.
3. Customizable: Adjust the LineRenderer properties (width, color, etc.) as needed for visual effect.

*/
public class SquareGridBorderOutline
{
    public int[,] squareGrid;
    public LineRenderer lineRenderer;
    
    private static readonly Vector2Int[] directions = {
        new Vector2Int(1, 0), // Right
        new Vector2Int(0, -1), // Down
        new Vector2Int(-1, 0), // Left
        new Vector2Int(0, 1)   // Up
    };

    void Start()
    {
        squareGrid = new int[,]
        {
            { 0, 1, 1, 0 },
            { 1, 1, 1, 0 },
            { 0, 1, 0, 0 },
            { 0, 0, 0, 0 }
        };

        List<Vector3> borderPoints = GetOrderedBorderPoints(squareGrid);
        CreateBorderLineRenderer(borderPoints);
    }

    List<Vector3> GetOrderedBorderPoints(int[,] grid)
    {
        List<Vector3> borderPoints = new List<Vector3>();
        int rows = grid.GetLength(0);
        int cols = grid.GetLength(1);

        // Find the starting border tile
        bool startFound = false;
        int startX = 0, startY = 0;

        for (int x = 0; x < rows; x++)
        {
            for (int y = 0; y < cols; y++)
            {
                if (grid[x, y] == 1 && IsBorderTile(grid, x, y))
                {
                    startX = x;
                    startY = y;
                    startFound = true;
                    break;
                }
            }
            if (startFound) break;
        }

        // Follow the outline in a clockwise direction
        Vector2Int current = new Vector2Int(startX, startY);
        Vector2Int previousDirection = directions[3]; // Start by going "up" (to close the shape correctly)

        do
        {
            // Check each direction for a border edge
            for (int i = 0; i < directions.Length; i++)
            {
                //cycle through the directions in a clockwise order, starting from the last direction (previousDirection)
                Vector2Int direction = directions[(Array.IndexOf(directions, previousDirection) + i) % directions.Length];
                Vector2Int neighbor = current + direction;

                if (IsBorderEdge(grid, current.x, current.y, direction))
                {
                    borderPoints.Add(GetEdgePoint(current.x, current.y, direction));

                    // Move to the next tile in that direction
                    current += direction;
                    previousDirection = direction;
                    break;
                }
            }
        }
        while (current != new Vector2Int(startX, startY)); // Stop when back at the start

        return borderPoints;
    }

    bool IsBorderTile(int[,] grid, int x, int y)
    {
        foreach (Vector2Int dir in directions)
        {
            int neighbour_x = x + dir.x;
            int neighbour_y = y + dir.y;
            if (neighbour_x < 0 || neighbour_y < 0 || neighbour_x >= grid.GetLength(0) || neighbour_y >= grid.GetLength(1) || grid[neighbour_x, neighbour_y] == 0)
            {
                return true;
            }
        }
        return false;
    }

    bool IsBorderEdge(int[,] grid, int x, int y, Vector2Int direction)
    {
        int nx = x + direction.x;
        int ny = y + direction.y;
        return nx < 0 || ny < 0 || nx >= grid.GetLength(0) || ny >= grid.GetLength(1) || grid[nx, ny] == 0;
    }

    bool IsBorderEdge(Vector2Int coords, Vector2Int direction, SquareGrid<ITile> grid, Range tiles)
    {
        Vector2Int neighbourCoord = coords + direction; //So simple it can be moved outside
        //Check if neighbout is outside grid borders or not contain in given Range
        return !grid.AreValidCoordinates(neighbourCoord) || !tiles.Contains(grid.GetElement(neighbourCoord));
    }


    Vector3 GetEdgePoint(int x, int y, Vector2Int direction)
    {
        float xOffset = direction == directions[0] ? 1 : direction == directions[2] ? -1 : 0;
        float yOffset = direction == directions[1] ? -1 : direction == directions[3] ? 1 : 0;
        return new Vector3(x + xOffset / 2, y + yOffset / 2, 0);
    }

    void CreateBorderLineRenderer(List<Vector3> borderPoints)
    {
        lineRenderer.positionCount = borderPoints.Count;
        lineRenderer.SetPositions(borderPoints.ToArray());
    }
}
