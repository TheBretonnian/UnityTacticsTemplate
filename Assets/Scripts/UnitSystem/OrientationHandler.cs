using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OrientationHandler : MonoBehaviour
{
    [SerializeField, Tooltip("True if sprite looks to the right")] bool spriteIsLookingRight = false;
    private Vector3 lastPosition;

    public bool LookingRight = false;
    
    // Start is called before the first frame update
    void Start()
    {
        lastPosition = transform.position;
        LookingRight = spriteIsLookingRight;
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 moveDirection = transform.position - lastPosition;
        if (moveDirection.x > 0)
        {
            SetOrientationToRight();
        }
        else if(moveDirection.x < 0)
        {
            SetOrientationToLeft();
        }
        lastPosition = transform.position;
    }

    public void SetOrientationToRight()
    {
        LookingRight = true;

        if (spriteIsLookingRight)
        {
            transform.rotation = Quaternion.identity;
        }
        else
        {
            transform.rotation = Quaternion.Euler(0, 180, 0);
        }
    }

    public void SetOrientationToLeft()
    {
        LookingRight = false;

        if (spriteIsLookingRight)
        {
            transform.rotation = Quaternion.Euler(0, 180, 0);
        }
        else
        {
            transform.rotation = Quaternion.identity;
        }
    }
}
