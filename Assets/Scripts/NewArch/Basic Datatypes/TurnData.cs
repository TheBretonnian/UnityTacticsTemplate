using System;
using System.Collections.Generic;
using UnityEngine;

public class TurnData : ScriptableObject
{
    public int CurrentRound { get; private set; }
    public int CurrentTurn { get; private set; }
    public Player CurrentPlayer { get; private set; }
    public List<IUnit> EligibleUnits { get; private set; }

    public void ResetData()
    {
        CurrentRound = 0;
        CurrentTurn = 0;
        CurrentPlayer = null;
        EligibleUnits = new List<IUnit>();
    }

    public void IncreaseRound() => CurrentRound++;
    public void IncreaseTurn() => CurrentTurn++;

    public void SetCurrentPlayer(Player p) => CurrentPlayer = p;
    public void SetEligibleUnits(List<IUnit> eligibleUnits) => EligibleUnits = eligibleUnits;
    
 
}