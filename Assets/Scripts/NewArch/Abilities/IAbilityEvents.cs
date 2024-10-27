public interface IAbilityEvents
{
    public void Selected();
    public void CleanUp();
    public void OnTargetHoverEnter(ITarget target);
    public void OnTargetHoverExit(ITarget target);
}