using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;



public class PathMoveController : MonoBehaviour
{
    [SerializeField] private State currentState = State.Idle;
    [SerializeField] private List<Vector3> PathPositions;
    [SerializeField] private float moveSpeed = 1.0f;
    [SerializeField] private bool Looping = false;
    [SerializeField] private bool Reverse = false;

    public class TargetReachedEventArgs : EventArgs
    {
        
        public Vector3 targetPosition;
        public GameObject gameObject;

        public TargetReachedEventArgs(Vector3 position, GameObject gameObject)
        {
            this.targetPosition = position;
            this.gameObject = gameObject;
        }
    }

    public event EventHandler<TargetReachedEventArgs> OnTargetReached;


    private int currentTarget = 0;

    private enum State
    {
        Idle,
        Moving
    };

    // Update is called once per frame
    void Update()
    {
        switch(currentState)
        {
            case State.Idle:
                break;

            case State.Moving:
                if(PathPositions.Count > 0 && currentTarget < PathPositions.Count)
                {
                    //Check if target reached
                    if (Vector3.Distance(PathPositions[currentTarget], transform.position) <= 0.01)
                    {
                        //Stop and pick next target
                        if (currentTarget < PathPositions.Count - 1)
                        {
                            currentTarget++;
                        }
                        else
                        {
                            //Reset path (case of looping)
                            currentTarget = 0;
                            if(Looping==false)
                            {
                                currentState = State.Idle;
                            }
                            if(Reverse)
                            {
                                PathPositions.Reverse();
                            }

                            OnTargetReached?.Invoke(this, new TargetReachedEventArgs(transform.position, this.gameObject));
                        }
                    }
                    else
                    {
                        //Target not reached, move to target
                        transform.position = Vector3.MoveTowards(transform.position, PathPositions[currentTarget], moveSpeed * Time.deltaTime);
                    }
                }

                break;
        }
    }

    public void SetPath(List<Vector3> listPathPositions)
    {
        PathPositions = listPathPositions;
        if(PathPositions?.Count > 0)
        {
            currentTarget = 0;
        }     
    }
    public void StartMoving()
    {
        if(PathPositions.Count > 0)
        {
            currentState = State.Moving;
        }
        
    }

    public void StoptMoving()
    {
        currentState = State.Idle;
    }
}
