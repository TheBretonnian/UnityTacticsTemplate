using UnityEngine;

public interface ITileVisual
{
    void Highlight(Color color);

    void Outline(Color color);

    void Reset();
}