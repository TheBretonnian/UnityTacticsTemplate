using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class GameGrid<TGridElement>
{
    private int _width, _height;
    private int _cellSize;
    private float _diagonalDistance;
    private Vector3 _origin;
    public event EventHandler<OnGridChangedEventArgs> OnGridChanged;

    private TGridElement[,] gridElements;
    
    //Debug Text
    private TextMesh[,] gridText;
    private const int fontSize = 2; 


    /* Delegate to initialize generic: */
    private Func<GameGrid<TGridElement>, int, int, TGridElement> createGridObjectDelegate;

    public int Width { get => _width;}
    public int Height { get => _height;}

    //Constructor with initialization
    public GameGrid(int width, int height, int cellSize, Vector3 origin, Func<GameGrid<TGridElement>, int, int, TGridElement> createGridObject, /* Optional parameter for arrange UnityGameObjects */ Transform debugTextParent = null)
    {
        _height = height;
        _width = width;
        _origin = origin;
        _cellSize = cellSize;
        _diagonalDistance = Mathf.Sqrt(2 * cellSize * cellSize);
        createGridObjectDelegate = createGridObject;

        gridElements = new TGridElement[width, height];
        gridText = new TextMesh[width, height];

        for (int x=0; x<_width; x++)
        {
            for (int y=0; y<_height; y++)
            {
                //Initialize every element of the grid with given delegate
                gridElements[x, y] = createGridObject(this, x, y);
                //Create Debug Text if debugTextParent is not null
                if(debugTextParent!=null)
                {
                    gridText[x, y] = CreateWorldText(debugTextParent, gridElements[x, y].ToString(), GetWorldCenterPosition(x, y), fontSize, Color.white, TextAnchor.MiddleCenter, TextAlignment.Center, 1000, $"{typeof(TGridElement).ToString()}_DebugText_{x}_{y}");
                    //gridText[x, y].gameObject.SetActive(true);
                    //Improve TextMesh sharpness
                    gridText[x, y].characterSize = 0.1f;
                    gridText[x, y].fontSize *= 10;
                }
                               
            }
        }
        //Use event for debug text
        if (debugTextParent != null)
        {
            OnGridChanged += (object sender, OnGridChangedEventArgs eventArgs) => gridText[eventArgs.x, eventArgs.y].text = gridElements[eventArgs.x, eventArgs.y].ToString();
        }
            

    }

    //Return cell origin (bottom left)
    public Vector3 GetWorldPosition(int x, int y)
    {
        return new Vector3(x, y) * _cellSize + _origin;
    }

    //Return cell center
    public Vector3 GetWorldCenterPosition(int x, int y)
    {
        return new Vector3(x + 0.5f, y + 0.5f) * _cellSize + _origin;
    }

    public bool GetXY(Vector3 position, out int x, out int y)
    {
        x = Mathf.FloorToInt(position.x - _origin.x);
        y = Mathf.FloorToInt(position.y - _origin.y);
        return (x >= 0 && x < _width && y >= 0 && y < _height);

    }

    public bool AreValidCoordenates(int x, int y)
    {
        return (x >= 0 && x < _width && y >= 0 && y < _height);
    }

    public void SetGridElement(int x, int y, TGridElement gridElement)
    {
        if (AreValidCoordenates(x,y))
        {
            gridElements[x, y] = gridElement;
            OnGridChanged?.Invoke(this, new OnGridChangedEventArgs { x = x, y = y });
        }

    }

    public void SetGridElement(Vector3 position, TGridElement gridElement)
    {
        if (GetXY(position, out int x, out int y))
        {
            SetGridElement(x, y, gridElement);
        }
    }

    public void TriggerGridChangedEvent(int x, int y)
    {
        OnGridChanged?.Invoke(this, new OnGridChangedEventArgs { x = x, y = y });
    }

    public TGridElement GetGridElement(int x, int y)
    {
        if (AreValidCoordenates(x,y))
            return gridElements[x, y];
        else
            return default(TGridElement); //defualt in classes is null
    }

    public TGridElement GetGridElement(Vector3 position)
    {
        GetXY(position, out int x, out int y);
        return GetGridElement(x, y);
    }


    public List<TGridElement> GetNeighbours(int x, int y, int distance, bool diagonalAllowed = true)
    {
        List<TGridElement> listOfNeighbours = new List<TGridElement>();

        if(AreValidCoordenates(x,y))
        {
            for(int i=x-distance;i<=x+distance;i++)
            {
                for(int j = y - distance; j <= y + distance; j++)
                {
                    if (i == x && j == y)
                    {
                        //Skip element in given coordenates. Only interest in neighbours.
                    }
                    else
                    {
                        if (AreValidCoordenates(i, j))
                        {
                            if(diagonalAllowed == false)
                            {
                                //Check distance
                                Vector2 origin = new Vector2(x, y);
                                Vector2 ElementPosition = new Vector2(i, j);
                                Vector2 Delta = ElementPosition - origin;
                                if((Mathf.Abs(Delta.x) + Mathf.Abs(Delta.y)) > distance)
                                {
                                    //skip this element since distance is greater
                                    continue;
                                }

                            }
                            listOfNeighbours.Add(GetGridElement(i, j));
                        }
                    }
                    
                }
            }
        }

        return listOfNeighbours;
    }

    public void ResetGrid()
    {
        for (int x = 0; x < _width; x++)
        {
            for (int y = 0; y < _height; y++)
            {
                gridElements[x, y] = createGridObjectDelegate(this,x,y);
                TriggerGridChangedEvent(x, y);
            }
        }
    }

    public void DrawGridLines(int drawDuration)
    {
        for (int x = 0; x < _width; x++)
        {
            for (int y = 0; y < _height; y++)
            {   
                Debug.DrawLine(GetWorldPosition(x,y), GetWorldPosition(x + 1, y), Color.white, drawDuration, false);
                Debug.DrawLine(GetWorldPosition(x,y), GetWorldPosition(x, y + 1), Color.white, drawDuration, false);
                
            }
        }
        Debug.DrawLine(GetWorldPosition(0, _height), GetWorldPosition(_width, _height), Color.white, drawDuration, false);
        Debug.DrawLine(GetWorldPosition(_width,  0), GetWorldPosition(_width, _height), Color.white, drawDuration, false);
    }

    public void ShowDebugText(bool enabled)
    {
        for (int x = 0; x < _width; x++)
        {
            for (int y = 0; y < _height; y++)
            {
                gridText[x, y]?.gameObject.SetActive(enabled);
            }
        }
    }

    // Create Text in the World
    public static TextMesh CreateWorldText(string text, Transform parent = null, Vector3 localPosition = default(Vector3), int fontSize = 40, Color? color = null, TextAnchor textAnchor = TextAnchor.UpperLeft, TextAlignment textAlignment = TextAlignment.Left, int sortingOrder = 5000, string objectName = "WorldText")
    {
        if (color == null) color = Color.white;
        return CreateWorldText(parent, text, localPosition, fontSize, (Color)color, textAnchor, textAlignment, sortingOrder, objectName);
    }

    // Create Text in the World
    public static TextMesh CreateWorldText(Transform parent, string text, Vector3 localPosition, int fontSize, Color color, TextAnchor textAnchor, TextAlignment textAlignment, int sortingOrder, string objectName)
    {
        GameObject gameObject = new GameObject(objectName, typeof(TextMesh));
        Transform transform = gameObject.transform;
        transform.SetParent(parent, false);
        transform.localPosition = localPosition;
        TextMesh textMesh = gameObject.GetComponent<TextMesh>();
        textMesh.anchor = textAnchor;
        textMesh.alignment = textAlignment;
        textMesh.text = text;
        textMesh.fontSize = fontSize;
        textMesh.color = color;
        textMesh.GetComponent<MeshRenderer>().sortingOrder = sortingOrder;
        return textMesh;
    }

    public float CalculateDistance(int start_x, int start_y, int end_x, int end_y, bool diagonalAllowed=true)
    {
        int horizontal = Math.Abs(end_x - start_x);
        int vertical = Math.Abs(end_y - start_y);
        int rest = Math.Abs(vertical - horizontal);

        if (diagonalAllowed)
        {
            return (float)_cellSize * (float)rest + (float)Math.Min(horizontal, vertical) * _diagonalDistance;
        }
        else
        {
            return (float)_cellSize * (float)horizontal + (float)_cellSize * (float)vertical;
        }

    }
}

public class OnGridChangedEventArgs : EventArgs
{
    public int x;
    public int y;
}
