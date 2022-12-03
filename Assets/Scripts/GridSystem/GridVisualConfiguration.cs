using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class GridVisualConfiguration
{
    public Color ReachableOneMoveColor = Color.green;
    public Color ReachableTwoMoveColor = Color.yellow;
    public Color EnemyInRangeAttackRangeColor = Color.red;
    public Color EnemyInMeleeAttackRangeColor = Color.red;
    public Color DangerZoneColor = Color.red;
    public Color SpecialAbilityColor = Color.cyan;

    [ContextMenu("Reset colors to default")]
    public void ResetColorsToDefault()
    {
             ReachableOneMoveColor = Color.green;
             ReachableTwoMoveColor = Color.yellow;
             EnemyInRangeAttackRangeColor = Color.red;
             EnemyInMeleeAttackRangeColor = Color.red;
             DangerZoneColor = Color.red;
             SpecialAbilityColor = Color.cyan;
    }
}
