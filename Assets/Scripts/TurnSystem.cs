using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurnSystem : MonoBehaviour
{
    [SerializeField] private List<Unit> Units;
    [SerializeField] private List<TurnSystem.Player> Players;
    [SerializeField, Tooltip("Set to false if Player 1 starts, Player 2 continues and so on...")] 
    private bool RandomOrder = true;

    private Queue<Unit> UnitsQueue = new Queue<Unit>();
    private Queue<TurnSystem.Player> PlayersQueue = new Queue<Player>();

    public delegate void NotificationNewRound(int newRound);
    public delegate void NotificationNewTurn(int currentTurn, TurnSystem.Player currentPlayer, Unit activeUnit);

    public event NotificationNewRound NewRoundEvent;
    public event NotificationNewTurn NewTurnEvent;


    public enum TurnSystemType
    {
        /* All units in one player turn -> Each Round has so many turns as players */
        TeamBased,
        /*Only one unit per player turn -> Each Round has so many turns as units */
        IniciativeOrder,
        PlayerPicks
    }
    [SerializeField] public TurnSystemType Type = TurnSystemType.TeamBased; //TO DO: Create proper Setter

    public Unit ActiveUnit { get; private set; }
    public int CurrentTurn { get; private set; }
    public int CurrentRound { get; private set; }
    public TurnSystem.Player CurrentPlayer { get; private set; }
    private int nextPlayer;

    private void Awake()
    {
        
    }

    private void Start()
    {
        //Shuffle List of Player to get random order if activated
        if(RandomOrder)
        {
            Players.Shuffle();
        }
        CurrentRound = 0;
        CurrentTurn = 0;
        nextPlayer = 0;

        //Subscribe to unit event
        foreach (Unit u in Units)
        {
            u.onNoMoreActions += OnUnitWithoutActions;
        }

        NewRound();

    }

    private void NewRound()
    {
        //Reset actions
        foreach(Unit u in Units)
        {
            u.ResetActions();
        }

        switch(Type)
        {
            case TurnSystemType.TeamBased:
                QueuePlayers();
                break;
            case TurnSystemType.IniciativeOrder:
                QueueUnits();
                break;
            default:
                break;
        }

        CurrentRound++;
        NewRoundEvent?.Invoke(CurrentRound);

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
                    CurrentPlayer = PlayersQueue.Dequeue();
                    CurrentTurn++;
                    NewTurnEvent?.Invoke(CurrentTurn, CurrentPlayer, null);
                }
                else
                {
                    //Players Queue is empty so Round is over
                    NewRound();
                }
                break;

            case TurnSystemType.IniciativeOrder:
                if(UnitsQueue.Count > 0)
                {
                    ActiveUnit = UnitsQueue.Dequeue();
                    CurrentPlayer = Players.Where(player => player.Number == ActiveUnit.team).ToList()[0];
                    CurrentTurn++;
                    NewTurnEvent?.Invoke(CurrentTurn, CurrentPlayer, ActiveUnit);
                }
                else
                {
                    //Units Queue is over so Round is over
                    NewRound();
                    

                }
                break;

            case TurnSystemType.PlayerPicks:
                if (Units.Where(unit => unit.HasActions()).Count() == 0)
                {
                    //Round is over
                    NewRound();
                }
                else
                {
                    CurrentPlayer = Players[nextPlayer];
                    CurrentTurn++;
                    nextPlayer++;
                    if (nextPlayer >= Players.Count) nextPlayer = 0;
                    NewTurnEvent?.Invoke(CurrentTurn, CurrentPlayer, null);
                }
                break;
        }


    }

    public void ForceNewTurn()
    {
        NewTurn();
    }

    public void SetActiveUnit(Unit unit)
    {
        if(Type == TurnSystemType.PlayerPicks)
        {
            ActiveUnit = unit;
        }
    }

    private void QueuePlayers()
    {
        PlayersQueue = new Queue<Player>(Players);
    }

    private void QueueUnits()
    {
        Units.Shuffle();
        //Sort Units descending (Higher Iniciative first)
        Units = Units.OrderByDescending(unit => unit.GetIniciative()).ToList();
        UnitsQueue = new Queue<Unit>(Units);
        string debugQueue = "";
        foreach(Unit u in UnitsQueue)
        {
            debugQueue += u.gameObject.name + ", ";
        }
        Debug.Log(debugQueue);
    }

    private void OnUnitWithoutActions(Unit senderUnit)
    {
        switch (Type)
        {
            case TurnSystemType.TeamBased:
                //Check if other units of same player (active player) have still actions, otherwise turn ends automatically
                if (senderUnit.team == CurrentPlayer.Number)
                {
                    if (Units.Where(unit => unit.HasActions() && unit.team == CurrentPlayer.Number).Count() == 0)
                    {
                        //End Turn
                        NewTurn();
                    }
                }
                break;

            // For both IniciativeOrder and PlayerPicks
            default:
                if (senderUnit == ActiveUnit)
                {
                    //End Turn
                    NewTurn();
                }
                break;

        }
    }

    [Serializable]
    public class Player
    {
        public int Number;
        public Color Color;
        public PlayerType Type;
        public enum PlayerType
        {
            Human,
            IA
        }

    }



}


