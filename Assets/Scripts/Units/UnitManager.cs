using NUnit.Framework;
using System;
using System.Collections.Generic;
using UnityEngine;

public class UnitManager : MonoBehaviour
{
    public static UnitManager Instance { get; private set; }

    public event EventHandler OnAllEnemiesDead;
    public event EventHandler OnAllFriendliesDead;
    public event EventHandler OnAnyRoundOutcome;

    private List<Unit> unitList;
    private List<Unit> friendlyUnitList;
    private List<Unit> enemyUnitList;

    private void Awake()
    {
        if (Instance != null)
        {
            Debug.LogError("There's more than one UnitManager! " + transform + " - " + Instance);
            Destroy(gameObject);
            return;
        }

        Instance = this;

        unitList = new List<Unit>();
        friendlyUnitList = new List<Unit>();
        enemyUnitList = new List<Unit>();
    }

    private void Start()
    {
        Unit.OnAnyUnitSpawned += Unit_OnAnyUnitSpawned;
        Unit.OnAnyUnitDead += Unit_OnAnyUnitDead;
    }

    private void Unit_OnAnyUnitSpawned(object sender, System.EventArgs e)
    {
        Unit unit = sender as Unit;

        unitList.Add(unit);

        if(unit.IsEnemy())
        {
            enemyUnitList.Add(unit);
        }

        else
        {
            friendlyUnitList.Add(unit);
        }
    }

    private void Unit_OnAnyUnitDead(object sender, System.EventArgs e)
    {
        Unit unit = sender as Unit;

        unitList.Remove(unit);

        if (unit.IsEnemy())
        {
            enemyUnitList.Remove(unit);

            if(enemyUnitList.Count <= 0)
            {
                Debug.Log("All Enemies Dead");
                OnAllEnemiesDead?.Invoke(this, EventArgs.Empty);
                OnAnyRoundOutcome?.Invoke(this, EventArgs.Empty);
            }
        }

        else
        {
            friendlyUnitList.Remove(unit);

            if(friendlyUnitList.Count <= 0)
            {
                OnAllFriendliesDead?.Invoke(this, EventArgs.Empty);
                OnAnyRoundOutcome?.Invoke(this, EventArgs.Empty);
            }
        }
    }

    public List<Unit> GetUnitList()
    {
        return unitList;
    }

    public List<Unit> GetFriendlyUnitList()
    {
        return friendlyUnitList;
    }

    public List<Unit> GetEnemyUnitList()
    {
        return enemyUnitList;
    }
}
