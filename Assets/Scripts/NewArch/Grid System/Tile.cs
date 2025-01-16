using UnityEngine;

public class Tile : MonoBehaviour, ITile, ITileVisual, IPathfindingNode , ISelectableTarget
{
    //Private fields
    ITileVisual tileVisual;
    [SerializeField] TileData tileData;
    //Indicates if this terrain is walkable at all (false if contains impassible obstacle)
    bool isWalkableTerrain;
    //Holds current status of passable (temporarily false if occupied by enemy unit)
    bool isPassable;
    
    //Public Properties (ITile)
    public Vector2Int LocalCoordinates{ get; private set;} //Shared by ITile and IPathfindingNoe
    public IUnit Unit {get; set;}
    
    //Public Properties (IPathfinding)
    public IPathfindingNode CameFrom { get; set; }
    public bool IsInZoC { get; set; }
    public bool IsPassable { get => isPassable; set => isPassable = value; }
    public int ZoCPenalty { get; set; }
    public float FCost { get => GCost + HCost;} //Total cost estimation of traversing through this node
    public float GCost { get; set; } //Cost of the path from origin to this node
    public float HCost { get; set; } //Heuristic cost of the path from this node to the goal node
    public float MovingCost { get => tileData.MovementCost; }

    //Public Properties ITarget
    public bool IsUnit{get => false;}
    public IUnit GetUnit{get => Unit;}
    

    //Public tileData
    public TileData TileData { get => tileData; set => tileData = value; }



    public void Initialize(Vector2Int localCoordinates)
    {
        LocalCoordinates = localCoordinates;
        //Set default values
        isWalkableTerrain = true;
        isPassable = true;
        IsInZoC = false;
        ZoCPenalty = 0;
    }

    //Public method ITileVisual: Act as wrapper or facade for tileVisuals
    public void Highlight(Color color) => tileVisual.Highlight(color);
    public void Outline(Color color) => tileVisual.Outline(color);

    public void ClearHighlight() =>  tileVisual.ClearHighlight();
    public void ClearOutline() =>  tileVisual.ClearOutline();
    public void Reset() => tileVisual.Reset();

    //Public methods IPathfinding
    // public void UpdateFCost()
    // {
    //      FCost = GCost + HCost;
    // }

    public bool IsWalkable()
    {
        //Implement walkable check rules here like:
        return isWalkableTerrain && isPassable;
    }

    public bool IsOccupied() => Unit != null;

    public void Selected()
    {
        throw new System.NotImplementedException();
    }

    public void Deselected()
    {
        throw new System.NotImplementedException();
    }

    public void OnHoverEnter()
    {
        throw new System.NotImplementedException();
    }

    public void OnHoverExit()
    {
        throw new System.NotImplementedException();
    }
}