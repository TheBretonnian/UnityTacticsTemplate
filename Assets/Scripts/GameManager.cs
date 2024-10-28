using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class GameManager : MonoBehaviour
{
    //Singleton property
    public static GameManager Instance;

    [Header("Units")]
    [SerializeField] private Unit[] units;

    [Header("Game Systems")]
    [SerializeField] private TurnSystem turnSystem;
    [SerializeField] private GridSystem gridSystem;

    private Unit selected_unit;
    private Controlled_Unit controlled_unit = new Controlled_Unit();
    private bool OneUnitPerTurn = false;

    [System.Serializable]
    private class Controlled_Unit
    {
        public Unit Unit;
        public bool Fixed;
    }


    private void Awake()
    {
        //Singleton
        if (Instance != null && Instance!=this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;

        //Setup
        //Turn System
        OneUnitPerTurn = !(turnSystem.Type == TurnSystem.TurnSystemType.TeamBased);
        if (turnSystem.Type == TurnSystem.TurnSystemType.IniciativeOrder)
        {
            turnSystem.NewTurnEvent += OnNewTurn;
        }


    }

    // Start is called before the first frame update
    void Start()
    {
        //Grid System
        gridSystem.CreateGrid();
        gridSystem.onCursorHoverGrid += OnCursorHoverGrid;
        gridSystem.ClearGrid();

        //Input events
        InputManager.Instance.OnMainCursorButtonClick += OnMainCursorButtonClick;
        InputManager.Instance.OnSecondaryCursorButtonClick += OnSecondaryCursorButtonClick;

        //Center Camera
        Camera.main.transform.position = new Vector3(gridSystem.Width / 2, gridSystem.Height / 2, Camera.main.transform.position.z);
        Camera.main.orthographicSize = ((float)gridSystem.Height + 3.0f) / 2.0f;

        //Set Units position
        foreach (Unit unit in units)
        {
            gridSystem.GetGridElement(unit.transform.position).SetUnit(unit);
            unit.onMoveCompleted += UpdateGridAfterAction;
            unit.onAttackCompleted += UpdateGridAfterAction;
        }

        //Testing
        if (units.Length > 0)
        {
            selected_unit = units[0];
            controlled_unit.Unit = units[0];
        }
    }

    private void Update()
    {
        //Handle by Turn System
        if (Input.GetKeyDown(KeyCode.Space))
        {
            //Clean actions before forcing new turn -> deselect unit and clear grid
            selected_unit?.Deselect();
            gridSystem.ClearAllVisible();
            controlled_unit.Fixed = false;
            //Force new turn, reset actions
            Debug.Log("Skip turned. (Force new turned)");
            turnSystem.ForceNewTurn();
        }
    }
    private void OnMainCursorButtonClick(Vector3 cursorPosition)
    {
        GridElement selected_gridElement = gridSystem.GetGridElement(cursorPosition);

        if (selected_gridElement?.HasUnit() == true)
        {
            //Select unit / Deselect previous
            selected_unit?.Deselect();
            selected_unit = selected_gridElement.GetUnit();
            selected_unit.Select();
            if (!controlled_unit.Fixed || !OneUnitPerTurn)
            {
                if (selected_unit.team == turnSystem.CurrentPlayer.Number && turnSystem.Type != TurnSystem.TurnSystemType.IniciativeOrder)
                {
                    controlled_unit.Unit = selected_unit;
                }
            }

            //After selecting, we need to update movement range if unit can move
            gridSystem.ClearGrid();
            if (selected_unit.canMove && selected_unit == controlled_unit.Unit)
            {
                gridSystem.UpdateMovementRange(selected_gridElement.x, selected_gridElement.y, selected_unit);
            }
            //After selecting, we need to update movement range if unit can attack
            if (selected_unit.canAttack && selected_unit == controlled_unit.Unit)
            {
                gridSystem.UpdateAttackRange(selected_gridElement.x, selected_gridElement.y, selected_unit);
            }
            //Update Danger Zone
            if (selected_unit.team != turnSystem.CurrentPlayer.Number)
            {
                gridSystem.UpdateDangerZone(selected_gridElement.x, selected_gridElement.y, selected_unit);
            }
        }
        else
        {
            selected_unit?.Deselect();
            gridSystem.ClearGrid();
        }
    }

    private void OnSecondaryCursorButtonClick(Vector3 cursorPosition)
    {
        if (controlled_unit.Unit != null)
        {
            //Get grid elements
            GridElement selected_gridElement = gridSystem.GetGridElement(cursorPosition);
            GridElement controlledUnit_gridElement = gridSystem.GetGridElement(controlled_unit.Unit.GetPosition());
            //Check if unit under controlled has focus (selected)
            if (selected_unit == controlled_unit.Unit)
            {
                //Check if valid move:
                if (selected_gridElement?.IsReachableOneMove == true && selected_unit.canMove)
                {

                    //Calculate path to selected move position
                    List<Vector3> MovePath = gridSystem.FindPath(controlled_unit.Unit, selected_gridElement);

                    //Command Move if possible path
                    if (MovePath != null)
                    {
                        //Clear Grid
                        gridSystem.ClearGrid();
                        //Clear unit from his grid element
                        controlledUnit_gridElement.SetUnit(null);

                        //Command move
                        controlled_unit.Unit.MoveTo(MovePath, null);

                    }
                }
                //Check if valid attack target:
                if (selected_gridElement?.EnemyInRange == true && controlled_unit.Unit.canAttack)
                {
                    //Clear Grid
                    gridSystem.ClearGrid();
                    //Command Attack unit (enemy) at selected grid element
                    controlled_unit.Unit.Attack(selected_gridElement.Unit, null);
                }
            }

        }
    }

    private void OnCursorHoverGrid(int grid_x, int grid_y)
    {
        //if (controlled_unit.Unit.IsBusy == false)
        //{
        //    //Draw tentative path
        //    gridSystem.ClearGrid();
        //    List<Vector3> pathSteps = gridSystem.FindPath(controlled_unit.Unit, new Vector3(grid_x, grid_y));
        //    if (pathSteps != null)
        //    {
        //        foreach (Vector3 position in pathSteps)
        //        {
        //            //MarkAsReachableOneMove
        //            gridSystem.GetGridElement(position).gridVisual.MarkAsReachableOneMove();
        //        }
        //    }
        //}
    }

    private void OnNewTurn(int currentTurn, TurnSystem.Player currentPlayer, Unit activeUnit)
    {
        controlled_unit.Unit = activeUnit;
        controlled_unit.Fixed = true;

        //TO DO: Make private function Select Unit or OnUnitSelect(Unit unit)
        selected_unit?.Deselect();
        selected_unit = controlled_unit.Unit;
        selected_unit.Select();

        Debug.Log($"Active Unit = {activeUnit}, currentPlayer = {currentPlayer}");
    }

    public void UpdateGridAfterAction(Unit sender_unit)
    {
        //Update unit grid position
        GridElement gridElement = gridSystem.GetGridElement(sender_unit.GetPosition());
        gridElement.SetUnit(sender_unit);
        //Snap unit to grid
        gridElement.SnapUnitToGrid();
        //Update grid
        gridSystem.ClearGrid();
        if (sender_unit.canMove)
        {
            gridSystem.UpdateMovementRange(gridSystem.GetGridElement(sender_unit.GetPosition()).x, gridSystem.GetGridElement(sender_unit.GetPosition()).y, sender_unit);
        }
        if (sender_unit.canAttack)
        {
            gridSystem.UpdateAttackRange(gridSystem.GetGridElement(sender_unit.GetPosition()).x, gridSystem.GetGridElement(sender_unit.GetPosition()).y, sender_unit);
        }
        if (sender_unit.HasActions() == false)
        {
            sender_unit.Deselect();
            selected_unit = null;
            controlled_unit.Unit = null;
            controlled_unit.Fixed = false;
        }
        else
        {
            if (OneUnitPerTurn)
            {
                if (sender_unit == controlled_unit.Unit)
                {
                    controlled_unit.Fixed = true;
                    turnSystem.SetActiveUnit(controlled_unit.Unit);
                }
            }
        }

    }
}



