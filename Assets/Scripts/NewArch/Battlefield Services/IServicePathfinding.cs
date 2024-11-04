using System.Collections.Generic;
using UnityEngine;

public interface IServicePathfinding
{
    Range GetRangeWalkable(ITile origin, int distance, IUnit movingUnit = null);
    bool HasPath(ITile orig, ITile dest, IUnit movingUnit = null);
    List<Vector3> GetPath(ITile orig, ITile dest, IUnit movingUnit = null);
}