using UnityEngine;

public interface IPathfindingNode : IBasicGridElement
{
    IPathfindingNode CameFrom { get; set; }
    bool IsInZoC { get; set; }
    bool IsPassable { get; set; }
    int ZoCPenalty { get; set; } //Consider creating a global parameter for this
    float FCost { get; }
    float GCost { get; set; }
    float HCost { get; set; }

    void UpdateFCost();

    bool IsWalkable();
}
