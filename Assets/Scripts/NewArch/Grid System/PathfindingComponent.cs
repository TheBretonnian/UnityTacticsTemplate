using System.Collections.Generic;
using UnityEngine;

public class PathfindingComponent : MonoBehaviour, IPathfinding
{
    private Pathfinding pathfinding;

    [SerializeField]
    private IGrid<ITile> grid; 

    public IGrid<ITile> Grid { get => grid;}

    [SerializeField]
    private bool diagonalAllowed = true;

    private void Awake()
    {
        //Get reference of concrete shape component
        if(grid==null)
        {
            if(!TryGetComponent<IGrid<ITile>>(out grid))
            {
                Debug.LogError("No Grid Component found");
            }
        }
        //Consistency check: actually redundant if ITile derives from IPathfinding, but only run max. once per scene.
        if(typeof(IPathfindingNode).IsAssignableFrom(typeof(ITile)))
        {
            pathfinding = new Pathfinding(new GridAdapter<ITile>(grid), diagonalAllowed);
        }
        else
        {
            Debug.LogError("ITile does not implement IPathfindingNode");
        }
    }

    public List<Vector3> FindPath(int start_x, int start_y, int goal_x, int goal_y, float maxGCost = 0.0f)
    {
        return pathfinding.FindPath(start_x, start_y, goal_x, goal_y, maxGCost);
    }

    public void ResetPathfindingProperties()
    {
        pathfinding.ResetPathfindingProperties();
    }
}