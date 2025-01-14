///////////////////////////////////////////////////////////
//  Unit.cs
//  Implementation of the Class Unit
//  Generated by Enterprise Architect
//  Created on:      09-Okt-2024 08:14:27
//  Original author: vistmm
///////////////////////////////////////////////////////////

using System;
using UnityEngine;



public class Unit : MonoBehaviour, IUnit, ISelectableTarget{
    private int teamNumber;

    //Public Methods IUnit
    public int TeamNumber { get => teamNumber; set => teamNumber = value; }

    public IAbility GetDefaultAbility()
    {
        throw new NotImplementedException();
    }
    public IAbility GetMoveAbility()
    {
        throw new NotImplementedException();
    }
    

    //ISelectableTarget:
    //Public Properties ITarget
    public bool IsUnit{get => true;}
    public IUnit GetUnit{get => this;}


    //Public Methods ISelectable
    public void Selected()
    {
        throw new NotImplementedException();
    }
    public void Deselected()
    {
        throw new NotImplementedException();
    }
    public void OnHoverEnter()
    {
        throw new NotImplementedException();
    }
    public void OnHoverExit()
    {
        throw new NotImplementedException();
    }

    public void ExhaustUnit()
    {
        //Set available action points to zero
        //fires UnitExhausted Global Event
    }
}//end Unit
