using System;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{

    //needs to be set on Editor
    [SerializeField] IInputController inputController;
    [SerializeField] ITurnController turnController;

    private IUnit _selectedUnit;
    private IUnit _activeUnit;
    private IAbility _activeAbility;
    private ITarget _selectedTarget;
    private List<IUnit> _eligibleUnits;
  
    public event Action<IUnit> UnitSelected;
    public event Action<IUnit, IAbility> AbilityActivated;
    //Events for cleaning up
    public event Action<IUnit> UnitDeselected;
    public event Action<IUnit, IAbility> AbilityDeactivated;


    private void Awake()
    {
        //Implement here logic to adapt to desired style
        //Subscribe to input controller events
        if(inputController!=null)
        {
            inputController.MainCursorButtonClicked+=OnlyUnitSelectionClickHandler;
            inputController.SecondaryCursorButtonClicked+=OnlyTargetSelectionClickHandler;
        }
        if(turnController!=null)
        {
            turnController.TurnStart+=TurnController_NewTurn;
        }
    }

    public void OnlyUnitSelectionClickHandler(ISelectable selectedElement)
    {
        HandleSelection(selectedElement);
    }

    public void OnlyTargetSelectionClickHandler(ISelectable selectedElement)
    {
        if (selectedElement != null)
        {
            ITarget target = selectedElement as ITarget;
            if (IsAbilityActivatable() && _activeAbility.IsValidTarget(_activeUnit, target))
            {
                SetTarget(target);
                OnAbilityCommanded();
                _activeAbility.Command(_activeUnit, target, OnAbilityExecuted);
            }
        }
    }

    //Handles both target and unit selection
    public void OneClickHandler(ISelectable selectedElement)
    {
        if (selectedElement != null)
        {
            ITarget target = selectedElement as ITarget;
            if (IsAbilityActivatable() && _activeAbility.IsValidTarget(_activeUnit, target))
            {
                SetTarget(target);
                OnAbilityCommanded();
                _activeAbility.Command(_activeUnit, target, OnAbilityExecuted);
            }
            else
            {
                ClearTarget();
                HandleSelection(selectedElement);
            }
        }
    }

    //Handles both target and unit selection when target needs to be confirmed with second click
    public void DoubleClickHandler(ISelectable selectedElement)
    {
        // Implement double-click logic if needed
    }

    public void SecondaryClickHandler(ISelectable selectedElement)
    {
        // Implement secondary click logic if needed (for example ability cancellation)
    }

    public void ActivateAbility(IUnit unit, IAbility ability)
    {
        if (unit == _activeUnit)
        {
            _activeAbility = ability;
        }

        if (ability.IsAutoTarget() && unit == _activeUnit)
        {
            _activeAbility.Command(_activeUnit, null, OnAbilityExecuted);
        }
        else
        {
            AbilityActivated?.Invoke(unit, ability);
        }
    }

    public void TurnController_NewTurn(int currentTurn, Player currentPlayer, List<IUnit> eligibleUnits)
    {
        if (currentPlayer.IsHuman())
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
        //If selectedElement = null, unit will be also null without throwing an exception
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
         //Optionally deselect unit if Player clicks on non unit (e.g. terrain)
        else
        {
            if(_selectedUnit!=null) UnitDeselected?.Invoke(_selectedUnit);
            _selectedUnit = null;
            _activeUnit = null;   
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

    private void OnAbilityCommanded()
    {
        DeactivatePlayerInput();
    }

    private void OnAbilityExecuted()
    {
        ActivatePlayerInput();
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
        inputController.DisableInput();
    }

    private void ActivatePlayerInput()
    {
        inputController.EnableInput();
    }
}
