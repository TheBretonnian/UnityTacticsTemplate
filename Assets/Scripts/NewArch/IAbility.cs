using System;

public interface IAbility
{
    public bool IsAvailable();
    
    //The IUnit argument can be removed
    public bool IsValidTarget(IUnit unit, ITarget target);
    
    public void Command(IUnit unit, ITarget target, Action onAbilityExecuted);

    //Player Events
    public void OnTargetHoverEnter(ITarget target);
    public void OnTargetHoverExit(ITarget target);

    public void Selected();
    public void Deselected();



    public bool IsAutoTarget(); //To do: As a property

    //Indicates whether this ability is active at the same time of default movement
    public bool AllowsMovement();
    
}

