using System.Collections.Generic;
using UnityEngine;

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

    //So this component can be used stand-alone without a GridManager
    private GameObject tilePrefab; 

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

    public void SetElement(Vector2Int localCoordinates, Tile tile)
    {
        grid.SetElement(localCoordinates, tile);
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

    public void CreateGrid()
    {
        for(int x=0; x < Width; x++)
        {
            for(int y=0; y < Height; y++)
            {
                Vector2Int localCoords = new Vector2Int(x,y);
                Tile newTile = CreateTile(localCoords);
                newTile.Initialize(localCoords);
                grid.SetElement(localCoords, newTile);
            }
        }
    }

    //The following methods can delegate to GridComponent
    public Tile CreateTile(Vector2Int localCoord)
    {
        Tile newTile;
        //TO DO: Simplify the code if this become a architecture constraint -> See CreateTileSimple
        // if(typeof(MonoBehaviour).IsAssignableFrom(typeof(Tile)))
        // {
        //     GameObject newChild = Instantiate(tilePrefab, grid.Local2WorldCenterPosition(localCoord), Quaternion.identity, transform);
        //     newChild.name = $"{tilePrefab.name}_{localCoord.x}_{localCoord.y}";
        //     if (newChild.TryGetComponent<Tile>(out newTile) == false)
        //     {
        //         Debug.LogError("Error getting Tile component. Make sure tilePrefab includes Tile component.");
        //     }
        // }
        // else
        // {
            newTile = new Tile();
        // }
        
        return newTile;
    }

    public Tile CreateTileSimple(Vector2Int localCoord)
    {
        // GameObject newChild;
        // newChild = Instantiate(tilePrefab, grid.Local2WorldCenterPosition(localCoord), Quaternion.identity, transform)
        // newChild.name = $"{tilePrefab.name}_{localCoord.x}_{localCoord.y}";
        // return newChild.GetComponent<Tile>();

        return new Tile();//Stub code
    }

}