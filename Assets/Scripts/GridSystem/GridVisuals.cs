using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridVisuals : MonoBehaviour, IGridVisual
{
    [SerializeField] private int _width = 10, _height = 10;
    [SerializeField] private int _cellSize = 1;

    [SerializeField] private GameObject cellVisual;

    [SerializeField, Tooltip("Only for standalone use: Set true only if script is not initialised in other script like GridSystem.")] 
    private bool generateOnStart = false;

    [Header("Grid debug")]
    [SerializeField] private Transform debugTextParent;

    //For 2D
    private GameGrid<GridVisualElement2D> grid;

    // Start is called before the first frame update
    void Start()
    {
        if (generateOnStart) { GenerateGrid(_width, _height, _cellSize); }         
    }

    #region Initialization
    public void GenerateGrid(int width, int height, int cellSize, bool IstantiateVisuals = true)
    {
        
        _width = width;
        _height = height;
        _cellSize = cellSize;

        grid = new GameGrid<GridVisualElement2D>(width, height, cellSize, transform.position, (GameGrid<GridVisualElement2D> grid, int x, int y) => new GridVisualElement2D(grid, x, y), debugTextParent);
        
        if(IstantiateVisuals)
        {
            for (int i = 0; i < width; i++)
            {
                for (int j = 0; j < height; j++)
                {
                    grid.GetGridElement(i, j).InstantiateVisualObject(cellVisual, this.transform);
                }
            }
        }
        

        grid.ShowDebugText(false);

    }

    #endregion

    #region generic methods 
    public void ClearAllVisible()
    {
        for (int i = 0; i < _width; i++)
        {
            for (int j = 0; j < _height; j++)
            {
                if(!grid.GetGridElement(i, j).locked)
                    grid.GetGridElement(i, j).SetVisible(false);
            }
        }
    }

    public void SetVisible(int x, int y, bool visible)
    {
        grid.GetGridElement(x, y)?.SetVisible(visible);
    }

    public void Highlight(int x, int y, Color color)
    {
        grid.GetGridElement(x, y)?.SetVisible(true,color);
    }

    public void SetLocked(int x, int y, bool locked)
    {
        grid.GetGridElement(x, y)?.SetLocked(locked);
    }

    public GameGrid<GridVisualElement2D> GetGrid()
    {
        return grid;
    }

    public void SetGrid(GameGrid<GridVisualElement2D> grid)
    {
        this.grid = grid;
    }

    #endregion

    public void DestroyGrid()
    {
        grid = null;

        int numberOfChildren = transform.childCount;
        int cnt = 0;
        while (transform.childCount > 0 && cnt < numberOfChildren)
        {
            DestroyImmediate(transform.GetChild(0).gameObject);
            cnt++;
        }

    }

    [ContextMenu("ShowDebugText")]
    public void ShowDebugText()
    {
        grid.ShowDebugText(true);
    }

    [ContextMenu("HideDebugText")]
    public void HideDebugText()
    {
        grid.ShowDebugText(false);
    }

 #region IGridVisual
    public void MarkAsReachableOneMove(int x, int y)
    {
        Highlight(x, y, Color.green);
    }

    public void MarkAsReachableTwoMove(int x, int y)
    {
        Highlight(x, y, Color.yellow);
    }

    public void MarkAsEnemyInMeeleAttackRange(int x, int y)
    {
        Highlight(x, y, Color.red);
    }

    public void MarkAsEnemyInRangeAttackRange(int x, int y)
    {
        Highlight(x, y, Color.red);
    }

    public void MarkAsPossibleTargetSpecialAbility(int x, int y)
    {
        Highlight(x, y, Color.cyan);
    }
    #endregion

    #region 2D
    [Serializable]
    public class GridVisualElement2D
    {
        public GameGrid<GridVisualElement2D> grid; //Reference to parent grid
        public int x, y;
        public bool visible;
        public bool locked; //If set, element will not be cleared
        public Color color;
        public GameObject gameObject;
        public SpriteRenderer spriteRenderer;

        public GridVisualElement2D(GameGrid<GridVisualElement2D> grid, int x, int y)
        {
            this.grid = grid;
            this.x = x;
            this.y = y;
            this.visible = false;
            this.locked = false;
            this.color = Color.white;
            this.spriteRenderer = null;
        }

        public override string ToString()
        {
            return locked.ToString();
        }
       
        public void SetSpriteRenderer(GameObject go = null)
        {
            if(go.TryGetComponent<SpriteRenderer>(out spriteRenderer))
            {
                //New game object reference valid, update it
                gameObject = go;
            }
            else
            {
                //game object reference invalid -> update spriteRenderer with gameObject
                gameObject.TryGetComponent<SpriteRenderer>(out spriteRenderer);
            }
        }

        public void InstantiateVisualObject(GameObject prefab, Transform parent)
        {
            gameObject = Instantiate(prefab, grid.GetWorldCenterPosition(x,y), Quaternion.identity, parent);
            gameObject.name = $"{prefab.name}_{x}_{y}"; //Rename using string interpolation  
            SetSpriteRenderer(gameObject);
            SetVisible(this.visible);
        }

        public void SetLocked(bool locked)
        {
            this.locked = locked;
            grid.TriggerGridChangedEvent(x, y);
        }

        public void SetVisible(bool visible)
        {
            this.visible = visible;
            //gameObject?.SetActive(visible);
            if(spriteRenderer!=null)
            {
                if (visible)
                {
                    spriteRenderer.color = new Color(spriteRenderer.color.r, spriteRenderer.color.g, spriteRenderer.color.b, 1.0f);
                }
                else
                {
                    //make transparent (set alpha to 0)
                    spriteRenderer.color = new Color(spriteRenderer.color.r, spriteRenderer.color.g, spriteRenderer.color.b, 0f);
                }
                grid.TriggerGridChangedEvent(x, y);
            }

        }
        public void SetVisible(bool visible, Color color)
        {  
            this.color = color;
            if(spriteRenderer!=null)
            {
                spriteRenderer.color = color;
            }
            this.SetVisible(visible);
        }
    }
#endregion
}
