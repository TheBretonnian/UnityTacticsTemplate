using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pathfinding
{
    private GameGrid<PathfindingNode> pathfindingGrid;
    private bool diagonalAllowed;
    public GameGrid<PathfindingNode> PathfindingGrid { get => pathfindingGrid;}

    public Pathfinding(int width, int height, int cellSize, Vector3 origin, bool diagonalAllowed = true, /* Optional parameter for arrange UnityGameObjects */ Transform debugTextParent = null)
    {
        this.diagonalAllowed = diagonalAllowed;
        //Generate helper Pathfinding grid with pathfinding data base on recieve data
        this.pathfindingGrid = new GameGrid<PathfindingNode>(width, height, cellSize, origin, (GameGrid<PathfindingNode> grid, int x, int y) => new PathfindingNode(grid, x, y), debugTextParent);      

    }

    public List<Vector3> FindPath(int start_x, int start_y, int goal_x, int goal_y)
    {
       
        List<PathfindingNode> openList = new List<PathfindingNode>();
        List<PathfindingNode> closedList = new List<PathfindingNode>();

        PathfindingNode origin = pathfindingGrid.GetGridElement(start_x, start_y);
        PathfindingNode goal = pathfindingGrid.GetGridElement(goal_x, goal_y);

        //Check input parameter
        if (origin == null || goal == null)
        {
            // Invalid coordinates
            return null;
        }

        // Initialize pathfinding grid for this search
        for (int x = 0; x < pathfindingGrid.Width; x++)
        {
            for(int y=0; y<pathfindingGrid.Height; y++)
            {
                PathfindingNode currentNode = pathfindingGrid.GetGridElement(x, y);
                currentNode.gcost = int.MaxValue;
                //Calculate heuristic using simple distance
                currentNode.hcost = pathfindingGrid.CalculateDistance(x, y, goal_x, goal_y, diagonalAllowed);
                currentNode.UpdateFCost();
                currentNode.cameFrom = null;
                pathfindingGrid.TriggerGridChangedEvent(currentNode.x, currentNode.y);
            }
        }
        //For origin set gcost at 0
        origin.gcost = 0;
        origin.UpdateFCost();
        pathfindingGrid.TriggerGridChangedEvent(origin.x, origin.y);
        //Add start node to open list
        openList.Add(origin);

        //Loop until we have nodes in openList
        while(openList.Count > 0)
        {
            PathfindingNode currentNode = GetNodeWithLowestFCost(openList);

            //Check if we are at goal
            if(currentNode == goal)
            {
                //Return reconstructed path, converted in Vector3 position
                return GetPath(currentNode);
            }
            //Remove currentNode from openList and add it to closed list (already searched)
            openList.Remove(currentNode);
            closedList.Add(currentNode);

            //Loop through neighbours
            foreach(PathfindingNode neighbour in pathfindingGrid.GetNeighbours(currentNode.x,currentNode.y,1,diagonalAllowed))
            {
                if(!closedList.Contains(neighbour) && neighbour.IsWalkable)
                {
                    // tentative_gScore is the distance from start to the neighbor through current
                    float tentative_gcost = currentNode.gcost + pathfindingGrid.CalculateDistance(currentNode.x, currentNode.y, neighbour.x, neighbour.y, diagonalAllowed);

                    if (tentative_gcost < neighbour.gcost)
                    {
                        // This path to neighbor is better than any previous one. Record it!
                        neighbour.cameFrom = currentNode;
                        neighbour.gcost = tentative_gcost;
                        neighbour.UpdateFCost();
                        pathfindingGrid.TriggerGridChangedEvent(neighbour.x, neighbour.y);

                        if (!openList.Contains(neighbour))
                        {
                            openList.Add(neighbour);
                        }
                    }
                }
            }
        }

        // Open list is empty but goal was never reached
        return null;
    }

    private PathfindingNode GetNodeWithLowestFCost(List<PathfindingNode> openList)
    {
        if(openList.Count > 0)
        {
            PathfindingNode lowestFCostNode = openList[0];

            foreach(PathfindingNode node in openList)
            {
                if(node.fcost < lowestFCostNode.fcost)
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

    private List<PathfindingNode> ReconstructPath(PathfindingNode goal)
    {
        List<PathfindingNode> path = new List<PathfindingNode>();
        PathfindingNode node = goal;

        //Loop until get a node without cameFrom (origin)
        do
        {
            path.Add(node);
            node = node.cameFrom;
        }
        while (node.cameFrom != null);

        //
        path.Reverse();

        return path;
    }

    private List<Vector3> GetPath(PathfindingNode goal)
    {
        List<PathfindingNode> path = ReconstructPath(goal);
        List<Vector3> pathCoordenates = new List<Vector3>();

        foreach(PathfindingNode node in path)
        {
            pathCoordenates.Add(pathfindingGrid.GetWorldCenterPosition(node.x, node.y));
        }

        return pathCoordenates;
    }
}

[Serializable]
public class PathfindingNode
{
    public GameGrid<PathfindingNode> grid; //Reference to parent grid
    public int x, y;
    public bool IsWalkable;
    public PathfindingNode cameFrom;


    public float fcost, gcost, hcost;

    public PathfindingNode(GameGrid<PathfindingNode> grid, int x, int y)
    {
        this.grid = grid;
        this.x = x;
        this.y = y;

        IsWalkable = true;
        cameFrom = null;
    }

    public void UpdateFCost()
    {
        fcost = gcost + hcost;
    }

    public override string ToString()
    {
        return $"G: {gcost} \n F: {fcost} \n H: {hcost} \n {IsWalkable}";
    }
}
