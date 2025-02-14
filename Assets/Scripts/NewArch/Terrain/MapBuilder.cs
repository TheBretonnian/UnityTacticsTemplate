using UnityEngine;

public class MapBuilder : MonoBehaviour
{
  private GridManager gridManager;

   void Awake()
   {
    if(gridManager!= null)
    {
        gridManager.OnGridCreated += UpdateMap;
    }
     
   }

    void UpdateMap()
    {
        // Get all TerrainDescriptor components in children
        TerrainDescriptor[] terrainDescriptors = GetComponentsInChildren<TerrainDescriptor>();
        // use for loop instead since destroying game object may affect Iterator next
        for (int i=0; i<terrainDescriptors.Length; i++)
        {
            TerrainDescriptor descriptor = terrainDescriptors[i];
            // Get ITile in world coordinates using transform.position of TerrainDescriptor's game object
            ITile tile = gridManager.Grid.GetElement(gridManager.Grid.WorldToLocal(descriptor.transform.position));
            
            if (tile is Tile targetTile)
            {
                // Set Tile.TileData with TerrainDescriptor.TileData
                targetTile.TileData = descriptor.TileData;

                // If TerrainDescriptor.RemoveAfterInit, destroy game object of TerrainDescriptor
                if (descriptor.RemoveAfterInit)
                {
                    Destroy(descriptor.gameObject);
                }
            }
        }
    }
}
