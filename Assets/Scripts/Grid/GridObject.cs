using System;
using System.Collections.Generic;
using UnityEngine;

public class GridObject
{
    private GridSystem<GridObject> gridSystem;
    private GridPosition gridPosition;

    private bool hasUnit;
    private List<Unit> unitList;
    private IInteractable interactable;

    public event EventHandler OnUnitAdded;
    public event EventHandler OnUnitRemoved;

    public GridObject(GridSystem<GridObject> gridSystem, GridPosition gridPosition)
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

    public bool HasAnyUnit()
    {
        return unitList.Count > 0;
    }

    public Unit GetUnit()
    {
        if (HasAnyUnit())
        {
            return unitList[0];
        }
        else
        {
            return null;
        }
    }

    public IInteractable GetInteractable()
    {
        return interactable;
    }

    public void SetInteractable(IInteractable interactable)
    {
        this.interactable = interactable;
    }
}
