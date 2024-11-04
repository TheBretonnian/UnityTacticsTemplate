using System.Collections.Generic;
using UnityEngine;


public interface IServiceUnitLocation
{
    ITile GetTileFromUnit(IUnit unit);
    IUnit GetUnitFromTile(ITile tile);
    float GetDistance(IUnit fromUnit, IUnit toUnit);

    HashSet<IUnit> GetUnitsInRange(Range tiles);
    HashSet<IUnit> GetUnitsInRange(ITile origin, int distance);

    HashSet<IUnit> GetAllUnits();
    HashSet<IUnit> GetEnemiesInSet(IUnit referenceUnit, HashSet<IUnit> units);
    HashSet<IUnit> GetAlliesInSet(IUnit referenceUnit, HashSet<IUnit> units);
}