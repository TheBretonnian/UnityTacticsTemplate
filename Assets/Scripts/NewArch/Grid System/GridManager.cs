using System.Runtime.InteropServices;
using UnityEngine;

public class GridManager  : MonoBehaviour
{
    //Reference to the grid component: it can be set in Editor or on Awake with GetComponent
    private IGrid<ITile> grid; 

    private GridAdapter<ITile> gridAdapter;

    //Consider to use a interface in case the Pathfinding becomes a component and for better decoupling
    private Pathfinding pathfinding;

    //Prefab for Tile (To be moved to a TileFactory?)
    private GameObject tilePrefab;
    
    //These parameters can be delegated to Grid and Pathfinding components
    private int width;
    private int height;
    private bool diagonalAllowed;

    public Pathfinding Pathfinding{get => pathfinding;}

    //Unity Messages
    void Awake()
    {
        //Get reference of grid
        //For demo: create a instance of SquareGrid component, TO DO replace with proper dependency injection
        grid = new SquareGridComponent();
        //Wrap the concrete SquareGrid into an adapter which implements the interface expected by Pathfinding => TypeSafe
        gridAdapter = new GridAdapter<ITile>(grid);

        //Consistency check: actually redundant if ITile derives from IPathfinding, but only run max. once per scene.
        if(typeof(IPathfindingNode).IsAssignableFrom(typeof(ITile)))
        {
            pathfinding = new Pathfinding(gridAdapter, diagonalAllowed);
        }
        else
        {
            Debug.LogError("ITile does not implement IPathfindingNode");
        }
    }

    //Public methods
    

}