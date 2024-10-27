using System;

public interface IAbility : IAbilityEvents
{
    public bool IsAvailable();
    
    //The IUnit argument can be removed
    public bool IsValidTarget(ITarget target);
    
    public void Command(ITarget target, Action onAbilityExecuted);

    public bool IsAutoTarget(); //To do: As a property

    //Indicates whether this ability is active at the same time of default movement
    public bool AllowsMovement();
    
}

