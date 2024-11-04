using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using UnityEngine;

public class ServicePathfinding : IServicePathfinding
{
    private IServiceGrid gridService;
    private IServiceUnitLocation unitLocationService;
    private Pathfinding pathfinding;
    private IUnit _lastMovingUnit = null;
    private bool AllowsAllyPassage = true;
    private bool AllowsEnemyPassage = false;


    public ServicePathfinding(Pathfinding pathfinding, IServiceGrid gridService, IServiceUnitLocation serviceUnitLocation, bool allowsEnemyPassage = false, bool allowsAllyPassage = true)
    {
        this.pathfinding = pathfinding;
        this.gridService = gridService;
        unitLocationService = serviceUnitLocation;
        AllowsEnemyPassage = allowsEnemyPassage;
        AllowsAllyPassage = allowsAllyPassage;
    }

    public List<Vector3> GetPath(ITile orig, ITile dest, IUnit movingUnit = null)
    {
        List<Vector3> path = new List<Vector3>();
        //First of all check if dest is free (not ocuppied by Unit)
        if(dest.Unit != null)
        {
            return path;
        }
        if(movingUnit != null)
        {
            pathfinding.ResetPathfindingProperties();
            //Optionally cached movingUnit for performance
            if(_lastMovingUnit != movingUnit)
            {
                _lastMovingUnit = movingUnit;
                //Update Pathfinding nodes for next check:                
                // 1. Update isPassable in Pathfinding Nodes based on
                // AllowsAllyPassage and AllowsEnemyPassable
                HashSet<IUnit> allUnits = unitLocationService.GetAllUnits();
                foreach(IUnit unit in allUnits)
                {
                    if(unit == movingUnit)
                    {
                        continue;
                    }
                    ITile tile = unitLocationService.GetTileFromUnit(unit);
                    //tile.IsPassable = (unit.TeamNumber == movingUnit.TeamNumber) ?  AllowsAllyPassage : AllowsEnemyPassage;
                    if(unit.TeamNumber == movingUnit.TeamNumber)
                    {
                        tile.IsPassable = AllowsAllyPassage;
                    }
                    else
                    {
                        tile.IsPassable = AllowsEnemyPassage;
                        //Update ZoC
                        foreach(ITile neighborTile in gridService.GetRange(tile,1))
                        {
                            neighborTile.IsInZoC = true;
                        }
                    }
                }
            }           
        }

        //Call Pathfinding
       return pathfinding.FindPath(orig.LocalCoordinates.x,orig.LocalCoordinates.y,
                                   dest.LocalCoordinates.x,dest.LocalCoordinates.y);
    }

    public Range GetRangeWalkable(ITile origin, int distance, IUnit movingUnit = null)
    {
        Range range = gridService.GetRange(origin,distance);
        Range walkableRange = new Range();
        foreach(ITile tile in range)
        {
            if(HasPath(origin,tile,movingUnit))
            {
                walkableRange.Add(tile);
            }
        }
        return walkableRange;
    }

    public bool HasPath(ITile orig, ITile dest, IUnit movingUnit = null)
    {
        return GetPath(orig,dest,movingUnit).Count > 0;
    }
}