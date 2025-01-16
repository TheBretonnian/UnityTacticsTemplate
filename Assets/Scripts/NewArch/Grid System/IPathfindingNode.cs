using UnityEngine;

public interface IPathfindingNode : IBasicGridElement
{
    IPathfindingNode CameFrom { get; set; }
    bool IsInZoC { get; set; }
    bool IsPassable { get; set; }
    int ZoCPenalty { get; set; } //Consider creating a global parameter for this
    float FCost { get; } //Total cost estimation of traversing through this node
    float GCost { get; set; } //Cost of the path from origin to this node
    float HCost { get; set; } //Heuristic cost of the path from this node to the goal node
    float MovingCost { get; } //Moving cost of this pathfinding node (standard = 1, MovingCost > 1 -> difficult terrain)

    //void UpdateFCost();

    bool IsWalkable();
}
