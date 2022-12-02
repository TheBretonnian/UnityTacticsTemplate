using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridVisualComponent : MonoBehaviour
{
    public bool visible = false;
    public bool locked = false;
    public Color color;
    public SpriteRenderer spriteRenderer;

    public void Initialize()
    {
        GetSpriteRenderer();
        this.color = spriteRenderer.color; //Redundant??
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

    #region IGridVisualComponent
    public void MarkAsReachableOneMove()
    {
        Highlight(Color.green);
    }

    public void MarkAsReachableTwoMove()
    {
        Highlight(Color.yellow);
    }

    public void MarkAsEnemyInMeeleAttackRange()
    {
        Highlight(Color.red);
    }

    public void MarkAsEnemyInRangeAttackRange()
    {
        Highlight(Color.red);
    }

    public void MarkAsPossibleTargetSpecialAbility()
    {
        Highlight(Color.cyan);
    }
    #endregion
}
