using System.Collections.Generic;
using UnityEngine;

public interface IServiceGrid
{
    Range GetRange(ITile origin, int distance);

    //Units
    HashSet<IUnit> GetUnitsInRange(Range tiles);
    HashSet<IUnit> GetUnitsInRange(ITile origin, int distance);

    HashSet<IUnit> GetEnemiesInSet(IUnit referenceUnit, HashSet<IUnit> units);
    HashSet<IUnit> GetAlliesInSet(IUnit referenceUnit, HashSet<IUnit> units);

    //Transformers
    ITile GetTileFromUnit(IUnit unit);
    IUnit GetUnitFromTile(ITile tile);

    //Line methods
    int GetDistance(ITile orig, ITile dest);
    int GetDistance(IUnit fromUnit, IUnit toUnit);
    Range GetLineOfTiles(ITile orig, ITile dest);

}