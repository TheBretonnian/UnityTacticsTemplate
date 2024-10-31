using System.Runtime.InteropServices;
using System.Runtime.Serialization;
using UnityEngine;

public class GridManager  : MonoBehaviour
{
    //Reference to the grid component: it can be set in Editor or on Awake with GetComponent
    private IGrid<ITile> grid; 
    private IBorderOutliner borderOutliner;
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
    public IGrid<ITile> Grid { get => grid;}
    public IBorderOutliner BorderOutliner { get => borderOutliner;}

    //Unity Messages
    void Awake()
    {
        //Get reference of concrete shape component
        if(grid==null)
        {
            if(!TryGetComponent<IGrid<ITile>>(out grid))
            {
                Debug.LogError("No Grid Component found");
            }
        }
        if(borderOutliner==null)
        {
            if(!TryGetComponent<IBorderOutliner>(out borderOutliner))
            {
                Debug.LogError("No Border Outliner Component found");
            }
        }
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