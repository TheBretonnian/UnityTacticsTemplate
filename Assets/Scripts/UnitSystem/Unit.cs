using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using System;

public class Unit : MonoBehaviour, IGetHealthSystem
{
    private PathMoveController pathMoveController;
    private AnimationHandler animationHandler;
    private OrientationHandler orientationHandler;
    private HealthSystem healthSystem;

    //Params: TO DO -> Scriptable Object
    [SerializeField] private int Iniciative = 2;
    [SerializeField] private int MoveDistance = 2;
    [SerializeField] private int AttackRange = 1;
    [SerializeField] private int ActionsPerTurn = 2;
    [SerializeField] private bool CanRepeatActions = true;

    private int availableActions;

    public bool selected;
    public bool canMove;
    public bool canAttack;
    public bool IsBusy;

    //Events
    public delegate void UnitNotification(Unit unit);
    public event UnitNotification onMoveCompleted;
    public event UnitNotification onAttackCompleted;
    public event UnitNotification onNoMoreActions;
    public event UnitNotification onSelected;
    public event UnitNotification onDeselected;

    public UnityEvent UnityOnSelected;
    public UnityEvent UnityOnDeselected;

    [SerializeField] public int team = 1;

    //Private
    private Action onMoveCompletedAction;

    #region Unity Functions
    private void Awake()
    {
        //Movement
        TryGetComponent<PathMoveController>(out pathMoveController);
        pathMoveController.OnTargetReached += OnPathMoveController_TargetReached;
        
        if(!TryGetComponent<OrientationHandler>(out orientationHandler))
        {
            orientationHandler = GetComponentInChildren<OrientationHandler>();
        }

        //Animation
        TryGetComponent<AnimationHandler>(out animationHandler);

        //Health
        healthSystem = new HealthSystem(100);
    }

    void Start()
    {
        availableActions = ActionsPerTurn;
        canMove = true;
        canAttack = true;
        selected = false;
        IsBusy = false;
    }

    #endregion

    #region Getters
    public Vector3 GetPosition()
    {
        return this.transform.position;
    }

    public int GetMoveDistance()
    {
        return MoveDistance;
    }

    public int GetAttackRange()
    {
        return AttackRange;
    }

    public HealthSystem GetHealthSystem()
    {
        return healthSystem;
    }

    public int GetIniciative()
    {
        return Iniciative;
    }

    #endregion

    #region Checkers
    public bool IsEnemy(int currentPlayer)
    {
        return team != currentPlayer;
    }

    public bool HasActions()
    {
        return availableActions > 0;
    }
    #endregion

    #region Selection
    public void Select()
    {
        this.selected = true;
        onSelected?.Invoke(this);
        UnityOnSelected?.Invoke();
    }

    public void Deselect()
    {
        this.selected = false;
        onDeselected?.Invoke(this);
        UnityOnDeselected?.Invoke();
    }
    #endregion 

    #region Movement
    public void MoveTo(List<Vector3> path, Action action)
    {
        IsBusy = true;

        //Change animation
        animationHandler?.PlayAnimation("move");

        pathMoveController.SetPath(path);
        pathMoveController.StartMoving();

        onMoveCompletedAction = action;

        canMove = ReduceAvailableActions();

    }

    //Simplified overload method
    public void MoveTo(Vector3 goal, Action action)
    {
        List<Vector3> simple_path = new List<Vector3>();
        simple_path.Add(goal);
        MoveTo(simple_path, action);
    }
    #endregion

    #region Combat
    public void Attack(Unit enemy, Action action)
    {
        //StartCoroutine(AttackSequence(enemy, action));
        AttackWithDelegate(enemy, action);
    }

    void AttackWithDelegate(Unit enemy, Action action)
    {
        IsBusy = true;

        //Dummy text
        Debug.Log($"{this.gameObject.name} attacks {enemy.gameObject.name}");

        //Face to enemy
        if (enemy.GetPosition().x - this.GetPosition().x > 0)
        {
            orientationHandler?.SetOrientationToRight();
        }
        else
        {
            orientationHandler?.SetOrientationToLeft();
        }

        //Play animation
        if(animationHandler!=null)
        {
            animationHandler.PlayAnimation("attack", () =>
            {
                OnAttack(enemy, action);
                //Change animation
                animationHandler.PlayAnimation("idle");
            });
        }
        else
        {
            OnAttack(enemy, action);
        }

        void OnAttack(Unit enemy, Action action)
        {
            //Update status
            IsBusy = false;
            canAttack = ReduceAvailableActions();
            //Damage
            enemy.GetHealthSystem().Damage(20f);
            
            //Call given delegate after attack is finished
            action?.Invoke();
            //Call public event after attack is finished
            onAttackCompleted?.Invoke(this);
            //Notify with an event that current unit has no more available actions always AFTER COMPLETING the last action 
            if (!HasActions())
            {
                onNoMoreActions?.Invoke(this);
            }
        }
    }

    IEnumerator AttackSequence(Unit enemy, Action action)
    {
        canAttack = ReduceAvailableActions();
        //Dummy
        Debug.Log($"{this.gameObject.name} attacks {enemy.gameObject.name}");

        //Face to enemy
        if (enemy.GetPosition().x - this.GetPosition().x > 0)
        {
            orientationHandler.SetOrientationToRight();
        }
        else
        {
            orientationHandler.SetOrientationToLeft();
        }

        //Play animation
        //animationHandler?.PlayAnimation("attack");
        yield return animationHandler.PlayAnimationAndWaitAnimationCompleted("attack");

        IsBusy = false;

        //Change animation
        animationHandler?.PlayAnimation("idle");

        enemy.GetHealthSystem().Damage(20f);
        //Call given delegate after attack is finished
        action?.Invoke();
        //Call public event after attack is finished
        onAttackCompleted?.Invoke(this);
    }

    #endregion

    #region Actions
    private bool ReduceAvailableActions()
    {
        if (availableActions > 0)
        {
            availableActions--;
        }
        if (availableActions == 0)
        {
            canAttack = false;
            canMove = false;
        }
        else
        {
            //We have still actions
            return CanRepeatActions;
        }
        return false;
    }

    public void ResetActions()
    {
        availableActions = ActionsPerTurn;
        canAttack = true;
        canMove = true;
    }
    #endregion

    #region private EventHandlers to encapsulate events from subcomponents  
    private void OnPathMoveController_TargetReached(object sender, PathMoveController.TargetReachedEventArgs args)
    {
        IsBusy = false;
        //Change animation
        animationHandler?.PlayAnimation("idle");

        //Invoke previous given delegate (only one time so set to null after Invokation) 
        onMoveCompletedAction?.Invoke();
        onMoveCompletedAction = null;

        //Call public event
        onMoveCompleted?.Invoke(this);

        //Notify with an event that current unit has no more available actions always AFTER COMPLETING the last action 
        if (!HasActions()) { onNoMoreActions?.Invoke(this); }
    }



    #endregion



}
