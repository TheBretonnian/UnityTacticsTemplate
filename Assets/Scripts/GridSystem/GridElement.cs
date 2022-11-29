using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class GridElement
{
    public GameGrid<GridElement> grid; //Reference to parent grid
    public int x, y;
    public float z = 0.0f;
    public bool IsReachableOneMove;
    public bool IsReachableTwoMoves;
    public bool EnemyInRange;
    public bool DangerZone;
    public Unit unit = null;

    public GridElement(GameGrid<GridElement> grid, int x, int y)
    {
        this.grid = grid;
        this.x = x;
        this.y = y;

        IsReachableOneMove = false;
        IsReachableTwoMoves = false;
        EnemyInRange = false;
    }

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


    public override string ToString()
    {
        //Print coordenates for easy orientation on editor
        return $"({x},{y})";
    }

    public void SnapUnitToGrid()
    {
        unit.transform.position = grid.GetWorldCenterPosition(this.x, this.y);
    }
}
