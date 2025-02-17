using System;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    /*[SerializeField]*/private IInputController inputController;

    private IUnit _selectedUnit;
    private IUnit _activeUnit;
    private IAbility _selectedAbility;
    //private IAbility _movementAbility;
    private ISelectableTarget _selectedTarget;
    private List<IUnit> _eligibleUnits;
  
    public event Action<IUnit> UnitSelected;
    public event Action<IUnit> UnitActivated;
    public event Action<IAbility> AbilitySelected;
    //Events for cleaning up
    public event Action<IUnit> UnitDeselected;
    public event Action<IUnit> UnitDeactivated;
    public event Action<IAbility> AbilityDeselected;


    private void OnEnable()
    {
        //Implement here logic to adapt to desired style (parameter)
        //Subscribe to input controller events
        if(inputController!=null)
        {
            inputController.MainCursorButtonClicked+=OnlyUnitSelectionClickHandler;
            inputController.SecondaryCursorButtonClicked+=OnlyTargetSelectionClickHandler;
            inputController.SelectableHoverEntered+=OnHoverEnter;
            inputController.SelectableHoverExit+=OnHoverExit;
        }
    }

    private void OnDisable()
    {
        //Unsubscribe to input controller events
        if(inputController!=null)
        {
            inputController.MainCursorButtonClicked-=OnlyUnitSelectionClickHandler;
            inputController.SecondaryCursorButtonClicked-=OnlyTargetSelectionClickHandler;
            inputController.SelectableHoverEntered-=OnHoverEnter;
            inputController.SelectableHoverExit-=OnHoverExit;
        }      
    }

    

    private void OnlyUnitSelectionClickHandler(ISelectableTarget selectedElement)
    {
        HandleSelection(selectedElement);
    }

    private void OnlyTargetSelectionClickHandler(ISelectableTarget selectedElement)
    {
        if (selectedElement != null) //To Do: Maybe this if is redundant if IsValidTarget already checks for null
        {
            if (IsAbilityActivatable() && _selectedAbility.IsValidTarget(selectedElement))
            {
                SetTarget(selectedElement);
                OnAbilityCommanded();
                _selectedAbility.Command(selectedElement, OnAbilityExecuted);
            }
        }
    }

    //Handles both target and unit selection
    private void OneClickHandler(ISelectableTarget selectedElement)
    {
        if (selectedElement != null)
        {
            if (IsAbilityActivatable() && _selectedAbility.IsValidTarget(selectedElement))
            {
                SetTarget(selectedElement);
                OnAbilityCommanded();
                _selectedAbility.Command(selectedElement, OnAbilityExecuted);
            }
            else
            {
                ClearTarget();
                HandleSelection(selectedElement);
            }
        }
    }

    //Handles both target and unit selection when target needs to be confirmed with second click
    private void DoubleClickHandler(ISelectableTarget selectedElement)
    {
        // Implement double-click logic if needed
    }

    private void SecondaryClickHandler(ISelectableTarget selectedElement)
    {
        // Implement secondary click logic if needed (for example ability cancellation)
    }

    private void OnHoverEnter(ISelectableTarget hoveredElement)
    {
        if (hoveredElement != null) //This checks is redundant since is already check in lower layer but add robustness
        {
            _selectedAbility?.OnTargetHoverEnter(hoveredElement);
            //_movementAbility?.OnTargetHoverEnter(target);
        }
    }

    private void OnHoverExit(ISelectableTarget hoveredElement)
    {
        if (hoveredElement != null) //This checks is redundant since is already check in lower layer but add robustness
        {
            _selectedAbility?.OnTargetHoverExit(hoveredElement);
            //_movementAbility?.OnTargetHoverExit(target);
        }
    }

    //Shall be possibe to access it from outside (e.g. GUI Controller)
    public void SelectAbility(IAbility ability)
    {
        if(_selectedAbility!= ability)
        {
            if(_selectedAbility != null)
            {
                AbilityDeselected?.Invoke(_selectedAbility);
                _selectedAbility.CleanUp(); //Notify ability directly -> no need of intermediate
            }

            _selectedAbility = ability;

            if(_selectedAbility != null)
            {
                AbilitySelected?.Invoke(_selectedAbility);
                _selectedAbility.Selected(); //Notify ability directly -> no need of intermediate
            }
            //Autolaunch if selected from GUI, should not be the case from default ability
            if ((_selectedAbility?.IsAutoTarget() == true) && _selectedUnit == _activeUnit)
            {
                _selectedAbility.Command(null, OnAbilityExecuted);
            }
        }

    }

    //Notifies the PlayerController about a new turn. 
    //It can be called directly by a class holding the reference
    //or subscribed to a public event as delegate
    public void Turn_NewTurn(TurnData turnData)
    {
        if (turnData.CurrentPlayer.IsHuman)
        {
            ActivatePlayerInput();
            _eligibleUnits = turnData.EligibleUnits;
            SelectFirstEligibleUnit();
        }
        else
        {
            DeactivatePlayerInput();
        }
    }

    //Notifies the PlayerController about a unit has exhausted all his action points. 
    //Typically subscribed to a public event as delegate
    public void UnitEvent_UnitExhausted(IUnit unit)
    {
        if (_eligibleUnits.Contains(unit))
        {
            _eligibleUnits.Remove(unit);
            SelectFirstEligibleUnit();
        }
    }

    private void HandleSelection(ISelectableTarget selectedElement)
    {
        if(selectedElement!=null)
        {
            //GetUnit will be null if a tile has no unit => Deselect unit
            SelectUnit(selectedElement.GetUnit); 
        }
        else
        {
            //The click can be ignore then and previously selected unit deselect
            SelectUnit(null); 
        }
    }

    public void SelectUnit(IUnit unit)
    {
        if(_selectedUnit!= unit)
        {
            if(_selectedUnit != null)
            {
                UnitDeselected?.Invoke(_selectedUnit);
            }

            _selectedUnit = unit;

            if (_selectedUnit != null)
            {
                UnitSelected?.Invoke(_selectedUnit);
                IAbility defaultAbility = _selectedUnit.GetDefaultAbility(); 
                SelectAbility(defaultAbility);
            }
            else
            {
                SelectAbility(null);
            }

            if (_eligibleUnits.Contains(unit))
            {
                ActivateUnit(unit);
            } 
            else 
            {
                ActivateUnit(null);
            }
        }
    }

    private void ActivateUnit(IUnit unit)
    {
        if(_activeUnit != unit)
        {
            if(_activeUnit != null)
            {
                UnitDeactivated?.Invoke(_activeUnit);
            }

            _activeUnit = unit;

            if(_activeUnit != null)
            {
                UnitActivated?.Invoke(_activeUnit);
            }           
        }
        
    }

    private void SelectFirstEligibleUnit()
    {
        if (_eligibleUnits.Count > 0)
        {
            SelectUnit(_eligibleUnits[0]);
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

    private void SetTarget(ISelectableTarget target)
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
        //enabled = false;
    }

    private void ActivatePlayerInput()
    {
        inputController.EnableInput();
        //enabled = true;
    }
}
