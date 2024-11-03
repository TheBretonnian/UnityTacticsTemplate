using System.Collections.Generic;
using UnityEngine;

public class ServiceUnitLocation : IServiceUnitLocation
{
    IGrid<ITile> grid;

    public ServiceUnitLocation(IGrid<ITile> grid)
    {
        this.grid = grid;
    }

    public HashSet<IUnit> GetUnitsInRange(Range tiles)
    {
        HashSet<IUnit> units = new HashSet<IUnit>();
        foreach(ITile tile in tiles)
        {
            if (tile.Unit != null)
            {
                units.Add(tile.Unit);
            }
        }
        return units;
    }
    public HashSet<IUnit> GetUnitsInRange(ITile origin, int distance)
    {
        return GetUnitsInRange(grid.GetNeighbours(origin.LocalCoordinates,distance,true) as Range);
    }
    public HashSet<IUnit> GetAlliesInSet(IUnit referenceUnit, HashSet<IUnit> units)
    {
        HashSet<IUnit> allies = new HashSet<IUnit>();
        foreach(IUnit unit in units)
        {
            if(unit.TeamNumber == referenceUnit.TeamNumber)
            {
                allies.Add(unit);
            }
        }
        return allies;
    }
    public HashSet<IUnit> GetEnemiesInSet(IUnit referenceUnit, HashSet<IUnit> units)
    {
        HashSet<IUnit> enemies = new HashSet<IUnit>();
        foreach(IUnit unit in units)
        {
            if(unit.TeamNumber != referenceUnit.TeamNumber)
            {
                enemies.Add(unit);
            }
        }
        return enemies;
    }
    
    //Unit Location Methods => Consider moving to another class to separate the dependency
    public ITile GetTileFromUnit(IUnit unit)
    {
        //This method depends on Tile directly, breaking the dependency inversion rule -> Separate?
        Unit concreteUnit = unit as Unit;
        Vector3 unitPosition = concreteUnit.transform.position;
        return grid.GetElement(grid.WorldToLocal(unitPosition));
    }
    public IUnit GetUnitFromTile(ITile tile)
    {
        return tile.Unit; //This is totally redundant at the moment
    }
    public float GetDistance(IUnit fromUnit, IUnit toUnit)
    {
        return grid.CalculateDistance(GetTileFromUnit(fromUnit).LocalCoordinates, GetTileFromUnit(toUnit).LocalCoordinates);
    }
}