using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class PlayerController : MonoBehaviour
{
    private IUnit _selectedUnit;
    private IUnit _activeUnit;
    private IAbility _activeAbility;
    private ITarget _selectedTarget;
    private List<IUnit> _eligibleUnits;

    private bool _isPlayerInputActive = true; 

    private Selector _selector;
    private TargetAdquisition _targeter;
    private AbilityCommander _abilityCommander;

    public event Action<IUnit> UnitSelected;
    public event Action<IUnit, IAbility> AbilityActivated;

    public void OnlyUnitSelectionClickHandler(Vector3 cursorPosition)
    {
        if (!isActive) return; //Event handler will be trigger even if MonoBehaviour is disabled

        //Selection Logic
        ISelectable selectedElement = _selector.GetSelectedElement(cursorPosition);
        if (selectedElement != null)
        {
            SelectionLogic(selectedElement);
        }
    }

    public void OnlyTargetSelectionClickHandler(Vector3 cursorPosition)
    {
        if (!isActive) return; //Event handler will be trigger even if MonoBehaviour is disabled

        //TargetValidation + Command Ability
        ISelectable selectedElement = _selector.GetSelectedElement(cursorPosition);
        if (selectedElement != null)
        {
            ITarget target = selectedElement as ITarget;
            if (IsAbilityActivable() && _targeter.IsValidTarget(_activeUnit, target) /* it shall check for null */)
            {
                SetTarget();
                _abilityCommander.Command(_activeUnit, _activeAbility, target);
            }
        }
    }

    //Handles both target and unit selection
    public void OneClickHandler(Vector3 cursorPosition)
    {
        if (!isActive) return; //Event handler will be trigger even if MonoBehaviour is disabled

        //On cliked on valid target, commands ability
        //Otherwise, selection logic

        //TargetValidation + Command Ability
        ISelectable selectedElement = _selector.GetSelectedElement(cursorPosition);
        if (selectedElement != null)
        {
            ITarget target = selectedElement as ITarget;
            if (IsAbilityActivable() && _targeter.IsValidTarget(_activeUnit, target) /* it shall check for null */)
            {
                SetTarget(target);
                _abilityCommander.Command(_activeUnit, _activeAbility, target);
            }
            else //Not a valid target or no ability selected
            {
                ClearTarget();
                //Fallback to Selection Logic
                SelectionLogic(selectedElement);
            }
        }
    }

    //Handles both target and unit selection when target needs to be confirmed with second click
    public void DoubleClickHandler(Vector3 cursorPosition)
    {
        if (!isActive) return; //Event handler will be trigger even if MonoBehaviour is disabled

        //Target needs to be selected with first click
        //Second click on target confirms the action
    }

    //Handles secondary click when OneClick and DoubleClick are used
    public void SecondaryClickHandler(Vector3 cursorPosition)
    {
        if (!isActive) return; //Event handler will be trigger even if MonoBehaviour is disabled

        //For example cancel active ability
    }

    public void ActivateAbility(IAbility ability, IUnit unit)
    {
        if (unit == _activeUnit)
        {
            _activeAbility = ability;
            //If list, add to current list
        }

        if (ability.TargetType == auto && unit == _activeUnit)
        {
            _abilityCommander.Command(_activeUnit, _activeAbility, null);
        }
        else
        {
            AbilityActivated?.Invoke(_activeUnit, _activeAbility);
        }
    }

    public void TurnController_NewTurn(int currentTurn, Player currentPlayer, List<IUnit> eligibleUnits)
    {
        if(currentPlayer.PlayerType == PlayerType.Human)
        {
            ActivePlayerInput();

            _eligibleUnits = eligibleUnits;
            SelectFirstEligibleUnit();
        }
        else //PlayerType.IA
        {
            DeactivatePlayerInput();
        }

    }

    public void UnitManager_UnitExhausted(IUnit unit)
    {
        if(_eligibleUnits.Contains(unit))
        {
            _eligibleUnits.Remove(unit);
            SelectFirstEligibleUnit();
        }
    }

    private void SelectionLogic(ISelectable selectedElement)
    {
        IUnit unit = selectedElement as IUnit;
        if (unit != null)
        {
            if (_eligibleUnits.contains(unit))
            {
                SelectActiveUnit(unit);
            }
            else
            {
                SelectNoActiveUnit(unit);
            }            
        }
    }

    private void SelectActiveUnit(IUnit unit)
    {
        _selectedUnit = unit;
        _activeUnit = unit;
        OnUnitSelected(unit);
    }

    private void SelectNoActiveUnit(IUnit unit)
    {
        _selectedUnit = unit;
        _activeUnit = null;
        OnUnitSelected(unit);
    }

    private void SelectFirstEligibleUnit()
    {
        if (_eligibleUnits.Count >= 1)
        {
            //Set first unit from list as the active unit
            SelectActiveUnit(_eligibleUnits[0]);
        }
    }

    private void OnUnitSelected(IUnit selectedUnit)
    {

        IAbility defaultUnitAbility = selectedUnit.GetDefaultAbility()
        ActivateAbility(defaultUnitAbility, selectedUnit);

        //Invoke Event to inform other components such as: 
        // HUDController -> Update HUD with panel of selected unit
        // VFXController -> Display/Play visuals and sounds
        UnitSelected?.Invoke(selectedUnit);

    }

    //Check condition for commanding an ability
    private bool IsAbilityActivable()
    {
        return _activeAbility != null && _activeUnit != null;
    }

    private void SetTarget(ITarget target)
    {
        _selectedTarget = target;
        //OnTargetSelected(target); //Fire events for VFX
    }

    private void ClearTarget()
    {
        //OnTargetClearing(_selectedTarget); //Fire event for VFX
        _selectedTarget = null;
    }

    private void DeactivatePlayerInput()
    {
        _isPlayerInputActive = false;
        //Optional: Unsubscribe to InputInterface events here
    }

    private void ActivePlayerInput()
    {
        _isPlayerInputActive = true;
        //Optional: Subscribe to InputInterface events here
    }
}

