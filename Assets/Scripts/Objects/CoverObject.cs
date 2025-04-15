using System;
using System.CodeDom;
using UnityEngine;

public class CoverObject : MonoBehaviour
{
    public event EventHandler OnCoverObjectDestroyed;

    private GridPosition gridPosition;
    private bool destroyable;

    private enum CoverType{
        None,
        Half,
        Full,
    }

    [SerializeField] private CoverType coverType;
    private double coverPoints;

    private void Awake()
    {
        if(coverType == CoverType.None)
        {
            coverPoints = 0;
        }

        else if (coverType == CoverType.Half)
        {
            coverPoints = 2.5;
        }

        else
        {
            coverPoints = 5;
        }

        gridPosition = LevelGrid.Instance.GetGridPosition(gameObject.transform.position);
    }

    private void Start()
    {
        if(gameObject.TryGetComponent<DestructableCrate>(out DestructableCrate destructableCrate))
        {
            destructableCrate.OnDestroy += DestructableCrate_OnDestroy;
            destroyable = true;
        }

        else
            { destroyable = false; }
    }

    private void DestructableCrate_OnDestroy(object sender, EventArgs e)
    {
        OnCoverObjectDestroyed?.Invoke(this, EventArgs.Empty);
    }

    public double GetCoverPoints()
    {
        return coverPoints;
    }

    public bool IsDestroyable()
    {
        return destroyable;
    }

    public void SetGridPosition(GridPosition gridPosition)
    {
        this.gridPosition = gridPosition;
    }

    public GridPosition GetGridPosition()
    {
        return gridPosition;
    }
}
