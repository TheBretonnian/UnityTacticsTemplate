using System.Drawing; //Dummy reference for Color class

public interface ITileVisual
{
    public void Highlight(Color color);

    public void Outline(Color color);

    public void Reset();
}