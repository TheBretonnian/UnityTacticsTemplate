public interface IAbility
{
    public bool IsValidTarget(IUnit unit, ITarget target);
    public void Command(IUnit unit, ITarget target, Action onAbilityExecuted);

    public bool IsAutoTarget(); //To do: As a property
    
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
