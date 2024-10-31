using UnityEngine;

public interface ITileVisual
{
    
    void Highlight(Color color);
    void Outline(Color color);

    void ClearHighlight();
    void ClearOutline();
    void Reset();
}