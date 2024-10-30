public interface IAbilityEvents
{
    void Selected();
    void CleanUp();
    void OnTargetHoverEnter(ITarget target);
    void OnTargetHoverExit(ITarget target);
}