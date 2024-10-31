using System.Collections.Generic;
using UnityEngine;


//This can serve as single entry point for Battlefiled services.
//This class may inherit from MonoBehaviour and be attach to GameObject
//This class depends on tiny classes which implement each specific interface.
public class BattlefieldServices : IServiceGrid, IServiceUnitLocation, IServicePathfinding, IServiceLoSandCover, IServiceGridVisual
{  
    GridManager gridManager; //Assign with dependency injection: Editor or SceneManager
    ServiceGrid serviceGrid;
    ServiceUnitLocation serviceUnitLocation;
    ServiceGridVisual serviceGridVisual;
    LineRenderer lineRendererPrefab; //Assign it with dependency injection if class promote to MonoBehaviour

    public BattlefieldServices(GridManager gridManager, LineRenderer lineRendererPrefab, Transform parent)
    {
        this.gridManager = gridManager;
        this.lineRendererPrefab = lineRendererPrefab;
        serviceGrid = new ServiceGrid(gridManager);
        serviceUnitLocation = new ServiceUnitLocation(gridManager);
        serviceGridVisual = new ServiceGridVisual(gridManager,lineRendererPrefab,parent);
    }
    

    //Pathfinding
    public Range GetRangeWalkable(ITile origin, int distance)
    {
        throw new System.NotImplementedException();
    }
    public bool HasPath(ITile orig, ITile dest, IUnit movingUnit = null)
    {
        throw new System.NotImplementedException();
    }
    public Range GetPath(ITile orig, ITile dest, IUnit movingUnit = null)
    {
        throw new System.NotImplementedException();
    }

    //LoS and Cover
    public Range GetRangeWithLoS(ITile origin, int distance)
    {
        throw new System.NotImplementedException();
    }
    public bool HasCover(IUnit attacker, IUnit defender)
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
    public bool HasLos(IUnit attacker, IUnit defender)
    {
        throw new System.NotImplementedException();
    }
    public bool HasLos(ITile orig, ITile dest)
    {
        throw new System.NotImplementedException();
    }

    //Grid Visuals
    public void HighlightRange(Range range, Color color)
    {
       serviceGridVisual.HighlightRange(range, color);
    }
    public void ClearHighlightRange(Range range)
    {
        serviceGridVisual.ClearHighlightRange(range);
    }

    public int OutlineRange(Range range, Color color, int lineType = 0)
    {
        return serviceGridVisual.OutlineRange(range, color,lineType);
    }

    public void ClearOutline(int outlineId)
    {
        serviceGridVisual.ClearOutline(outlineId);
    }
    public void ClearAllOutlines()
    {
        serviceGridVisual.ClearAllOutlines();
    }

    //ServiceGrid
    public Range GetRange(ITile origin, int distance)
    {
        return serviceGrid.GetRange(origin,distance);
    }

    public ITile GetTileFromWorldPosition(Vector3 worldPosition)
    {
        return serviceGrid.GetTileFromWorldPosition(worldPosition);
    }

    public float GetDistance(ITile orig, ITile dest)
    {
        return serviceGrid.GetDistance(orig,dest);
    }

    public Range GetLineOfTiles(ITile orig, ITile dest)
    {
        return serviceGrid.GetLineOfTiles(orig,dest);
    }

    //ServiceUnitLocation
    public ITile GetTileFromUnit(IUnit unit)
    {
        return serviceUnitLocation.GetTileFromUnit(unit);
    }

    public IUnit GetUnitFromTile(ITile tile)
    {
        return serviceUnitLocation.GetUnitFromTile(tile);
    }

    public float GetDistance(IUnit fromUnit, IUnit toUnit)
    {
        return serviceUnitLocation.GetDistance(fromUnit,toUnit);
    }

    public HashSet<IUnit> GetUnitsInRange(Range tiles)
    {
        return serviceUnitLocation.GetUnitsInRange(tiles);
    }

    public HashSet<IUnit> GetUnitsInRange(ITile origin, int distance)
    {
        return serviceUnitLocation.GetUnitsInRange(origin,distance);
    }

    public HashSet<IUnit> GetEnemiesInSet(IUnit referenceUnit, HashSet<IUnit> units)
    {
        return serviceUnitLocation.GetEnemiesInSet(referenceUnit,units);
    }

    public HashSet<IUnit> GetAlliesInSet(IUnit referenceUnit, HashSet<IUnit> units)
    {
        return serviceUnitLocation.GetAlliesInSet(referenceUnit,units);
    }

    //Services:
    // + IServiceGrid
    // + IServiceUnitLocation
    // + IServiceGridVisual
    // + IServicePathfinding
    // + IServiceLoSandCover
}
