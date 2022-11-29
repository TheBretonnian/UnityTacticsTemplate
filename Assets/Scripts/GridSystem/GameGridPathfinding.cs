using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameGridWithPathfinding<TGridElement>: GameGrid<TGridElement>
{
    private Pathfinding pathfinding;
    private GameGrid<TGridElement> grid;

    public GameGridWithPathfinding(int width, int height, int cellSize, Vector3 origin, Func<GameGrid<TGridElement>, int, int, TGridElement> createGridObject, /* Optional parameter for arrange UnityGameObjects */ Transform debugTextParent = null, bool diagonalAllowed = true) : base(width, height, cellSize, origin, createGridObject, debugTextParent)
    {
        this.grid = this; //GameGrid reference from himself (inherit class) 
        this.grid.ShowDebugText(false);
        //Instantiate Pathfinding with given parameters
        pathfinding = new Pathfinding(width, height, cellSize, origin, diagonalAllowed, debugTextParent);
    }



    #region Pathfinding
    //Encapsulate public method from private class Pathfinding
    public List<Vector3> FindPath(int start_x, int start_y, int goal_x, int goal_y)
    {
        //Debug.Log($"Try to get path from ({start_x},{start_y}) to ({goal_x},{goal_y})");
        return pathfinding.FindPath(start_x, start_y, goal_x, goal_y);
    }

    //Wrappers from pathfinding grid
    public PathfindingNode GetPathfindingElement(int x, int y)
    {
        return pathfinding.PathfindingGrid.GetGridElement(x, y);
    }

    public void SetPathfindingElement(int x, int y, PathfindingNode node)
    {
        pathfinding.PathfindingGrid.SetGridElement(x, y, node);
    }

    public bool IsWalkable(int x, int y)
    {
        if (pathfinding.PathfindingGrid.AreValidCoordenates(x, y))
        {
            return pathfinding.PathfindingGrid.GetGridElement(x, y).IsWalkable;
        }
        return false;
    }

    public void SetWalkable(int x, int y, bool IsWalkable)
    {
        if (pathfinding.PathfindingGrid.AreValidCoordenates(x, y))
        {
            pathfinding.PathfindingGrid.GetGridElement(x, y).IsWalkable = IsWalkable;
            pathfinding.PathfindingGrid.TriggerGridChangedEvent(x, y);
        }

    }

    public void ToggleWalkable(int x, int y)
    {
        if (pathfinding.PathfindingGrid.AreValidCoordenates(x, y))
        {
            SetWalkable(x,y,!pathfinding.PathfindingGrid.GetGridElement(x, y).IsWalkable);
        }
    }

    public Pathfinding GetPathfinding()
    {
        return pathfinding;
    }
    #endregion
}