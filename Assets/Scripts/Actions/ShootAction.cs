using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Animations;

public class ShootAction : BaseAction
{
    public static event EventHandler<OnShootEventArgs> OnAnyShoot;
    public static event EventHandler<double> OnAnyAttackAccuracyChanged;
    public event EventHandler<OnShootEventArgs> OnShoot;

    public class OnShootEventArgs : EventArgs
    {
        public Unit targetUnit;
        public Unit shootingUnit;
        public bool attackHits;
    }
    private enum State
    {
        Aiming,
        Shooting,
        Cooloff,
    }

    private enum AttackDirection
    {
        North,
        South,
        East,
        West,
    }

    [SerializeField] private LayerMask obstaclesLayerMask;

    private State state;
    private int maxShootDistance = 7;
    private float stateTimer;
    private Unit targetUnit;
    private double attackAccuracy;
    private bool canShootBullet;
    List<Unit> availableTargets = new List<Unit>();

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

        

        if(AttackHits())
        {
            targetUnit.Damage(40);
            OnShoot?.Invoke(this, new OnShootEventArgs
            {
                targetUnit = targetUnit,
                shootingUnit = unit,
                attackHits = true
            });
            Debug.Log("Attack hit");
        }

        else
        {
            OnShoot?.Invoke(this, new OnShootEventArgs
            {
                targetUnit = targetUnit,
                shootingUnit = unit,
                attackHits = false
            });
            Debug.Log("Attack miss");
        }
        
    }

    private bool AttackHits()
    {
        double attackAccuracy = CalculateAttackAccuracy(targetUnit);
        OnAnyAttackAccuracyChanged?.Invoke(this, attackAccuracy);
        Debug.Log($"Attack Accuracy: {attackAccuracy}");


        System.Random r = new System.Random();
        double num = r.NextDouble();

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

    private double CalculateAttackAccuracy(Unit targetUnit)
    {
        GridPosition shootingUnitGridPosition = unit.GetGridPosition();
        GridPosition targetUnitGridPosition = targetUnit.GetGridPosition();

        double attackAccuracy;

        double baseAccuracy = 1.0;
        double targetAccuracy = 0.5;
        double targetDistance = 7.0;

        //This coverFactor reduces accuracy at a perdictable amount. 5 cover points cuts accuracy in half
        double coverFactor = 0.1386;

        double k = -Math.Log(targetAccuracy / baseAccuracy) / targetDistance;

        int attackDistance = (Pathfinding.Instance.CalculateDistance(shootingUnitGridPosition, targetUnitGridPosition)) / 10;

        AttackDirection attackDirection = CalculateAttackDirection();
        double coverPoints = 0;

        switch (attackDirection)
        {
            case AttackDirection.North:
                coverPoints = targetUnit.GetCoverSystem().GetSouthCoverPoints();
                break;
            case AttackDirection.South:
                coverPoints = targetUnit.GetCoverSystem().GetNorthCoverPoints();
                break;
            case AttackDirection.East:
                coverPoints = targetUnit.GetCoverSystem().GetWestCoverPoints();
                break;
            case AttackDirection.West:
                coverPoints = targetUnit.GetCoverSystem().GetEastCoverPoints();
                break;
        }

        attackAccuracy = baseAccuracy * Math.Exp(-(k * attackDistance + coverFactor * coverPoints));

        /*Debug.Log($"Attack Distance: {attackDistance}");
        Debug.Log($"Attack Direction: {attackDirection.ToString()}");
        Debug.Log($"Target Unit Accounted Cover Points: {coverPoints}");
        Debug.Log($"TU North: {targetUnit.GetCoverSystem().GetNorthCoverPoints()}; TU South: {targetUnit.GetCoverSystem().GetSouthCoverPoints()}; TU East: {targetUnit.GetCoverSystem().GetEastCoverPoints()}; TU West: {targetUnit.GetCoverSystem().GetWestCoverPoints()};");*/

        return attackAccuracy;
    }

    private AttackDirection CalculateAttackDirection()
    {
        AttackDirection direction;

        Vector3 aimDir = (targetUnit.GetWorldPosition() - unit.GetWorldPosition()).normalized;

        string cardinalNS = aimDir.z > 0 ? "North" : "South";
        string cardianlEW = aimDir.x > 0 ? "East" : "West";

        if (Math.Abs(aimDir.x) > Math.Abs(aimDir.z))
        {
            if(aimDir.x > 0)
            {
                direction = AttackDirection.East;
            }

            else
            {
                direction = AttackDirection.West;
            }
        }

        else
        {
            if (aimDir.z > 0)
            {
                direction = AttackDirection.North;
            }

            else
            {
                direction = AttackDirection.South;
            }
        }

        return direction;
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
        availableTargets.Clear();

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
                GridObject gridobject = LevelGrid.Instance.GetGridObject(testGridPosition);
                availableTargets.Add(gridobject.GetUnit());
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

    public Unit GetClosestTarget()
    {
        if(availableTargets.Count != 0)
            return availableTargets[0];
        else return null;
    }

    public void AimCharacterForActionCamera(Unit aimTarget)
    {
        targetUnit = aimTarget;
        state = State.Aiming;
    }

    public void StopAimForActionCamera(Unit aimTarget)
    {
        state = State.Cooloff;
    }

    public List<Unit> GetAvailableTargets()
    {
        return availableTargets;
    }

    private void UnitActionSystem_OnShootActionSelected(object sender, EventArgs e)
    {
        availableTargets.Clear();
    }

    public void SetTargetUnit(Unit targetUnit)
    {
        this.targetUnit = targetUnit;
    }

    public double GetCalculatedAttackAccuracy(Unit targetUnit)
    {
        return CalculateAttackAccuracy(targetUnit);
    }
}
