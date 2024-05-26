//New file including Player Interaction Logic

private IUnit _selectedUnit;
private IUnit _activeUnit; 
private IAbility _activeAbility;
private ITarget _selectedTarget;
private List<IUnit> _eligibleUnits;

private Selector _selector;
private TargetAdquisition _targeter;
private AbilityCommander _abilityCommander;

public event Action<Unit> UnitSelected;

public void OnlyUnitSelectionClickHandler(Vector3 cursorPosition)
{
  //Selection Logic
  ISelectable selectedElement = _selector.GetSelectedElement(cursorPosition);
  if(selectedElement != null)
  {
	  SelectionLogic(selectedElement);
  }
}

public void OnlyTargetSelectionClickHandler(Vector3 cursorPosition)
{
  //TargetValidation + Command Ability
  ISelectable selectedElement = _selector.GetSelectedElement(cursorPosition);
  if(selectedElement != null)
  {
    ITarget target = selectedElement as ITarget;
    if(IsAbilityActive() && _targeter.IsValidTarget(target) /* it shall check for null */)
    {
      SetTarget(target);
      _abilityCommander.Command(_activeUnit,_activeAbility, target);
    }
    else //Not a valid target or no ability selected
    {
      ClearTarget();
      //Fallback to Selection Logic
      SelectionLogic(selectedElement);
    }
  }
} 

//Handles both target and unit selection
public void OneClickHandler(Vector3 cursorPosition)
{
  //On cliked on valid target, commands ability
  //Otherwise, selection logic
}

//Handles both target and unit selection when target needs to be confirmed with second click
public void DoubleClickHandler(Vector3 cursorPosition)
{
  //Target needs to be selected with first click
  //Second click on target confirms the action
}

//Handles secondary click when OneClick and DoubleClick are used
public void SecondaryClickHandler(Vector3 cursorPosition)
{
  //For example cancel active ability
}

private void SelectionLogic(ISelectable selectedElement){
  IUnit unit = selectedElement as IUnit;
  if(unit != null)
  {
    if(_eligibleUnits.contains(unit))
    {
      SelectActiveUnit(unit);
    }
    else
    {
      SelectNoActiveUnit(unit);
    }
    //Inform other components about the event -> Decouple VFX Logic
    OnUnitSelected(unit);  
  }
}

private void OnUnitSelected(Unit selectedUnit)
{
  UnitSelected?.Invoke(selectedUnit);
}