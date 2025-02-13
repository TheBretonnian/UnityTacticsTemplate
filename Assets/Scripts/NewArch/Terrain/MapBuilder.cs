using UnityEngine;

public class MapBuilder : MoboBehaviour
{
  private GridManager gridManager;

   void Awake()
   {
     GridManager?.OnGridCreated+=UpdateMap;
   }

  void UpdateMap()
  {
    //Get all TerrainDescriptor components in children
    //For each TerrainDescriptor:
    //  Get ITile in world coordinates using transform.position of TerrainDescriptor's game object
    //  If Valid ITile, cast to Tile
    //  Set Tile.TileData with TerrainDescriptor.TileData
    //  If TerrainDescriptor.RemoveAfterInit, destroy game object of TerrainDescriptor
  }
}
