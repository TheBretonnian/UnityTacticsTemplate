using System.Collections.Generic;
using UnityEngine;

public interface IServiceGridVisual
{
    //Highlight IEnumerable to be more flexible: accepts either Range (HashSet) or List (for lines)
    void HighlightRange(IEnumerable<ITile> range, Color color);
    void ClearHighlightRange(IEnumerable<ITile> range);

    //Outlines
    //return index of Outline as int (Object Pool)
    int OutlineRange(Range range, Color color, int lineType = 0); //lineType optional -> eventually use Enum in concrete class
    void ClearOutline(int outlineId);
    void ClearAllOutlines();
}