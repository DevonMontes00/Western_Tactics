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

    public enum CoverDirection
    {
        North,
        South,
        West,
        East,
    }
    
    private double coverPointsNorth;
    private double coverPointsSouth;
    private double coverPointsWest;
    private double coverPointsEast;

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

    public double GetCoverPointsNorth()
    {
        return coverPointsNorth;
    }

    public void SetCoverPointsNorth(double coverPointsNorth)
    {
        this.coverPointsNorth = coverPointsNorth;
    }

    public double GetCoverPointsSouth()
    {
        return coverPointsSouth;
    }

    public void SetCoverPointsSouth(double coverPointsSouth)
    {
        this.coverPointsSouth = coverPointsSouth;
    }
    public double GetCoverPointsEast()
    {
        return coverPointsEast;
    }

    public void SetCoverPointsEast(double coverPointsEast)
    {
        this.coverPointsEast = coverPointsEast;
    }

    public double GetCoverPointsWest()
    {
        return coverPointsWest;
    }

    public void SetCoverPointsWest(double coverPointsWest)
    {
        this.coverPointsWest = coverPointsWest;
    }
}
