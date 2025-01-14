using System;
using UnityEngine;

//To be referenced by Tile class
[CreateAssetMenu(menuName = "Terrain/TileData")]
public class TileData : ScriptableObject
{   
    public int MovementCost = 1;
    public bool IsDifficultTerrain = false;
    public bool BlocksMovement = false; //IsWalkable
    public bool BlocksLoS = false;

    //"Cover" data
    public int MeleeDefenseBonus = 0;
    public int RangedDefenseBonus = 0;

    //Special effects
    public int MeleeAttackBonus = 0;
    public int RangeAttackBonus = 0;
    public int MagicAttackBonus = 0;

    public int DamagePerRound = 0; //Damage caused to units occuping this tile
    public int HealingPerRound = 0; //Healing caused to units occuping this tile

    //For later
    public float EvasionBonus = 0;
}