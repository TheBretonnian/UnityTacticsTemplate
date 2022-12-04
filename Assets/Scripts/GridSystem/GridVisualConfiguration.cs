using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable, CreateAssetMenu(fileName ="GridVisualConfig", menuName ="TacticsTemplate/GridVisualConfig",order= 0)]
public class GridVisualConfiguration : ScriptableObject
{
    public Color ReachableOneMoveColor;
    public Color ReachableTwoMoveColor;
    public Color EnemyInRangeAttackRangeColor;
    public Color EnemyInMeleeAttackRangeColor;
    public Color DangerZoneColor;
    public Color SpecialAbilityColor;

    //Tip: call this when you create the asset
    [ContextMenu("Reset colors to default")]
    public void ResetColorsToDefault()
    {
        //Tip: You may use Utils/HexColor to parse hex codes if you one to use different colors as default
        ReachableOneMoveColor = Color.green;
        ReachableTwoMoveColor = Color.yellow;
        EnemyInRangeAttackRangeColor = Color.red;
        EnemyInMeleeAttackRangeColor = Color.red;
        DangerZoneColor = Color.red;
        SpecialAbilityColor = Color.cyan;
    }
}
