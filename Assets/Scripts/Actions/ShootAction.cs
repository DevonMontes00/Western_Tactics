using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ShootAction : BaseAction
{
    public static event EventHandler<OnShootEventArgs> OnAnyShoot;
    public event EventHandler<OnShootEventArgs> OnShoot;

    public class OnShootEventArgs : EventArgs
    {
        public Unit targetUnit;
        public Unit shootingUnit;
    }
    private enum State
    {
        Aiming,
        Shooting,
        Cooloff,
    }

    [SerializeField] private LayerMask obstaclesLayerMask;
    [SerializeField] private int guarenteedHitDistance = 2;

    private State state;
    private int maxShootDistance = 7;
    private float stateTimer;
    private Unit targetUnit;
    private bool canShootBullet;

    private void Update()
    {
        if (!isActive)
        {
            return;
        }

        stateTimer -= Time.deltaTime;

        switch(state)
        {
            case State.Aiming:
                Aim();
                break;
            case State.Shooting:
                if(canShootBullet)
                {
                    Shoot();
                    canShootBullet = false;
                }
                break;
            case State.Cooloff:
                break;
        }

        if (stateTimer <= 0f)
        {
            NextState();
        }
    }

    private void NextState()
    {
        switch (state)
        {
            case State.Aiming:
                if (stateTimer <= 0f)
                {
                    state = State.Shooting;
                    float shootingStateTime = 0.1f;
                    stateTimer = shootingStateTime;
                }
                break;
            case State.Shooting:
                if (stateTimer <= 0f)
                {
                    state = State.Cooloff;
                    float coolOffStateTime = 0.5f;
                    stateTimer = coolOffStateTime;
                }
                break;
            case State.Cooloff:
                ActionComplete();
                break;
        }
    }

    private void Aim()
    {
        Vector3 aimDir = (targetUnit.GetWorldPosition() - unit.GetWorldPosition()).normalized;
        float rotateSpeed = 10f;
        transform.forward = Vector3.Lerp(transform.forward, aimDir, Time.deltaTime * rotateSpeed);
    }

    private void Shoot()
    {
        OnAnyShoot?.Invoke(this, new OnShootEventArgs
        {
            targetUnit = targetUnit,
            shootingUnit = unit,
        });

        OnShoot?.Invoke(this, new OnShootEventArgs
        {
            targetUnit = targetUnit,
            shootingUnit = unit,
        });

        if(AttackHits())
        {
            targetUnit.Damage(40);
            Debug.Log("Attack hit");
        }

        else
        {
            Debug.Log("Attack miss");
        }
        
    }

    private bool AttackHits()
    {
        double attackAccuracy = CalculateAttackAccuracy(unit.GetGridPosition(), targetUnit.GetGridPosition());
        

        System.Random r = new System.Random();
        int num = r.Next(0, 100);

        Debug.Log($"Random Number: {num}");

        if (attackAccuracy > num)
        {
            return true;
        }

        else
        {
            return false;
        }

    }

    private double CalculateAttackAccuracy(GridPosition shootingUnitGridPosition, GridPosition targetUnitGridPosition)
    {
        double attackAccuracy;

        double baseAccuracy = 1.0;
        double targetAccuracy = 0.5;
        double targetDistance = 7.0;

        //This coverFactor reduces accuracy at a perdictable amount. 5 cover points cuts accuracy in half
        double coverFactor = 0.1386;

        double k = -Math.Log(targetAccuracy / baseAccuracy) / targetDistance;

        int attackDistance = (Pathfinding.Instance.CalculateDistance(shootingUnitGridPosition, targetUnitGridPosition)) / 10;

        if (attackDistance < guarenteedHitDistance)
        {
            attackAccuracy = 100;
            return attackAccuracy;
        }

        else
        {
            attackAccuracy = baseAccuracy * Math.Exp(-k * attackDistance + coverFactor * targetUnit.GetCoverPoints()); 
        }

        Debug.Log($"Attack Distance: {attackDistance}, Accuracy {attackAccuracy}");

        return attackAccuracy * 100;
    }

    public override string GetActionName()
    {
        return "Shoot";
    }

    public override List<GridPosition> GetValidActionGridPositionList()
    {
        GridPosition unitGridPosition = unit.GetGridPosition();
        return GetValidActionGridPositionList(unitGridPosition);
    }

    public List<GridPosition> GetValidActionGridPositionList(GridPosition unitGridPosition)
    {
        List<GridPosition> validGridPositionList = new List<GridPosition>();

        for (int x = -maxShootDistance; x <= maxShootDistance; x++)
        {
            for (int z = -maxShootDistance; z <= maxShootDistance; z++)
            {
                GridPosition offsetGridPosition = new GridPosition(x, z);
                GridPosition testGridPosition = unitGridPosition + offsetGridPosition;

                if (!LevelGrid.Instance.IsValidGridPosition(testGridPosition))
                {
                    continue;
                }

                int testDistance = Mathf.Abs(x) + Math.Abs(z);
                if(testDistance > maxShootDistance)
                {
                    //Calculating for a circular radius
                    continue;
                }

                if (!LevelGrid.Instance.HasAnyUnitOnGridPosition(testGridPosition))
                {
                    //Grid position is empty, no Unit
                    continue;
                }

                Unit targetUnit = LevelGrid.Instance.GetUnitAtGridPosition(testGridPosition);

                if(targetUnit.IsEnemy() == unit.IsEnemy())
                {
                    // Both Units on same "team"
                    continue;
                }

                Vector3 unitWorldPostion = LevelGrid.Instance.GetWorldPosition(unitGridPosition);
                Vector3 shootDir = (targetUnit.GetWorldPosition() - unitWorldPostion).normalized;

                float unitShoulderHeight = 1.7f;
                if (Physics.Raycast(
                    unitWorldPostion + Vector3.up * unitShoulderHeight,
                    shootDir,
                    Vector3.Distance(unitWorldPostion, targetUnit.GetWorldPosition()),
                    obstaclesLayerMask))
                {
                    // Blocked by obstacle
                    continue;
                };

                validGridPositionList.Add(testGridPosition);
            }
        }

        return validGridPositionList;
    }

    public override void TakeAction(GridPosition gridPosition, Action onActionComplete)
    {
        targetUnit = LevelGrid.Instance.GetUnitAtGridPosition(gridPosition);

        state = State.Aiming;
        float aimingStateTime = 1f;
        stateTimer = aimingStateTime;

        canShootBullet = true;

        ActionStart(onActionComplete);
    }

    public Unit GetTargetUnit()
    {
        return targetUnit;
    }

    public int GetMaxShootDistance()
    {
        return maxShootDistance;
    }

    public override EnemyAIAction GetEnemyAIAction(GridPosition gridPosition)
    {
        Unit targetUnit = LevelGrid.Instance.GetUnitAtGridPosition(gridPosition);

        return new EnemyAIAction
        {
            gridPosition = gridPosition,
            actionValue = 100 + Mathf.RoundToInt((1 - targetUnit.GetHealthNormalized()) * 100f),
        };
    }

    public int GetTargetCountAtPosition(GridPosition gridPosition)
    {
        return GetValidActionGridPositionList(gridPosition).Count;
    }
}
