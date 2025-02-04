using System;
using System.Collections.Generic;
using UnityEngine;

public class GridObject
{
    private GridSystem gridSystem;
    private GridPosition gridPosition;

    private bool hasUnit;
    private List<Unit> unitList;

    public event EventHandler OnUnitAdded;
    public event EventHandler OnUnitRemoved;

    public GridObject(GridSystem gridSystem, GridPosition gridPosition)
    {
        this.gridSystem = gridSystem;
        this.gridPosition = gridPosition;
        unitList = new List<Unit>();
    }

    public override string ToString()
    {   
        string unitString = string.Empty;
        foreach(Unit unit in unitList)
        {
            unitString += unit + "\n";
        }
        return gridPosition.ToString() + "\n" + unitString;
    }

    public bool GetHasUnit()
    {
        return hasUnit;
    }

    public void SetHasUnit(bool hasUnit)
    {
        this.hasUnit = hasUnit;

        if (hasUnit)
        {
            OnUnitAdded?.Invoke(this, EventArgs.Empty);
        }

        else
        {
            OnUnitRemoved?.Invoke(this, EventArgs.Empty);
        }
    }

    public List<Unit> GetUnitList()
    {
        return this.unitList;
    }

    public void AddUnit(Unit unit)
    {
        unitList.Add(unit);
    }

    public void RemoveUnit(Unit unit)
    {
        unitList.Remove(unit);
    }
    
    public GridPosition GetGridPosition()
    {
        return gridPosition;
    }
}
