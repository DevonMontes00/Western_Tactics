using System;
using UnityEngine;

public class MouseWorld : MonoBehaviour
{
    private static MouseWorld instance;

    [SerializeField] private LayerMask mousePlaneLayerMask;
    [SerializeField] private LayerMask unit;

    private Unit hoverUnit;

    private void Awake()
    {
        instance = this;
    }

    public void Update()
    {
        if (UnitActionSystem.Instance.GetSelectedAction() is ShootAction)
        {
            if (IsHoveringEnemyGridPosition() || IsHoveringEnemyUnit())
            {
                ShowEnemyUnitHitPercentage();
            }

            else if (hoverUnit != null)
            {
                HideEnemyUnitHitPercentage();
            }
        }
        
    }

    public static Vector3 GetPosition()
    {

        Ray ray = Camera.main.ScreenPointToRay(InputManager.Instance.GetMouseScreenPosition());
        Physics.Raycast(ray, out RaycastHit raycastHit, float.MaxValue, instance.mousePlaneLayerMask);
        return raycastHit.point;
        
    }

    private bool IsHoveringEnemyGridPosition()
    {
        bool isHoveringEnemyGridPosition = false;

        Vector3 mousePosition = GetPosition();
        
        GridPosition hoveringGridPosition = LevelGrid.Instance.GetGridPosition(mousePosition);

        if (LevelGrid.Instance.IsValidGridPosition(hoveringGridPosition))
        {
            GridObject hoveringGridObject = LevelGrid.Instance.GetGridObject(hoveringGridPosition);

            if (hoveringGridObject.HasAnyUnit())
            {
                Unit hoveringUnit = hoveringGridObject.GetUnit();

                if (hoveringUnit.IsEnemy() && HoverUnitIsInRange(hoveringUnit))
                {
                    hoverUnit = hoveringUnit;
                    isHoveringEnemyGridPosition = true;
                }
            }
        }

        return isHoveringEnemyGridPosition;
    }

    private bool IsHoveringEnemyUnit()
    {
        bool isHoveringEnemyUnit = false;

        Ray ray = Camera.main.ScreenPointToRay(InputManager.Instance.GetMouseScreenPosition());

        if (Physics.Raycast(ray, out RaycastHit raycastHit, float.MaxValue, instance.unit))
        {
            if (raycastHit.transform.TryGetComponent<Unit>(out Unit hoveringUnit))
            {
                GridPosition hoverUnitGridPosition = hoveringUnit.GetGridPosition();

                ShootAction shootAction = (ShootAction)UnitActionSystem.Instance.GetSelectedAction();

                if (hoveringUnit.IsEnemy() && shootAction.IsValidActionGridPosition(hoverUnitGridPosition))
                {
                    hoverUnit = hoveringUnit;
                    isHoveringEnemyUnit = true;
                }
            }
        }

        return isHoveringEnemyUnit;
    }

    private bool HoverUnitIsInRange(Unit hoverUnit)
    {
        bool hoverUnitIsInRange = false;

        ShootAction shootAction = (ShootAction)UnitActionSystem.Instance.GetSelectedAction();
        if(shootAction.IsValidActionGridPosition(hoverUnit.GetGridPosition()))
        {
            hoverUnitIsInRange = true;
        }

        return hoverUnitIsInRange;
    }

    private void ShowEnemyUnitHitPercentage()
    {
        ShootAction shootAction = (ShootAction)UnitActionSystem.Instance.GetSelectedAction();
        Unit selectedUnit = UnitActionSystem.Instance.GetSelectedUnit();

        double tempHitPercentageDouble = shootAction.GetCalculatedAttackAccuracy(hoverUnit, selectedUnit) * 100;
        double hitPercentageDouble = Math.Round(tempHitPercentageDouble);
        string hitPercentageString = hitPercentageDouble.ToString() + "%";

        hoverUnit.SetIsHoveredForTarget(true, hitPercentageString);

    }

    private void HideEnemyUnitHitPercentage()
    {
        hoverUnit.SetIsHoveredForTarget(false);
    }
}
