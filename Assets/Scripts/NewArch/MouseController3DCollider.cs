using System;
using System.Collections.Generic;
using UnityEngine;

//The purpose of this class is to abstract the used Input Hardware from the game logic: either Mouse, Touch or or Controller. 
//This is abstracted in the interface implemented by this class to reduce the dependency with the client.
//This class depends on ISelectable interface
public class MouseController3DCollider : MonoBehaviour, IInputController
{

    //Events
    public event Action<ISelectable> MainCursorButtonClicked;
    public event Action<ISelectable> SecondaryCursorButtonClicked;

    public event Action<ISelectable> SelectableHoverEntered;
    public event Action<ISelectable> SelectableHoverExit;
    

    //Private members
    private ISelectable _currentHoverObject;
    //Can be set to serializable to allow customization on Editor
    private Camera _mainCamera;
    
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
        //Detect mouse clicks and fire events -> Abstract Input method and environment/2D/3D) from other scripts
        if (Input.GetMouseButtonDown(0))
        {
            ISelectable selectable = GetSelectableUnderCursor(Input.mousePosition);
            if(selectable != null)
            {
                MainCursorButtonClicked?.Invoke(selectable)
            }
        }
        else if(Input.GetMouseButtonDown(1)) //Both left and right not allowed
        {
            ISelectable selectable = GetSelectableUnderCursor(Input.mousePosition);
            //Allow invoking with null to implement logic with secondary cursor button click such as Cancel Ability
            SecondaryCursorButtonClicked?.Invoke(selectable)
        }

    }

    private void DetectHover()
    {
        ISelectable selectable = GetSelectableUnderCursor(Input.mousePosition);

        if (selectable != null)
        {
            if (_currentHoverObject != selectable)
            {
                if (_currentHoverObject != null)
                {
                    _currentHoverObject.OnMouseExit();
                }

                _currentHoverObject = selectable;
                _currentHoverObject.OnMouseEnter();
                SelectableHoverEntered?.Invoke(_currentHoverObject);
            }
        }
        else
        {
            if (_currentHoverObject != null)
            {
                _currentHoverObject.OnMouseExit();
                SelectableHoverExit?.Invoke(_currentHoverObject);
                _currentHoverObject = null;
                
            }
        }
    }

    private ISelectable GetSelectableUnderCursor(Vector3 cursorPosition)
    {
        ISelectable selectable = null; 
        Ray ray = _mainCamera.ScreenPointToRay(cursorPosition);
        RaycastHit hit;
        
        if (Physics.Raycast(ray, out hit))
        {
            ISelectable selectable = hit.collider.GetComponent<ISelectable>();
            //TO DO: Try parents of Game Object with collider too
        }

        return selectable;
    }

    /* Reference for 2d class without colliders */
    private Vector3 GetMouseWorldPosition()
    {
        Vector3 vec = GetMouseWorldPositionWithZ(Input.mousePosition, _mainCamera);
        vec.z = 0f;
        return vec;
    }
    private Vector3 GetMouseWorldPositionWithZ()
    {
        return GetMouseWorldPositionWithZ(Input.mousePosition, _mainCamera);
    }
    private Vector3 GetMouseWorldPositionWithZ(Camera worldCamera)
    {
        return GetMouseWorldPositionWithZ(Input.mousePosition, worldCamera);
    }
    private Vector3 GetMouseWorldPositionWithZ(Vector3 screenPosition, Camera worldCamera)
    {
        Vector3 worldPosition = worldCamera.ScreenToWorldPoint(screenPosition);
        return worldPosition;
    }

}