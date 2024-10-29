using UnityEngine;

public interface IServiceGridVisual
{
    //Highlights
    void HighlightRange(Range range, Color color);
    void ClearHighlightRange(Range range);

    //Outlines
    //return index of Outline as int (Object Pool)
    int OutlineRange(Range range, Color color, int lineType = 0); //lineType optional -> eventually use Enum in concrete class
    void ClearOutline(int outlineId);
    void ClearAllOutlines();
}