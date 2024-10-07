using System;
using UnityEngine;

public class TurnData : ScriptableObject
{
    public int CurrentRound { get; private set; }
    public int CurrentTurn { get; private set: }
    public int CurrentPlayer { get; private set; }
    public IUnit ActiveUnit { get; private set; }
 
}