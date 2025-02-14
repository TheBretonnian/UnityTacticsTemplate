using System.Collections.Generic;
using UnityEngine;

public class ServiceLoSandCover : IServiceLoSandCover
{
    private IServiceGrid _grid;
    private IServiceUnitLocation _unitLocator;

    public ServiceLoSandCover(IServiceGrid grid, IServiceUnitLocation unitLocator)
    {
        _grid = grid;
        _unitLocator = unitLocator;
    }

    public Range GetRangeWithLoS(ITile origin, int distance)
    {
        Range normalRange = _grid.GetRange(origin,distance);
        Range rangeWithLoS = new Range();
        foreach(ITile tile in normalRange)
        {
            if(tile == origin)
            {
                rangeWithLoS.Add(tile);
            }
            else if(HasLos(origin,tile))
            {
                rangeWithLoS.Add(tile);
            }
        }
        return rangeWithLoS;
    }

    public bool HasLos(IUnit attacker, IUnit defender)
    {
        ITile attacker_tile = _unitLocator.GetTileFromUnit(attacker);
        ITile defender_tile = _unitLocator.GetTileFromUnit(defender);

        return HasLos(attacker_tile,defender_tile);
    }

    public bool HasLos(IUnit attacker, IUnit defender, out List<ITile> interferingCovers, out List<IUnit> interferingUnits)
    {
        ITile attacker_tile = _unitLocator.GetTileFromUnit(attacker);
        ITile defender_tile = _unitLocator.GetTileFromUnit(defender);

        return HasLos(attacker_tile,defender_tile,out interferingCovers, out interferingUnits);
    }

    public bool HasLos(ITile orig, ITile dest)
    {
        List<ITile> lineOfTiles = _grid.GetLineOfTiles(orig,dest);

        foreach (ITile tile in lineOfTiles)
        {
            if (tile is Tile targetTile)
            {
                bool isLoSBlocked = false;

                //Check terrain
                if(targetTile.TileData.BlocksLoS)
                {
                    isLoSBlocked = true;
                }
                //Check unit in middle tiles: assumption: all units block line of sight
                if(tile != lineOfTiles[0] && tile != lineOfTiles[lineOfTiles.Count - 1])
                {
                    if(targetTile.Unit != null) //Has Unit
                    {
                        isLoSBlocked = true;
                    }
                }

                // LoS is blocked, return false and stop checking
                if(isLoSBlocked){ return false;}
            }
        }
        return true;
    }

    public bool HasLos(ITile orig, ITile dest, out List<ITile> interferingCovers, out List<IUnit> interferingUnits)
    {
        List<ITile> lineOfTiles = _grid.GetLineOfTiles(orig,dest);

        bool isLoSBlocked = false;

        interferingCovers = new List<ITile>();
        interferingUnits = new List<IUnit>();

        foreach (ITile tile in lineOfTiles)
        {
            if (tile is Tile targetTile)
            {
                //Check terrain
                if(targetTile.TileData.BlocksLoS)
                {
                    interferingCovers.Add(tile);
                    isLoSBlocked = true;
                }
                //Check unit in middle tiles: assumption: all units block line of sight
                if(tile != lineOfTiles[0] && tile != lineOfTiles[lineOfTiles.Count - 1])
                {
                    if(targetTile.Unit != null) //Has Unit
                    {
                        isLoSBlocked = true;
                        interferingUnits.Add(targetTile.Unit);
                    }
                }
            }
        }
        return !isLoSBlocked;
    }

    private float GetCover(IUnit defender)
    {
        float cover = 0.0f;
        ITile tile = _unitLocator.GetTileFromUnit(defender);
        if (tile is Tile targetTile)
        {
            cover = targetTile.TileData.EvasionBonus;
        }
        return cover;
    }

    public bool HasCover(IUnit attacker, IUnit defender)
    {
        return GetCover(defender) > 0.0f;
        
    }

    public float GetNormalizedCover(List<ITile> interferingCovers)
    {
        float normalizedCover = 0.0f;
        foreach(ITile tile in interferingCovers)
        {
            if (tile is Tile targetTile)
            {
                if(targetTile.TileData.BlocksLoS)
                {
                    normalizedCover = 1.0f;
                }
            }
        }
        return normalizedCover;
    }

    public bool GetNormalizedCoverIfLos(IUnit attacker, IUnit defender, out float cover)
    {
        cover = GetCover(defender);
        return HasLos(attacker,defender);
    }

    public bool GetNormalizedCoverIfLoS(IUnit attacker, IUnit defender, out float cover, out List<ITile> interferingCovers, out List<IUnit> interferingUnits)
    {
        cover = GetCover(defender);
        return HasLos(attacker,defender,out interferingCovers,out interferingUnits);
    }
}