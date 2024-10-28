using System.Collections.Generic;
using UnityEngine;

public interface IServicePathfinding
{
    Range GetRangeWalkable(ITile origin, int distance);
    bool HasPath(ITile orig, ITile dest, IUnit movingUnit = null);
    Range GetPath(ITile orig, ITile dest, IUnit movingUnit = null);
}