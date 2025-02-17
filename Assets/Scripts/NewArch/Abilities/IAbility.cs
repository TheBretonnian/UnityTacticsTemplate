using System;

public interface IAbility : IAbilityEvents
{
    bool IsAvailable();
    
    //The IUnit argument can be removed
    bool IsValidTarget(ISelectableTarget target);
    
    void Command(ISelectableTarget target, Action onAbilityExecuted);

    bool IsAutoTarget(); //To do: As a property

    //Indicates whether this ability is active at the same time of default movement
    bool AllowsMovement();
    
}

