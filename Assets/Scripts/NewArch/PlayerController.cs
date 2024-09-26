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
    private IAbility _selectedAbility;
    private ITarget _selectedTarget;
    private List<IUnit> _eligibleUnits;
  
    public event Action<IUnit> UnitSelected;
    public event Action<IUnit, IAbility> AbilitySelected;
    //Events for cleaning up
    public event Action<IUnit> UnitDeselected;
    public event Action<IUnit, IAbility> AbilityDeselected;


    private void Awake()
    {
        //Implement here logic to adapt to desired style (parameter)
        //Subscribe to input controller events
        if(inputController!=null)
        {
            inputController.MainCursorButtonClicked+=OnlyUnitSelectionClickHandler;
            inputController.SecondaryCursorButtonClicked+=OnlyTargetSelectionClickHandler;
        }
        if(turnController!=null)
        {
            turnController.TurnStart+=TurnController_NewTurn; //Can be a global event: GameEvent (SO Event)
        }
    }

    private void OnlyUnitSelectionClickHandler(ISelectable selectedElement)
    {
        HandleSelection(selectedElement);
    }

    private void OnlyTargetSelectionClickHandler(ISelectable selectedElement)
    {
        if (selectedElement != null)
        {
            ITarget target = selectedElement as ITarget;
            if (IsAbilityActivatable() && _selectedAbility.IsValidTarget(_activeUnit, target))
            {
                SetTarget(target);
                OnAbilityCommanded();
                _activeAbility.Command(_activeUnit, target, OnAbilityExecuted);
            }
        }
    }

    //Handles both target and unit selection
    private void OneClickHandler(ISelectable selectedElement)
    {
        if (selectedElement != null)
        {
            ITarget target = selectedElement as ITarget;
            if (IsAbilityActivatable() && _selectedAbility.IsValidTarget(_activeUnit, target))
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
    private void DoubleClickHandler(ISelectable selectedElement)
    {
        // Implement double-click logic if needed
    }

    private void SecondaryClickHandler(ISelectable selectedElement)
    {
        // Implement secondary click logic if needed (for example ability cancellation)
    }

    //Shall be possibe to access it from outside (e.g. GUI Controller)
    public void SelectAbility(IUnit unit, IAbility ability)
    {
        if(_selectedAbility!= ability)
        {
           if(_selectedAbility != null)
           {
              AbilityDeselected?.Invoke(unit, _selectedAbility);
           }
           _selectedAbility = ability;
           if(_selectedAbility != null)
           {
               AbilitySelected?.Invoke(unit, _selectedAbility);
           }
        }
        //Autolaunch if selected from GUI, should not be the case from default ability
        if (_selectedAbility?.IsAutoTarget() && unit == _activeUnit)
        {
            _selectedAbility.Command(_activeUnit, null, OnAbilityExecuted);
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
            SelectUnit(unit);
            if (_eligibleUnits.Contains(unit))
            {
                ActivateUnit(unit);
            } 
            else 
            {
                ActivateUnit(null);
            }
        }
        //Optionally deselect unit if Player clicks on non unit (e.g. terrain)
        else
        {
            SelectUnit(null);
            ActivateUnit(null);
        }
    }

    private void SelectUnit(IUnit unit)
    {
        if(_selectedUnit!= unit)
        {
            if(_selectedUnit != null)
             {
                UnitDeselected?.Invoke(_selectedUnit);
             } 
            _selectedUnit = unit;
            //Invoke Event to inform other components such as: 
            // HUDController -> Update HUD with panel of selected unit
            // VFXController -> Display/Play visuals and sounds
            if (_selectedUnit != null)
            {
                UnitSelected?.Invoke(_selectedUnit);

                IAbility defaultAbility = _selectedUnit.GetDefaultAbility(); 
                SelectAbility(_selectedUnit, defaultAbility);
            }
            else
            {
                SelectAbility(null, null);
            }
        }
    }

    private void ActivateUnit(IUnit unit)
    {
        _activeUnit = unit;
    }

    private void SelectFirstEligibleUnit()
    {
        if (_eligibleUnits.Count > 0)
        {
            SelectActiveUnit(_eligibleUnits[0]);
        }
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
        return _selectedAbility != null && _activeUnit != null;
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

    //Consider these two for public? Maybe use-case in the future
    private void DeactivatePlayerInput()
    {
        inputController.DisableInput();
    }

    private void ActivatePlayerInput()
    {
        inputController.EnableInput();
    }
}
