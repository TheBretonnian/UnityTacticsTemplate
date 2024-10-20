using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks.Dataflow;
using UnityEngine;

//As MonoBehaviour you can add this component to your Grid Object to decide
// whether it is a square grid or hex grid. The game designer can decide directly in Unity Editor.
// Implementing the interface allow that other components access directly to the grid methods 
// without needing to know:
// 1. the grid field
// 2. the grid layout (geometry) : square / hex / others...
public class SquareGridComponent : MonoBehaviour, IGrid<ITile>
{
    //Fields
    private int _height;
    private int _width;
    private int _cellSite;

    //So this component can be used stand-alone without a GridManager
    private GameObject tilePrefab; 
    private SquareGrid<ITile> grid; //This can be converted to SquareGrid<Tile> in case this class goes into User Case layer
    

    //Properties
    public int Width {get => _width;}
    public int Height {get => _height;}
    public int CellSize {get => _cellSite;}


    //Public methods
    //TO DO: For all public wrapper methods => Add protection code to check if grid is initialized
    public float CalculateDistance(Vector2Int orig, Vector2Int dest)
    {
        return grid.CalculateDistance(orig,dest);
    }

    public ITile GetElement(Vector2Int localCoordinates)
    {
        return grid.GetElement(localCoordinates);
    }

    public void SetElement(Vector2Int localCoordinates, ITile tile)
    {
        grid.SetElement(localCoordinates, tile);
    }

    public HashSet<ITile> GetNeighbours(Vector2Int orig, int distance, bool diagonalAllowed)
    {
        return grid.GetNeighbours(orig,distance,diagonalAllowed);
    }

    public bool AreValidCoordinates(Vector2Int localCoordinates)
    {
        return grid.AreValidCoordinates(localCoordinates);
    }

    public Vector3 LocalToCellWorld(Vector2Int localCoordinates)
    {
        return grid.LocalToCellWorld(localCoordinates);
    }

    public Vector3 LocalToCellCenterWorld(Vector2Int localCoordinates)
    {
        return grid.LocalToCellCenterWorld(localCoordinates);
    }

    public Vector2Int WorldToLocal(Vector3 worldPosition)
    {
        return grid.WorldToLocal(worldPosition);
    }

    //Mockup for Unity Message
    private void Start()
    {
        Vector3 origin = new Vector3(0.0f,0.0f,0.0f);
        //origin = transform.position;
        grid = new SquareGrid<ITile>(Width,Height,CellSize,origin);
    }

    public void CreateGrid()
    {
        for(int x=0; x < Width; x++)
        {
            for(int y=0; y < Height; y++)
            {
                Vector2Int localCoords = new Vector2Int(x,y);
                ITile newTile = CreateTile(localCoords);
                newTile.Initialize(localCoords);
                grid.SetElement(localCoords, newTile);
            }
        }
    }

    //The following methods can delegate to GridComponent
    public ITile CreateTile(Vector2Int localCoord)
    {
        Tile newTile;
        //TO DO: Simplify the code if this become a architecture constraint -> See CreateTileSimple
        // if(typeof(MonoBehaviour).IsAssignableFrom(typeof(Tile)))
        // {
        //     GameObject newChild = Instantiate(tilePrefab, grid.Local2WorldCenterPosition(localCoord), Quaternion.identity, transform);
        //     newChild.name = $"{tilePrefab.name}_{localCoord.x}_{localCoord.y}";
        //     if (newChild.TryGetComponent<ITile>(out newTile) == false)
        //     {
        //         Debug.LogError("Error getting Tile component. Make sure tilePrefab includes Tile component.");
        //     }
        // }
        // else
        // {

        //A factory will be required to separete entities (GridComponent) from User Case layer (Tile) -> Althoug maybe GridComponent could belong to use case layer replacing the factory
            newTile = new Tile(); 
            
        // }
        
        return newTile;
    }

    public ITile CreateTileSimple(Vector2Int localCoord)
    {
        // GameObject newChild;
        // newChild = Instantiate(tilePrefab, grid.Local2WorldCenterPosition(localCoord), Quaternion.identity, transform)
        // newChild.name = $"{tilePrefab.name}_{localCoord.x}_{localCoord.y}";
        // return newChild.GetComponent<ITile>();

        return new Tile();//Stub code
    }
}