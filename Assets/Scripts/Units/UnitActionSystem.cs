using UnityEngine;
using System;
using UnityEngine.EventSystems;
using NUnit.Framework;
using System.Collections.Generic;

public class UnitActionSystem : MonoBehaviour
{
    public static UnitActionSystem Instance { get; private set; }

    public event EventHandler OnSelectedUnitChanged;
    public event EventHandler OnSelectedActionChanged;
    public event EventHandler <ShootAction>OnShootActionSelected;
    public event EventHandler<bool> OnBusyChanged;
    public event EventHandler OnActionStarted;

    [SerializeField] private Unit selectedUnit;
    [SerializeField] private LayerMask unitLayerMask;
    [SerializeField] private LayerMask mousePlaneLayerMask;

    private BaseAction selectedAction;
    private bool isBusy = false;

    private void Awake()
    {
        if (Instance != null)
        {
            Debug.LogError("There's more than one UnitActionSystem! " + transform + " - " + Instance);
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    private void Start()
    {
        SetSelectedUnit(selectedUnit);

        ShootActionCameraUI.OnFireButtonClicked += ShootActionCameraUI_OnFireButtonClicked;
        //ActionButtonUI.OnShootActionButtonPressed += ActionButtonUI_OnShootActionButtonPressed;
    }

    

    private void Update()
    {
        if (isBusy)
        {
            return;
        }

        if (!TurnSystem.Instance.IsPlayerTurn())
        {
            return;
        }
        
        if(EventSystem.current.IsPointerOverGameObject())
        {
            return;
        }

        if(TryHandleUnitSelection())
        {
            return;
        }

        if (InputManager.Instance.IsEscapeDownThisFrame())
        {
            if(CameraManager.Instance.IsActionCameraActive())
            {
                CameraManager.Instance.HideActionCamera();
                
            }
        }

        HandleSelectedAction();

    }

    private void HandleSelectedAction()
    {
        if (InputManager.Instance.IsMouseButtonDownThisFrame())
        {
            GridPosition mouseGridPosition = LevelGrid.Instance.GetGridPosition(MouseWorld.GetPosition());

            if (selectedAction.IsValidActionGridPosition(mouseGridPosition))
            {
                if (selectedUnit.TrySpendActionPointsToTakeAction(selectedAction))
                {
                    SetBusy();
                    selectedAction.TakeAction(mouseGridPosition, ClearBusy);

                    OnActionStarted?.Invoke(this, EventArgs.Empty);
                } 
            }
        }
    }

    private void OutsideHandleSelectedAction(GridPosition gridPosition)
    {
        if (selectedUnit.TrySpendActionPointsToTakeAction(selectedAction))
        {
            SetBusy();
            selectedAction.TakeAction(gridPosition, ClearBusy);

            OnActionStarted?.Invoke(this, EventArgs.Empty);
        }
    }

    private void SetBusy()
    {
        isBusy = true;
        OnBusyChanged?.Invoke(this, isBusy);
    }

    private void ClearBusy()
    {
        isBusy = false;
        OnBusyChanged?.Invoke(this, isBusy);
    }

    private bool TryHandleUnitSelection()
    {
        if (InputManager.Instance.IsMouseButtonDownThisFrame())
        {
            Ray ray = Camera.main.ScreenPointToRay(InputManager.Instance.GetMouseScreenPosition());
            if (Physics.Raycast(ray, out RaycastHit raycastHit, float.MaxValue, unitLayerMask))
            {
                if (raycastHit.transform.TryGetComponent<Unit>(out Unit unit))
                {
                    if (unit == selectedUnit)
                    {
                        //Unit is already selected
                        return false;
                    }

                    if (unit.IsEnemy())
                    {
                        return false;
                    }
                    SetSelectedUnit(unit);
                    return true;
                }
            }
        }
        return false;
    }

    private void SetSelectedUnit(Unit unit)
    {
        selectedUnit = unit;

        SetSelectedAction(unit.GetAction<MoveAction>());

        OnSelectedUnitChanged?.Invoke(this, EventArgs.Empty);
            
    }

    public void SetSelectedAction(BaseAction baseAction)
    {
        selectedAction = baseAction;

        OnSelectedActionChanged?.Invoke(this, EventArgs.Empty);
    }

    public Unit GetSelectedUnit()
    {
        return selectedUnit;
    }

    public BaseAction GetSelectedAction()
    {
        return selectedAction;
    }

    private void ShootActionCameraUI_OnFireButtonClicked(object sender, GridPosition gridPosition)
    {
        OutsideHandleSelectedAction(gridPosition);
    }

    private void ActionButtonUI_OnShootActionButtonPressed(object sender, ShootAction shootAction)
    {
        OnShootActionSelected?.Invoke(this, shootAction);
    }
}
