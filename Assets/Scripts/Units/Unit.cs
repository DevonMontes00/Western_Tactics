using System;
using UnityEngine;
using UnityEngine.EventSystems;

public class Unit : MonoBehaviour
{
    private const int ACTION_POINTS_MAX = 9;
    public static event EventHandler OnAnyActionPointsChanged;
    public static event EventHandler OnAnyUnitSpawned;
    public static event EventHandler OnAnyUnitDead;


    private GridPosition gridPosition;
    private HealthSystem healthSystem;
    private CoverSystem coverSystem;
    private BaseAction[] baseActionArray;
    private int actionPoints;

    [SerializeField] private bool isEnemy;

    private void Awake()
    {
        healthSystem = GetComponent<HealthSystem>();
        baseActionArray = GetComponents<BaseAction>();
        coverSystem = GetComponent<CoverSystem>();
        actionPoints = ACTION_POINTS_MAX;
    }

    private void Start()
    {
        gridPosition = LevelGrid.Instance.GetGridPosition(transform.position);

        LevelGrid.Instance.AddUnitAtGridPosition(gridPosition, this);

        TurnSystem.Instance.OnTurnChanged += TurnSystem_OnTurnChanged;

        healthSystem.OnDead += HealthSystem_OnDead;

        OnAnyUnitSpawned?.Invoke(this, EventArgs.Empty);

        coverSystem.SetStartingCoverPoints(gridPosition);
    }

    

    private void TurnSystem_OnTurnChanged(object sender, System.EventArgs e)
    {
        if ((IsEnemy() &&! TurnSystem.Instance.IsPlayerTurn()) || (!IsEnemy() && TurnSystem.Instance.IsPlayerTurn()))
        {
            actionPoints = ACTION_POINTS_MAX;

            OnAnyActionPointsChanged?.Invoke(this, EventArgs.Empty);
        }
        
    }

    private void Update()
    {
        GridPosition newgridPosition = LevelGrid.Instance.GetGridPosition(transform.position);

        if (newgridPosition != gridPosition)
        {
            GridPosition oldGridPosition = gridPosition;
            gridPosition = newgridPosition;

            LevelGrid.Instance.UnitMovedGridPosition(this, oldGridPosition, newgridPosition);
        }

    }

    public T GetAction<T>() where T : BaseAction
    {
        foreach(BaseAction baseAction in baseActionArray)
        {
            if(baseAction is T)
            {
                return (T)baseAction;
            }
        }

        return null;
    }

    public GridPosition GetGridPosition()
    {
        return gridPosition;
    }

    public Vector3 GetWorldPosition()
    {
        return this.transform.position;
    }

    public BaseAction[] GetBaseActionArray()
    {
        return baseActionArray;
    }
    public bool TrySpendActionPointsToTakeAction(BaseAction baseAction)
    {
        if(CanSpendActionPointsToTakeAction(baseAction))
        {
            SpendActionPoints(baseAction.GetActionPointsCost());
            return true;
        }

        else { return false; }
    }
    public bool CanSpendActionPointsToTakeAction(BaseAction baseAction)
    {
        if(actionPoints >= baseAction.GetActionPointsCost())
        {
            return true;
        }

        else
        {
            return false;
        }
    }

    private void SpendActionPoints(int amount)
    {
        actionPoints -= amount;

        OnAnyActionPointsChanged?.Invoke(this, EventArgs.Empty);
    }

    public int GetActionPoints()
    {
        return actionPoints;
    }

    public bool IsEnemy()
    {
        return isEnemy;
    }

    public void Damage(int damageAmount)
    {
        healthSystem.Damage(damageAmount);
    }
    private void HealthSystem_OnDead(object sender, EventArgs e)
    {
        LevelGrid.Instance.RemoveUnitAtGridPosition(gridPosition, this);
        Destroy(gameObject);

        OnAnyUnitDead?.Invoke(this, EventArgs.Empty);
    }

    public float GetHealthNormalized()
    {
        return healthSystem.GetHealthNormalized();
    }

    public CoverSystem GetCoverSystem()
    {
        return coverSystem;
    }
}
