using System.Collections.Generic;

//As MonoBehaviour you can add this component to your Grid Object to decide
// whether it is a square grid or hex grid. The game designer can decide directly in Unity Editor.
// Implementing the interface allow that other components access directly to the grid methods 
// without needing to know:
// 1. the grid field
// 2. the grid layout (geometry) : square / hex / others...
public class SquareGridComponent : MonoBehaviour, IGrid<Tile>
{
    //Fields
    private int _height;
    private int _width;

    private SquareGrid<Tile> grid;

    //Properties
    public int Height {get => _height;}
    public int Width {get => _width;}


    //Public methods
    //TO DO: For all public wrapper methods => Add protection code to check if grid is initialized
    public float CalculateDistance(Vector2Int orig, Vector2Int dest)
    {
        return grid.CalculateDistance(orig,dest);
    }

    public Tile GetElement(Vector2Int localCoordinates)
    {
        return grid.GetElement(localCoordinates);
    }

    public HashSet<Tile> GetNeighbours(Vector2Int orig, int distance, bool diagonalAllowed)
    {
        return grid.GetNeighbours(orig,distance,diagonalAllowed);
    }

    //Mockup for Unity Message
    private void Start()
    {
        grid = new SquareGrid<Tile>(Width,Height);
    }
}



public class Tile{};