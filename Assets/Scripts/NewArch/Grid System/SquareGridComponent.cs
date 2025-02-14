using System.Collections.Generic;
using UnityEngine;

//As MonoBehaviour you can add this component to your Grid Object to decide
// whether it is a square grid or hex grid. The game designer can decide directly in Unity Editor.
// Implementing the interface allow that other components access directly to the grid methods 
// without needing to know:
// 1. the grid field
// 2. the grid layout (geometry) : square / hex / others...
public class SquareGridComponent : MonoBehaviour, IGrid<ITile>, IBorderOutliner, IGridManagement
{
    [SerializeField] private int _height = 10;
    [SerializeField] private int _width = 10;
    [SerializeField] private int _cellSize = 1;
    [SerializeField] private bool _isGridXZ = false;

    [SerializeField] private bool createGridOnStart = true;

    //So this component can be used stand-alone without a GridManager
    private GameObject tilePrefab; 
    private SquareGrid<ITile> grid; //This can be converted to SquareGrid<Tile> in case this class goes into User Case layer
    private SquareGridBorderOutline borderOutliner;
    

    //Properties
    public int Width {get => _width;}
    public int Height {get => _height;}
    public int CellSize {get => _cellSize;}
    public bool IsInitialized {get => grid.IsInitialized;}


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
        return grid.LocalToWorld(localCoordinates);
    }

    public Vector3 LocalToCellCenterWorld(Vector2Int localCoordinates)
    {
        return grid.LocalToCellCenterWorld(localCoordinates);
    }

    public Vector2Int WorldToLocal(Vector3 worldPosition)
    {
        return grid.WorldToLocal(worldPosition);
    }

    public List<ITile> GetLine(Vector2Int orig, Vector2Int dest)
    {
        return grid.GetLine(orig,dest);
    }

    public void OutlineBorderOfRange(Range range, LineRenderer lineRenderer)
    {
        borderOutliner.OutlineBorderOfRange(range,lineRenderer);
    }

    
    void Awake()
    {
        grid = new SquareGrid<ITile>(Width,Height,CellSize,transform.position,_isGridXZ);
        borderOutliner = new SquareGridBorderOutline(grid);
    }

    void Start()
    {
        if(createGridOnStart){ CreateGrid();}
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
        grid.SetInitialized();
    }
    public void DestroyGrid()
    {
        throw new System.NotImplementedException();
    }
    public void ResetGrid()
    {
        DestroyGrid();
        CreateGrid();
    }

    public ITile CreateTile(Vector2Int localCoord)
    {
        ITile newTile;
        //TO DO: Simplify the code if this become a architecture constraint -> See CreateTileSimple
        if(typeof(MonoBehaviour).IsAssignableFrom(typeof(Tile)))
        {
            GameObject newChild = Instantiate(tilePrefab, grid.LocalToCellCenterWorld(localCoord), Quaternion.identity, transform);
            newChild.name = $"{tilePrefab.name}_{localCoord.x}_{localCoord.y}";
            if (newChild.TryGetComponent<ITile>(out newTile) == false)
            {
                Debug.LogError("Error getting Tile component. Make sure tilePrefab includes Tile component.");
            }
        }
        else
        {
            //A factory will be required to separete entities (GridComponent) from User Case layer (Tile) -> Althoug maybe GridComponent could belong to use case layer replacing the factory
            newTile = new Tile();             
        }
        
        return newTile;
    }

    public ITile CreateTileSimple(Vector2Int localCoord)
    {
        GameObject newChild;
        newChild = Instantiate(tilePrefab, grid.LocalToCellCenterWorld(localCoord), Quaternion.identity, transform);
        newChild.name = $"{tilePrefab.name}_{localCoord.x}_{localCoord.y}";
        return newChild.GetComponent<ITile>();
    }
}