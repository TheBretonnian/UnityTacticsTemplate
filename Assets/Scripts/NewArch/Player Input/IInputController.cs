using System;

public interface IInputController
{
    //Cursor press button events
    event Action<ISelectable> MainCursorButtonClicked;
    event Action<ISelectable> SecondaryCursorButtonClicked;
    //Cursor hover events
    event Action<ISelectable> SelectableHoverEntered;
    event Action<ISelectable> SelectableHoverExit;
    
    //Control methods
    void EnableInput();
    void DisableInput();
}