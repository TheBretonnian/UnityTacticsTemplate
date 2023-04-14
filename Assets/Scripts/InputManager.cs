using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputManager : MonoBehaviour
{
    //Singleton
    public static InputManager Instance;

    //TO DO: Differentiate 2D / 3D (Make private Enum as config parameter)

    public delegate void NotifyClick(Vector3 cursorPosition);
    public event NotifyClick OnMainCursorButtonClick;
    public event NotifyClick OnSecondaryCursorButtonClick;

    private void Awake()
    {
        //Singleton
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        //Detect mouse clicks and fire events -> Abstract Input method and environment/2D/3D) from other scripts
        if (Input.GetMouseButtonDown(0))
        {
            //For 2D
            OnMainCursorButtonClick?.Invoke(GetMouseWorldPosition());
        }
        else if(Input.GetMouseButtonDown(1)) //Both left and right not allowed
        {
            //For 2D
            OnSecondaryCursorButtonClick?.Invoke(GetMouseWorldPosition());
        }
    }

    #region CodeMonkey Code
    // Get Mouse Position in World with Z = 0f
    public static Vector3 GetMouseWorldPosition()
    {
        Vector3 vec = GetMouseWorldPositionWithZ(Input.mousePosition, Camera.main);
        vec.z = 0f;
        return vec;
    }
    public static Vector3 GetMouseWorldPositionWithZ()
    {
        return GetMouseWorldPositionWithZ(Input.mousePosition, Camera.main);
    }
    public static Vector3 GetMouseWorldPositionWithZ(Camera worldCamera)
    {
        return GetMouseWorldPositionWithZ(Input.mousePosition, worldCamera);
    }
    public static Vector3 GetMouseWorldPositionWithZ(Vector3 screenPosition, Camera worldCamera)
    {
        Vector3 worldPosition = worldCamera.ScreenToWorldPoint(screenPosition);
        return worldPosition;
    }
    #endregion
}
