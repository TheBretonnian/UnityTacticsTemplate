using UnityEngine;

public interface IPathfindingNode
{
    public Vector2Int LocalCoordinates{ get; }
    IPathfindingNode CameFrom { get; set; }
    bool IsInZoC { get; set; }
    int ZoCPenalty { get; set; }
    float FCost { get; }
    float GCost { get; set; }
    float HCost { get; set; }

    void UpdateFCost();

    bool IsWalkable();
}
