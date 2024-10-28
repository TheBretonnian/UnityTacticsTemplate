using UnityEngine;

public interface ITile : IBasicGridElement, IPathfindingNode 
{
    IUnit Unit {get; set;}
    void Initialize(Vector2Int localCoordinates);
}