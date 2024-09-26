using System;
using System.Collections.Generic;
using UnityEngine;

//The purpose of this class is to abstract the used Input Hardware from the game logic: either Mouse, Touch or or Controller. 
//This is abstracted in the interface implemented by this class to reduce the dependency with the client.
//This class depends on ISelectable interface
public class MouseController3DCollider : InputController
{
    private override ISelectable GetSelectableUnderCursor(Vector3 cursorPosition)
    {
        (void)cursorPosition; //Not used

        GridElement selected_gridElement = gridSystem.GetGridElement(GetMouseWorldPosition());
        return selected_gridElement as ISelectable;
    }
	
	private override IsMainButtonPressed() => Input.GetMouseButtonDown(0);
	private override IsSecundaryButtonPressed() => Input.GetMouseButtonDown(1);

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