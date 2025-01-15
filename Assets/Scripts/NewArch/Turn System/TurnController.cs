using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;

public class TurnController : MonoBehaviour
{
    [SerializeField] private List<IUnit> Units;
    [SerializeField] private GameEvent<IUnit> unitExhaustedGameEvent; //Replace GameEvent<IUnit> with UnitGameEvent??
    [SerializeField] private List<Player> Players;
    [SerializeField, Tooltip("Set to false if Player 1 starts, Player 2 continues and so on...")] 
    private bool PlayerRandomOrder = true;

    [SerializeField, Tooltip("Global ScriptableObject with Turn Data information. Also responsible for new Round and Turn Events")] 
    private TurnData turnData;

    public enum TurnSystemType
    {
        /* All units in one player turn -> Each Round has so many turns as players */
        TeamBased,
        /*Only one unit per player turn -> Each Round has so many turns as units */
        InitiativeOrder,
        PlayerPicks
    }
    [SerializeField] private TurnSystemType Type = TurnSystemType.TeamBased; //TO DO: Create proper Setter 

    [SerializeField, Tooltip("If true, turn ends automatically if all eligible units in this turn are exhaused. Set false for manual control.")] 
    private bool endsTurnAutomatically = true;

    //LOCAL DATA

    private Queue<IUnit> UnitsQueue = new Queue<IUnit>();
    private Queue<Player> PlayersQueue = new Queue<Player>();
    private IUnit activeUnit;

    private void Awake()
    {
        //Subscribe to GLOBAL unit event
        if(unitExhaustedGameEvent!=null)
        {
            unitExhaustedGameEvent.gameEvent+=OnUnitExhausted;
        } 
    }

    public void StartGame()
    {
        //Init TurnData
        turnData.ResetData();

        if(PlayerRandomOrder)
        {
            if(UnityEngine.Random.Range(0.0f,1.0f) < 0.5f)
            {
                Players.Reverse();
            }
        }

        NewRound();
    }

    private void NewRound()
    {
        switch(Type)
        {
            case TurnSystemType.TeamBased:
                QueuePlayers();
                break;
            case TurnSystemType.InitiativeOrder:
                QueueUnits();
                break;
            default:
                break;
        }

        turnData.IncreaseRound();

        //New Turn
        NewTurn();
    }

    private void NewTurn()
    {
        switch(Type)
        {
            case TurnSystemType.TeamBased:
                if (PlayersQueue.Count > 0)
                {
                    //Dequeue Player and Start New Turn
                    turnData.SetCurrentPlayer(PlayersQueue.Dequeue());
                    //Adapt TurnData
                        // - Get Eligible Units
                    List<IUnit> eligibleUnits = new List<IUnit>();
                    foreach(IUnit unit in Units)//Replace with IServiceUnitLocation GetAllUnits()
                    {
                        if(unit.TeamNumber == turnData.CurrentPlayer.Number)
                        {
                            eligibleUnits.Add(unit);
                        }
                    }
                    turnData.SetEligibleUnits(eligibleUnits);
                    //Increase turn
                    turnData.IncreaseTurn();
                }
                else
                {
                    //Players Queue is empty so Round is over
                    NewRound();
                }
                break;

            case TurnSystemType.InitiativeOrder:
                if(UnitsQueue.Count > 0)
                {
                    activeUnit = UnitsQueue.Dequeue();
                    turnData.SetEligibleUnits(new List<IUnit>{activeUnit});
                    turnData.SetCurrentPlayer(Players.Where(player => player.Number == activeUnit.TeamNumber).ToList()[0]);
                    //Increase turn
                    turnData.IncreaseTurn();
                }
                else
                {
                    //Units Queue is empty so Round is over
                    NewRound();
                }
                break;

            case TurnSystemType.PlayerPicks:
                // if (Units.Where(unit => unit.HasActions()).Count() == 0) //Replace with GetAllUnits
                // {
                //     //Round is over
                //     NewRound();
                // }
                // else
                // {
                //     CurrentPlayer = Players[nextPlayer];
                //     CurrentTurn++;
                //     nextPlayer++;
                //     if (nextPlayer >= Players.Count) nextPlayer = 0;
                //     NewTurnEvent?.Invoke(CurrentTurn, CurrentPlayer, null);
                // }
                break;
        }
    }

    public void ForceNewTurn()
    {
        NewTurn();
    }

    private void QueuePlayers()
    {
        PlayersQueue = new Queue<Player>(Players);
    }

    private void QueueUnits()
    {
        //TO DO
        //Sort Units descending (Higher Initiative first)
        //Units = Units.OrderByDescending(unit => unit.GetInitiative()).ToList();
        UnitsQueue = new Queue<IUnit>(Units); //Replace Units with GetAllUnits
    }

    private void OnUnitExhausted(IUnit senderUnit)
    {
        switch (Type)
        {
            case TurnSystemType.TeamBased:
                //Check if other units of same player (active player) have still actions, otherwise turn ends automatically
                if (turnData.EligibleUnits.Where(unit => unit.IsExhausted() == false).Count() == 0)
                {
                    //End Turn
                    if (endsTurnAutomatically)
                    {
                        NewTurn();
                    }
                }
                break;

            // For both InitiativeOrder and PlayerPicks
            default:
                if (senderUnit == activeUnit)
                {
                    //End Turn
                    if (endsTurnAutomatically)
                    {
                        NewTurn();
                    }
                }
                break;

        }
    }
}


