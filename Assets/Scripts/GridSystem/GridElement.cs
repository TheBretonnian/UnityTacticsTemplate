using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class GridElement : MonoBehaviour
{
    //References from GameGrid
    public GameGrid<GridElement> grid; //Reference to parent grid
    public int x, y;

    //GameData
    public float z = 0.0f; //TODO: Is really necessary or can be the game object position used instead?
    public bool IsReachableOneMove;
    public bool IsReachableTwoMoves;
    public bool EnemyInRange;
    public bool DangerZone;
    public bool IsWalkable = true;
    [SerializeField] private Unit unit = null;

    //Visuals
    public GridVisual gridVisual;

    //Pathfinding (reference)
    public PathfindingNode pathfindingNode;

    //Encapsulate unit to force using method SetUnit but still offers easy access .Unit
    public Unit Unit { get => unit; private set => unit = value; }

    #region Init & Setup
    public void Setup(GameGrid<GridElement> grid, int x, int y)
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
        InitGridVisual();
    }

    private void InitGridVisual()
    {
        //Check if GridElement derives from MonoBehaviour a.k.a. is a Component
        if(typeof(MonoBehaviour).IsAssignableFrom(typeof(GridVisual)))
        {
            //Get gridElementComponent from prefab or add new component
            if (gameObject.TryGetComponent<GridVisual>(out gridVisual) == false)
            {
                gridVisual = gameObject.AddComponent<GridVisual>();
            }
        }
        else
        {
            //Instantiate with new since it is not a component
            gridVisual = new GridVisual();
        }       
        gridVisual.Initialize();
    }
    #endregion

    #region Unit
    public Unit GetUnit()
    {
        return this.unit;
    }

    public void SetUnit(Unit unit)
    {
        this.unit = unit;
        grid.TriggerGridChangedEvent(x, y);
        if(this.unit !=null)
        {
            //Trigger OnUnitEnter
        }
        else
        {
            //Trigger OnUnitExit
        }

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
        gridVisual.SetLocked(!IsWalkable);
        gridVisual.SetVisible(!IsWalkable, Color.black);
    }

    public override string ToString()
    {
        //Print coordenates for easy orientation on editor
        return $"({x},{y})";
    }
}
