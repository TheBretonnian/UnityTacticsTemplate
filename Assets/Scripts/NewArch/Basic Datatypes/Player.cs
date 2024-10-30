using System;
using UnityEngine;


[Serializable]
public class Player
{

    [SerializeField] private readonly int teamNumber;
    [SerializeField] private readonly Color color;
    [SerializeField] private readonly bool isHuman;

    public int Number { get => teamNumber;}
    public Color GetColor { get => color;}
    public bool IsHuman { get => isHuman;}
     
    public Player(int teamNumber, Color color, bool isHuman = false)
    {
        this.teamNumber = teamNumber;
        this.color = color;
        this.isHuman = isHuman;
    }

}
