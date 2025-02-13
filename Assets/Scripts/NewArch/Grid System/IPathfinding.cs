using System.Collections.Generic;
using UnityEngine;

public interface IPathfinding
{
    List<Vector3> FindPath(int start_x, int start_y, int goal_x, int goal_y, float maxGCost = 0.0f);
    void ResetPathfindingProperties();
}