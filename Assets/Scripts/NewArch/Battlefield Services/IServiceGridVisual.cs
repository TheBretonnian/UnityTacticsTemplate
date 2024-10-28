using UnityEngine;

public interface IServiceGridVisual
{
    void HighlightRange(Range range, Color color);
    //return index of Outline as int (Object Pool)
    int OutlineRange(Range range, Color color, int lineType = 0); //lineType optional -> eventually use Enum in concrete class
}