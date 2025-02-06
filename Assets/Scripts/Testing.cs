using System.Collections.Generic;
using UnityEngine;

public class Testing : MonoBehaviour
{
    private void Start()
    {
        GridSystemVisual.Instance.HideAllGridPosition();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.T))
        {
            Unit unit = UnitActionSystem.Instance.GetSelectedUnit();
            List<GridPosition> validGridPositions = unit.GetMoveAction().GetValidActionGridPositionList();
            GridSystemVisual.Instance.ShowGridPositionList(validGridPositions);
        }

        if(Input.GetKeyDown(KeyCode.R))
        {
            GridSystemVisual.Instance.HideAllGridPosition();
        }
    }
}
