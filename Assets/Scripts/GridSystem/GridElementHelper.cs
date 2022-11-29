using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class GridElementHelper : MonoBehaviour
{
    public GridElement gridElement = null;
    public PathfindingNode pathfindingNode = null;
    public GridVisuals.GridVisualElement2D gridVisualElement = null;

    public void Setup(GridElement gridElement, PathfindingNode pathfindingNode, GridVisuals.GridVisualElement2D gridVisualElement)
    {
        //Setup references
        this.gridElement = gridElement;
        this.pathfindingNode = pathfindingNode;
        this.gridVisualElement = gridVisualElement;
    }

    [ContextMenu("ToggleObstacle")]
    public void ToggleObstacle()
    {
        //Toggle pathfinding
        pathfindingNode.IsWalkable = !pathfindingNode.IsWalkable;
        gridVisualElement.SetLocked(!pathfindingNode.IsWalkable);
        gridVisualElement.SetVisible(!pathfindingNode.IsWalkable, Color.black);
    }
}
