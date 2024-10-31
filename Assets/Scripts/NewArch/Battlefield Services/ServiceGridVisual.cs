using System.Collections.Generic;
using UnityEngine;

public class ServiceGridVisual : IServiceGridVisual
{

    private readonly GridManager gridManager;
    private readonly ObjectPool<LineRenderer> lineRendererPool;
    private readonly IBorderOutliner squareGridBorderOutline;

    private List<LineRenderer> activeLineRenderer = new List<LineRenderer>();

    public ServiceGridVisual(GridManager gridManager, LineRenderer lineRendererPrefab, Transform parent = null)
    {
        this.gridManager = gridManager;
        lineRendererPool = new ObjectPool<LineRenderer>(lineRendererPrefab,3, parent);
        squareGridBorderOutline = new SquareGridBorderOutline(gridManager.Grid); //TO DO: Square vs Hex? Wrong place to be concrete
    }

    public void HighlightRange(Range range, Color color)
    {
        foreach(ITile tile in range)
        {
            if(tile is Tile concreteTile)
            {
                concreteTile.Highlight(color);
            }
        }
    }

    public void ClearHighlightRange(Range range)
    {
        foreach(ITile tile in range)
        {
            if(tile is Tile concreteTile)
            {
                concreteTile.ClearHighlight();
            }
        }
    }

    public void ClearOutline(int outlineId)
    {
        if(outlineId < activeLineRenderer.Count)
        {
            LineRenderer lineRenderer = activeLineRenderer[outlineId];
            activeLineRenderer.RemoveAt(outlineId);
            lineRenderer.positionCount = 0;
            lineRendererPool.ReturnToPool(lineRenderer);
        }      
    }

    public void ClearAllOutlines()
    {
        foreach(LineRenderer lineRenderer in activeLineRenderer)
        {
            lineRenderer.positionCount = 0;
            lineRendererPool.ReturnToPool(lineRenderer);
        }
        activeLineRenderer.Clear();
    }

    public int OutlineRange(Range range, Color color, int lineType = 0)
    {
        //Prepare lineRenderer
        LineRenderer newLineRenderer = lineRendererPool.Get();
        newLineRenderer.startColor = newLineRenderer.endColor = color;
        //Apply logic for lineType
        //Populate it using a BorderOutliner
        squareGridBorderOutline.OutlineBorderOfRange(range, newLineRenderer);
        //Add to the list of active lineRenderer
        activeLineRenderer.Add(newLineRenderer);
        return activeLineRenderer.Count - 1;

    }
}