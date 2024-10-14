using System.Diagnostics;
using System.Runtime.InteropServices;
using UnityEngine;

public class GridManager  : MonoBehaviour
{
    //Reference to the grid component: it can be set in Editor or on Awake with GetComponent
    private IGrid<Tile> grid; 

    private GridAdapter<Tile> gridAdapter;

    //Consider to use a interface in case the Pathfinding becomes a component and for better decoupling
    private Pathfinding pathfinding;

    //Prefab for Tile (To be moved to a TileFactory?)
    private GameObject tilePrefab;
    
    //These parameters can be delegated to Grid and Pathfinding components
    private int width;
    private int height;
    private bool diagonalAllowed;

    //Unity Messages
    void Awake()
    {
        //Get reference of grid
        //For demo: create a instance of SquareGrid, TO DO replace with getting reference if null
        grid = new SquareGrid<Tile>(width,height); //This is normally done in GridComponent

        //Wrap the concrete SquareGrid into an adapter which implements the interface expected by Pathfinding => TypeSafe
        gridAdapter = new GridAdapter<Tile>(grid);

        //Consistency check
        if(typeof(IPathfindingNode).IsAssignableFrom(typeof(Tile)))
        {
            pathfinding = new Pathfinding(gridAdapter, diagonalAllowed);
        }
        else
        {
            //Debug.LogError("Tile does not implement IPathfindingNode");
        }        
    }

    //Public methods
    

}