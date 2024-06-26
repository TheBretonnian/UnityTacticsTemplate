public interface IInputController
{
    //Cursor press button events
    public event Action<Iselectable> MainCursorButtonClicked;
    public event Action<Iselectable> SecondaryCursorButtonClicked;
    //Cursor hover events
    public event Action<Iselectable> SelectableHoverEntered;
    public event Action<Iselectable> SelectableHoverExit;
    
    //Control methods
    public void EnableInput();
    public void DisableInput();
}