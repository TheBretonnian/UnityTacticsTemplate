using UnityEngine;
//This class is a place holder for TileData, 
// to be able to define terrain feature with GameObjects in Editor.
// After Grid Creation using stamdard prefab, the TileData 
// is copied into corresponding Tile.
// Afterwards, the game object this script belongs to is removed
// if RemoveAfterInit is true.
public class TerrainDescriptor : MonoBehaviour
{
  public TileData TileData;
  public bool RemoveAfterInit = false;
}
