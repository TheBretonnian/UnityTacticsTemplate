using UnityEngine;

public interface ITile
{
    Vector2Int LocalCoordinates{ get; }
    IUnit Unit {get; set;}
}