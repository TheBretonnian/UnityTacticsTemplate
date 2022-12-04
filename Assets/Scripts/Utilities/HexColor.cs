using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// From CodeMonkey Utilities -> Methods for handling colors with string hexcodes
/// </summary>
public class HexColor
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


