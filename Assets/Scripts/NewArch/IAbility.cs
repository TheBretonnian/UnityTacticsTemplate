public interface IAbility
{
    public bool IsValidTarget(IUnit unit, ITarget target);

    //TO DO: Segregate into another Interface, used by Unit.AbilityMgr client
    public void Command(IUnit unit, ITarget target, Action onAbilityExecuted);

    public bool IsAutoTarget(); //To do: As a property

    //Indicates whether this ability is active at the same time of default movement
    public bool AllowsMovement();
    
}

// Example implementation of IAbility from chatGPT
public class FireballAbility : IAbility
{
    public bool IsValidTarget(IUnit unit, ITarget target)
    {
        // Implement validation logic
        return target != null && target.IsEnemy;
    }

    public void Command(IUnit unit, ITarget target, Action onAbilityExecuted)
    {
        // Implement the command logic
        // e.g., reduce target health, play animation, etc.
        Debug.Log("Executing Fireball Ability");
        target.TakeDamage(50);

        // Call the callback action
        onAbilityExecuted?.Invoke();
    }

    public bool IsAutoTarget()
    {
        return false;
    }
}
