using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(GridVisuals))]
public class GridSystem : MonoBehaviour
{
    private GameGridWithPathfinding<GridElement> gameGrid;

    [Header("Grid basics")]
    [SerializeField] private int width = 10, height = 10;
    [SerializeField] private int cellSize = 1;
    [SerializeField] private bool diagonalAllowed = true;
    [SerializeField, Tooltip("Reference to gameObjects which instantiates a gridElement (Visuals). Do not reference manually")] private List<GameObject> gridElements;

[Header("Grid Visuals")]
    [SerializeField, Tooltip("Script containing all logic for grid visualization")] private GridVisuals gridVisuals;

    [Header("Grid debug")]
    [SerializeField] private bool showGrid = true;
    [SerializeField] private bool showDebugText = false;
    [SerializeField] private Transform debugTextParent;

    //-> GameHandler
    //[Header("Units")]
    //[SerializeField] private Unit[] units;
    //[SerializeField] private TurnSystem turnSystem;

    //private Unit selected_unit;
    //private Controlled_Unit controlled_unit = new Controlled_Unit();
    //private bool OneUnitPerTurn = false;

    //[System.Serializable]
    //private class Controlled_Unit
    //{
    //    public Unit Unit;
    //    public bool Fixed;
    //}

    //-> GameHandler

    //private void Awake()
    //{
    //    //Setup
    //    //Turn System
    //    OneUnitPerTurn = !(turnSystem.Type == TurnSystem.TurnSystemType.TeamBased);
    //    if (turnSystem.Type == TurnSystem.TurnSystemType.IniciativeOrder)
    //    {
    //        turnSystem.NewTurnEvent += OnNewTurn;
    //    }
    //}

    // Start is called before the first frame update
    void Start()
    {
        if(gameGrid == null)
        {
            CreateGrid(); //If grid is not null, then it has been created on Editor
        }

        ////Center Camera //-> GameHandler
        //Camera.main.transform.position = new Vector3(width / 2, height / 2, Camera.main.transform.position.z);
        //Camera.main.orthographicSize = ((float)height + 3.0f) / 2.0f;

        ////Set Units position //-> GameHandler
        //foreach (Unit unit in units)
        //{
        //    gameGrid.GetGridElement(unit.transform.position).SetUnit(unit);
        //    unit.onMoveCompleted += UpdateGridAfterMove;
        //    unit.onAttackCompleted += UpdateGridAfterMove;
        //}
    }

    //-> GameHandler
    //void Update()
    //{
    //    if (Input.GetMouseButtonDown(0))
    //    {
    //        GridElement selected_gridElement = gameGrid.GetGridElement(UtilsClass.GetMouseWorldPosition());

    //        if (selected_gridElement?.HasUnit() == true)
    //        {
    //            //Select unit / Deselect previous
    //            selected_unit?.Deselect();
    //            selected_unit = selected_gridElement.GetUnit();
    //            selected_unit.Select();
    //            if(!controlled_unit.Fixed || !OneUnitPerTurn)
    //            {
    //                if(selected_unit.team == turnSystem.CurrentPlayer.Number && turnSystem.Type != TurnSystem.TurnSystemType.IniciativeOrder)
    //                {
    //                    controlled_unit.Unit = selected_unit;
    //                }                    
    //            }

    //            //After selecting, we need to update movement range if unit can move
    //            ClearVisuals();
    //            if (selected_unit.canMove && selected_unit == controlled_unit.Unit)
    //            {
    //                UpdateMovementRange(selected_gridElement.x, selected_gridElement.y, selected_unit);
    //            }
    //            //After selecting, we need to update movement range if unit can attack
    //            if (selected_unit.canAttack && selected_unit == controlled_unit.Unit)
    //            {
    //                UpdateAttackRange(selected_gridElement.x, selected_gridElement.y, selected_unit);
    //            }
    //            //Update Danger Zone
    //            if( selected_unit.team != turnSystem.CurrentPlayer.Number)
    //            {
    //                UpdateDangerZone(selected_gridElement.x, selected_gridElement.y, selected_unit);
    //            }
    //        }
    //    }
    //    if (Input.GetMouseButtonDown(1) && controlled_unit.Unit != null)
    //    {
    //        //Get grid elements
    //        GridElement selected_gridElement = GetGridElement(UtilsClass.GetMouseWorldPosition());
    //        GridElement controlledUnit_gridElement = GetGridElement(controlled_unit.Unit.GetPosition());
    //        //Check if valid move:
    //        if(selected_unit == controlled_unit.Unit)
    //        {
    //            if (selected_gridElement?.IsReachableOneMove == true && selected_unit.canMove)
    //            {

    //                //Calculate path to selected move position
    //                List<Vector3> MovePath = FindPath(controlled_unit.Unit, selected_gridElement);

    //                //Command Move if possible path
    //                if (MovePath != null)
    //                {
    //                    //Clear Grid
    //                    ClearGrid();
    //                    //Clear unit from his grid element
    //                    controlledUnit_gridElement.SetUnit(null);

    //                    //Command move
    //                    controlled_unit.Unit.MoveTo(MovePath, null);

    //                }
    //            }
    //            if (selected_gridElement?.EnemyInRange == true && controlled_unit.Unit.canAttack)
    //            {
    //                //Clear Grid
    //                ClearGrid();
    //                //Command Attack unit (enemy) at selected grid element
    //                controlled_unit.Unit.Attack(selected_gridElement.unit, null);

    //            }
    //        }

    //    }

    //    //Handle by Turn System
    //    if (Input.GetKeyDown(KeyCode.Space))
    //    {
    //        //Clean actions before forcing new turn -> deselect unit and clear grid
    //        selected_unit?.Deselect();
    //        ClearGrid();
    //        controlled_unit.Fixed = false;
    //        //Force new turn, reset actions
    //        turnSystem.ForceNewTurn();
    //    }

    //    if (Input.GetKeyDown(KeyCode.LeftControl))
    //    {
    //        //Toggle pathfinding
    //        gameGrid.GetXY(UtilsClass.GetMouseWorldPosition(), out int x, out int y);
    //        gameGrid.ToggleWalkable(x, y);
    //        gridVisuals.SetLocked(x, y, !gameGrid.IsWalkable(x, y));
    //        gridVisuals.SetVisible(x, y,!gameGrid.IsWalkable(x, y));
    //        if (!gameGrid.IsWalkable(x, y)) { gridVisuals.Highlight(x, y, Color.black); }


    //    }

    //    if (showGrid)
    //    {
    //        gameGrid?.DrawGridLines(0);
    //    }

    //}

    //private void OnNewTurn(int currentTurn, TurnSystem.Player currentPlayer, Unit activeUnit)
    //{
    //    controlled_unit.Unit = activeUnit;
    //    controlled_unit.Fixed = true;

    //    //TO DO: Make private function Select Unit or OnUnitSelect(Unit unit)
    //    selected_unit?.Deselect();
    //    selected_unit = controlled_unit.Unit;
    //    selected_unit.Select();

    //    Debug.Log($"Active Unit = {activeUnit}, currentPlayer = {currentPlayer}");
    //}

    //private List<Vector3> CalculateSimplePath(int target_x, int target_y)
    //{
    //    gameGrid.GetXY(selected_unit.transform.position, out int unit_x, out int unit_y);
    //    List<Vector3> MovePath = new List<Vector3>();
    //    int x = unit_x;
    //    int y = unit_y;

    //    for (x = unit_x; Mathf.Abs(target_x - x) != 0; x += (target_x - x) / Mathf.Abs(target_x - x))
    //    {
    //        MovePath.Add(gameGrid.GetWorldCenterPosition(x, y));
    //    }
    //    for (y = unit_y; Mathf.Abs(target_y - y) != 0; y += (target_y - y) / Mathf.Abs(target_y - y))
    //    {
    //        MovePath.Add(gameGrid.GetWorldCenterPosition(x, y));
    //    }

    //    MovePath.Add(gameGrid.GetWorldCenterPosition(target_x, target_y));

    //    return MovePath;
    //}

    #region Initializer

    [ContextMenu("Create grid")]
    public void CreateGrid()
    {
        gameGrid = new GameGridWithPathfinding<GridElement>(width, height, cellSize, transform.position, (GameGrid<GridElement> grid, int x, int y) => new GridElement(grid,x,y), debugTextParent, diagonalAllowed);
        gameGrid.OnGridChanged += OnGridValueChanged;
        gameGrid.ShowDebugText(showDebugText);
        gameGrid.GetPathfinding().PathfindingGrid.ShowDebugText(false);

        //Create visuals
        //Get GridVisuals from this gameObject if reference is not set in Editor
        if (gridVisuals == null)
        {
            TryGetComponent<GridVisuals>(out gridVisuals);
        }
        //Check if gridElements already created
        if(gridElements.Count == 0)
        {
            gridVisuals?.GenerateGrid(width, height, cellSize);

            gridElements = new List<GameObject>();

            //Setup references at GridElement helper -> to be able to edit and see values from UnityEditor
            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    //Get instantiated object from gridVisuals and add it to list
                    GameObject instantiatedObject = gridVisuals.GetGrid().GetGridElement(x, y).gameObject;
                    gridElements.Add(instantiatedObject);
                    //Get GridElement Helper and setup with references to grid elements data
                    instantiatedObject.TryGetComponent<GridElementHelper>(out GridElementHelper gHelper);
                    gHelper.Setup(gameGrid.GetGridElement(x, y), gameGrid.GetPathfinding().PathfindingGrid.GetGridElement(x, y), gridVisuals.GetGrid().GetGridElement(x, y));
                }
            }
        }
        else
        {
            //Otherwise generate gridVisuals without Instantiating the visuals (just grid matrix)
            gridVisuals?.GenerateGrid(width, height, cellSize,false);
            //Setup references FROM GridElement helper -> to be able to update values from Editor
            foreach (GameObject gE in gridElements)
            {
                GridElementHelper gHelper = null;
                gE.TryGetComponent<GridElementHelper>(out gHelper);

                gridVisuals.GetGrid().SetGridElement(gHelper.gridVisualElement.x, gHelper.gridVisualElement.y, gHelper.gridVisualElement);
                gameGrid.SetGridElement(gHelper.gridElement.x, gHelper.gridElement.y, gHelper.gridElement);
                gameGrid.SetPathfindingElement(gHelper.pathfindingNode.x, gHelper.pathfindingNode.y, gHelper.pathfindingNode);

                //Update grid references
                gridVisuals.GetGrid().GetGridElement(gHelper.gridVisualElement.x, gHelper.gridVisualElement.y).grid = gridVisuals.GetGrid();
                gameGrid.GetGridElement(gHelper.gridElement.x, gHelper.gridElement.y).grid = gameGrid;
                gameGrid.GetPathfindingElement(gHelper.pathfindingNode.x, gHelper.pathfindingNode.y).grid = gameGrid.GetPathfinding().PathfindingGrid;
            }
        }
        
        

    }

    public void DeleteGrid()
    {
        gameGrid = null;
        gridVisuals?.DestroyGrid();
        //Clear gridElements list: this list has the reference to gameObjects which holds gridVisuals and the GridElementHelperScript
        gridElements = null;
    }

#endregion

#region Clear methods

    public void ClearGrid()
    {
        ClearMovementRange();
        ClearAttackRange();
        gridVisuals?.ClearAllVisible();
    }

    public void ClearVisuals()
    {
        gridVisuals?.ClearAllVisible();
    }

    public void ClearMovementRange()
    {
        for(int x=0; x<width; x++)
        {
            for (int y = 0;y < height; y++)
            {
                gameGrid.GetGridElement(x, y).ClearMoveRangeVariables();
            }
        }
    }
    public void ClearAttackRange()
    {
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                gameGrid.GetGridElement(x, y).ClearAttackRangeVariables();
            }
        }
    }
    #endregion

#region Update methods (for data and visuals)
    public void UpdateMovementRange(int selected_x, int selected_y, Unit unit)
    {
        ClearMovementRange();

        foreach (GridElement tile in gameGrid.GetNeighbours(selected_x, selected_y, unit.GetMoveDistance(), diagonalAllowed))
        {
            if (tile.HasUnit() == false && gameGrid.IsWalkable(tile.x, tile.y))
            {
                //Check the neighbour in range has a feasible path and its distance (number of movements) is within Range
                if (gameGrid.FindPath(selected_x, selected_y, tile.x, tile.y)?.Count <= unit.GetMoveDistance())
                {
                    tile.IsReachableOneMove = true;
                    gameGrid.SetGridElement(tile.x, tile.y, tile);
                    gridVisuals?.MarkAsReachableOneMove(tile.x,tile.y);
                }

            }
        }
    }

    public void UpdateAttackRange(int selected_x, int selected_y, Unit unit)
    {
        ClearAttackRange();

        foreach (GridElement tile in gameGrid.GetNeighbours(selected_x, selected_y, unit.GetAttackRange(), diagonalAllowed))
        {
            if (tile.HasUnit())
            {
                if (tile.unit.IsEnemy(unit.team))
                {
                    tile.EnemyInRange = true;
                    gameGrid.SetGridElement(tile.x, tile.y, tile);
                    gridVisuals?.MarkAsEnemyInMeeleAttackRange(tile.x, tile.y);
                }

            }
        }
    }

    public void UpdateDangerZone(int selected_x, int selected_y, Unit unit)
    {
        ClearAttackRange();

        foreach (GridElement tile in gameGrid.GetNeighbours(selected_x, selected_y, unit.GetMoveDistance() + unit.GetAttackRange(), diagonalAllowed))
        {
            tile.EnemyInRange = true;
            gameGrid.SetGridElement(tile.x, tile.y, tile);
            gridVisuals?.MarkAsEnemyInMeeleAttackRange(tile.x, tile.y);
        }
    }

    //public void UpdateGridAfterMove(Unit sender_unit)
    //{
    //    //Update unit grid position
    //    GridElement gridElement = gameGrid.GetGridElement(sender_unit.GetPosition());
    //    gridElement.SetUnit(sender_unit);
    //    //Snap unit to grid
    //    gridElement.SnapUnitToGrid();
    //    //Update grid
    //    ClearGrid();
    //    if (sender_unit.canMove)
    //    {
    //        UpdateMovementRange(gameGrid.GetGridElement(sender_unit.GetPosition()).x, gameGrid.GetGridElement(sender_unit.GetPosition()).y, sender_unit);
    //    }
    //    if (sender_unit.canAttack)
    //    {
    //        UpdateAttackRange(gameGrid.GetGridElement(sender_unit.GetPosition()).x, gameGrid.GetGridElement(sender_unit.GetPosition()).y, sender_unit);
    //    }
    //    if (sender_unit.HasActions() == false)
    //    {
    //        sender_unit.Deselect();
    //        selected_unit = null;
    //        controlled_unit.Unit = null;
    //        controlled_unit.Fixed = false;
    //    }
    //    else
    //    {
    //        if(OneUnitPerTurn)
    //        {
    //            if(sender_unit == controlled_unit.Unit)
    //            {
    //                controlled_unit.Fixed = true;
    //                turnSystem.SetActiveUnit(controlled_unit.Unit);
    //            }
    //        }
    //    }

    //}
    #endregion

#region Wrappers
    public void OnGridValueChanged(object sender, OnGridChangedEventArgs onGridChangedEventArgs)
    {
       // Debug.Log(sender.ToString() + " changed on " + onGridChangedEventArgs.x.ToString() + "," + onGridChangedEventArgs.y.ToString());
    }

    public GridElement GetGridElement(Vector3 position)
    {
        return gameGrid.GetGridElement(position);
    }

    public GridElement GetGridElement(int x, int y)
    {
        return gameGrid.GetGridElement(x,y);
    }

    public List<Vector3> FindPath(Unit selectedUnit, Vector3 goal_position)
    {
        //Get grid element of goal position
        GridElement goalGridElement = gameGrid.GetGridElement(goal_position);
        return FindPath(selectedUnit, goalGridElement);

    }

    public List<Vector3> FindPath(Unit selectedUnit, GridElement goalGridElement)
    {
        //Get grid element of selected unit
        GridElement SelectedUnitGridElement = gameGrid.GetGridElement(selectedUnit.GetPosition());

        if (SelectedUnitGridElement != null && goalGridElement != null)
        {
            //Calculate path to selected move position
            return gameGrid.FindPath(SelectedUnitGridElement.x, SelectedUnitGridElement.y, goalGridElement.x, goalGridElement.y);
        }
        else
        {
            return null;
        }
    }

 #endregion


    #region Debug
    public void ShowDebugText(bool showDebugText)
    {
        this.showDebugText = showDebugText;
        gameGrid.ShowDebugText(showDebugText);
    }

    private void OnDrawGizmos()
    {
        if(showGrid)
        {
            gameGrid?.DrawGridLines(0);
        }
        
    }
 #endregion

}

