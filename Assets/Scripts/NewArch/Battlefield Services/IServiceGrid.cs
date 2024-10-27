using System.Collections.Generic;
using UnityEngine;

public interface IServiceGrid
{
    Range GetRange(ITile origin, int distance);
    Range GetRangeWalkable(ITile origin, int distance);
    Range GetRangeWithLoS(ITile origin, int distance);

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

    //Pathfinding
    bool HasPath(ITile orig, ITile dest, IUnit movingUnit = null);
    Range GetPath(ITile orig, ITile dest, IUnit movingUnit = null);

    //Visuals
    void HighlightRange(Range range, Color color);
    //return index of Outline as int (Object Pool)
    int OutlineRange(Range range, Color color, int lineType = 0); //lineType optional -> eventually use Enum in concrete class

    //Cover
    bool HasCover(IUnit attacker, IUnit defender);
    float GetNormalizedCover(IUnit attacker, IUnit defender); //Returns [0..1]
    bool GetNormalizedCover(IUnit attacker, IUnit defender, out float cover);

    //Line of Sight
    bool HasLos(IUnit attacker, IUnit defender);
    bool HasLos(ITile orig, ITile dest);

    //Services:
    // + IServiceGrid
    // + IServiceGridVisual
    // + IServicePathfinding
    // + IServiceLoSandCover

}