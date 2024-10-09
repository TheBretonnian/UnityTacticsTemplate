using System;
using UnityEngine;

public class TurnData : ScriptableObject
{
    public int CurrentRound { get; private set; }
    public int CurrentTurn { get; private set; }
    public Player CurrentPlayer { get; private set; }
    public List<IUnit> EligibleUnits { get; private set; }
 
}