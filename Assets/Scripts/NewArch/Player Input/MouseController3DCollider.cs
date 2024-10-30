using System;
using System.Collections.Generic;
using UnityEngine;

//The purpose of this class is to abstract the used Input Hardware from the game logic: either Mouse, Touch or or Controller. 
//This is abstracted in the interface implemented by this class to reduce the dependency with the client.
//This class depends on ISelectable interface
public class MouseController3DCollider : InputController
{
    protected override Vector3 GetCursorPosition() => Input.mousePosition;
    protected override ISelectable GetSelectableUnderCursor(Vector3 cursorPosition)
    {
        ISelectable selectable = null; 
        Ray ray = _mainCamera.ScreenPointToRay(cursorPosition);

        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            selectable = hit.collider.GetComponent<ISelectable>();
            //TO DO: Try parents of Game Object with collider too
        }

        return selectable;
    }
	
	protected override bool IsMainButtonPressed() => Input.GetMouseButtonDown(0);
	protected override bool IsSecundaryButtonPressed() => Input.GetMouseButtonDown(1);
}