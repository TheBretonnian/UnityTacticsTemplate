using System.Drawing;
using System.Threading;

public class Tile : MonoBehaviour, ITile, IPathfindingNode, ITileVisual
{
    //Private fields
    ITileVisual tileVisual;
    
    //Public Properties
    public Vector2Int LocalCoordinates{ get; private set;} //Shared by ITile and IPathfindingNoe
    public IUnit Unit {get; set;}
    public bool _IsWalkable { get; set;}
    public IPathfindingNode CameFrom { get; set; }
    public bool IsInZoC { get; set; }
    public int ZoCPenalty { get; set; }
    public float FCost { get; private set;}
    public float GCost { get; set; }
    public float HCost { get; set; }

    public void Initialize(Vector2Int localCoordinates)
    {
        LocalCoordinates = localCoordinates;
        //Set default values
        _IsWalkable = true;
        IsInZoC = false;
        ZoCPenalty = 0;
    }

    public void Highlight(Color color)
    {
        tileVisual.Highlight(color); //Act as wrapper or facade for tileVisuals
    }

    public void Outline(Color color)
    {
        tileVisual.Outline(color);
    }

    public void Reset()
    {
        tileVisual.Reset();
    }

    public void UpdateFCost()
    {
         FCost = GCost + HCost;
    }

    public bool IsWalkable()
    {
        //Implement walkable check rules here like:
        return _IsWalkable || IsOccupied(); 
    }

    public bool IsOccupied() => (Unit != null);
}