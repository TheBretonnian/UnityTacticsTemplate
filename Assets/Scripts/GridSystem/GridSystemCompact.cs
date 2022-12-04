using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GridSystemCompact : MonoBehaviour, IGridVisual
{
    private GameGridWithPathfinding<GridElementComponent> gameGrid;

    [Header("Grid basics")]
    [SerializeField] private int width = 10, height = 10;
    [SerializeField] private int cellSize = 1;
    [SerializeField] private bool diagonalAllowed = true;
    [SerializeField, Tooltip("Reference to gameObjects which instantiates a gridElement (Visuals). Do not reference manually")]
    private List<GridElementComponent> gridElements = new List<GridElementComponent>();
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

    #region Initializer

    private GridElementComponent SmartInitializer(GameGrid<GridElementComponent> grid, int x, int y)
    {
        GridElementComponent gridElementComponent = null;

        //Check if gridElement was already created
        if (gridElements.Count > 0)
        {
            //Search reference
            gridElementComponent = gridElements.Find(gridElement => gridElement.x == x && gridElement.y == y);
        }
        //If component was not found then we have to create it (Note: this can happend in both cases: grid not created, grid extended -> TODO: Validate grid status and clean up or log warning)
        if(gridElementComponent == null)
        {
            //Check if GridElementComponent derives from MonoBehaviour a.k.a. is a Component
            if(typeof(MonoBehaviour).IsAssignableFrom(typeof(GridElementComponent)))
            {
                //Instantiate visuals prefab and attach GridElementComponent
                GameObject newGameObject = Instantiate(gridVisualPrefab, grid.GetWorldCenterPosition(x, y), Quaternion.identity, transform);
                newGameObject.name = $"{gridVisualPrefab.name}_{x}_{y}";
                //Get gridElementComponent from prefab or add new component
                if (newGameObject.TryGetComponent<GridElementComponent>(out gridElementComponent) == false)
                {
                    gridElementComponent = newGameObject.AddComponent<GridElementComponent>();
                }
            }
            else
            {
                //GridElement is not a component, instantiate with new
                gridElementComponent = new GridElementComponent();
            }

            //Add component to list (for easy tracking)
            gridElements.Add(gridElementComponent);
        }

        //Setup & Initialize
        gridElementComponent?.Setup(grid, x, y);
        gridElementComponent.Initialize();
        //Setup GridVisual config
        gridElementComponent.gridVisualElement.SetupConfig(gridVisualConfig);

        return gridElementComponent;
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

        gameGrid = new GameGridWithPathfinding<GridElementComponent>(width, height, cellSize, transform.position, (GameGrid<GridElementComponent> grid, int x, int y) => SmartInitializer(grid,x,y), debugTextParent, diagonalAllowed);
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
        foreach(GridElementComponent gridElementComponent in gridElements)
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
            if (!gameGrid.GetGridElement(x, y).gridVisualElement.locked) { 
                gameGrid.GetGridElement(x, y).gridVisualElement.SetVisible(false); } });
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

        foreach (GridElementComponent tile in gameGrid.GetNeighbours(selected_x, selected_y, unit.GetMoveDistance(), diagonalAllowed))
        {
            //Occupied tiles (by units) and not walkable ones are not valid move targets
            if (tile.HasUnit() == false && tile.IsWalkable)
            {
                //Check if neighbour in range has a feasible path and its distance (number of movements) is within unit's move distance
                if (gameGrid.FindPath(selected_x, selected_y, tile.x, tile.y)?.Count <= unit.GetMoveDistance())
                {
                    tile.IsReachableOneMove = true;
                    tile.gridVisualElement.MarkAsReachableOneMove();
                    //Use to fire event
                    gameGrid.SetGridElement(tile.x, tile.y, tile);
                }

            }
        }
    }

    public void UpdateAttackRange(int selected_x, int selected_y, Unit unit)
    {
        ClearAttackRange();

        foreach (GridElementComponent tile in gameGrid.GetNeighbours(selected_x, selected_y, unit.GetAttackRange(), diagonalAllowed))
        {
            if (tile.HasUnit())
            {
                if (tile.unit.IsEnemy(unit.team))
                {
                    tile.EnemyInRange = true;
                    tile.gridVisualElement.MarkAsEnemyInMeleeAttackRange();
                    gameGrid.SetGridElement(tile.x, tile.y, tile);
                }

            }
        }
    }

    public void UpdateDangerZone(int selected_x, int selected_y, Unit unit)
    {
        ClearAttackRange();

        foreach (GridElementComponent tile in gameGrid.GetNeighbours(selected_x, selected_y, unit.GetMoveDistance() + unit.GetAttackRange(), diagonalAllowed))
        {
            tile.EnemyInRange = true;
            tile.gridVisualElement.MarkAsEnemyInMeleeAttackRange(); //TODO: Create proper method to differentiate between use cases
            gameGrid.SetGridElement(tile.x, tile.y, tile);
        }
    }

    public void UpdateTransparency()
    {
        //Why not LoopThroughGrid here? gridElements list will be populated and stay populated in Editor because of SerializeField. Not the case for gameGrid.
        foreach(GridElementComponent tile in gridElements)
        {
            tile.gridVisualElement.SetTransparency(transparency);
        }
    }

    #endregion

    #region Wrappers
    public void OnGridValueChanged(object sender, OnGridChangedEventArgs onGridChangedEventArgs)
    {
       // Debug.Log(sender.ToString() + " changed on " + onGridChangedEventArgs.x.ToString() + "," + onGridChangedEventArgs.y.ToString());
    }

    public GridElementComponent GetGridElement(Vector3 position)
    {
        return gameGrid.GetGridElement(position);
    }

    public GridElementComponent GetGridElement(int x, int y)
    {
        return gameGrid.GetGridElement(x,y);
    }

    public List<Vector3> FindPath(Unit selectedUnit, Vector3 goal_position)
    {
        //Get grid element of goal position
        GridElementComponent goalGridElement = gameGrid.GetGridElement(goal_position);
        return FindPath(selectedUnit, goalGridElement);

    }

    public List<Vector3> FindPath(Unit selectedUnit, GridElementComponent goalGridElement)
    {
        //Get grid element of selected unit
        GridElementComponent SelectedUnitGridElement = gameGrid.GetGridElement(selectedUnit.GetPosition());

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

    #region IGridVisual
    public void MarkAsReachableOneMove(int x, int y)
    {
        GetGridElement(x, y).gridVisualElement.MarkAsReachableOneMove();
    }

    public void MarkAsReachableTwoMove(int x, int y)
    {
        GetGridElement(x, y).gridVisualElement.MarkAsReachableTwoMove();
    }

    public void MarkAsEnemyInMeeleAttackRange(int x, int y)
    {
        GetGridElement(x, y).gridVisualElement.MarkAsEnemyInMeleeAttackRange();
    }

    public void MarkAsEnemyInRangeAttackRange(int x, int y)
    {
        GetGridElement(x, y).gridVisualElement.MarkAsEnemyInRangeAttackRange();
    }

    public void MarkAsPossibleTargetSpecialAbility(int x, int y)
    {
        GetGridElement(x, y).gridVisualElement.MarkAsPossibleTargetSpecialAbility();
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

