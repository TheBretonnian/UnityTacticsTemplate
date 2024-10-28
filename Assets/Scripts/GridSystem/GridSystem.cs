using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GridSystem : MonoBehaviour
{
    private GameGridWithPathfinding<GridElement> gameGrid;

    [Header("Grid basics")]
    [SerializeField] private int width = 10, height = 10;
    [SerializeField] private int cellSize = 1;
    [SerializeField] private bool diagonalAllowed = true;
    [SerializeField, Tooltip("Reference to gameObjects which instantiates a gridElement (Visuals). Do not reference manually")]
    private List<GridElement> gridElements = new List<GridElement>();
    public int Width { get => width; }
    public int Height { get => height; }
    

    [Header("Grid Visuals")]
    [SerializeField] private GameObject gridVisualPrefab;
    [SerializeField] private GridVisualConfiguration gridVisualConfig;// = new GridVisualConfiguration();
    [SerializeField, Range(0.0f, 1.0f)] private float transparency = 1.0f;
    public float Transparency { get => transparency; set => transparency = Mathf.Clamp01(value); }

    [Header("Grid debug")]
    [SerializeField] private bool showGrid = true;
    [SerializeField] private bool showDebugText = false;
    [SerializeField] private Transform debugTextParent;

    //Cursor Hover Logic
    private Vector2Int lastMouseGridPosition = Vector2Int.zero;
    public delegate void OnCursorHoverGrid(int grid_x, int grid_y);
    public event OnCursorHoverGrid onCursorHoverGrid;

    #region UnityMethods
    private void Update()
    {
        //Hover logic - detect when the user hoves the mouse over a gridElement
        GridElement currentGridElement = GetGridElement(InputManager.GetMouseWorldPosition());
        if (currentGridElement != null)
        {
            Vector2Int currentMouseGridPosition = new Vector2Int(currentGridElement.x, currentGridElement.y);
            if (currentMouseGridPosition != lastMouseGridPosition)
            {
                //Update position
                lastMouseGridPosition = currentMouseGridPosition;

                //Fire Event
                onCursorHoverGrid?.Invoke(currentMouseGridPosition.x, currentMouseGridPosition.y);
            }
        }
    }
    #endregion

    #region Initializer

    private GridElement SmartInitializer(GameGrid<GridElement> grid, int x, int y)
    {
        GridElement gridElement = null;

        //Check if gridElement was already created
        if (gridElements.Count > 0)
        {
            //Search reference
            gridElement = gridElements.Find(gElement => gElement.x == x && gElement.y == y);
        }
        //If component was not found then we have to create it (Note: this can happend in both cases: grid not created, grid extended -> TODO: Validate grid status and clean up or log warning)
        if(gridElement == null)
        {
            //Check if GridElement derives from MonoBehaviour a.k.a. is a Component
            if(typeof(MonoBehaviour).IsAssignableFrom(typeof(GridElement)))
            {
                //Instantiate visuals prefab and attach GridElement
                GameObject newGameObject = Instantiate(gridVisualPrefab, grid.GetWorldCenterPosition(x, y), Quaternion.identity, transform);
                newGameObject.name = $"{gridVisualPrefab.name}_{x}_{y}";
                //Get gridElementComponent from prefab or add new component
                if (newGameObject.TryGetComponent<GridElement>(out gridElement) == false)
                {
                    gridElement = newGameObject.AddComponent<GridElement>();
                }
            }
            else
            {
                //GridElement is not a component, instantiate with new
                gridElement = new GridElement();
            }

            //Add component to list (for easy tracking)
            gridElements.Add(gridElement);
        }

        //Setup & Initialize
        gridElement?.Setup(grid, x, y);
        gridElement.Initialize();
        //Setup GridVisual config
        gridElement.gridVisual.SetupConfig(gridVisualConfig);

        return gridElement;
    }

    [ContextMenu("Create grid")]
    public void CreateGrid()
    {
        if(showDebugText && debugTextParent==null)
        {
            GameObject debugTextParentGameObject = new GameObject("DebugText");
            debugTextParentGameObject.transform.SetParent(transform);
            debugTextParent = debugTextParentGameObject.transform;
        }

        gameGrid = new GameGridWithPathfinding<GridElement>(width, height, cellSize, transform.position, (GameGrid<GridElement> grid, int x, int y) => SmartInitializer(grid,x,y), debugTextParent, diagonalAllowed);
        gameGrid.OnGridChanged += OnGridValueChanged;
        gameGrid.ShowDebugText(showDebugText);
        gameGrid.GetPathfinding().PathfindingGrid.ShowDebugText(false);

        //Update Pathfinding
        for(int x=0; x<width; x++)
        {
            for(int y=0; y<height; y++)
            {
                //Update walkable value
                gameGrid.GetPathfindingElement(x, y).IsWalkable = gameGrid.GetGridElement(x, y).IsWalkable;
                //Update reference
                gameGrid.GetGridElement(x, y).pathfindingNode = gameGrid.GetPathfindingElement(x, y);
            }
        }
        
    }

    public void DeleteGrid()
    {
        //Clear gameGrid
        gameGrid = null;
        //Clear all children from gridElements list
        foreach(GridElement gridElementComponent in gridElements)
        {
            DestroyImmediate(gridElementComponent.gameObject);
        }
        gridElements.Clear();
        //Clear DebugText
        if(debugTextParent!=null)
        {
            List<Transform> children = new List<Transform>();
            for(int i=0; i<debugTextParent.childCount; i++)
            {
                children.Add(debugTextParent.GetChild(i));
            }
            foreach(Transform t in children)
            {
                DestroyImmediate(t.gameObject);
            }
        }
        
    }

#endregion

    #region Clear methods

    public void ClearGrid()
    {
        ClearMovementRange();
        ClearAttackRange();
        ClearAllVisible();
    }

    public void ClearAllVisible()
    {
        LoopThroughGrid((int x, int y) =>{ 
            if (!gameGrid.GetGridElement(x, y).gridVisual.locked) { 
                gameGrid.GetGridElement(x, y).gridVisual.SetVisible(false); } });
    }

    public void ClearMovementRange()
    {
        LoopThroughGrid((int x, int y) => gameGrid.GetGridElement(x, y).ClearMoveRangeVariables());
    }
    public void ClearAttackRange()
    {
        LoopThroughGrid((int x, int y) => gameGrid.GetGridElement(x, y).ClearAttackRangeVariables());
    }
    #endregion

    #region Update methods (for data and visuals)
    public void UpdateMovementRange(int selected_x, int selected_y, Unit unit)
    {
        ClearMovementRange();

        foreach (GridElement tile in gameGrid.GetNeighbours(selected_x, selected_y, unit.GetMoveDistance(), diagonalAllowed))
        {
            //Occupied tiles (by units) and not walkable ones are not valid move targets
            if (tile.HasUnit() == false && tile.IsWalkable)
            {
                //Check if neighbour in range has a feasible path and its distance (number of movements) is within unit's move distance
                if (gameGrid.FindPath(selected_x, selected_y, tile.x, tile.y)?.Count <= unit.GetMoveDistance())
                {
                    tile.IsReachableOneMove = true;
                    tile.gridVisual.MarkAsReachableOneMove();
                    //Notify possible grid's subscriber
                    gameGrid.TriggerGridChangedEvent(x,y);
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
                if (tile.Unit.IsEnemy(unit.team))
                {
                    tile.EnemyInRange = true;
                    tile.gridVisual.MarkAsEnemyInMeleeAttackRange();
                    //Notify possible grid's subscriber
                    gameGrid.TriggerGridChangedEvent(x,y);
                }

            }
        }
    }

    public void UpdateDangerZone(int selected_x, int selected_y, Unit unit)
    {
        ClearAttackRange();

        foreach (GridElement tile in gameGrid.GetNeighbours(selected_x, selected_y, unit.GetMoveDistance() + unit.GetAttackRange(), diagonalAllowed))
        {
            if(tile.IsWalkable)
            {
                tile.DangerZone = true;
                tile.gridVisual.MarkAsEnemyInMeleeAttackRange(); //TODO: Create proper method to differentiate between use cases
                //Notify possible grid's subscriber
                gameGrid.TriggerGridChangedEvent(x,y);
            }
            
        }
    }

    public void UpdateTransparency()
    {
        //Why not LoopThroughGrid here? gridElements list will be populated and stay populated in Editor because of SerializeField. Not the case for gameGrid.
        foreach(GridElement tile in gridElements)
        {
            tile.gridVisual.SetTransparency(transparency);
        }
        //BUT...if you want to do something like this in Editor, do it in Editor script...
    }

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

    public delegate void Looper(int x, int y);
    public void LoopThroughGrid(Looper actionProTile)
    {
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                actionProTile?.Invoke(x, y);
            }
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

