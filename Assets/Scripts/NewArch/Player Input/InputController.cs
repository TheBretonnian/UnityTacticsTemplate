using System;
using System.Collections.Generic;
using UnityEngine;

//The purpose of this class is to abstract the used Input Hardware from the game logic: either Mouse, Touch or or Controller. 
//This is abstracted in the interface implemented by this class to reduce the dependency with the client.
//This class depends on ISelectable interface
public abstract class InputController : MonoBehaviour, IInputController
{

    //Events
    public event Action<ISelectable> MainCursorButtonClicked;
    public event Action<ISelectable> SecondaryCursorButtonClicked;

    public event Action<ISelectable> SelectableHoverEntered;
    public event Action<ISelectable> SelectableHoverExit;
    

    //Protected members to be used by derived
    protected ISelectable _currentSelectedObject;
    protected ISelectable _currentHoverObject;
    //Can be set to serializable to allow customization on Editor
    protected Camera _mainCamera;

    //Protected abstract methods to be override and used only in derived classes
    protected abstract Vector3 GetCursorPosition();
    protected abstract ISelectable GetSelectableUnderCursor(Vector3 cursorPosition);
    protected abstract bool IsMainButtonPressed();
    protected abstract bool IsSecundaryButtonPressed();
    
    //This two public methods allow abstraction from MonoBehaviour
    public void EnableInput()
    {
        this.enabled = true;
    }

    public void DisableInput()
    {
        this.enabled = false;
    }

    void Start()
    {
        _mainCamera = Camera.main;
    }

    void Update()
    {
        DetectClick();

        DetectHover();
    }

    private void DetectClick()
    {
        //Detect input events -> Abstract Input method and environment/2D/3D) from other scripts
        if (IsMainButtonPressed())
        {
            ISelectable selectable = GetSelectableUnderCursor(GetCursorPosition());
            //New object selected?
            if(_currentSelectedObject != selectable) 
            {
                _currentSelectedObject?.Deselected();
                _currentSelectedObject = selectable;
                _currentSelectedObject?.Selected();
            }
            if (selectable != null) //Is this really needed here? To do: Check use cases in future
            {
                MainCursorButtonClicked?.Invoke(selectable);
            }
        }
        else if(IsSecundaryButtonPressed()) //Both left and right not allowed
        {
            ISelectable selectable = GetSelectableUnderCursor(GetCursorPosition());
            //Allow invoking with null to implement logic with secondary cursor button click such as Cancel Ability
            SecondaryCursorButtonClicked?.Invoke(selectable);
        }

    }

    private void DetectHover()
    {
        ISelectable selectable = GetSelectableUnderCursor(GetCursorPosition());

        if(_currentHoverObject != selectable)
        {
            if(_currentHoverObject!=null)
            {
                _currentHoverObject.OnHoverExit();
                SelectableHoverExit?.Invoke(_currentHoverObject);
            }
            
            _currentHoverObject = selectable;

            if(_currentHoverObject !=null)
            {
                _currentHoverObject.OnHoverEnter();
                SelectableHoverEntered?.Invoke(_currentHoverObject);
            }
        }
    }   

}