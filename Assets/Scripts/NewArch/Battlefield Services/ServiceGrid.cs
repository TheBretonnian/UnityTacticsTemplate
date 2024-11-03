using System.Collections.Generic;
using UnityEngine;

public class ServiceGrid : IServiceGrid
{
    IGrid<ITile> grid;

    public ServiceGrid(IGrid<ITile> grid)
    {
        this.grid = grid;
    }

    public Range GetRange(ITile origin, int distance)
    {
        //diagonalAllowed hardcoded to true -> Consider parametrization
        return grid.GetNeighbours(origin.LocalCoordinates,distance,true) as Range;
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
        return GetUnitsInRange(GetRange(origin,distance));
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

    //Locators
    //The worldPosition can be easy to obtain from use case layers 
    //so the bussines rules are not contaminated with a concrete tile dependency
    public ITile GetTileFromWorldPosition(Vector3 worldPosition)
    {
        return grid.GetElement(grid.WorldToLocal(worldPosition));
    }
    

    //Line Methods
    public float GetDistance(ITile orig, ITile dest)
    {
        return grid.CalculateDistance(orig.LocalCoordinates, dest.LocalCoordinates);
    }
    public Range GetLineOfTiles(ITile orig, ITile dest)
    {
        throw new System.NotImplementedException();
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