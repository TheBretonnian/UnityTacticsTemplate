using System.Collections.Generic;
using UnityEngine;

public class BattlefieldServices : IServiceGrid
{
    public BattlefieldServices()
    {
    }

    public HashSet<IUnit> GetAlliesInSet(IUnit referenceUnit, HashSet<IUnit> units)
    {
        throw new System.NotImplementedException();
    }

    public int GetDistance(ITile orig, ITile dest)
    {
        throw new System.NotImplementedException();
    }

    public int GetDistance(IUnit fromUnit, IUnit toUnit)
    {
        throw new System.NotImplementedException();
    }

    public HashSet<IUnit> GetEnemiesInSet(IUnit referenceUnit, HashSet<IUnit> units)
    {
        throw new System.NotImplementedException();
    }

    public Range GetLineOfTiles(ITile orig, ITile dest)
    {
        throw new System.NotImplementedException();
    }

    public float GetNormalizedCover(IUnit attacker, IUnit defender)
    {
        throw new System.NotImplementedException();
    }

    public bool GetNormalizedCover(IUnit attacker, IUnit defender, out float cover)
    {
        throw new System.NotImplementedException();
    }

    public Range GetPath(ITile orig, ITile dest, IUnit movingUnit = null)
    {
        throw new System.NotImplementedException();
    }

    public Range GetRange(ITile origin, int distance)
    {
        throw new System.NotImplementedException();
    }

    public Range GetRangeWalkable(ITile origin, int distance)
    {
        throw new System.NotImplementedException();
    }

    public Range GetRangeWithLoS(ITile origin, int distance)
    {
        throw new System.NotImplementedException();
    }

    public ITile GetTileFromUnit(IUnit unit)
    {
        throw new System.NotImplementedException();
    }

    public IUnit GetUnitFromTile(ITile tile)
    {
        throw new System.NotImplementedException();
    }

    public HashSet<IUnit> GetUnitsInRange(Range tiles)
    {
        throw new System.NotImplementedException();
    }

    public HashSet<IUnit> GetUnitsInRange(ITile origin, int distance)
    {
        throw new System.NotImplementedException();
    }

    public bool HasCover(IUnit attacker, IUnit defender)
    {
        throw new System.NotImplementedException();
    }

    public bool HasLos(IUnit attacker, IUnit defender)
    {
        throw new System.NotImplementedException();
    }

    public bool HasLos(ITile orig, ITile dest)
    {
        throw new System.NotImplementedException();
    }

    public bool HasPath(ITile orig, ITile dest, IUnit movingUnit = null)
    {
        throw new System.NotImplementedException();
    }

    public void HighlightRange(Range range, Color color)
    {
        throw new System.NotImplementedException();
    }

    public int OutlineRange(Range range, Color color, int lineType = 0)
    {
        throw new System.NotImplementedException();
    }
}
