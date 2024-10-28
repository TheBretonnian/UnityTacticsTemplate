using UnityEngine;

public interface ITileVisual
{
    public void Highlight(Color color);

    public void Outline(Color color);

    public void Reset();
}