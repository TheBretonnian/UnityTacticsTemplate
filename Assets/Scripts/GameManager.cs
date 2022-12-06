using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Singletons
{
    public class GameManager : MonoBehaviour
    {
        //Singleton property
        public static GameManager Instance;

        [Header("Units")]
        [SerializeField] private Unit[] units;

        [Header("Game Systems")]
        //[SerializeField] private TurnSystem turnSystem;
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
            //OneUnitPerTurn = !(turnSystem.Type == TurnSystem.TurnSystemType.TeamBased);
            //if (turnSystem.Type == TurnSystem.TurnSystemType.IniciativeOrder)
            //{
            //    turnSystem.NewTurnEvent += OnNewTurn;
            //}

            
        }

        // Start is called before the first frame update
        void Start()
        {
            //Grid System
            gridSystem.CreateGrid();
            gridSystem.onCursorHoverGrid += GameManager_onCursorHoverGrid;

            //Input events
            InputManager.Instance.OnMainCursorButtonClick += GameManager_onMainCursorButtonClick;
            InputManager.Instance.OnSecondaryCursorButtonClick += GameManager_onSecondaryCursorButtonClick;

            //Center Camera
            Camera.main.transform.position = new Vector3(gridSystem.Width / 2, gridSystem.Height / 2, Camera.main.transform.position.z);
            Camera.main.orthographicSize = ((float)gridSystem.Height + 3.0f) / 2.0f;

            //Set Units position
            foreach (Unit unit in units)
            {
                gridSystem.GetGridElement(unit.transform.position).SetUnit(unit);
                //unit.onMoveCompleted += UpdateGridAfterMove;
                //unit.onAttackCompleted += UpdateGridAfterMove;
            }

            //Testing
            if (units.Length > 0)
            {
                selected_unit = units[0];
                controlled_unit.Unit = units[0];
            }
        }

        // Update is called once per frame
        void Update()
        {
            if (Input.GetMouseButtonDown(1) && controlled_unit.Unit != null)
            {
                //Get grid elements
                GridElement selected_gridElement = gridSystem.GetGridElement(InputManager.GetMouseWorldPosition());
                GridElement controlledUnit_gridElement = gridSystem.GetGridElement(controlled_unit.Unit.GetPosition());
                //Check if valid move:
                //if (selected_unit == controlled_unit.Unit)
                //{
                //if (selected_gridElement?.IsReachableOneMove == true && selected_unit.canMove)
                //{

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
                //}
                //if (selected_gridElement?.EnemyInRange == true && controlled_unit.Unit.canAttack)
                //{
                //    //Clear Grid
                //    ClearGrid();
                //    //Command Attack unit (enemy) at selected grid element
                //    controlled_unit.Unit.Attack(selected_gridElement.unit, null);

                //}
                //}

            }
        }

        private void GameManager_onMainCursorButtonClick(Vector3 cursorPosition)
        {
            return;
        }

        private void GameManager_onSecondaryCursorButtonClick(Vector3 cursorPosition)
        {
            if (controlled_unit.Unit != null)
            {
                //Get grid elements
                GridElement selected_gridElement = gridSystem.GetGridElement(InputManager.GetMouseWorldPosition());
                GridElement controlledUnit_gridElement = gridSystem.GetGridElement(controlled_unit.Unit.GetPosition());
                //Check if valid move:
                //if (selected_unit == controlled_unit.Unit)
                //{
                //if (selected_gridElement?.IsReachableOneMove == true && selected_unit.canMove)
                //{

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
                //}
                //if (selected_gridElement?.EnemyInRange == true && controlled_unit.Unit.canAttack)
                //{
                //    //Clear Grid
                //    ClearGrid();
                //    //Command Attack unit (enemy) at selected grid element
                //    controlled_unit.Unit.Attack(selected_gridElement.unit, null);

                //}
                //}

            }
        }

        private void GameManager_onCursorHoverGrid(int grid_x, int grid_y)
        {
            if (controlled_unit.Unit.IsBusy == false)
            {
                //Draw tentative path
                gridSystem.ClearGrid();
                List<Vector3> pathSteps = gridSystem.FindPath(controlled_unit.Unit, new Vector3(grid_x, grid_y));
                if (pathSteps != null)
                {
                    foreach (Vector3 position in pathSteps)
                    {
                        //MarkAsReachableOneMove
                        gridSystem.GetGridElement(position).gridVisual.MarkAsReachableOneMove();
                    }
                }
            }
        }
    }
}



