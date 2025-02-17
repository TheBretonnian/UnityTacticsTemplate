public interface IAbilityEvents
{
    void Selected();
    void CleanUp();
    void OnTargetHoverEnter(ISelectableTarget target);
    void OnTargetHoverExit(ISelectableTarget target);
}