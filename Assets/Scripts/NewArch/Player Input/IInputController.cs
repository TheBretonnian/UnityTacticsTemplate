using System;

public interface IInputController
{
    //Cursor press button events
    event Action<ISelectableTarget> MainCursorButtonClicked;
    event Action<ISelectableTarget> SecondaryCursorButtonClicked;
    //Cursor hover events
    event Action<ISelectableTarget> SelectableHoverEntered;
    event Action<ISelectableTarget> SelectableHoverExit;
    
    //Control methods
    void EnableInput();
    void DisableInput();
}