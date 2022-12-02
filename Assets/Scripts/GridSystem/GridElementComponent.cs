using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class GridElementComponent: MonoBehaviour
{
    //References from GameGrid
    public GameGrid<GridElementComponent> grid; //Reference to parent grid
    public int x, y;

    //GameData
    public float z = 0.0f; //TODO: Is really necessary?
    public bool IsReachableOneMove;
    public bool IsReachableTwoMoves;
    public bool EnemyInRange;
    public bool DangerZone;
    public bool IsWalkable = true;
    public Unit unit = null;

    //Visuals
    public GridVisualComponent gridVisualElement;

    //Pathfinding (reference)
    public PathfindingNode pathfindingNode;

    public void Setup(GameGrid<GridElementComponent> grid, int x, int y)
    {
        this.grid = grid;
        this.x = x;
        this.y = y;
    }

    public void Initialize()
    {
        IsReachableOneMove = false;
        IsReachableTwoMoves = false;
        EnemyInRange = false;
        DangerZone = false;
        InitGridVisualComponent();

    }

    private void InitGridVisualComponent()
    {
        //Check if GridElementComponent derives from MonoBehaviour a.k.a. is a Component
        if(typeof(MonoBehaviour).IsAssignableFrom(typeof(GridVisualComponent)))
        {
            //Get gridElementComponent from prefab or add new component
            if (gameObject.TryGetComponent<GridVisualComponent>(out gridVisualElement) == false)
            {
                gridVisualElement = gameObject.AddComponent<GridVisualComponent>();
            }
        }
        else
        {
            //Instantiate with new since it is not a component
            gridVisualElement = new GridVisualComponent();
        }       
        gridVisualElement.Initialize();
    }

    #region Unit
    public Unit GetUnit()
    {
        return this.unit;
    }

    public void SetUnit(Unit unit)
    {
        this.unit = unit;
        grid.TriggerGridChangedEvent(x, y);
    }

    public bool HasUnit()
    {
        return (unit != null);
    }

    public void SnapUnitToGrid()
    {
        unit.transform.position = grid.GetWorldCenterPosition(this.x, this.y);
    }
#endregion

    public void ClearMoveRangeVariables()
    {
        IsReachableOneMove = false;
        IsReachableTwoMoves = false;
        grid.TriggerGridChangedEvent(x, y);
    }

    public void ClearAttackRangeVariables()
    {
        EnemyInRange = false;
        grid.TriggerGridChangedEvent(x, y);
    }

    public void SetWalkable(bool walkable)
    {
        this.IsWalkable = walkable;
        pathfindingNode.IsWalkable = walkable;
    }

    [ContextMenu("ToggleObstacle")]
    public void ToggleObstacle()
    {
        //Toggle pathfinding
        SetWalkable(!IsWalkable);
        gridVisualElement.SetLocked(!IsWalkable);
        gridVisualElement.SetVisible(!IsWalkable, Color.black);
    }

    public override string ToString()
    {
        //Print coordenates for easy orientation on editor
        return $"({x},{y})";
    }
}
