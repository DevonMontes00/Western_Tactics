using NUnit.Framework;
using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class MoveAction : BaseAction
{
    public event EventHandler OnStartMoving;
    public event EventHandler OnStopMoving;
    [SerializeField] private int maxMoveDistance = 4;

    private List<Vector3> positionList;
    private int currentPositionIndex;

    // Update is called once per frame
    private void Update()
    {
        if (!isActive)
        {
            return;
        }
        float stoppingDistance = .1f;
        Vector3 targetPosition = positionList[currentPositionIndex];
        Vector3 moveDirection = (targetPosition - transform.position).normalized;

        float rotateSpeed = 10f;
        transform.forward = Vector3.Lerp(transform.forward, moveDirection, Time.deltaTime * rotateSpeed);

        if (Vector3.Distance(transform.position, targetPosition) > stoppingDistance)
        {
            float moveSpeed = 4f;
            transform.position += moveDirection * moveSpeed * Time.deltaTime;
        }

        else
        {
            currentPositionIndex++;
            if (currentPositionIndex >= positionList.Count)
            {
                OnStopMoving?.Invoke(this, EventArgs.Empty);
                UpdateUnitCoverPoints(targetPosition);
                ActionComplete();
            }
        }

        
    }

    public override void TakeAction(GridPosition gridPosition, Action onActionComplete)
    {
        List<GridPosition> pathGridPositonList = Pathfinding.Instance.FindPath(unit.GetGridPosition(), gridPosition, out int pathLength);

        currentPositionIndex = 0;
        positionList = new List<Vector3>();

        foreach(GridPosition pathGridPositon in pathGridPositonList)
        {
            positionList.Add(LevelGrid.Instance.GetWorldPosition(pathGridPositon));
        }

        GridObject go = LevelGrid.Instance.GetGridObject(gridPosition);;

        OnStartMoving?.Invoke(this, EventArgs.Empty);
        ActionStart(onActionComplete);
    }

    public override List<GridPosition> GetValidActionGridPositionList()
    {
        List<GridPosition> validGridPositionList = new List<GridPosition>();
        GridPosition unitGridPosition = unit.GetGridPosition();

        for(int x = -maxMoveDistance; x <= maxMoveDistance; x++)
        {
            for(int z = -maxMoveDistance; z <= maxMoveDistance; z++)
            {
                GridPosition offsetGridPosition = new GridPosition(x, z);
                GridPosition testGridPosition = unitGridPosition + offsetGridPosition;

                if (!LevelGrid.Instance.IsValidGridPosition(testGridPosition))
                {
                    continue;
                }

                if(unitGridPosition == testGridPosition)
                {
                    //Same Grid Position where the unit is already at
                    continue;
                }

                if(LevelGrid.Instance.HasAnyUnitOnGridPosition(testGridPosition))
                {
                    //Grid position already occupied with another unit
                    continue;
                }

                if(!Pathfinding.Instance.IsWalkableGridPosition(testGridPosition))
                {
                    continue;
                }

                if (!Pathfinding.Instance.HasPath(unit.GetGridPosition(), testGridPosition))
                {
                    continue;
                }

                int pathfindingDistanceMultiplyer = 10;
                if (Pathfinding.Instance.GetPathLength(unit.GetGridPosition(), testGridPosition) > maxMoveDistance * pathfindingDistanceMultiplyer)
                {
                    //Path length is too long
                    continue;
                }


                validGridPositionList.Add(testGridPosition);
            }
        }

        return validGridPositionList;
    }

    public override string GetActionName()
    {
        return "Move";
    }
    public override EnemyAIAction GetEnemyAIAction(GridPosition gridPosition)
    {
        int targetCountAtGridPosition = unit.GetAction<ShootAction>().GetTargetCountAtPosition(gridPosition);
        return new EnemyAIAction
        {
            gridPosition = gridPosition,
            actionValue = targetCountAtGridPosition * 10,
        };
    }

    public int GetMaxMoveDistance()
    {
        return maxMoveDistance;
    }

    private void UpdateUnitCoverPoints(Vector3 targetPosition)
    {
        GridPosition gp = LevelGrid.Instance.GetGridPosition(targetPosition);
        GridObject go = LevelGrid.Instance.GetGridObject(gp);

        unit.GetCoverSystem().SetNorthCoverPoints(go.GetNorthCoverPoints());
        unit.GetCoverSystem().SetSouthCoverPoints(go.GetSouthCoverPoints());
        unit.GetCoverSystem().SetWestCoverPoints(go.GetWestCoverPoints());
        unit.GetCoverSystem().SetNorthCoverPoints(go.GetEastCoverPoints());
    }
}
