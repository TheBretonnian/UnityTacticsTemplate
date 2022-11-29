using System.Collections;
using UnityEngine;

public interface IGridVisual
{
    public void MarkAsReachableOneMove(int x, int y);
    public void MarkAsReachableTwoMove(int x, int y);
    public void MarkAsEnemyInMeeleAttackRange(int x, int y);
    public void MarkAsEnemyInRangeAttackRange(int x, int y);
    public void MarkAsPossibleTargetSpecialAbility(int x, int y);

}