using System;

public interface IInputController
{
    //Cursor press button events
    public event Action<ISelectable> MainCursorButtonClicked;
    public event Action<ISelectable> SecondaryCursorButtonClicked;
    //Cursor hover events
    public event Action<ISelectable> SelectableHoverEntered;
    public event Action<ISelectable> SelectableHoverExit;
    
    //Control methods
    public void EnableInput();
    public void DisableInput();
}