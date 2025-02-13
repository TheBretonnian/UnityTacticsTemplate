using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pathfinding : IPathfinding
{
    private IGrid<IPathfindingNode> pathfindingGrid;
    private bool diagonalAllowed;

    public Pathfinding(IGrid<IPathfindingNode> grid, bool diagonalAllowed = true)
    {
        this.pathfindingGrid = grid;
        this.diagonalAllowed = diagonalAllowed;
    }

    public List<Vector3> FindPath(int start_x, int start_y, int goal_x, int goal_y, float maxGCost = 0.0f)
    {
        List<IPathfindingNode> openList = new List<IPathfindingNode>();
        List<IPathfindingNode> closedList = new List<IPathfindingNode>();

        IPathfindingNode origin = pathfindingGrid.GetElement(new Vector2Int(start_x, start_y));
        IPathfindingNode goal = pathfindingGrid.GetElement(new Vector2Int(goal_x, goal_y));

        //Check input parameter
        if (origin == null || goal == null)
        {
            // Invalid coordinates
            return null;
        }

        // Initialize pathfinding grid for this search
        for (int x = 0; x < pathfindingGrid.Width; x++)
        {
            for (int y = 0; y < pathfindingGrid.Height; y++)
            {
                IPathfindingNode currentNode = pathfindingGrid.GetElement(new Vector2Int(x, y));
                currentNode.GCost = int.MaxValue;
                //Calculate heuristic using simple distance
                currentNode.HCost = pathfindingGrid.CalculateDistance(new Vector2Int(x, y), new Vector2Int(goal_x, goal_y));
                //currentNode.UpdateFCost();
                currentNode.CameFrom = null;
                
            }
        }
        //For origin set gcost at 0
        origin.GCost = 0;
        //origin.UpdateFCost();
        
        //Add start node to open list
        openList.Add(origin);

        //Loop until we have nodes in openList
        while(openList.Count > 0)
        {
            IPathfindingNode currentNode = GetNodeWithLowestFCost(openList);

            //Check if we are at goal
            if(currentNode == goal)
            {
                //Return reconstructed path, converted in Vector3 position
                if(maxGCost == 0.0f || currentNode.GCost <= maxGCost)
                {
                    return GetPath(currentNode);
                }
                else
                {
                    return null;
                }
                
            }
            //Remove currentNode from openList and add it to closed list (already searched)
            openList.Remove(currentNode);
            closedList.Add(currentNode);

            //Loop through neighbours
            foreach(IPathfindingNode neighbor in pathfindingGrid.GetNeighbours(currentNode.LocalCoordinates,1,diagonalAllowed))
            {
                if (closedList.Contains(neighbor) || !neighbor.IsWalkable())
                {
                    continue;
                }
                // tentativeGCost is the distance from start to the neighbor through current
                // Adjust the movement cost with ZoC penalty
                float tentativeGCost = currentNode.GCost + pathfindingGrid.CalculateDistance(currentNode.LocalCoordinates, neighbor.LocalCoordinates)*neighbor.MovingCost;

                // Apply ZoC penalty if in a ZoC
                if (neighbor.IsInZoC)
                {
                    tentativeGCost += neighbor.ZoCPenalty; // Add penalty for moving through ZoC
                }

                if (tentativeGCost < neighbor.GCost)
                {
                    neighbor.CameFrom = currentNode;
                    neighbor.GCost = tentativeGCost;
                    //neighbor.UpdateFCost();

                    if (!openList.Contains(neighbor))
                    {
                        openList.Add(neighbor);
                    }
                }
            }
        }

        // Open list is empty but goal was never reached = No path found
        return null;
    }

    public void ResetPathfindingProperties()
    {
        for(int x = 0; x < pathfindingGrid.Width; x++)
        {
            for(int y = 0; y < pathfindingGrid.Height; y++)
            {
                IPathfindingNode node = pathfindingGrid.GetElement(new Vector2Int(x,y));
                node.IsInZoC = false;
                node.IsPassable = true;
            }
        }
    }

    private IPathfindingNode GetNodeWithLowestFCost(List<IPathfindingNode> openList)
    {
        if(openList.Count > 0)
        {
            IPathfindingNode lowestFCostNode = openList[0];

            foreach(IPathfindingNode node in openList)
            {
                if(node.FCost < lowestFCostNode.FCost)
                {
                    lowestFCostNode = node;
                }
            }

            return lowestFCostNode;
        }
        else
        {
            return null;
        }        
    }

    private List<IPathfindingNode> ReconstructPath(IPathfindingNode goal)
    {
        List<IPathfindingNode> path = new List<IPathfindingNode>();
        IPathfindingNode node = goal;

        //Loop until get a node without cameFrom (origin)
        do
        {
            path.Add(node);
            node = node.CameFrom;
        }
        while (node.CameFrom != null);

        path.Reverse();
        return path;
    }

    private List<Vector3> GetPath(IPathfindingNode goal)
    {
        List<IPathfindingNode> path = ReconstructPath(goal);
        List<Vector3> pathCoordenates = new List<Vector3>();

        foreach(IPathfindingNode node in path)
        {
            pathCoordenates.Add(pathfindingGrid.LocalToCellCenterWorld(node.LocalCoordinates));
        }

        return pathCoordenates;
    }
}
