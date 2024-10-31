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

HashSet<ITile> can be replaced by Range later

*/
public class SquareGridBorderOutline
{
    private readonly LineRenderer lineRenderer;
    private readonly IGrid<ITile> grid;

    private static readonly Vector2Int[] directions = {
        new Vector2Int(0, 1),  // Up
        new Vector2Int(1, 0),  // Right
        new Vector2Int(0, -1), // Down
        new Vector2Int(-1, 0)  // Left  
    };

    private static readonly Vector2Int[,] corners = {
        { new Vector2Int(0, 1), new Vector2Int(1, 1)}, // Up
        { new Vector2Int(1, 1), new Vector2Int(0, 1)}, // Right
        { new Vector2Int(0, 1), new Vector2Int(0, 0)}, // Down
        { new Vector2Int(0, 0), new Vector2Int(0, 1)} // Left  
    };

    //Dictionary ensures consistency with directions
    private static readonly Dictionary<Vector2Int, Vector2Int[]> direction2Corners = new Dictionary<Vector2Int, Vector2Int[]>
    {
        { new Vector2Int(0, 1),  new[] { new Vector2Int(0, 1), new Vector2Int(1, 1) } },  // Up
        { new Vector2Int(1, 0),  new[] { new Vector2Int(1, 1), new Vector2Int(1, 0) } },  // Right
        { new Vector2Int(0, -1), new[] { new Vector2Int(1, 0), new Vector2Int(0, 0) } },  // Down
        { new Vector2Int(-1, 0), new[] { new Vector2Int(0, 0), new Vector2Int(0, 1) } }   // Left
}   ;

    public SquareGridBorderOutline(LineRenderer lineRenderer, IGrid<ITile> grid)
    {
        this.lineRenderer = lineRenderer;
        this.grid = grid;
        //List<Vector3> borderPoints = GetOrderedBorderPoints(squareGrid);
        //CreateBorderLineRenderer(borderPoints);
    }

    public void OutlineBorderOfRange(HashSet<ITile> range)
    {
        if(!FindOrigin(range, out Vector2Int origin))
        {
            return; //Origin could not be found
        }

        HashSet<Vector2Int> unsortedBorderPoints = GetBorderPoints(range);

        HashSet<Vector2Int> sortedBorderPoints = SortBorderPoints(origin, unsortedBorderPoints);

        DrawBorder(lineRenderer, sortedBorderPoints);
    }

    private bool FindOrigin(HashSet<ITile> range, out Vector2Int origin)
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

    private HashSet<Vector2Int> GetBorderPoints(HashSet<ITile> range)
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

    private HashSet<Vector2Int> SortBorderPoints(Vector2Int origin, HashSet<Vector2Int> unsortedBorderPoints)
    {
        HashSet<Vector2Int> sortedBorderPoints = new HashSet<Vector2Int>();

        if(!unsortedBorderPoints.Contains(origin)) //Initial plausibility check
        {
            return sortedBorderPoints;
        }
        TransferPoint(origin, unsortedBorderPoints, sortedBorderPoints);

        bool NoMorePointsFound = false;
        Vector2Int currentPoint = origin;

        while(!NoMorePointsFound)
        {
            //Search through all directions in clockwise direction
            for (int i = 0; i < directions.Length; i++)
            {
                //Try to search next border point in unsorted set
                Vector2Int nextPoint = currentPoint + directions[i];
                if(unsortedBorderPoints.Contains(nextPoint))
                {
                    TransferPoint(nextPoint,unsortedBorderPoints,sortedBorderPoints);
                    currentPoint = nextPoint;
                    break; //Exit this search
                }
                else if((nextPoint == origin) || (unsortedBorderPoints.Count == 0))
                {
                    //We have reach the origin again so Or no more points
                    NoMorePointsFound = true;
                    break; //Exit this search
                }
            }
            //If we cannot find any further point but the set of unsorted points is not empty 
            //(e.g. invalid points like inner islands) we should stop the algorith as we cannot continue
            NoMorePointsFound = true;
            //If we really reach this, lets try to do our best with the outline and see the bug
        }            
        return sortedBorderPoints;

        //Tansfer a point from unsorted set to sorted one.
        static void TransferPoint(Vector2Int origin, HashSet<Vector2Int> unsortedBorderPoints, HashSet<Vector2Int> sortedBorderPoints)
        {
            sortedBorderPoints.Add(origin);
            unsortedBorderPoints.Remove(origin);
        }

    }

    private void DrawBorder(LineRenderer lineRenderer, HashSet<Vector2Int> sortedBorderPoints)
    {
        List<Vector3> borderPoints = new List<Vector3>();
        foreach(Vector2Int borderLocalCoord in sortedBorderPoints)
        {
            borderPoints.Add(grid.LocalToCellWorld(borderLocalCoord));
        }
        lineRenderer.positionCount = borderPoints.Count;
        lineRenderer.SetPositions(borderPoints.ToArray());
        lineRenderer.loop = true;
    }
 
}


