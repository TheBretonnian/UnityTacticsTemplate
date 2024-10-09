using System;
using UnityEngine;


[Serializable]
public class Player
{

    [SerializeField] private int number;
    [SerializeField] private Color color;
    [SerializeField] private bool isHuman;

    public int Number { get => number;}
    public Color GetColor { get => color;}
    public bool IsHuman { get => isHuman;}
     
    public Player(int number, Color color, isHuman = false)
    {
        this.number = number;
        this.color = color;
        this.isHuman = isHuman;
    }

}
