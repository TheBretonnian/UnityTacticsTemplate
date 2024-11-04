using System.Collections.Generic;
using UnityEngine;

public interface IServiceGrid
{
    Range GetRange(ITile origin, int distance);

    //Locators
    ITile GetTileFromWorldPosition(Vector3 worldPosition);

    //Line methods
    float GetDistance(ITile orig, ITile dest);
    List<ITile> GetLineOfTiles(ITile orig, ITile dest);
}