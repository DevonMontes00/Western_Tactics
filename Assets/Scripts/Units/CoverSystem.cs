using System;
using UnityEngine;

public class CoverSystem : MonoBehaviour
{
    public event EventHandler OnCoverPointsChanged;

    [SerializeField] private double northCoverPoints;
    [SerializeField] private double southCoverPoints;
    [SerializeField] private double westCoverPoints;
    [SerializeField] private double eastCoverPoints;

    public void SetStartingCoverPoints(GridPosition gridPosition)
    {
        GridObject gridObject = LevelGrid.Instance.GetGridObject(gridPosition);
        SetNorthCoverPoints(gridObject.GetSouthCoverPoints());
        SetSouthCoverPoints(gridObject.GetNorthCoverPoints());
        SetEastCoverPoints(gridObject.GetWestCoverPoints());
        SetWestCoverPoints(gridObject.GetEastCoverPoints());
    }

    public void SetNorthCoverPoints(double northCoverPoints)
    {
        this.northCoverPoints = northCoverPoints;
        OnCoverPointsChanged?.Invoke(this, EventArgs.Empty);
    }
    public double GetNorthCoverPoints()
    {
        return northCoverPoints;
    }

    public void SetSouthCoverPoints(double southCoverPoints)
    {
        this.southCoverPoints = southCoverPoints;
        OnCoverPointsChanged?.Invoke(this, EventArgs.Empty);
    }
    public double GetSouthCoverPoints()
    {
        return southCoverPoints;
    }

    public void SetWestCoverPoints(double westCoverPoints)
    {
        this.westCoverPoints = westCoverPoints;
        OnCoverPointsChanged?.Invoke(this, EventArgs.Empty);
    }
    public double GetWestCoverPoints()
    {
        return westCoverPoints;
    }

    public void SetEastCoverPoints(double eastCoverPoints)
    {
        this.eastCoverPoints = eastCoverPoints;
        OnCoverPointsChanged?.Invoke(this, EventArgs.Empty);
    }
    public double GetEastCoverPoints()
    {
        return eastCoverPoints;
    }

    public bool IsInCover()
    {
        if (northCoverPoints > 0 || southCoverPoints > 0 || westCoverPoints > 0 || eastCoverPoints > 0)
        {
            return true;
        }

        else
        {
            return false;
        }
    }
}
