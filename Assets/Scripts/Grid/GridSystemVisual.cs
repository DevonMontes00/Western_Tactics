using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;
using System;
using Unity.Mathematics;

public class GridSystemVisual : MonoBehaviour
{
    public static GridSystemVisual Instance { get; private set; }

    [Serializable]
    public struct GridVisualTypeMaterial
    {
        public GridVisualType gridVisualType;
        public Material material;
    }
    public enum GridVisualType
    {
        White,
        Blue,
        Red,
        RedSoft,
        Yellow
    }

    [SerializeField] private Transform gridSystemVisualSinglePrefab;
    [SerializeField] private List<GridVisualTypeMaterial> gridVisualTypeMaterialList;

    private GridSystemVisualSingle[,] gridSystemVisualSinglesArray;

    private void Awake()
    {
        if (Instance != null)
        {
            Debug.LogError("There's more than one GridSystemVisual! " + transform + " - " + Instance);
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    private void Start()
    {
        gridSystemVisualSinglesArray = new GridSystemVisualSingle[
            LevelGrid.Instance.GetWidth(), 
            LevelGrid.Instance.GetHeight()];

        for (int x = 0; x < LevelGrid.Instance.GetWidth(); x++)
        {
            for(int z = 0; z < LevelGrid.Instance.GetHeight(); z++)
            {
                GridPosition gridPosition = new GridPosition(x,z);
                Transform gridSystemVisualSingleTransform = Instantiate(gridSystemVisualSinglePrefab, LevelGrid.Instance.GetWorldPosition(gridPosition), Quaternion.identity);

                gridSystemVisualSinglesArray[x,z] = gridSystemVisualSingleTransform.GetComponent<GridSystemVisualSingle>();
            }
        }

        UnitActionSystem.Instance.OnSelectedActionChanged += UnitActionSystem_OnSelectedActionChanged;
        LevelGrid.Instance.OnAnyUnitMovedGridPosition += LevelGrid_OnAnyUnitMovedGridPosition;

        UpdateGridVisual();
    }

    

    public void HideAllGridPosition()
    {
        foreach (GridSystemVisualSingle visual in gridSystemVisualSinglesArray) 
        {
            visual.HideMesh();
            visual.HideAllShields();
        }
    }
    private void ShowGridPositionRange(GridPosition gridPosition, int range, GridVisualType gridVisualType)
    {
        List<GridPosition> gridPositionList = new List<GridPosition>();
        for (int x = -range; x <= range; x++)
        {
            for (int z = -range; z <= range; z++)
            {
                GridPosition testGridPosition = gridPosition + new GridPosition(x, z);

                if (!LevelGrid.Instance.IsValidGridPosition(testGridPosition))
                {
                    continue;
                }

                int testDistance = Mathf.Abs(x) + Math.Abs(z);
                if (testDistance > range)
                {
                    //Calculating for a circular radius
                    continue;
                }

                gridPositionList.Add(testGridPosition);
            }
        }

        ShowGridPositionList(gridPositionList, gridVisualType);
    }

    private void ShowGridPositionRangeSquare(GridPosition gridPosition, int range, GridVisualType gridVisualType)
    {
        List<GridPosition> gridPositionList = new List<GridPosition>();
        for (int x = -range; x <= range; x++)
        {
            for (int z = -range; z <= range; z++)
            {
                GridPosition testGridPosition = gridPosition + new GridPosition(x, z);

                if (!LevelGrid.Instance.IsValidGridPosition(testGridPosition))
                {
                    continue;
                }

                gridPositionList.Add(testGridPosition);
            }
        }

        ShowGridPositionList(gridPositionList, gridVisualType);
    }

    private void ShowGridPositionCoverVisuals(GridPosition gridPosition, int range)
    {
        List<GridPosition> gridPositionList = new List<GridPosition>();
        for (int x = -range; x <= range; x++)
        {
            for (int z = -range; z <= range; z++)
            {
                GridPosition testGridPosition = gridPosition + new GridPosition(x, z);

                if (!LevelGrid.Instance.IsValidGridPosition(testGridPosition))
                {
                    continue;
                }

                int testDistance = Mathf.Abs(x) + Math.Abs(z);
                if (testDistance > range)
                {
                    //Calculating for a circular radius
                    continue;
                }

                if(LevelGrid.Instance.GetGridObject(testGridPosition).GetCoverPointsNorth() < 0 &&
                    LevelGrid.Instance.GetGridObject(testGridPosition).GetCoverPointsSouth() < 0 &&
                    LevelGrid.Instance.GetGridObject(testGridPosition).GetCoverPointsEast() < 0 &&
                    LevelGrid.Instance.GetGridObject(testGridPosition).GetCoverPointsWest() < 0)
                {
                    //No cover points in any direction
                    continue;
                }

                gridPositionList.Add(testGridPosition);
            }
        }

        

        foreach (GridPosition testGridPosition in gridPositionList)
        {
            if (LevelGrid.Instance.GetGridObject(testGridPosition).GetCoverPointsNorth() > 0)
            {
                gridSystemVisualSinglesArray[testGridPosition.x, testGridPosition.z].ShowSouthShield();
            }

            if (LevelGrid.Instance.GetGridObject(testGridPosition).GetCoverPointsSouth() > 0)
            {
                gridSystemVisualSinglesArray[testGridPosition.x, testGridPosition.z].ShowNorthShield();
            }

            if (LevelGrid.Instance.GetGridObject(testGridPosition).GetCoverPointsWest() > 0)
            {
                gridSystemVisualSinglesArray[testGridPosition.x, testGridPosition.z].ShowEastShield();
            }

            if (LevelGrid.Instance.GetGridObject(testGridPosition).GetCoverPointsEast() > 0)
            {
                gridSystemVisualSinglesArray[testGridPosition.x, testGridPosition.z].ShowWestShield();
            }
        }
    }

    public void ShowGridPositionList(List<GridPosition> gridPositionsList, GridVisualType gridVisualType)
    {
        foreach(GridPosition gridPosition in gridPositionsList)
        {
            gridSystemVisualSinglesArray[gridPosition.x, gridPosition.z].ShowMesh(GetGridVisualTypeMaterial(gridVisualType));
        }
    }

    private void UpdateGridVisual()
    {
        HideAllGridPosition();

        Unit selectedUnit = UnitActionSystem.Instance.GetSelectedUnit();
        BaseAction selectedAction = UnitActionSystem.Instance.GetSelectedAction();

        GridVisualType gridVisualType;
        switch(selectedAction)
        {
            case MoveAction moveAction:
                gridVisualType = GridVisualType.White;
                ShowGridPositionCoverVisuals(selectedUnit.GetGridPosition(), moveAction.GetMaxMoveDistance());
                break;
            case SpinAction spinAction:
                gridVisualType = GridVisualType.Blue;
                break;
            case ShootAction shootAction:
                gridVisualType = GridVisualType.Red;
                ShowGridPositionRange(selectedUnit.GetGridPosition(), shootAction.GetMaxShootDistance(), GridVisualType.RedSoft);
                break;
            case GrenadeAction grenadeAction:
                gridVisualType = GridVisualType.Yellow;
                break;
            case SwordAction swordAction:
                gridVisualType = GridVisualType.Red;
                ShowGridPositionRangeSquare(selectedUnit.GetGridPosition(), swordAction.GetMaxSwordDistance(), GridVisualType.RedSoft);
                break;
            case InteractAction interactAction:
                gridVisualType = GridVisualType.Blue;
                break;
            default:
                gridVisualType = GridVisualType.White;
                break;
        }
        ShowGridPositionList(selectedAction.GetValidActionGridPositionList(), gridVisualType);
    }

    private void UnitActionSystem_OnSelectedActionChanged(object sender, EventArgs e)
    {
        UpdateGridVisual();
    }

    private void LevelGrid_OnAnyUnitMovedGridPosition(object sender, EventArgs e)
    {
        UpdateGridVisual();
    }

    private Material GetGridVisualTypeMaterial(GridVisualType gridVisualType)
    {
        foreach (GridVisualTypeMaterial gridVisualTypeMaterial in gridVisualTypeMaterialList)
        {
            if(gridVisualTypeMaterial.gridVisualType == gridVisualType)
            {
                return gridVisualTypeMaterial.material;
            }
        }

        Debug.LogError("Could not find GridVisualTypeMaterial for GridVisualType " + gridVisualType);
        return null;
    }
}
