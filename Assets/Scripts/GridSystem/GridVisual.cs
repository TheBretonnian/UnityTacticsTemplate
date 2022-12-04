using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridVisual : MonoBehaviour
{
    public bool visible = false;
    public bool locked = false;
    public Color color;
    public SpriteRenderer spriteRenderer;

    [SerializeField] private GridVisualConfiguration config; 

    public void Initialize()
    {
        GetSpriteRenderer();
        this.color = spriteRenderer.color; //Redundant??
    }

    public void SetupConfig(GridVisualConfiguration config)
    {
        this.config = config;
    }

    private bool GetSpriteRenderer()
    {
        return gameObject.TryGetComponent<SpriteRenderer>(out spriteRenderer);
    }

    public void SetLocked(bool locked)
    {
        this.locked = locked;
    }

    public void SetVisible(bool visible)
    {
        this.visible = visible;
        //gameObject?.SetActive(visible);
        if (spriteRenderer != null)
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
        }

    }
    public void SetVisible(bool visible, Color color)
    {
        this.color = color;
        if (spriteRenderer != null)
        {
            spriteRenderer.color = color;
        }
        this.SetVisible(visible);
    }

    public void Highlight(Color? color = null)
    {
        if (color == null)
        {
            color = Color.white;
        }
        this.SetVisible(true, (Color)color);
    }

    #region IGridVisual
    public void MarkAsReachableOneMove()
    {
        Highlight(config.ReachableOneMoveColor);
    }

    public void MarkAsReachableTwoMove()
    {
        Highlight(config.ReachableTwoMoveColor);
    }

    public void MarkAsEnemyInMeleeAttackRange()
    {
        Highlight(config.EnemyInMeleeAttackRangeColor);
    }

    public void MarkAsEnemyInRangeAttackRange()
    {
        Highlight(config.EnemyInRangeAttackRangeColor);
    }

    public void MarkAsPossibleTargetSpecialAbility()
    {
        Highlight(config.SpecialAbilityColor);
    }

    public void SetTransparency(float transparency)
    {
        transparency = Mathf.Clamp01(transparency);
        if(spriteRenderer!= null) 
        {
            spriteRenderer.color = new Color(spriteRenderer.color.r, spriteRenderer.color.g, spriteRenderer.color.b, transparency);
        }        
    }
    #endregion
}