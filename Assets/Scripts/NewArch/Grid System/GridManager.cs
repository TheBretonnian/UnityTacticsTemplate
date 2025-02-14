using System;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;
using UnityEngine;

public class GridManager  : MonoBehaviour
{
    //This component acts as Facade for the exterior to the underlaying GridSystem
    //It's then completely optional but makes the dependency injection and 
    //referencing more confortable for the service layer
    //Interface reference to concrete component: it can be set in Editor or on Awake with GetComponent
    private IGrid<ITile> grid; 
    private IBorderOutliner borderOutliner;
    private IPathfinding pathfinding;
    private IGridManagement gridManagement;

    public IGrid<ITile> Grid { get => grid;}
    public IBorderOutliner BorderOutliner { get => borderOutliner;}
    public IPathfinding Pathfinding{get => pathfinding;}

    public event Action OnGridCreated;

    //Unity Messages
    void Awake()
    {
        // Get reference of concrete components
        if (grid == null)
        {
            if (!TryGetComponent<IGrid<ITile>>(out grid))
            {
                Debug.LogError("No Grid Component found");
            }
        }
        if (borderOutliner == null)
        {
            if (!TryGetComponent<IBorderOutliner>(out borderOutliner))
            {
                Debug.LogError("No Border Outliner Component found");
            }
        }
        if (pathfinding == null)
        {
            if (!TryGetComponent<IPathfinding>(out pathfinding))
            {
                Debug.LogError("No Pathfinding Component found");
            }
        }
        if (gridManagement == null)
        {
            if (!TryGetComponent<IGridManagement>(out gridManagement))
            {
                Debug.LogError("No Grid Management Component found");
            }
        }
    }

    public void CreateGrid()
    {
        gridManagement.CreateGrid();
        OnGridCreated?.Invoke();
    }
    

}