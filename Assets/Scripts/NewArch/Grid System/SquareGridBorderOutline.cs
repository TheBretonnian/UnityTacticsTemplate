using System.Collections.Generic;
using UnityEngine;

/*
Explanation:
1. Finding the origin point: The origin point will be always the corner of the L tile with the lesser x and smaller y.
2. Creating a unsorted SET of border points: The SET ensures each point (in Local coordinates) is added only once. 
We loop through each tile in given Range to obtain the border points: we don't need to worry about any order yet, 
just identify border points. They will be ordered later.
3. Creating an Ordered Outline Sequence: Starting from the origin point, we follow a clockwise path around the edge, 
identifying which directions form valid border edges. This ensures we create a continuous path. 
We search for valid borders in the unsorted set obtained before and once found: 
- we add to the ordered outline sequence (Set or List)
- we removed from unsorted set (already used)
4. Adding Border Points to the LineRenderer: Each border edge point is converted to World Space coordinates 
and added in sequence to form the outline. This is about the external border, inner islands will be ommited.
The list of ordered points is passed to the LineRenderer, drawing the entire border in one component. 
(Using loop in lineRenderer will close the border Loop)

Notes:
1. Continuous Outline: This approach ensures a continuous outline by following the border in order.
2. Efficiency: Using a single LineRenderer reduces overhead and is better suited for dynamic or larger grids.
3. Customizable: Adjust the LineRenderer properties (width, color, etc.) as needed for visual effect.
*/
public class SquareGridBorderOutline : IBorderOutliner
{
    private readonly IGrid<ITile> grid;

    private static readonly Vector2Int[] directions = {
        new Vector2Int(0, 1),  // Up
        new Vector2Int(1, 0),  // Right
        new Vector2Int(0, -1), // Down
        new Vector2Int(-1, 0)  // Left  
    };

    //Dictionary ensures consistency with directions
    private static readonly Dictionary<Vector2Int, Vector2Int[]> direction2Corners = new Dictionary<Vector2Int, Vector2Int[]>
    {
        { new Vector2Int(0, 1),  new[] { new Vector2Int(0, 1), new Vector2Int(1, 1) } },  // Up
        { new Vector2Int(1, 0),  new[] { new Vector2Int(1, 1), new Vector2Int(1, 0) } },  // Right
        { new Vector2Int(0, -1), new[] { new Vector2Int(1, 0), new Vector2Int(0, 0) } },  // Down
        { new Vector2Int(-1, 0), new[] { new Vector2Int(0, 0), new Vector2Int(0, 1) } }   // Left
    };

    public SquareGridBorderOutline(IGrid<ITile> grid)
    {
        this.grid = grid;
    }

    public void OutlineBorderOfRange(Range range, LineRenderer lineRenderer)
    {
        if(!FindOrigin(range, out Vector2Int origin))
        {
            return; //Origin could not be found
        }

        HashSet<Vector2Int> unsortedBorderPoints = GetBorderPoints(range);

        List<Vector2Int> sortedBorderPoints = SortBorderPoints(origin, unsortedBorderPoints);

        DrawBorder(lineRenderer, sortedBorderPoints);
    }

    private bool FindOrigin(Range range, out Vector2Int origin)
    {  
        for (int x = 0; x < grid.Width; x++)
        {
            for (int y = 0; y < grid.Height; y++)
            {
                origin = new Vector2Int(x,y);
                if(range.Contains(grid.GetElement(origin)))
                {
                    return true;
                }
            }
        }
        origin = new Vector2Int(0,0);
        return false;
    }

    private HashSet<Vector2Int> GetBorderPoints(Range range)
    {
        HashSet<Vector2Int> unsortedBorderPoints = new HashSet<Vector2Int>();

        foreach(ITile tile in range)
        {
            //Loop through all directions
            for (int i = 0; i < directions.Length; i++)
            {
                //Check if border: neighbour is outside grid Boundary or is not included in range
                Vector2Int neighbourCoord = tile.LocalCoordinates + directions[i];
                if(!grid.AreValidCoordinates(neighbourCoord) || !range.Contains(grid.GetElement(neighbourCoord)))
                {
                    //Border found. Add points of the edge:
                    unsortedBorderPoints.Add(tile.LocalCoordinates + direction2Corners[directions[i]][0]);
                    unsortedBorderPoints.Add(tile.LocalCoordinates + direction2Corners[directions[i]][1]);
                }
            }
        }

        return unsortedBorderPoints;
    }

    private List<Vector2Int> SortBorderPoints(Vector2Int origin, HashSet<Vector2Int> unsortedBorderPoints)
    {
        List<Vector2Int> sortedBorderPoints = new List<Vector2Int>();

        // Check if origin is a valid starting point
        if (!unsortedBorderPoints.Contains(origin))
        {
            Debug.LogError("Invalid origin: Starting point is not part of the border.");
            return sortedBorderPoints;
        }

        // Start ordering from the origin
        TransferPoint(origin, unsortedBorderPoints, sortedBorderPoints);
        Vector2Int currentPoint = origin;

        // Continue until we've either looped back to the origin or run out of valid points
        while (unsortedBorderPoints.Count > 0)
        {
            bool pointFound = false;

            // Search clockwise around the current point for the next border point
            for (int i = 0; i < directions.Length; i++)
            {
                Vector2Int nextPoint = currentPoint + directions[i];

                if (unsortedBorderPoints.Contains(nextPoint))
                {
                    TransferPoint(nextPoint, unsortedBorderPoints, sortedBorderPoints);
                    currentPoint = nextPoint;
                    pointFound = true;
                    break; //exit for loop search
                }
            }

            if (!pointFound)
            {
                Debug.LogWarning("Encountered isolated border points or an unexpected shape. Stopping outline.");
                break; //exit entire while search
            }
        }

        // If we didn’t return to the origin, this could indicate an incomplete outline or unconnected points.
        if (currentPoint != origin)
        {
            Debug.LogWarning("Border outline did not return to origin. Possible unconnected areas.");
        }

        // Final check for remaining points (indicative of inner islands or disconnected border points)
        if (unsortedBorderPoints.Count > 0)
        {
            Debug.LogWarning($"Unsorted points remain: {unsortedBorderPoints.Count}. These may be inner islands or disconnected regions.");
        }

        return sortedBorderPoints;

        // Helper function to transfer points
        static void TransferPoint(Vector2Int point, HashSet<Vector2Int> unsorted, List<Vector2Int> sorted)
        {
            sorted.Add(point);
            unsorted.Remove(point);
        }
    }

    private void DrawBorder(LineRenderer lineRenderer, List<Vector2Int> sortedBorderPoints)
    {
        List<Vector3> borderPoints = new List<Vector3>();
        foreach(Vector2Int borderLocalCoord in sortedBorderPoints)
        {
            borderPoints.Add(grid.LocalToCellWorld(borderLocalCoord));
        }
        lineRenderer.positionCount = borderPoints.Count;
        lineRenderer.SetPositions(borderPoints.ToArray());
        //Close the loop if this is not the case
        lineRenderer.loop = borderPoints[0] != borderPoints[^1];
    }
 
}


