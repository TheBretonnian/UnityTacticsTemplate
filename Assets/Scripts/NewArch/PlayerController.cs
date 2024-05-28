using System;
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
    private TargetAcquisition _targeter;
    private AbilityCommander _abilityCommander;

    public event Action<IUnit> UnitSelected;
    public event Action<IUnit, IAbility> AbilityActivated;

    public void OnlyUnitSelectionClickHandler(Vector3 cursorPosition)
    {
        if (!_isPlayerInputActive) return;

        ISelectable selectedElement = _selector.GetSelectedElement(cursorPosition);
        if (selectedElement != null)
        {
            HandleSelection(selectedElement);
        }
    }

    public void OnlyTargetSelectionClickHandler(Vector3 cursorPosition)
    {
        if (!_isPlayerInputActive) return;

        ISelectable selectedElement = _selector.GetSelectedElement(cursorPosition);
        if (selectedElement != null)
        {
            ITarget target = selectedElement as ITarget;
            if (IsAbilityActivatable() && _targeter.IsValidTarget(_activeUnit, target))
            {
                SetTarget(target);
                _abilityCommander.Command(_activeUnit, _activeAbility, target);
            }
        }
    }

    //Handles both target and unit selection
    public void OneClickHandler(Vector3 cursorPosition)
    {
        if (!_isPlayerInputActive) return;

        //TargetValidation + Command Ability
        ISelectable selectedElement = _selector.GetSelectedElement(cursorPosition);
        if (selectedElement != null)
        {
            ITarget target = selectedElement as ITarget;
            if (IsAbilityActivatable() && _targeter.IsValidTarget(_activeUnit, target))
            {
                SetTarget(target);
                _abilityCommander.Command(_activeUnit, _activeAbility, target);
            }
            else
            {
                ClearTarget();
                HandleSelection(selectedElement);
            }
        }
    }

    //Handles both target and unit selection when target needs to be confirmed with second click
    public void DoubleClickHandler(Vector3 cursorPosition)
    {
        if (!_isPlayerInputActive) return;

        // Implement double-click logic if needed
    }

    public void SecondaryClickHandler(Vector3 cursorPosition)
    {
        if (!_isPlayerInputActive) return;

        // Implement secondary click logic if needed (for example ability cancellation)
    }

    public void ActivateAbility(IUnit unit, IAbility ability)
    {
        if (unit == _activeUnit)
        {
            _activeAbility = ability;
        }

        if (ability.TargetType == TargetType.Auto && unit == _activeUnit)
        {
            _abilityCommander.Command(_activeUnit, _activeAbility, null);
        }
        else
        {
            AbilityActivated?.Invoke(unit, ability);
        }
    }

    public void TurnController_NewTurn(int currentTurn, Player currentPlayer, List<IUnit> eligibleUnits)
    {
        if (currentPlayer.PlayerType == PlayerType.Human)
        {
            ActivatePlayerInput();
            _eligibleUnits = eligibleUnits;
            SelectFirstEligibleUnit();
        }
        else
        {
            DeactivatePlayerInput();
        }
    }

    public void UnitManager_UnitExhausted(IUnit unit)
    {
        if (_eligibleUnits.Contains(unit))
        {
            _eligibleUnits.Remove(unit);
            SelectFirstEligibleUnit();
        }
    }

    private void HandleSelection(ISelectable selectedElement)
    {
        IUnit unit = selectedElement as IUnit;
        if (unit != null)
        {
            if (_eligibleUnits.Contains(unit))
            {
                SelectActiveUnit(unit);
            }
            else
            {
                SelectInactiveUnit(unit);
            }            
        }
    }

    private void SelectActiveUnit(IUnit unit)
    {
        _selectedUnit = unit;
        _activeUnit = unit;
        OnUnitSelected(unit);
    }

    private void SelectInactiveUnit(IUnit unit)
    {
        _selectedUnit = unit;
        _activeUnit = null;
        OnUnitSelected(unit);
    }

    private void SelectFirstEligibleUnit()
    {
        if (_eligibleUnits.Count > 0)
        {
            SelectActiveUnit(_eligibleUnits[0]);
        }
    }

    private void OnUnitSelected(IUnit selectedUnit)
    {
        IAbility defaultAbility = selectedUnit.GetDefaultAbility(); 
        ActivateAbility(selectedUnit, defaultAbility);

        //Invoke Event to inform other components such as: 
        // HUDController -> Update HUD with panel of selected unit
        // VFXController -> Display/Play visuals and sounds
        UnitSelected?.Invoke(selectedUnit);
    }

    //Check condition for commanding an ability
    private bool IsAbilityActivatable()
    {
        return _activeAbility != null && _activeUnit != null;
    }

    private void SetTarget(ITarget target)
    {
        _selectedTarget = target;
        // OnTargetSelected(target); // Fire events for VFX
    }

    private void ClearTarget()
    {
        // OnTargetClearing(_selectedTarget); // Fire event for VFX
        _selectedTarget = null;
    }

    private void DeactivatePlayerInput()
    {
        _isPlayerInputActive = false;
        // Optional: Unsubscribe from input events here
    }

    private void ActivatePlayerInput()
    {
        _isPlayerInputActive = true;
        // Optional: Subscribe to input events here
    }
}
