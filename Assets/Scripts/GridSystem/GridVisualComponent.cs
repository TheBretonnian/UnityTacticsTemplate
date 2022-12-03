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

    private string ColorReachableOneMove;
    private string ColorReachableTwoMove;
    private string ColorEnemyInRangeAttackRange;
    private string ColorEnemyInMeleeAttackRange;
    private string ColorDangerZone;
    private string ColorSpecialAbility;

    public void Initialize()
    {
        GetSpriteRenderer();
        this.color = spriteRenderer.color; //Redundant??

        //Set default colors
        if(ColorReachableOneMove == "") ColorReachableOneMove = HexColorUtils.GetStringFromColor(Color.magenta);
        if (ColorReachableTwoMove == "") ColorReachableTwoMove = HexColorUtils.GetStringFromColor(Color.yellow);
        if (ColorEnemyInMeleeAttackRange == "") ColorEnemyInMeleeAttackRange = HexColorUtils.GetStringFromColor(Color.red);
        if (ColorEnemyInRangeAttackRange == "") ColorEnemyInRangeAttackRange = HexColorUtils.GetStringFromColor(Color.red);
        if (ColorDangerZone == "") ColorDangerZone = HexColorUtils.GetStringFromColor(Color.red);
        if (ColorSpecialAbility == "") ColorSpecialAbility = HexColorUtils.GetStringFromColor(Color.cyan);
    }

    public void SetupConfig(GridVisualConfiguration colorConfig)
    {
        ColorReachableOneMove = HexColorUtils.GetStringFromColor(colorConfig.ReachableOneMoveColor);
        ColorReachableTwoMove = HexColorUtils.GetStringFromColor(colorConfig.ReachableTwoMoveColor);
        ColorEnemyInMeleeAttackRange = HexColorUtils.GetStringFromColor(colorConfig.EnemyInMeleeAttackRangeColor);
        ColorEnemyInRangeAttackRange = HexColorUtils.GetStringFromColor(colorConfig.EnemyInRangeAttackRangeColor);
        ColorDangerZone = HexColorUtils.GetStringFromColor(colorConfig.DangerZoneColor);
        ColorSpecialAbility = HexColorUtils.GetStringFromColor(colorConfig.SpecialAbilityColor);
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
        Highlight(HexColorUtils.GetColorFromString(ColorReachableOneMove));
    }

    public void MarkAsReachableTwoMove()
    {
        Highlight(HexColorUtils.GetColorFromString(ColorReachableTwoMove));
    }

    public void MarkAsEnemyInMeleeAttackRange()
    {
        Highlight(HexColorUtils.GetColorFromString(ColorEnemyInMeleeAttackRange));
    }

    public void MarkAsEnemyInRangeAttackRange()
    {
        Highlight(HexColorUtils.GetColorFromString(ColorEnemyInRangeAttackRange));
    }

    public void MarkAsPossibleTargetSpecialAbility()
    {
        Highlight(HexColorUtils.GetColorFromString(ColorSpecialAbility));
    }

    public void SetTransparency(float transparency)
    {
        transparency = Mathf.Clamp01(transparency);
        if(spriteRenderer!= null) {
            spriteRenderer.color = new Color(spriteRenderer.color.r, spriteRenderer.color.g, spriteRenderer.color.b, transparency);
        }

        
    }
    #endregion
}


/// <summary>
/// From CodeMonkey Utilities -> Methods for handling colors with string hexcodes
/// </summary>
public class HexColorUtils
{
    // Returns 00-FF, value 0->255
    public static string Dec_to_Hex(int value)
    {
        return value.ToString("X2");
    }

    // Returns 0-255
    public static int Hex_to_Dec(string hex)
    {
        return Convert.ToInt32(hex, 16);
    }

    // Returns a hex string based on a number between 0->1
    public static string Dec01_to_Hex(float value)
    {
        return Dec_to_Hex((int)Mathf.Round(value * 255f));
    }

    // Returns a float between 0->1
    public static float Hex_to_Dec01(string hex)
    {
        return Hex_to_Dec(hex) / 255f;
    }

    // Get Hex Color FF00FF
    public static string GetStringFromColor(Color color)
    {
        string red = Dec01_to_Hex(color.r);
        string green = Dec01_to_Hex(color.g);
        string blue = Dec01_to_Hex(color.b);
        return red + green + blue;
    }

    // Get Hex Color FF00FFAA
    public static string GetStringFromColorWithAlpha(Color color)
    {
        string alpha = Dec01_to_Hex(color.a);
        return GetStringFromColor(color) + alpha;
    }

    // Sets out values to Hex String 'FF'
    public static void GetStringFromColor(Color color, out string red, out string green, out string blue, out string alpha)
    {
        red = Dec01_to_Hex(color.r);
        green = Dec01_to_Hex(color.g);
        blue = Dec01_to_Hex(color.b);
        alpha = Dec01_to_Hex(color.a);
    }

    // Get Hex Color FF00FF
    public static string GetStringFromColor(float r, float g, float b)
    {
        string red = Dec01_to_Hex(r);
        string green = Dec01_to_Hex(g);
        string blue = Dec01_to_Hex(b);
        return red + green + blue;
    }

    // Get Hex Color FF00FFAA
    public static string GetStringFromColor(float r, float g, float b, float a)
    {
        string alpha = Dec01_to_Hex(a);
        return GetStringFromColor(r, g, b) + alpha;
    }

    // Get Color from Hex string FF00FFAA
    public static Color GetColorFromString(string color)
    {
        float red = Hex_to_Dec01(color.Substring(0, 2));
        float green = Hex_to_Dec01(color.Substring(2, 2));
        float blue = Hex_to_Dec01(color.Substring(4, 2));
        float alpha = 1f;
        if (color.Length >= 8)
        {
            // Color string contains alpha
            alpha = Hex_to_Dec01(color.Substring(6, 2));
        }
        return new Color(red, green, blue, alpha);
    }
}

