using UnityEngine;

public interface ITile : IBasicGridElement 
{
    IUnit Unit {get; set;}
    void Initialize(Vector2Int localCoordinates);
}